using MahApps.Metro.Controls;

namespace CAD.UserInterface
{
    /// <summary>
    /// Окно отображения анимации ожидания
    /// </summary>
    public partial class WaitingAnimationWindow : MetroWindow
    {
        /// <summary>
        /// Сообщение, отображаемое рядом с анимацией ожидания
        /// </summary>
        public string Message
        {
            get => (string)MessageLabel.Content;
            set => MessageLabel.Content = value;
        }

        /// <summary>
        /// Инициализация окна
        /// </summary>
        public WaitingAnimationWindow()
        {
            InitializeComponent();
        }
    }
}