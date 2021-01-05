using System;
using CodeContracts;
using CAD.DomainModel.Schema;
using CAD.Presentation.Common;

namespace CAD.Presentation.Presenters.Events
{
    /// <summary>
    /// Событие окончания загрузки схемы соединений из файла
    /// </summary>
    public class SchemaLoadedEvent : IEvent<Schema>
    {
        /// <summary>
        /// Обработчик события
        /// </summary>
        public EventHandler<Schema> Handler { get; }

        /// <summary>
        /// Создание экземпляра класса
        /// </summary>
        /// <param name="handler">Обработчик события</param>
        public SchemaLoadedEvent(EventHandler<Schema> handler)
        {
            Requires.NotNull(handler, nameof(handler));

            Handler = handler;
        }
    }
}