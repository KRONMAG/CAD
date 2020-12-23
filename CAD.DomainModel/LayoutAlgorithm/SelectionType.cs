using System;

namespace CAD.DomainModel.LayoutAlgorithm
{
    /// <summary>
    /// Тип оператора селекции
    /// </summary>
    public enum SelectionType
    {
        /// <summary>
        /// Элитарная селекция
        /// </summary>
        Elitism,

        /// <summary>
        /// Турнирная селекция
        /// </summary>
        Tournament
    }
}