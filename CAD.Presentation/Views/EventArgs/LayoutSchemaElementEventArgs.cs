namespace CAD.Presentation.Views.EventArgs
{
    /// <summary>
    /// Параметры события запроса компоновки элементов
    /// </summary>
    public class LayoutSchemaElementEventArgs
    {
        /// <summary>
        /// Количество узлов, по которым надо распределить элементы
        /// </summary>
        public int NodesCount { get; }

        /// <summary>
        /// Создание экземпляра класса
        /// </summary>
        /// <param name="nodesCount">Количество узлов, по которым надо распределить элементы</param>
        public LayoutSchemaElementEventArgs(int nodesCount)
        {
            NodesCount = nodesCount;
        }
    }
}