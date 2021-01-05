using System.Linq;
using System.Collections.Generic;
using CodeContracts;
using CAD.DomainModel.Schema;

namespace CAD.DomainModel.Graph
{
    /// <summary>
    /// Взвешенный граф схемы
    /// </summary>
    public class WeightedSchemaGraph
    {
        /// <summary>
        /// Ребра взвешенного графа схемы
        /// </summary>
        public IList<Edge> Edges { get; }

        /// <summary>
        /// Создание графа на основе схемы соединений
        /// </summary>
        /// <param name="schema">Схема соединений</param>
        public WeightedSchemaGraph(Schema.Schema schema)
        {
            Requires.NotNull(schema, nameof(schema));

            var edges = new Dictionary<(Element, Element), Edge>();
            var matrixOfConnections = schema.MatrixOfConnections;
            var chains = schema.Chains;

            foreach (var chain in chains)
                for (var i = 0; i < chain.Elements.Count; i++)
                    for (var j = i + 1; j < chain.Elements.Count; j++)
                    {
                        var firstElement = chain.Elements[i];
                        var secondElement = chain.Elements[j];
                        if (!edges.ContainsKey((firstElement, secondElement)) &&
                            !edges.ContainsKey((secondElement, firstElement)))
                            edges.Add
                            (
                                (firstElement, secondElement),
                                new Edge
                                (
                                    firstElement,
                                    secondElement,
                                    matrixOfConnections[firstElement, secondElement]
                                )
                            );
                    }

            Edges = edges.Values.ToList().AsReadOnly();
        }
    }
}