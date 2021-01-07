using System;

namespace CAD.Presentation.Common
{
    /// <summary>
    /// Описание функционала для всех представлений
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// Событие закрытия представления, программно или пользователем
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// Показ представления
        /// </summary>
        void Show();

        /// <summary>
        /// Закрытие представления
        /// </summary>
        void Close();

        /// <summary>
        /// Показ анимации ожидания в представлении с указанным сообщением
        /// </summary>
        /// <param name="message">Текст анимации ожидания</param>
        void StartAnimation(string message);

        /// <summary>
        /// Остановка анимации ожидания
        /// </summary>
        void StopAnimation();

        /// <summary>
        /// Показ диалогового окна с указанным заголовком и текстом
        /// </summary>
        /// <param name="title">Заголовок сообщения</param>
        /// <param name="message">Текст сообщения</param>
        void ShowMessageDialog(string title, string message);
    }
}