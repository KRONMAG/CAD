using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using CodeContracts;
using GraphX.Controls;
using Gu.Wpf.DataGrid2D;
using CAD.DomainModel.Schema;
using CAD.Presentation.Views;
using CAD.DomainModel.Graph;
using CAD.Presentation.Views.EventArgs;

namespace CAD.UserInterface.ShowSchema
{
    /// <summary>
    /// Окно отображения схемы соединений
    /// </summary>
    public partial class ShowSchemaWindow : BaseWindow, IShowSchemaView
    {
        /// <summary>
        /// Инициализация окна
        /// </summary>
        public ShowSchemaWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработчик нажатия кнопки перехода к представлению загрузки схемы соединений
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void GoToLoadSchemaViewClick(object sender, MouseButtonEventArgs e) =>
            GoToSchemaLoadView?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Обработчик события нажатия кнопки компоновки элементов
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void GoToLayoutElementsViewClick(object sender, RoutedEventArgs e) =>
            GoToLayoutElementsView?.Invoke(null, EventArgs.Empty);

        /// <summary>
        /// Обработчик нажатия кнопки сохранения данных компоновки
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void SaveElementsDistributionClick(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "Табличные данные (.csv)|*.csv|Все файлы|*.*";
            if (dialog.ShowDialog() == true)
                SaveElementsDistribution?.Invoke
                (
                    this,
                    new SaveElementsDistributionEventArgs(dialog.FileName)
                );
        }

        #region IShowSchemaView

        /// <summary>
        /// Событие запроса перехода к представлению загрузки схемы соединений
        /// </summary>
        public event EventHandler GoToSchemaLoadView;

        /// <summary>
        /// Событие запроса перехода к представлению компоновки элементов схемы
        /// </summary>
        public event EventHandler GoToLayoutElementsView;

        /// <summary>
        /// Событие запроса сохранения данных распределения элементов по узлам
        /// </summary>
        public event EventHandler<SaveElementsDistributionEventArgs> SaveElementsDistribution;

        /// <summary>
        /// Отображение текстового представления схемы
        /// </summary>
        /// <param name="text">Текстовое представление схемы</param>
        public void ShowSchemaText(string text)
        {
            Requires.NotNull(text, nameof(text));

            Dispatcher.Invoke(() =>
            {
                SchemaFileTextTextBox.Text = text;
            });
        }

        /// <summary>
        /// Отображение матрицы комплексов схемы
        /// </summary>
        /// <param name="matrix">Матрица комплесков схемы</param>
        public void ShowMatrixOfComplexes(LabeledMatrix<Element, Chain, int> matrix)
        {
            Requires.NotNull(matrix, nameof(matrix));

            Dispatcher.Invoke(() =>
            {
                MatrixOfComplexesDataGrid.SetRowHeadersSource(matrix.RowLabels);
                MatrixOfComplexesDataGrid.SetColumnHeadersSource(matrix.ColumnLabels);
                MatrixOfComplexesDataGrid.SetArray2D(matrix);
            });
        }

        /// <summary>
        /// Отображение матрицы соединений схемы
        /// </summary>
        /// <param name="matrix">Матрица соединений схемы</param>
        public void ShowMatrixOfConnections(LabeledMatrix<Element, Element, int> matrix)
        {
            Requires.NotNull(matrix, nameof(matrix));

            Dispatcher.Invoke(() =>
            {
                MatrixOfConnectionsDataGrid.SetRowHeadersSource(matrix.RowLabels);
                MatrixOfConnectionsDataGrid.SetColumnHeadersSource(matrix.ColumnLabels);
                MatrixOfConnectionsDataGrid.SetArray2D(matrix);
            });
        }

        /// <summary>
        /// Отображение взвешенного графа схемы
        /// </summary>
        /// <param name="graph">Взвешенный граф схемы</param>
        public void ShowWeightedSchemaGraph(WeightedSchemaGraph graph)
        {
            Requires.NotNull(graph, nameof(graph));

            Dispatcher.Invoke(() =>
            {
                var graphArea = GraphAreaCreator.Create(graph);
                ZoomControl.Content = graphArea;
                ZoomControl.TranslateX = 0;
                ZoomControl.TranslateY = 0;
                ZoomControl.ResetZoom.Execute(ZoomControl, graphArea);
                ZoomControl.ZoomToFill();
            });
        }

        /// <summary>
        /// Отображение распределения элементов по узлам
        /// </summary>
        /// <param name="matrix">
        /// Матрица распределения элементов по узлам:
        /// заголовки строк матрицы - элементы схемы,
        /// заголовок столбца - номер узла, в котором расположен элемент
        /// </param>
        public void ShowElementsDistribution(IReadOnlyList<Element> elements)
        {
            Requires.NullOrWithNoNullElements(elements, nameof(elements));

            Dispatcher.Invoke(() => ElementsDistributionDataGrid.ItemsSource = elements);
        }

        /// <summary>
        /// Отображение количества межузловых соединений
        /// </summary>
        /// <param name="count">Количество межузловых соединений</param>
        public void ShowInternodeConnectionsCount(int count) =>
            Dispatcher.Invoke(() =>
                WeightedSchemaGraphGroupBox.Header =
                    $"Взвешенный граф схемы (межузловых соединений: {count})");

        #endregion
    }
}