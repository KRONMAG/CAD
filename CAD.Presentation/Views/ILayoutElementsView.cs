using System;
using CAD.DomainModel.LayoutAlgorithm;
using CAD.Presentation.Common;
using CAD.Presentation.Views.EventArgs;

namespace CAD.Presentation.Views
{
    /// <summary>
    /// Представление компоновки элементов схемы соединений в узлы
    /// </summary>
    public interface ILayoutElementsView : IView
    {
        /// <summary>
        /// Событие запроса запуска алгоритма компоновки
        /// </summary>
        event EventHandler<RunLayoutAlgorithmEventArgs> RunLayoutAlgorithm;

        /// <summary>
        /// Событие запроса остановки алгоритма компоновки
        /// </summary>
        event EventHandler StopLayoutAlgorithm;

        /// <summary>
        /// Событие запроса принятия результатов выполнения алгоритма
        /// </summary>
        event EventHandler AcceptLayoutResult;

        /// <summary>
        /// Отображение очередного результата выполнения алгоритма:
        /// каждому поколению особей соответствует отдельный результат
        /// </summary>
        /// <param name="result">Результат выполнения алгоритма</param>
        void AddResult(GeneticAlgorithmResult result);

        /// <summary>
        /// Удаление всех ранее отображенных результатов
        /// </summary>
        void ClearResults();
    }
}