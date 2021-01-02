using System;
using System.Threading;
using CodeContracts;
using CAD.DomainModel.LayoutAlgorithm;
using CAD.DomainModel.Schema;
using CAD.Presentation.Common;
using CAD.Presentation.Views;
using CAD.Presentation.Views.EventArgs;

namespace CAD.Presentation.Presenters
{
    public class LayoutAlgoritmPresenter : BasePresenter<ILayoutAlgorithmView, Schema>
    {
        private bool _isLayoutAlgorithmRunning;

        public LayoutAlgoritmPresenter
            (ApplicationController controller,
            ILayoutAlgorithmView view) :
            base(controller, view)
        {
            view.RunLayoutAlgorithm += RunLayoutAlgorithm;
            _isLayoutAlgorithmRunning = false;
        }

        private void RunLayoutAlgorithm(object sender, RunLayoutAlgorithmEventArgs args)
        {
            Requires.NotNull(sender, nameof(sender));
            Requires.NotNull(args, nameof(args));

            if (_isLayoutAlgorithmRunning)
                view.MessageDialog("Ошибка", "Алгоритм компоновки уже запущен");
            else if (args.NodesCount < 2)
                view.MessageDialog("Ошибка", "Количество узлов должно быть больше единицы");
            else if (args.GenerationsCount < 1)
                view.MessageDialog("Ошибка", "Количество поколений дожно быть больше нуля");
            else if (args.PopulationSize < 2)
                view.MessageDialog("Ошибка", "Размер популяции должен быть больше единицы");
            else
            {
                _isLayoutAlgorithmRunning = true;
                var cancellationTokenSource = new CancellationTokenSource();
                var algorithmArgs = new GeneticAlgorithmArgs
                (
                    schema: parameter,
                    nodesCount: args.NodesCount,
                    generationsCount: args.GenerationsCount,
                    populationSize: args.PopulationSize,
                    parentSelection: args.ParentSelection,
                    selection: args.Selection,
                    runAsynchronously: true,
                    cancellationToken: cancellationTokenSource.Token
                );
                var algorithm = new GeneticAlgorithm();

                void ViewClosed(object _, EventArgs __) =>
                    cancellationTokenSource.Cancel();

                void IterationCompleted(object _, GeneticAlgorithmResult result) =>
                    view.UpdateResult(result);

                void StopLayoutAlgorithm(object sender, EventArgs e) =>
                    LayoutCanceled(null, null);

                void LayoutCanceled(object _, GeneticAlgorithmResult __)
                {
                    view.Closed -= ViewClosed;
                    view.StopLayoutAlgorithm -= StopLayoutAlgorithm;
                    algorithm.IterationCompleted -= IterationCompleted;
                    algorithm.LayoutCanceled -= LayoutCanceled;
                    algorithm.LayoutCompleted -= LayoutCanceled;
                    _isLayoutAlgorithmRunning = false;
                }

                view.Closed += ViewClosed;
                view.StopLayoutAlgorithm += StopLayoutAlgorithm;
                algorithm.IterationCompleted += IterationCompleted;
                algorithm.LayoutCanceled += LayoutCanceled;
                algorithm.LayoutCompleted += LayoutCanceled;

                algorithm.Run(algorithmArgs);
            }
        }

        public override void Run(Schema parameter)
        {
            Requires.NotNull(parameter, nameof(parameter));

            base.Run(parameter);
        }
    }
}