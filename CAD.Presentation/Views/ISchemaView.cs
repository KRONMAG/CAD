using System;
using CAD.DomainModel.Graph;
using CAD.DomainModel.Schema;
using CAD.Presentation.Common;
using CAD.Presentation.Views.EventArgs;

namespace CAD.Presentation.Views
{
    /// <summary>
    /// Описание представления отображения схемы соединений
    /// </summary>
    public interface ISchemaView : IView
    {
        /// <summary>
        /// Событие запроса компоновки элементов
        /// </summary>
        event EventHandler<LayoutSchemaElementEventArgs> LayoutSchemaElements;

        /// <summary>
        /// Отображение текстового представления схемы
        /// </summary>
        /// <param name="text">Текстовое представление схемы</param>
        void ShowSchemaText(string text);

        /// <summary>
        /// Отображение матрицы комплексов схемы
        /// </summary>
        /// <param name="matrix">Матрица комплесков схемы</param>
        void ShowMatrixOfComplexes(LabeledMatrix<Element, Chain, int> matrix);

        /// <summary>
        /// Отображение матрицы соединений схемы
        /// </summary>
        /// <param name="matrix">Матрица соединений схемы</param>
        void ShowMatrixOfConnections(LabeledMatrix<Element, Element, int> matrix);

        /// <summary>
        /// Отображение взвешенного графа схемы
        /// </summary>
        /// <param name="graph">Взвешенный граф схемы</param>
        void ShowWeightedSchemaGraph(WeightedSchemaGraph graph);

        /// <summary>
        /// Отображение количества межузловых соединений
        /// </summary>
        /// <param name="count">Количество межузловых соединений</param>
        void ShowInternodeConnectionsCount(int count);
    }
}