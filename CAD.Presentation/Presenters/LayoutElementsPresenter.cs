using System;
using System.Linq;
using System.Threading;
using CodeContracts;
using CAD.DomainModel.LayoutAlgorithm;
using CAD.DomainModel.Schema;
using CAD.Presentation.Common;
using CAD.Presentation.Presenters.Events;
using CAD.Presentation.Views;
using CAD.Presentation.Views.EventArgs;

namespace CAD.Presentation.Presenters
{
    /// <summary>
    /// Представитель представления компоновки элементов схемы в узлы
    /// </summary>
    public class LayoutElementsPresenter : BasePresenter<ILayoutElementsView, Schema>
    {
        /// <summary>
        /// Выполняется ли в текущий момент алгоритм компоновки
        /// </summary>
        private bool _isLayoutAlgorithmRunning;

        /// <summary>
        /// Результат работы алгоритма компоновки
        /// </summary>
        private GeneticAlgorithmResult _result;

        /// <summary>
        /// Создание экземпляра класса
        /// </summary>
        /// <param name="controller">Контроллер приложения</param>
        /// <param name="view">Представление компоновки элементов схемы в узлы</param>
        public LayoutElementsPresenter
            (ApplicationController controller,
            ILayoutElementsView view) :
            base(controller, view)
        {
            view.RunLayoutAlgorithm += RunLayoutAlgorithm;
            view.AcceptLayoutResult += AcceptAlgorithmResult;
        }

        /// <summary>
        /// Обработчик события запроса запуска алгоритма компоновки:
        /// если параметры алгоритма корректны, выполняется его запуск с
        /// отслеживанием событий:
        /// 1) закрытия представления
        /// 2) запрос останова алгоритма
        /// 3) окончание итерации алгоритма
        /// 4) окончания работы алгоритма
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="args">Параметры события</param>
        private void RunLayoutAlgorithm(object sender, RunLayoutAlgorithmEventArgs args)
        {
            Requires.NotNull(args, nameof(args));

            if (_isLayoutAlgorithmRunning)
                view.ShowMessageDialog("Ошибка", "Алгоритм компоновки в данный момент уже запущен");
            else if (args.NodesCount < 2)
                view.ShowMessageDialog("Ошибка", "Количество узлов должно быть больше единицы");
            else if (args.NodesCount > parameter.Elements.Count)
                view.ShowMessageDialog("Ошибка", "Количество узлов превышает число элементов схемы");
            else if (args.GenerationsCount < 1)
                view.ShowMessageDialog("Ошибка", "Указано неположительное количество поколений");
            else if (args.PopulationSize < 2)
                view.ShowMessageDialog("Ошибка", "Задан размер популяции менее двух особей");
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

                void StopLayoutAlgorithm(object _, EventArgs __) =>
                    cancellationTokenSource.Cancel();

                void IterationCompleted(object _, GeneticAlgorithmResult result)
                {
                    _result = result;
                    view.AddResult(result);
                }

                void LayoutCompletedOrCanceled(object _, GeneticAlgorithmResult __)
                {
                    view.Closed -= ViewClosed;
                    view.StopLayoutAlgorithm -= StopLayoutAlgorithm;
                    algorithm.IterationCompleted -= IterationCompleted;
                    algorithm.LayoutCanceled -= LayoutCompletedOrCanceled;
                    algorithm.LayoutCompleted -= LayoutCompletedOrCanceled;
                    _isLayoutAlgorithmRunning = false;
                }

                view.Closed += ViewClosed;
                view.StopLayoutAlgorithm += StopLayoutAlgorithm;
                algorithm.IterationCompleted += IterationCompleted;
                algorithm.LayoutCanceled += LayoutCompletedOrCanceled;
                algorithm.LayoutCompleted += LayoutCompletedOrCanceled;

                view.ClearResults();
                algorithm.Run(algorithmArgs);
            }
        }

        /// <summary>
        /// Обработчик события запроса принятия результата выполнения алгоритма компоновки
        /// Изменяет распределение элементов по узлам в соответствии с результатом
        /// Генерирует соответствующее событие и закрывает представление компоновки элементов
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void AcceptAlgorithmResult(object sender, EventArgs args)
        {
            if (_isLayoutAlgorithmRunning)
                view.ShowMessageDialog
                (
                    "Ошибка",
                    "Дождитесь окончания выполнения алгоритма компоновки или остановите его"
                );
            else if (_result == null)
                view.ShowMessageDialog
                (
                    "Ошибка",
                    "Запустите алгоритм компоновки для получения результата"
                );
            else
            {
                parameter
                    .Elements
                    .ToList()
                    .ForEach
                    (
                        element => element.NodeId = _result.BestDistribution[element]
                    );

                view.Close();

                controller.RaiseEvent<LayoutResultAcceptedEvent, Schema>(this, parameter);
            }
        }

        /// <summary>
        /// Показ представления
        /// </summary>
        /// <param name="parameter">Схема соединений</param>
        public override void Run(Schema parameter)
        {
            Requires.NotNull(parameter, nameof(parameter));

            base.Run(parameter);
        }
    }
}