using System.Collections.Generic;
using System.Linq;
using CodeContracts;

namespace CAD.DomainModel.Schema
{
    /// <summary>
    /// Схема соединений
    /// </summary>
    public class Schema
    {
        /// <summary>
        /// Матрица комплексов
        /// </summary>
        private LabeledMatrix<Element, Chain, int> _matrixOfComplexes;

        /// <summary>
        /// Матрица соединений
        /// </summary>
        private LabeledMatrix<Element, Element, int> _matrixOfConnections;

        /// <summary>
        /// Цепи схемы соединений
        /// </summary>
        public IReadOnlyList<Chain> Chains { get; }

        /// <summary>
        /// Элементы схемы соединений
        /// </summary>
        public IReadOnlyList<Element> Elements { get; }

        /// <summary>
        /// Матрица комплексов
        /// </summary>
        /// <returns>Матрица комплексов</returns>
        public LabeledMatrix<Element, Chain, int> MatrixOfComplexes
        {
            get
            {
                if (_matrixOfComplexes == null)
                {
                    _matrixOfComplexes = new LabeledMatrix<Element, Chain, int>(Elements, Chains);

                    foreach (var chain in Chains)
                        foreach (var element in chain.Elements)
                            _matrixOfComplexes[element, chain] = 1;
                }

                return _matrixOfComplexes;
            }
        }

        /// <summary>
        /// Матрица соединений
        /// </summary>
        public LabeledMatrix<Element, Element, int> MatrixOfConnections
        {
            get
            {
                if (_matrixOfConnections == null)
                {
                    _matrixOfConnections = new LabeledMatrix<Element, Element, int>(Elements, Elements);

                    foreach (var chain in Chains)
                        for (var i = 0; i < chain.Elements.Count; i++)
                            for (var j = 0; j < chain.Elements.Count; j++)
                                if (i != j)
                                    _matrixOfConnections[chain.Elements[i], chain.Elements[j]] += 1;
                }
                return _matrixOfConnections;
            }
        }

        /// <summary>
        /// Количество межузловых соединений
        /// </summary>
        public int InternodeConnectionsCount
        {
            get
            {
                var internodeConnectionsCount = 0;

                for (var i = 0; i < Elements.Count; i++)
                    for (var j = 0; j < i; j++)
                        if (Elements[i].NodeId != Elements[j].NodeId)
                            internodeConnectionsCount += MatrixOfConnections[Elements[i], Elements[j]];

                return internodeConnectionsCount;
            }
        }

        /// <summary>
        /// Создание экземпляра класса
        /// </summary>
        /// <param name="chains">Цепи схемы соединений</param>
        public Schema(IReadOnlyList<Chain> chains)
        {
            Requires.NullOrWithNoNullElements(chains, nameof(chains));
            Requires.True
            (
                chains.Count > 0,
                "Схема должна содержать не менее одной цепи"
            );
            Requires.True
            (
                chains.Distinct().Count() == chains.Count,
                "Схема не должна содержать дублирующихся цепей"
            );

            Chains = chains;
            Elements = chains.SelectMany(chain => chain.Elements)
                .Distinct()
                .ToList()
                .AsReadOnly();
        }

        /// <summary>
        /// Преобразование схемы соединений в строковое представление
        /// </summary>
        /// <returns>Описание схемы соединений в виде списка ее цепей и их элементов</returns>
        public override string ToString() =>
            string.Join
            (
                "\n",
                Chains.Select
                (
                    chain => $"Цепь {chain.Name}: " +
                    string.Join
                    (
                        ", ",
                        chain.Elements.Select(element => element.Name)
                    )
                )
            );
    }
}