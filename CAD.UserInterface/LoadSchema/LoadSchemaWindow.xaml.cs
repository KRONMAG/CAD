﻿using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using CAD.Presentation.Views;
using CAD.Presentation.Views.EventArgs;
using CAD.Presentation.Common;
using CAD.DomainModel.Schema;

namespace CAD.UserInterface.LoadSchema
{
    /// <summary>
    /// Окно загрузки схемы соединений из файла
    /// </summary>
    public partial class LoadSchemaWindow : BaseWindow, ILoadSchemaView
    {
        /// <summary>
        /// Инициализация окна
        /// </summary>
        public LoadSchemaWindow() =>
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
                new LoadSchemaEventArgs
                (
                    SchemaFilePathTextBox.Text,
                    (SchemaFormat)((ComboBoxItem)SchemaFormatComboBox.SelectedItem).Tag,
                    E0PrefixTextBox.Text
                )
            );

        #region ILoadSchemaView

        /// <summary>
        /// Событие запроса загрузки схемы соединений из файла
        /// </summary>
        public event EventHandler<LoadSchemaEventArgs> LoadSchema;

        /// <summary>
        /// Отображение окна как диалогового
        /// </summary>
        void IView.Show()
        {
            ShowDialog();
        }

        #endregion
    }
}