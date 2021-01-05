using System;
using System.Windows;
using System.Collections.Generic;
using OxyPlot;
using CAD.DomainModel.LayoutAlgorithm;
using CAD.Presentation.Views;
using CAD.Presentation.Views.EventArgs;
using OxyPlot.Wpf;
using LineSeries = OxyPlot.Series.LineSeries;

namespace CAD.UserInterface.LayoutElements
{
    /// <summary>
    /// Окно компоновки элементов схемы в узлы
    /// </summary>
    public partial class LayoutElementsWindow : BaseWindow, ILayoutElementsView
    {
        /// <summary>
        /// Элементы списка выбора типа оператора формирования родительских пар
        /// </summary>
        private Dictionary<string, ParentSelectionType> _parentSelections;

        /// <summary>
        /// Элементы списка выбора типа оператора селекции
        /// </summary>
        private Dictionary<string, SelectionType> _selections;

        /// <summary>
        /// Элемент управления для отображения результата выполнения компоновки
        /// </summary>
        private PlotView _plotView;

        /// <summary>
        /// Слой графика для отображения зависимости приспособленности лучшей особи от номера поколения
        /// </summary>
        private LineSeries _minSeries;

        /// <summary>
        /// Слой графика для отображения зависимости средней приспособленности популяции от номера поколения
        /// </summary>
        private LineSeries _avgSeries;

        /// <summary>
        /// Слой графика для отображения зависимости приспособленности худшей особи от номера поколения
        /// </summary>
        private LineSeries _maxSeries;

        /// <summary>
        /// Инициализация окна
        /// </summary>
        public LayoutElementsWindow()
        {
            InitializeComponent();
            _plotView = PlotViewCreator.Create
            (
                out _minSeries,
                out _avgSeries,
                out _maxSeries
            );
            LayoutResultsContentControl.Content = _plotView;
            _parentSelections = new Dictionary<string, ParentSelectionType>
            {
                ["Аутбридинг"] = ParentSelectionType.Outbreeding,
                ["Инбридинг"] = ParentSelectionType.Inbreeding,
                ["Панмиксия"] = ParentSelectionType.Panmixia
            };
            _selections = new Dictionary<string, SelectionType>
            {
                ["Турнирная селекция"] = SelectionType.Tournament,
                ["Элитарная селекция"] = SelectionType.Elitism
            };
            ParentSelectionComboBox.ItemsSource = _parentSelections.Keys;
            SelectionComboBox.ItemsSource = _selections.Keys;
        }

        /// <summary>
        /// Обработчик нажатия кнопки запуска алгоритма компоновки
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void RunLayoutAlgorithmClick(object sender, RoutedEventArgs e) =>
            RunLayoutAlgorithm?.Invoke
            (
                this,
                new RunLayoutAlgorithmEventArgs
                (
                    (int)(NodesCountNumericUpDown.Value ?? 2),
                    (int)(GenerationsCountNumericUpDown.Value ?? 500),
                    (int)(PopulationSizeNumericUpDown.Value ?? 20),
                    _parentSelections[(string)ParentSelectionComboBox.SelectedItem],
                    _selections[(string)SelectionComboBox.SelectedItem]
                )
            );

        /// <summary>
        /// Обработчик нажатия кнопки остановки выполнения алгоритма компоновки
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void StopLayoutAlgorithmClick(object sender, RoutedEventArgs e) =>
            StopLayoutAlgorithm?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Обработчик нажатия кнопки принятия результата выполнения алгоритма компоновки
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void AcceptLayoutResultClick(object sender, RoutedEventArgs e) =>
            AcceptLayoutResult?.Invoke(this, EventArgs.Empty);

        #region ILayoutElementsView

        /// <summary>
        /// Событие запроса запуска алгоритма компоновки
        /// </summary>
        public event EventHandler<RunLayoutAlgorithmEventArgs> RunLayoutAlgorithm;

        /// <summary>
        /// Событие запроса остановки работы алгоритма компоновки
        /// </summary>
        public event EventHandler StopLayoutAlgorithm;

        /// <summary>
        /// Событие запроса принятия результатов компоновки элементов схемы
        /// </summary>
        public event EventHandler AcceptLayoutResult;

        /// <summary>
        /// Отображение результата
        /// </summary>
        /// <param name="result"></param>
        public void AddResult(GeneticAlgorithmResult result) =>
            Dispatcher.Invoke(() =>
            {
                _minSeries.Points.Add
                (
                    new DataPoint
                    (
                        result.GenerationNumber,
                        result.MinInternodeConnectionsCount
                    )
                );
                _avgSeries.Points.Add
                (
                    new DataPoint
                    (
                        result.GenerationNumber,
                        result.AvgInternodeConnectionsCount
                    )
                );
                _maxSeries.Points.Add
                (
                    new DataPoint
                    (
                        result.GenerationNumber,
                        result.MaxInternodeConnectionsCount
                    )
                );
                _plotView.InvalidatePlot();

                var item = new[]
                {
                    result.GenerationNumber,
                    result.MinInternodeConnectionsCount,
                    result.AvgInternodeConnectionsCount,
                    result.MaxInternodeConnectionsCount
                };
                LayoutResultsDataGrid.Items.Add(item);
                LayoutResultsDataGrid.ScrollIntoView(item);
            });

        /// <summary>
        /// Отображение окна как диалогового
        /// </summary>
        void Presentation.Common.IView.Show() =>
            ShowDialog();

        /// <summary>
        /// Очистка отображенных результатов выполнения алгоритма
        /// </summary>
        public void ClearResults()
        {
            _minSeries.Points.Clear();
            _avgSeries.Points.Clear();
            _maxSeries.Points.Clear();
            _plotView.ResetAllAxes();
            LayoutResultsDataGrid.Items.Clear();
        }

        #endregion
    }
}