using CodeContracts;
using GraphX.Common.Models;

namespace CAD.UserInterface.ShowSchema
{
    /// <summary>
    /// Вершина графа
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
        /// <param name="groupId">Номер группы, в которой расположена вершина</param>
        internal Vertex(string label, int groupId)
        {
            Requires.NotNull(label, nameof(label));
            Requires.True
            (
                !string.IsNullOrWhiteSpace(label),
                "Метка вершины не может быть пустой"
            );

            base.ID = label.GetHashCode();
            base.GroupId = groupId;

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