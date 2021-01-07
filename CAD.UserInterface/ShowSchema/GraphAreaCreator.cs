using System;
using System.Linq;
using System.Windows.Media;
using CodeContracts;
using GraphX.Common.Interfaces;
using GraphX.Logic.Algorithms.LayoutAlgorithms;
using GraphX.Logic.Algorithms.LayoutAlgorithms.Grouped;
using GraphX.Logic.Models;
using GraphX.Controls;
using QuickGraph;
using CAD.DomainModel.Graph;

namespace CAD.UserInterface.ShowSchema
{
    /// <summary>
    /// Средство настройки отображения графа в элементе управления GraphArea
    /// </summary>
    public static class GraphAreaCreator
    {
        /// <summary>
        /// Создание элемента управления для отображения графа
        /// на основе указанного взвешенного графа схемы
        /// </summary>
        /// <param name="graph">Взвешенный граф схемы</param>
        /// <returns>Элемент управления с размещенным в нем графом</returns>
        public static GraphArea<Vertex, Edge, BidirectionalGraph<Vertex, Edge>> Create(WeightedSchemaGraph graph)
        {
            Requires.NotNull(graph, nameof(graph));

            var logicCore = new GXLogicCore<Vertex, Edge, BidirectionalGraph<Vertex, Edge>>
            (
                CreateInternalGraph(graph)
            );

            logicCore.AsyncAlgorithmCompute = true;

            ConfigureExternalLayoutAlgorithm(logicCore);

            var graphArea = new GraphArea<Vertex, Edge, BidirectionalGraph<Vertex, Edge>>();

            graphArea.LogicCore = logicCore;
            graphArea.ShowAllEdgesArrows(false);
            graphArea.SetVerticesDrag(true);

            ConfigureVertexControls(graphArea);
            ConfigureEdgeControls(graphArea);

            return graphArea;
        }

        /// <summary>
        /// Создание внутреннего представления графа
        /// для отображения взвешенного графа схемы
        /// </summary>
        /// <param name="graph">Взвешенный граф схемы</param>
        /// <returns>Представление взвешенного графа схемы для отображеняи</returns>
        private static BidirectionalGraph<Vertex, Edge> CreateInternalGraph(WeightedSchemaGraph graph)
        {
            var internalGraph = new BidirectionalGraph<Vertex, Edge>();

            var vertices = graph.Edges
                .SelectMany(edge => new[]
                {
                    edge.FirstElement,
                    edge.SecondElement
                })
                .Distinct()
                .ToDictionary
                (
                    element => element,
                    element => new Vertex(element.Name, element.NodeId)
                );

            internalGraph.AddVerticesAndEdgeRange
            (
                graph.Edges.Select
                (
                    edge => new Edge
                    (
                        vertices[edge.FirstElement],
                        vertices[edge.SecondElement],
                        edge.CommonChainsCount
                    )
                )
            );

            return internalGraph;
        }

