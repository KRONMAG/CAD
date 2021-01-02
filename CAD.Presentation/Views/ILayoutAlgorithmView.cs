using System;
using CAD.DomainModel.LayoutAlgorithm;
using CAD.Presentation.Common;
using CAD.Presentation.Views.EventArgs;

namespace CAD.Presentation.Views
{
    public interface ILayoutAlgorithmView : IView
    {
        event EventHandler<RunLayoutAlgorithmEventArgs> RunLayoutAlgorithm;

        event EventHandler StopLayoutAlgorithm;

        void UpdateResult(GeneticAlgorithmResult result);
    }
}