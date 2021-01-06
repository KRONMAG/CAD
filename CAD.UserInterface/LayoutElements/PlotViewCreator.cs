using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Wpf;
using LineSeries = OxyPlot.Series.LineSeries;

namespace CAD.UserInterface.LayoutElements
{
    /// <summary>
    /// Средство создания элемента управления для отображения процесса компоновки элементов в узлы
    /// </summary>
    public static class PlotViewCreator
    {
        /// <summary>
        /// Создание элемента управления для отображения графика хода выполнения генетического алгоритма компоновки
        /// </summary>
        /// <param name="minSeries">
        /// Слой графика для отображения зависимости приспособленности лучшей особи от номера поколения
        /// </param>
        /// <param name="avgSeries">
        /// Слой графика для отображения зависимости средней приспособленности в популяции от номера поколения
        /// </param>
        /// <param name="maxSeries">
        /// Слой графика для отображения зависимости приспособленности худшей особи от номера поколения
        /// </param>
        /// <returns>
        /// Элемент управления для отображения графика
        /// </returns>
        public static PlotView Create(out LineSeries minSeries, out LineSeries avgSeries, out LineSeries maxSeries)
        {
            var plotModel = new PlotModel();

            plotModel.LegendBackground = OxyColors.White;
            plotModel.LegendBorder = OxyColors.Black;
            plotModel.LegendBorderThickness = 1;
            plotModel.LegendMargin = 0;
            plotModel.LegendPosition = LegendPosition.BottomLeft;

            plotModel.Axes.Add
            (
                new OxyPlot.Axes.LinearAxis
                {
                    Title = "Номер поколения",
                    Minimum = 1,
                    Position = AxisPosition.Bottom,
                }
            );
            plotModel.Axes.Add
            (
                new OxyPlot.Axes.LinearAxis
                {
                    Title = "Количество межузловых соединений",
                    Minimum = 1,
                    Position = AxisPosition.Left
                }
            );

            const int strokeThickness = 1;

            minSeries = new LineSeries
            {
                Title = "Наилучшая особь",
                Color = OxyColors.Green,
                StrokeThickness = strokeThickness
            };
            avgSeries = new LineSeries
            {
                Title = "Среднее по популяции",
                Color = OxyColors.Yellow,
                StrokeThickness = strokeThickness
            };
            maxSeries = new LineSeries
            {
                Title = "Наихудшая особь",
                Color = OxyColors.Red,
                StrokeThickness = strokeThickness
            };

            plotModel.Series.Add(minSeries);
            plotModel.Series.Add(avgSeries);
            plotModel.Series.Add(maxSeries);

            return new PlotView
            {
                Model = plotModel
            };
        }
    }
}