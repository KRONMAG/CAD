using System.Collections.Generic;
using System.Linq;
using CAD.DomainModel.Schema;

namespace CAD.DomainModel.LayoutAlgorithm
{
    /// <summary>
    /// Данные о популяции, сформированной генетичеким алгоритмом
    /// </summary>
    public class GeneticAlgorithmResult
    {
        /// <summary>
        /// Номер поколения популяции
        /// </summary>
        public int GenerationNumber { get; }

        /// <summary>
        /// Приспособленность наилучшей особи
        /// </summary>
        public int MinConnectionsCount { get; }

        /// <summary>
        /// Средняя приспособленность популяции
        /// </summary>
        public double AverageConnectionsCount { get; }

        /// <summary>
        /// Приспособленность наихудшей особи
        /// </summary>
        public int MaxConnectionsCount { get; }

        /// <summary>
        /// Наилучшая особь популяции
        /// </summary>
        public IReadOnlyDictionary<Element, int> Solution { get; }

        /// <summary>
        /// Создание экземпляра класса
        /// </summary>
        /// <param name="schema">Коммутационная схема</param>
        /// <param name="generationNumber">Номер поколения популяции</param>
        /// <param name="population">Популяция особей</param>
        /// <param name="fitness">Значения функции приспособленности особей популяции</param>
        internal GeneticAlgorithmResult
            (Schema.Schema schema,
            int generationNumber,
            int[][] population,
            Dictionary<int[], int> fitness)
        {
            GenerationNumber = generationNumber;
            MinConnectionsCount = fitness.Min(pair => pair.Value);
            AverageConnectionsCount = fitness.Average(pair => pair.Value);
            MaxConnectionsCount = fitness.Max(pair => pair.Value);
            var bestIndividual = population.OrderBy(individual => fitness[individual]).First();
            Solution = Enumerable
                .Range(0, schema.Elements.Count)
                .ToDictionary(index => schema.Elements[index], index => bestIndividual[index]);
        }
    }
}