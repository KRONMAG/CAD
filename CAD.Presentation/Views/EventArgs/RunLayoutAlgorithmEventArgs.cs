using CAD.DomainModel.LayoutAlgorithm;

namespace CAD.Presentation.Views.EventArgs
{
    /// <summary>
    /// Параметры события запроса запуска алгоритма компоновки
    /// </summary>
    public class RunLayoutAlgorithmEventArgs
    {
        /// <summary>
        /// Количество узлов, в которые надо распределить элементы
        /// </summary>
        public int NodesCount { get; }

        /// <summary>
        /// Количество поколений потомков
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
        /// Создание экземпляра класса
        /// </summary>
        /// <param name="nodesCount">
        /// Количество узлов, в которые надо распределить элементы
        /// </param>
        /// <param name="generationsCount">Количество поколений потомков</param>
        /// <param name="populationSize">Размер популяции</param>
        /// <param name="parentSelection">Тип оператора выбора родителей</param>
        /// <param name="selection">Тип оператора селекции</param>
        public RunLayoutAlgorithmEventArgs
            (int nodesCount,
            int generationsCount,
            int populationSize,
            ParentSelectionType parentSelection,
            SelectionType selection)
        {
            NodesCount = nodesCount;
            GenerationsCount = generationsCount;
            PopulationSize = populationSize;
            ParentSelection = parentSelection;
            Selection = selection;
        }
    }
}