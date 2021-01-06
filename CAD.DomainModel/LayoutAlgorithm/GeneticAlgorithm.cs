using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using CodeContracts;

namespace CAD.DomainModel.LayoutAlgorithm
{
    /// <summary>
    /// Генетический алгоритм компоновки
    /// </summary>
    public class GeneticAlgorithm
    {
        /// <summary>
        /// Событие окончания генерации популяции нового поколения
        /// </summary>
        public EventHandler<GeneticAlgorithmResult> IterationCompleted;

        /// <summary>
        /// Событие остановки работы алгоритма компоновки
        /// </summary>
        public EventHandler<GeneticAlgorithmResult> LayoutCanceled;

        /// <summary>
        /// Событие окончания работы алгоритма компоновки
        /// </summary>
        public EventHandler<GeneticAlgorithmResult> LayoutCompleted;

        /// <summary>
        /// Генерация начальной популяции
        /// </summary>
        /// <param name="args">Параметры генетического алгоритма</param>
        /// <returns>Оособи начальной популяции</returns>
        private int[][] InitialPopulation(GeneticAlgorithmArgs args)
        {
            var random = new Random();
            var template = Enumerable
                .Range(1, args.Schema.Elements.Count)
                .Select(index => index % args.NodesCount + 1);
            return Enumerable
                .Range(1, args.PopulationSize)
                .Select(index => template.OrderBy(node => random.Next()).ToArray())
                .ToArray();
        }

        /// <summary>
        /// Вычисление пригодности особи - количества межузловых соединений,
        /// глобальный минимум которого для заданной коммутационной схемы и числа узлов необходимо найти
        /// </summary>
        /// <param name="args">Параметры генетического алгоритма</param>
        /// <param name="individual">Особь</param>
        /// <returns>Количество межузловых соединений, вычисленных по генотипу особи</returns>
        private int Fitness(GeneticAlgorithmArgs args, int[] individual)
        {
            var matrixOfConnections = args.Schema.MatrixOfConnections;
            var elements = args.Schema.Elements;
            var connectionsCount = 0;
            for (var i = 0; i < individual.Length; i++)
                for (var j = 0; j < i; j++)
                    if (individual[i] != individual[j])
                        connectionsCount += matrixOfConnections[elements[i], elements[j]];
            return connectionsCount;
        }

        /// <summary>
        /// Панмиксия - случайный отбор особей в родительские пары для дальнейшего скрещивания
        /// Количество формируемых пар равно половине числа особей в популяции,
        /// округленной до целого в большую сторону
        /// </summary>
        /// <param name="population">Популяция особей</param>
        /// <returns>Родительские пары</returns>
        private Tuple<int[], int[]>[] Panmixia(int[][] population)
        {
            var random = new Random();
            return Enumerable
                .Range(1, population.Length / 2 + population.Length % 2)
                .Select
                (
                    pairNumber =>
                    {
                        var firstParentIndex = 0;
                        var secondParentIndex = 0;
                        while (firstParentIndex == secondParentIndex)
                        {
                            firstParentIndex = random.Next(population.Length);
                            secondParentIndex = random.Next(population.Length);
                        }
                        return new Tuple<int[], int[]>
                        (
                            population[firstParentIndex],
                            population[secondParentIndex]
                        );
                    }
                )
                .ToArray();
        }

        /// <summary>
        /// Аутбридинг - метод отбора особей в родительскую пару,
        /// при котором первая особь выбирается случайно,
        /// второй выбирается наиболее близкая по генотипу к первой
        /// Количество формируемых пар равно половине числа особей в популяции,
        /// округленной до целого в большую сторону
        /// </summary>
        /// <param name="population">Популяций особей</param>
        /// <returns>Родительские пары</returns>
        private Tuple<int[], int[]>[] Outbreeding(int[][] population)
        {
            var random = new Random();
            return Enumerable
                .Range(1, population.Length / 2 + population.Length % 2)
                .Select
                (
                    pairNumber =>
                    {
                        var firstParent = population[random.Next(population.Length)];
                        var secondParent = population
                            .OrderBy
                            (
                                individual => Enumerable
                                    .Range(0, individual.Length)
                                    .Count(index => firstParent[index] == individual[index])
                            )
                            .First();
                        return new Tuple<int[], int[]>(firstParent, secondParent);
                    }
                ).ToArray();
        }

