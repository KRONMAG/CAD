using System;
using System.Windows;
using Microsoft.Win32;
using CAD.Presentation.Views;
using CAD.Presentation.Views.EventArgs;

namespace CAD.UserInterface
{
    /// <summary>
    /// Главное окно программы
    /// </summary>
    public partial class MainWindow : BaseWindow, IMainView
    {
        /// <summary>
        /// Событие запроса загрузки схемы соединений из файла
        /// </summary>
        public event EventHandler<LoadSchemaEventArgs> LoadSchema;

        /// <summary>
        /// Инициализация окна
        /// </summary>
        public MainWindow() =>
            InitializeComponent();

        /// <summary>
        /// Обработчик события нажатия кнопки обзора файлов
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие</param>
        /// <param name="e">Параметры события</param>
        private void OpenSchemaFileClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Файлы списков соединений (*.net)|*.net|Все файлы|*.*";
            if (dialog.ShowDialog() == true)
                SchemaFilePathTextBox.Text = dialog.FileName;
        }

        /// <summary>
        /// Обработчик нажатия кнопки загрузки схемы
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие</param>
        /// <param name="e">Параметры события</param>
        private void LoadSchemaClick(object sender, RoutedEventArgs e) =>
            LoadSchema?.Invoke
            (
                this,
                new LoadSchemaEventArgs(SchemaFilePathTextBox.Text, E0PrefixTextBox.Text)
            );
    }
}