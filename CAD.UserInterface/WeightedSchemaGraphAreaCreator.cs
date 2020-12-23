using System;
using System.Linq;
using System.Windows.Media;
using QuickGraph;
using GraphX.Common.Interfaces;
using GraphX.Logic.Algorithms.LayoutAlgorithms;
using GraphX.Logic.Algorithms.LayoutAlgorithms.Grouped;
using GraphX.Logic.Models;
using CAD.DomainModel;
using CAD.DomainModel.Graph;

namespace CAD.UserInterface
{
    /// <summary>
    /// Средство настройки отображения графа в элементе управления WeightedSchemaGraphArea
    /// </summary>
    public static class WeightedSchemaGraphAreaCreator
    {
        /// <summary>
        /// Создание элемента управления для отображения графа
        /// на основе указанного взвешенного графа схемы
        /// </summary>
        /// <param name="graph">Взвешенный граф схемы</param>
        /// <returns>Элемент управления с размещенным в нем графом</returns>
        public static WeightedSchemaGraphArea Create(WeightedSchemaGraph graph)
        {
            var logicCore = new GXLogicCore<Vertex, Edge, WeightedSchemaGraph>(graph);

            logicCore.AsyncAlgorithmCompute = true;

            ConfigureExternalLayoutAlgorithm(logicCore);

            var graphArea = new WeightedSchemaGraphArea();

            graphArea.LogicCore = logicCore;
            graphArea.ShowAllEdgesArrows(false);
            graphArea.SetVerticesDrag(true);

            ConfigureVertexControls(graphArea);
            ConfigureEdgeControls(graphArea);

            return graphArea;
        }

        /// <summary>
        /// Настройка алгоритма компоновки вершин графа
        /// </summary>
        /// <param name="logicCore">Логика отображения графа</param>
        private static void ConfigureExternalLayoutAlgorithm(GXLogicCore<Vertex, Edge, WeightedSchemaGraph> logicCore)
        {
            var graph = logicCore.Graph;
            var groupsCount = graph.Vertices
                .Select(vertex => vertex.GroupId)
                .Distinct()
                .Count();
            logicCore.ExternalLayoutAlgorithm = new GroupingLayoutAlgorithm<Vertex, Edge, BidirectionalGraph<Vertex, Edge>>
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
                            var groupGraph = new WeightedSchemaGraph();
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
                            LayoutAlgorithm = (groupsCount > 1 ?
                                new KKLayoutAlgorithm<Vertex, Edge, WeightedSchemaGraph>(groupGraph, new KKLayoutParameters()) :
                                (IExternalLayout<Vertex, Edge>)new CircularLayoutAlgorithm<Vertex, Edge, WeightedSchemaGraph>
                                (
                                    groupGraph,
                                    groupGraph.Vertices.ToDictionary(vertex => vertex, vertex => new GraphX.Measure.Point()),
                                    groupGraph.Vertices.ToDictionary(vertex => vertex, vertex => new GraphX.Measure.Size(50, 50)),
                                    new CircularLayoutParameters()
                                ))
                        })
                        .ToList()
                )
                {
                    OverlapRemovalAlgorithm = logicCore.AlgorithmFactory.CreateFSAA<object>(50, 50),
                    ArrangeGroups = true
                }
            );;
        }

        /// <summary>
        /// Настройка внешнего вида вершин графа
        /// </summary>
        /// <param name="graphArea">Элемент управления для отображения графа</param>
        private static void ConfigureVertexControls(WeightedSchemaGraphArea graphArea)
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
        private static void ConfigureEdgeControls(WeightedSchemaGraphArea graphArea)
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
