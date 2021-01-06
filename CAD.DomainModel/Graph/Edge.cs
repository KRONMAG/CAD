using CAD.DomainModel.Schema;
using CodeContracts;

namespace CAD.DomainModel.Graph
{
    /// <summary>
    /// Ребро взвешенного графа схемы
    /// </summary>
    public class Edge
    {
        /// <summary>
        /// Элемент схемы - первая вершина, инцидентная ребру
        /// </summary>
        public Element FirstElement { get; }

        /// <summary>
        /// Элемент схемы - вторая вершина, инцидентная ребру
        /// </summary>
        public Element SecondElement { get; }

        /// <summary>
        /// Количестов цепей между элементами
        /// </summary>
        public int CommonChainsCount { get; }

        /// <summary>
        /// Создание экземпляра класса
        /// </summary>
        /// <param name="firstElement">Элемент схемы - первая вершина, инцидентная ребру</param>
        /// <param name="secondElement">Элемент схемы - вторая вершна, инцидентная ребру/param>
        /// <param name="commonChainsCount">Количество общих цепей элементов</param>
        public Edge(Element firstElement, Element secondElement, int commonChainsCount)
        {
            Requires.NotNull(firstElement, nameof(firstElement));
            Requires.NotNull(secondElement, nameof(secondElement));
            Requires.True(firstElement != secondElement, "Наличие петель в графе не допускается");
            Requires.InRange
            (
                commonChainsCount > 0,
                nameof(commonChainsCount),
                "Количество обших цепей элементов должно быть больше нуля"
            );

            FirstElement = firstElement;
            SecondElement = secondElement;
            CommonChainsCount = commonChainsCount;
        }
    }
}