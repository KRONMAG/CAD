using CodeContracts;
using GraphX.Common.Models;

namespace CAD.DomainModel.Graph
{
    /// <summary>
    /// Вершина взвешенного графа схемы
    /// </summary>
    public class Vertex : VertexBase
    {
        /// <summary>
        /// Метка вершины
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// Создание вершины
        /// </summary>
        /// <param name="label">Метка вершины</param>
        public Vertex(string label)
        {
            Requires.NotNull(label, nameof(label));
            Requires.NotNullOrEmpty(label, nameof(label), "Метка вершины не может быть пустой");

            base.ID = label.GetHashCode();
            base.GroupId = 1;

            Label = label;
        }

        /// <summary>
        /// Получение текстового представления вершины
        /// </summary>
        /// <returns>Метка вершины</returns>
        public override string ToString() =>
            Label;
    }
}