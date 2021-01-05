using System;

namespace CAD.Presentation.Common
{
    /// <summary>
    /// Описание функционала события
    /// </summary>
    /// <typeparam name="T">Тип параметра обработчика события</typeparam>
    public interface IEvent<T>
    {
        /// <summary>
        /// Обработчик события
        /// </summary>
        EventHandler<T> Handler { get; }
    }
}