        /// <summary>
        /// Инбридинг - способ формирования родительских пар, при котором
        /// первым родителем выбирается случайная особь из популяции,
        /// вторым родителем - та, что наиболее не похожа на первую по генотипу
        /// Количество формируемых пар равно половине числа особей в популяции,
        /// округленной до целого в большую сторону
        /// </summary>
        /// <param name="population">Популяция особей</param>
        /// <returns>Сформированные родительские пары</returns>
        private Tuple<int[], int[]>[] Inbreeding(int[][] population)
        {
            var random = new Random();
            return Enumerable
                .Range(1, population.Length / 2 + population.Length % 2)
                .Select
                (
                    pairNumber =>
                    {
                        var firstParent = population[random.Next(population.Length)];
                        var secondParent = population
                            .OrderBy
                            (
                                individual => Enumerable
                                    .Range(0, individual.Length)
                                    .Count(index => firstParent[index] != individual[index])
                            )
                            .Skip(1)
                            .First();
                        return new Tuple<int[], int[]>(firstParent, secondParent);
                    }
                ).ToArray();
        }

        /// <summary>
        /// Адаптивная мутация: обмен местами генов особи-потомка с вероятностью,
        /// равной частному количества одинаковых пар генов его родителей и общего числа генов особи
        /// </summary>
        /// <param name="offspring">Потомок</param>
        /// <param name="firstParent">Первый родитель</param>
        /// <param name="secondParent">Второй родитель</param>
        /// <returns>Потомок</returns>
        private int[] Mutation(int[] offspring, int[] firstParent, int[] secondParent)
        {
            var random = new Random();
            var sameGenesCount = Enumerable
                .Range(0, firstParent.Length)
                .Count(index => firstParent[index] == secondParent[index]);
            if (random.Next(offspring.Length) < sameGenesCount)
                for (var i = 0; i < Math.Max(1, offspring.Length / 10); i++)
                {
                    var firstGeneIndex = 0;
                    var secondGeneIndex = 0;
                    while (firstGeneIndex == secondGeneIndex)
                    {
                        firstGeneIndex = random.Next(offspring.Length);
                        secondGeneIndex = random.Next(offspring.Length);
                    }
                    var tempGene = offspring[firstGeneIndex];
                    offspring[firstGeneIndex] = offspring[secondGeneIndex];
                    offspring[secondGeneIndex] = tempGene;
                }
            return offspring;
        }

        /// <summary>
        /// Скрещивание родительских пар, основанное на смешивании их генотипа
        /// Для каждой пары формируются два потомка
        /// </summary>
        /// <param name="pairs">Родительские пары</param>
        /// <returns>Особи-потомки</returns>
        private int[][] Crossover(Tuple<int[], int[]>[] pairs)
        {
            var nodesCount = pairs.First().Item1.Max(node => node);
            var elementsCount = pairs.First().Item1.Length;
            var maxElementsInNodeCount = elementsCount / nodesCount;

            if (elementsCount % nodesCount > 0)
                maxElementsInNodeCount++;

            int[] GenerateOffspring((int id, int node)[] combinedGenome)
            {
                var distribution = new Dictionary<int, int>();
                var offspring = new Dictionary<int, int>();
                combinedGenome
                    .ToList()
                    .ForEach
                    (
                        gene =>
                        {
                            if (offspring.Keys.Count < elementsCount && !offspring.ContainsKey(gene.id))
                            {
                                if (!distribution.ContainsKey(gene.node))
                                    distribution.Add(gene.node, 0);
                                if (distribution[gene.node] < maxElementsInNodeCount)
                                {
                                    distribution[gene.node]++;
                                    offspring.Add(gene.id, gene.node);
                                }
                            }
                        }
                    );
                foreach (var gene in combinedGenome)
                    if (!offspring.ContainsKey(gene.id))
                        for (var i = 1; i <= nodesCount; i++)
                        {
                            if (!distribution.ContainsKey(i))
                                distribution.Add(i, 0);
                            if (distribution[i] < maxElementsInNodeCount)
                            {
                                offspring.Add(gene.id, i);
                                distribution[i]++;
                                break;
                            }
                        }
                return offspring
                    .OrderBy(gene => gene.Key)
                    .Select(gene => gene.Value)
                    .ToArray();
            }

            var random = new Random();

            return pairs
                .Select(pair =>
                {
                    var mixedGenome = Enumerable.Union
                    (
                        Enumerable
                            .Range(0, pair.Item1.Length)
                            .Select(index => (index, pair.Item1[index])),
                        Enumerable
                            .Range(0, pair.Item2.Length)
                            .Select(index => (index, pair.Item2[index]))
                    )
                    .OrderBy(gene => random.Next());

                    return new[]
                    {
                        Mutation(GenerateOffspring(mixedGenome.ToArray()), pair.Item1, pair.Item2),
                        Mutation(GenerateOffspring(mixedGenome.Reverse().ToArray()), pair.Item1, pair.Item2)
                    };
                })
                .SelectMany(offsprings => offsprings)
                .ToArray();
        }

