using CAD.DomainModel.LayoutAlgorithm;

namespace CAD.Presentation.Views.EventArgs
{
    /// <summary>
    /// Параметры события запроса компоновки элементов
    /// </summary>
    public class RunLayoutAlgorithmEventArgs
    {
        /// <summary>
        /// Количество узлов, по которым надо распределить элементы
        /// </summary>
        public int NodesCount { get; }

        public int GenerationsCount { get; }

        public int PopulationSize { get; }

        public ParentSelectionType ParentSelection { get; }

        public SelectionType Selection { get; }

        /// <summary>
        /// Создание экземпляра класса
        /// </summary>
        /// <param name="nodesCount">Количество узлов, по которым надо распределить элементы</param>
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