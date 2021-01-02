using System;
using System.Windows;
using System.Windows.Input;
using GraphX.Controls;
using CAD.DomainModel.Schema;
using CAD.Presentation.Views;
using CAD.DomainModel.Graph;

namespace CAD.UserInterface
{
    /// <summary>
    /// Окно отображения схемы соединений
    /// </summary>
    public partial class ShowSchemaWindow : BaseWindow, IShowSchemaView
    {
        public event EventHandler GoToSchemaLoadView;

        public event EventHandler GoToLayoutElementsView;

        /// <summary>
        /// Инициализация окна
        /// </summary>
        public ShowSchemaWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Отображение текстового представления схемы
        /// </summary>
        /// <param name="text">Текстовое представление схемы</param>
        public void ShowSchemaText(string text) =>
            Dispatcher.Invoke(() =>
            {
                SchemaFileTextTextBox.Text = text;
            });

        /// <summary>
        /// Отображение матрицы комплексов схемы
        /// </summary>
        /// <param name="matrix">Матрица комплесков схемы</param>
        public void ShowMatrixOfComplexes(LabeledMatrix<Element, Chain, int> matrix) =>
            Dispatcher.Invoke(() =>
                MatrixOfComplexesContentControl.Content = DataGrid2DCreator.Create(matrix));

        /// <summary>
        /// Отображение матрицы соединений схемы
        /// </summary>
        /// <param name="matrix">Матрица соединений схемы</param>
        public void ShowMatrixOfConnections(LabeledMatrix<Element, Element, int> matrix) =>
            Dispatcher.Invoke(() =>
                MatrixOfConnectionsContentControl.Content = DataGrid2DCreator.Create(matrix));

        /// <summary>
        /// Отображение взвешенного графа схемы
        /// </summary>
        /// <param name="graph">Взвешенный граф схемы</param>
        public void ShowWeightedSchemaGraph(WeightedSchemaGraph graph) =>
            Dispatcher.Invoke(() =>
            {
                var graphArea = WeightedSchemaGraphAreaCreator.Create(graph);
                ZoomControl.Content = graphArea;
                ZoomControl.TranslateX = 0;
                ZoomControl.TranslateY = 0;
                ZoomControl.ResetZoom.Execute(ZoomControl, graphArea);
                ZoomControl.ZoomToFill();
            });

        public void ShowElementsDistribution(LabeledMatrix<Element, string, int> matrix) =>
            Dispatcher.Invoke(() =>
                ElementsDistributionGroupBox.Content = DataGrid2DCreator.Create(matrix));

        /// <summary>
        /// Отображение количества межузловых соединений
        /// </summary>
        /// <param name="count">Количество межузловых соединений</param>
        public void ShowInternodeConnectionsCount(int count) =>
            Dispatcher.Invoke(() =>
                WeightedSchemaGraphGroupBox.Header =
                    $"Взвешенный граф схемы (межузловых соединений: {count})");


        private void GoToLoadSchemaViewClick(object sender, MouseButtonEventArgs e) =>
            GoToSchemaLoadView?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Обработчик события нажатия кнопки компоновки элементов
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void GoToLayoutElementsViewClick(object sender, RoutedEventArgs e) =>
            GoToLayoutElementsView?.Invoke(null, EventArgs.Empty);
    }
}