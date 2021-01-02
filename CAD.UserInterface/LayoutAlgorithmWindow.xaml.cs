using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using OxyPlot;
using OxyPlot.Series;
using CAD.DomainModel.LayoutAlgorithm;
using CAD.Presentation.Views;
using CAD.Presentation.Views.EventArgs;

namespace CAD.UserInterface
{
    public partial class LayoutAlgorithmWindow : BaseWindow, ILayoutAlgorithmView
    {
        private Dictionary<object, ParentSelectionType> _parentSelections;

        private Dictionary<object, SelectionType> _selections;

        public LayoutAlgorithmWindow()
        {
            InitializeComponent();
            LayoutResultsPlotView.Model = PlotModelCreator.Create();
            _parentSelections = new Dictionary<object, ParentSelectionType>
            {
                ["Аутбридинг"] = ParentSelectionType.Outbreeding,
                ["Инбридинг"] = ParentSelectionType.Inbreeding,
                ["Панмиксия"] = ParentSelectionType.Panmixia
            };
            _selections = new Dictionary<object, SelectionType>
            {
                ["Турнирная селекция"] = SelectionType.Tournament,
                ["Элитарная селекция"] = SelectionType.Elitism
            };
            ParentSelectionComboBox.ItemsSource = _parentSelections.Keys;
            SelectionComboBox.ItemsSource = _selections.Keys;
        }

        public event EventHandler<RunLayoutAlgorithmEventArgs> RunLayoutAlgorithm;

        public event EventHandler StopLayoutAlgorithm;

        public void UpdateResult(GeneticAlgorithmResult result) =>
            Dispatcher.Invoke(() =>
            {
                if (result.GenerationNumber == 1)
                    LayoutResultsPlotView.Model = PlotModelCreator.Create();
                var series = LayoutResultsPlotView.Model.Series
                .Cast<LineSeries>()
                .ToList();
                series[0].Points.Add
                (
                    new DataPoint
                    (
                        result.GenerationNumber,
                        result.MinConnectionsCount
                    )
                );
                series[1].Points.Add
                (
                    new DataPoint
                    (
                        result.GenerationNumber,
                        result.AverageConnectionsCount
                    )
                );
                series[2].Points.Add
                (
                    new DataPoint
                    (
                        result.GenerationNumber,
                        result.MaxConnectionsCount
                    )
                );
                LayoutResultsPlotView.InvalidatePlot();
            });

        void Presentation.Common.IView.Show()
        {
            this.ShowDialog();
        }

        private void RunLayoutAlgorithmClick(object sender, RoutedEventArgs e) =>
            RunLayoutAlgorithm?.Invoke
            (
                this,
                new RunLayoutAlgorithmEventArgs
                (
                    (int)NodesCountNumericUpDown.Value,
                    (int)GenerationsCountNumericUpDown.Value,
                    (int)PopulationSizeNumericUpDown.Value,
                    _parentSelections[ParentSelectionComboBox.SelectedItem],
                    _selections[SelectionComboBox.SelectedItem]
                )
            );

        private void StopLayloutAlgorithmClick(object sender, RoutedEventArgs e) =>
            StopLayoutAlgorithm?.Invoke(this, EventArgs.Empty);
    }
}