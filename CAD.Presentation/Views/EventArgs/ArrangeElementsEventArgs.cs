namespace CAD.Presentation.Views.EventArgs
{
    /// <summary>
    /// Параметры события запроса компоновки элементов
    /// </summary>
    public class ArrangeElementsEventArgs
    {
        /// <summary>
        /// Количество узлов, по которым надо распределить элементы
        /// </summary>
        public int NodesCount { get; }

        /// <summary>
        /// Создание экземпляра класса
        /// </summary>
        /// <param name="nodesCount">Количество узлов, по которым надо распределить элементы</param>
        public ArrangeElementsEventArgs(int nodesCount)
        {
            NodesCount = nodesCount;
        }
    }
}