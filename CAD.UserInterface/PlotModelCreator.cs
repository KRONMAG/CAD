using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace CAD.UserInterface
{
    public static class PlotModelCreator
    {
        public static PlotModel Create()
        {
            var plotModel = new PlotModel();
            const int markerSize = 1;
            plotModel.Axes.Add
            (
                new LinearAxis
                {
                    Title = "Номер поколения",
                    Minimum = 1,
                    Position = AxisPosition.Bottom,
                }
            );
            plotModel.Axes.Add
            (
                new LinearAxis
                {
                    Title = "Приспособленность особи",
                    Minimum = 1,
                    Position = AxisPosition.Left
                }
            );
            plotModel.Series.Add
            (
                new LineSeries
                {
                    Title = "Наилучшая особь",
                    Color = OxyColors.Green,
                    StrokeThickness = 1
                }
            );
            plotModel.Series.Add
            (
                new LineSeries
                {
                    Title = "Среднее по популяции",
                    Color = OxyColors.Yellow,
                    StrokeThickness = 1
                }
            );
            plotModel.Series.Add
            (
                new LineSeries
                {
                    Title = "Наихудшая особь",
                    Color = OxyColors.Red,
                    StrokeThickness = 1
                }
            );
            return plotModel;
        }
    }
}
