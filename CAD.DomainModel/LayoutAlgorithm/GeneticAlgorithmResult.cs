using System.Collections.Generic;
using System.Linq;
using CodeContracts;
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
        public int MinInternodeConnectionsCount { get; }

        /// <summary>
        /// Средняя приспособленность популяции
        /// </summary>
        public double AvgInternodeConnectionsCount { get; }

        /// <summary>
        /// Приспособленность наихудшей особи
        /// </summary>
        public int MaxInternodeConnectionsCount { get; }

        /// <summary>
        /// Наилучшая особь популяции
        /// </summary>
        public IReadOnlyDictionary<Element, int> BestDistribution { get; }

        /// <summary>
        /// Создание экземпляра класса
        /// </summary>
        /// <param name="schema">Коммутационная схема</param>
        /// <param name="generationNumber">Номер поколения популяции</param>
        /// <param name="bestIndividual">Лучшая особь популяции</param>
        /// <param name="fitness">Значения функции приспособленности особей популяции</param>
        internal GeneticAlgorithmResult
            (Schema.Schema schema,
            int generationNumber,
            int[] bestIndividual,
            int[] fitness)
        {
            Requires.NotNull(schema, nameof(schema));
            Requires.NotNull(bestIndividual, nameof(bestIndividual));
            Requires.NotNull(fitness, nameof(fitness));
            Requires.InRange
            (
                generationNumber > 0,
                "Номер поколения должен быть положительным числом"
            );
            Requires.True
            (
                bestIndividual.Length == schema.Elements.Count,
                "Длина генотипа лучшей особи должна быть равна количеству элементов схемы"
            );
            Requires.True
            (
                fitness.All(fitness => fitness > 0),
                "Значение приспособленности должно быть положительным числом"
            );

            GenerationNumber = generationNumber;
            MinInternodeConnectionsCount = fitness.Min();
            AvgInternodeConnectionsCount = fitness.Average();
            MaxInternodeConnectionsCount = fitness.Max();
            BestDistribution = Enumerable
                .Range(0, schema.Elements.Count)
                .ToDictionary(index => schema.Elements[index], index => bestIndividual[index]);
        }
    }
}