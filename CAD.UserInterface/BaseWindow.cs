using CodeContracts;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using CAD.Presentation.Common;

namespace CAD.UserInterface
{
    /// <summary>
    /// Базовый класс окна
    /// </summary>
    public abstract class BaseWindow : MetroWindow, IView
    {
        /// <summary>
        /// Окно показа анимации ожидания
        /// </summary>
        private readonly WaitingAnimationWindow _window;

        /// <summary>
        /// Инициализация элемента управления
        /// </summary>
        public BaseWindow() =>
            _window = new WaitingAnimationWindow();

        /// <summary>
        /// Показ анимации ожидания в представлении с указанным сообщением
        /// </summary>
        /// <param name="message">Текст анимации ожидания</param>
        public void StartAnimation(string message)
        {
            Requires.NotNull(message, nameof(message));

            Dispatcher.Invoke(() =>
            {
                _window.Owner = this;
                IsEnabled = false;
                _window.Message = message;
                _window.Show();
            });
        }

        /// <summary>
        /// Остановка анимации ожидания
        /// </summary>
        public void StopAnimation() =>
            Dispatcher.Invoke(() =>
            {
                IsEnabled = true;
                _window.Hide();
            });

        /// <summary>
        /// Показ диалогового окна с указанным заголовком и текстом
        /// </summary>
        /// <param name="title">Заголовок сообщения</param>
        /// <param name="message">Текст сообщения</param>
        public void ShowMessageDialog(string title, string message)
        {
            Requires.NotNull(title, nameof(title));
            Requires.NotNull(message, nameof(message));

            Dispatcher.Invoke
            (
                () => DialogManager.ShowMessageAsync
                (
                    this,
                    title,
                    message,
                    settings: new MetroDialogSettings
                    {
                        DialogMessageFontSize = 14
                    }
                )
            );
        }
    }
}