        /// <summary>
        /// Настройка алгоритма компоновки вершин графа
        /// </summary>
        /// <param name="logicCore">Логика отображения графа</param>
        private static void ConfigureExternalLayoutAlgorithm
            (GXLogicCore<Vertex, Edge, BidirectionalGraph<Vertex, Edge>> logicCore)
        {
            var graph = logicCore.Graph;
            var groupsCount = graph.Vertices
                .Select(vertex => vertex.GroupId)
                .Distinct()
                .Count();

            logicCore.ExternalLayoutAlgorithm = new GroupingLayoutAlgorithm
                <Vertex, Edge, BidirectionalGraph<Vertex, Edge>>
            (
                graph,
                null,
                new GroupingLayoutAlgorithmParameters<Vertex, Edge>
                (
                    graph.Vertices
                        .GroupBy(vertex => vertex.GroupId, vertex => vertex)
                        .Select(group => group.ToList())
                        .Select(vertices =>
                        {
                            var groupGraph = new BidirectionalGraph<Vertex, Edge>();
                            groupGraph.AddVertexRange(vertices);
                            groupGraph.AddEdgeRange
                            (
                                graph.Edges.Where
                                (
                                    edge => vertices.Contains(edge.Source) && vertices.Contains(edge.Target)
                                )
                            );
                            return groupGraph;
                        })
                        .Select(groupGraph => new AlgorithmGroupParameters<Vertex, Edge>()
                        {
                            GroupId = groupGraph.Vertices.First().GroupId,
                            LayoutAlgorithm = groupsCount > 1
                                ?
                                new KKLayoutAlgorithm
                                <Vertex, Edge, BidirectionalGraph<Vertex, Edge>>
                                (groupGraph, new KKLayoutParameters())
                                :
                                (IExternalLayout<Vertex, Edge>)
                                new CircularLayoutAlgorithm
                                <Vertex, Edge, BidirectionalGraph<Vertex, Edge>>
                                (
                                    groupGraph,
                                    groupGraph.Vertices.ToDictionary
                                    (
                                        vertex => vertex,
                                        vertex => new GraphX.Measure.Point()
                                    ),
                                    groupGraph.Vertices.ToDictionary
                                    (
                                        vertex => vertex,
                                        vertex => new GraphX.Measure.Size(50, 50)
                                    ),
                                    new CircularLayoutParameters()
                                )
                        })
                        .ToList()
                )
                {
                    OverlapRemovalAlgorithm = logicCore.AlgorithmFactory.CreateFSAA<object>(50, 50),
                    ArrangeGroups = true
                }
            ); ;
        }

        /// <summary>
        /// Настройка внешнего вида вершин графа
        /// </summary>
        /// <param name="graphArea">Элемент управления для отображения графа</param>
        private static void ConfigureVertexControls
            (GraphArea<Vertex, Edge, BidirectionalGraph<Vertex, Edge>> graphArea)
        {
            graphArea.GenerateGraph();

            var random = new Random((int)(DateTime.Now.Ticks % int.MaxValue));

            var verticesColors = graphArea
                .LogicCore
                .Graph
                .Vertices
                .Select(vertex => vertex.GroupId)
                .Distinct()
                .ToDictionary
                (
                    groupId => groupId,
                    groupId =>
                    {
                        return new SolidColorBrush
                        (
                            Color.FromRgb
                            (
                                (byte)(random.Next(0, int.MaxValue) % 192),
                                (byte)(random.Next(0, int.MaxValue) % 256),
                                (byte)(random.Next(0, int.MaxValue) % 256)
                            )
                        );
                    }
                );

            graphArea.VertexList
                .Select(pair => pair.Value)
                .ToList()
                .ForEach(vertexControl =>
                {
                    var groupId = ((Vertex)vertexControl.Vertex).GroupId;
                    vertexControl.Background = verticesColors[groupId];
                    vertexControl.Foreground = Brushes.White;
                    vertexControl.ToolTip = $"Узел {groupId}";
                });
        }

        /// <summary>
        /// Настройка внешнего вида ребер графа
        /// </summary>
        /// <param name="graphArea">Элемент управления для отображения графа</param>
        private static void ConfigureEdgeControls
            (GraphArea<Vertex, Edge, BidirectionalGraph<Vertex, Edge>> graphArea)
        {
            graphArea.GenerateAllEdges();

            graphArea.EdgesList
                .Select(pair => pair.Value)
                .ToList()
                .ForEach(edgeControl =>
                {
                    edgeControl.DetachLabels();
                    var edge = (Edge)edgeControl.Edge;
                    edgeControl.ToolTip = $"Количество цепей: {edge.Weight}";
                    if (edge.Source.GroupId == edge.Target.GroupId)
                        edgeControl.Foreground = Brushes.LightGray;
                    else
                        edgeControl.Foreground = Brushes.LightBlue;
                });
        }
    }
}