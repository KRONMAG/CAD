using System.Threading;
using CodeContracts;

namespace CAD.DomainModel.LayoutAlgorithm
{
    /// <summary>
    /// Параметры генетического алгоритма
    /// </summary>
    public class GeneticAlgorithmArgs
    {
        /// <summary>
        /// Схема соединений
        /// </summary>
        public Schema.Schema Schema { get; }

        /// <summary>
        /// Количество узлов, по которым нужно распределить элементы схемы
        /// </summary>
        public int NodesCount { get; }

        /// <summary>
        /// Количество поколений
        /// </summary>
        public int GenerationsCount { get; }

        /// <summary>
        /// Размер популяции
        /// </summary>
        public int PopulationSize { get; }

        /// <summary>
        /// Тип оператора выбора родителей
        /// </summary>
        public ParentSelectionType ParentSelection { get; }

        /// <summary>
        /// Тип оператора селекции
        /// </summary>
        public SelectionType Selection { get; }

        /// <summary>
        /// Следует ли алгоритм выполнять асинхронно
        /// </summary>
        public bool RunAsynchronously { get; }

        /// <summary>
        /// Токен отмены выполнения генетического алгоритма
        /// </summary>
        public CancellationToken? CancellationToken { get; }

        /// <summary>
        /// Создание экземпляра класса
        /// </summary>
        /// <param name="schema">Коммутационная схема</param>
        /// <param name="nodesCount">Количество узлов для компоновки элементов схемы</param>
        /// <param name="generationsCount">Количество поколений</param>
        /// <param name="populationSize">Размер популяции</param>
        /// <param name="parentSelection">Тип оператора выбора родителей</param>
        /// <param name="selection">Тип оператора селекции</param>
        /// <param name="runAsynchronously">Требуется ли асинхронное выполнение алгоритма</param>
        /// <param name="cancellationToken">Токен отмены асинхронного выполнения алгоритма</param>
        public GeneticAlgorithmArgs
            (Schema.Schema schema,
            int nodesCount,
            int generationsCount,
            int populationSize,
            ParentSelectionType parentSelection,
            SelectionType selection,
            bool runAsynchronously = false,
            CancellationToken? cancellationToken = null)
        {
            Requires.NotNull(schema, nameof(schema));
            Requires.InRange
            (
                nodesCount > 0,
                "Количество узлов должно быть положительным числом"
            );
            Requires.InRange
            (
                generationsCount > 0,
                "Количество поколений должно быть положительным числом"
            );
            Requires.InRange
            (
                populationSize >= 2,
                "Размер популяции должен составлять не менее двух особей"
            );
            Requires.True
            (
                nodesCount <= schema.Elements.Count,
                nameof(nodesCount),
                "Количество узлов не может превышать количество элементов схемы"
            );

            Schema = schema;
            NodesCount = nodesCount;
            GenerationsCount = generationsCount;
            PopulationSize = populationSize;
            ParentSelection = parentSelection;
            Selection = selection;
            RunAsynchronously = runAsynchronously;
            CancellationToken = cancellationToken;
        }
    }
}