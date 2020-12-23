using System.Collections.Generic;
using CodeContracts;
using QuickGraph;
using CAD.DomainModel.Schema;

namespace CAD.DomainModel.Graph
{
    /// <summary>
    /// Взвешенный граф схемы
    /// </summary>
    public class WeightedSchemaGraph : BidirectionalGraph<Vertex, Edge>
    {
        /// <summary>
        /// Создание пустого графа
        /// </summary>
        public WeightedSchemaGraph()
        { 
        
        }

        /// <summary>
        /// Создание графа на основе схемы соединений
        /// </summary>
        /// <param name="schema">Схема соединений</param>
        public WeightedSchemaGraph(Schema.Schema schema)
        {
            Requires.NotNull(schema, nameof(schema));

            var edges = new List<Edge>();
            var vertices = new Dictionary<string, Vertex>();

            void AddVertex(Element element)
            {
                if (!vertices.ContainsKey(element.Name))
                    vertices.Add
                    (
                        element.Name,
                        new Vertex(element.Name)
                        {
                            GroupId = element.NodeId
                        }
                    );
            }

            var matrixOfConnections = schema.MatrixOfConnections;

            for (var i = 0; i < matrixOfConnections.RowLabels.Count; i++)
                for (var j = 0; j < i; j++)
                {
                    var rowLabel = matrixOfConnections.RowLabels[i];
                    var columnLabel = matrixOfConnections.ColumnLabels[j];
                    var connectionsCount = matrixOfConnections[rowLabel, columnLabel];
                    if (connectionsCount > 0)
                    {
                        AddVertex(rowLabel);
                        AddVertex(columnLabel);
                        edges.Add
                        (
                            new Edge
                            (
                                vertices[rowLabel.Name],
                                vertices[columnLabel.Name],
                                connectionsCount
                            )
                        );
                    }
                }

            base.AddVerticesAndEdgeRange(edges);
        }
    }
}