        /// <summary>
        /// Элитарная селекция - отбор наиболее приспособленных особей популяции,
        /// Количество отбираемых особей - половина размера популяции, округленная
        /// до целого в меньшую сторону
        /// </summary>
        /// <param name="population">Популяция особей</param>
        /// <param name="fitness">Значения присопосбленности особей</param>
        /// <returns>Особи, прошедшие селекцию</returns>
        private int[][] Elitism(int[][] population, Dictionary<int[], int> fitness) =>
            population
                .OrderBy(individual => fitness[individual])
                .Take(population.Length / 2)
                .ToArray();

        /// <summary>
        /// Турнирная селекция - случайное разбиение особей на группы и выбор в каждой группе наилучшей особи
        /// Количество отбираемых особей - половина размера популяции, округленная
        /// до целого в меньшую сторону
        /// </summary>
        /// <param name="population">Популяция особей</param>
        /// <param name="fitness">Значения приспособленности особей</param>
        /// <returns>Оособи, прошедшие отбор</returns>
        private int[][] Tournament(int[][] population, Dictionary<int[], int> fitness)
        {
            var random = new Random();
            var mixedPopulation = population.OrderBy(individual => random.Next()).ToArray();
            return Enumerable
                .Range(1, population.Length / 2)
                .Select(groupNumber =>
                {
                    var group = new List<int[]>
                    {
                        mixedPopulation[(groupNumber - 1) * 2],
                        mixedPopulation[(groupNumber - 1) * 2 + 1]
                    };
                    if (groupNumber * 2 + 1 == population.Length)
                        group.Add(mixedPopulation[groupNumber * 2]);
                    return group.OrderBy(individual => fitness[individual]).First();
                })
                .ToArray();
        }

        /// <summary>
        /// Запуск генетического алгоритма компоновки
        /// </summary>
        /// <param name="args">Параметры генетического алгоритма</param>
        /// <returns>Данные о популяции особей последнего поколения</returns>
        public Task<GeneticAlgorithmResult> Run(GeneticAlgorithmArgs args)
        {
            Requires.NotNull(args, nameof(args));

            GeneticAlgorithmResult Layout()
            {
                var population = InitialPopulation(args);
                var fitness = population.ToDictionary(individual => individual, individual => Fitness(args, individual));
                GeneticAlgorithmResult result = null;

                for (var i = 0; i < args.GenerationsCount; i++)
                {
                    var pairs = args.ParentSelection switch
                    {
                        ParentSelectionType.Panmixia => Panmixia(population),
                        ParentSelectionType.Outbreeding => Outbreeding(population),
                        ParentSelectionType.Inbreeding => Inbreeding(population)
                    };

                    var offsprings = Crossover(pairs);

                    population = population.Union(offsprings).ToArray();

                    foreach (var offspring in offsprings)
                        fitness.Add(offspring, Fitness(args, offspring));

                    population = args.Selection switch
                    {
                        SelectionType.Elitism => Elitism(population, fitness),
                        SelectionType.Tournament => Tournament(population, fitness)
                    };

                    fitness = fitness
                        .Where(pair => population.Contains(pair.Key))
                        .ToDictionary(pair => pair.Key, pair => pair.Value);

                    result = new GeneticAlgorithmResult
                    (
                        args.Schema,
                        i + 1,
                        population.OrderBy(individual => fitness[individual]).First(),
                        fitness.Values.ToArray()
                    );

                    IterationCompleted?.Invoke(this, result);

                    if (args.CancellationToken.HasValue &&
                        args.CancellationToken.Value.IsCancellationRequested)
                    {
                        LayoutCanceled?.Invoke(this, result);
                        return result;
                    }
                }

                LayoutCompleted?.Invoke(this, result);

                return result;
            }

            if (args.RunAsynchronously)
                return Task.Run(() => Layout());
            else
                return Task.FromResult(Layout());
        }
    }
}