using System;
using CodeContracts;
using CAD.DomainModel.Schema;
using CAD.Presentation.Common;

namespace CAD.Presentation.Presenters.Events
{
    /// <summary>
    /// Событие принятия результата выполнения алгоритма компоновки
    /// </summary>
    public class LayoutResultAcceptedEvent : IEvent<Schema>
    {
        /// <summary>
        /// Обработчик события
        /// </summary>
        public EventHandler<Schema> Handler { get; }

        /// <summary>
        /// Создание экземпляра класса
        /// </summary>
        /// <param name="handler">Обработчик события</param>
        public LayoutResultAcceptedEvent(EventHandler<Schema> handler)
        {
            Requires.NotNull(handler, nameof(handler));

            Handler = handler;
        }
    }
}