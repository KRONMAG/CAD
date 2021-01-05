using GraphX.Common.Models;

namespace CAD.UserInterface.ShowSchema
{
    /// <summary>
    /// Ребро графа
    /// </summary>
    public class Edge : EdgeBase<Vertex>
    {
        /// <summary>
        /// Создание экземпляра класса
        /// </summary>
        /// <param name="source">Первая вершина</param>
        /// <param name="target">Вторая вершина</param>
        /// <param name="weight">Вес ребра</param>
        public Edge(Vertex source, Vertex target, double weight):
            base(source, target, weight)
        {

        }
    }
}