using GraphX.Common.Models;

namespace CAD.DomainModel.Graph
{
    /// <summary>
    /// Ребро взвешенного графа схемы
    /// </summary>
    public class Edge : EdgeBase<Vertex>
    {
        /// <summary>
        /// Создание экземпляра класса
        /// </summary>
        /// <param name="source">Первая вершина, инцидентная ребру</param>
        /// <param name="target">Вторая вершина, инцидентная ребру</param>
        /// <param name="weight">Вес ребра</param>
        public Edge(Vertex source, Vertex target, int weight) : base(source, target, weight)
        {
            base.ID = (source.Label + target.Label).GetHashCode();
        }
    }
}