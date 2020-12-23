using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Gu.Wpf.DataGrid2D;
using CAD.DomainModel.Schema;

namespace CAD.UserInterface
{
    /// <summary>
    /// Средство отображения матриц в элементе управления DataGrid
    /// </summary>
    public static class DataGrid2DCreator
    {
        /// <summary>
        /// Создание элемента управления для отображения матрицы
        /// </summary>
        /// <typeparam name="TRowLabel">Тип меток строк матрицы</typeparam>
        /// <typeparam name="TColumnLabel">Тип меток столбцов матрицы</typeparam>
        /// <typeparam name="TElement">Тип значение матрицы</typeparam>
        /// <param name="matrix">Матрица</param>
        /// <returns>Элемент управления с размещенными в нем заголовками и элементами матрицы</returns>
        public static DataGrid Create<TRowLabel, TColumnLabel, TElement>(LabeledMatrix<TRowLabel, TColumnLabel, TElement> matrix)
        {
            var dataGrid = new DataGrid();
            dataGrid.CanUserAddRows = false;
            dataGrid.HeadersVisibility = DataGridHeadersVisibility.All;
            dataGrid.SetRowHeadersSource(GetHeaders(matrix.RowLabels));
            dataGrid.SetColumnHeadersSource(GetHeaders(matrix.ColumnLabels));
            dataGrid.SetArray2D(matrix);
            dataGrid.SetTemplate
            (
                (DataTemplate)XamlReader.Parse
                (
                    @"<DataTemplate 
                        xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                        xmlns:mahapps=""http://metro.mahapps.com/winfx/xaml/controls""> 
                        <TextBlock
                            Text=""{Binding .}""
                            TextAlignment=""Center""/>
                    </DataTemplate>"
                )
            );
            return dataGrid;
        }

        /// <summary>
        /// Создание списка заголовков
        /// </summary>
        /// <typeparam name="T">Тип элементов заголовков</typeparam>
        /// <param name="names">Элементы заголовков</param>
        /// <returns>Список заголовков</returns>
        private static Label[] GetHeaders<T>(IEnumerable<T> names) =>
            names.Select
            (
                name => new Label
                {
                    Content = name,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    FontWeight = FontWeights.Bold
                }
            ).ToArray();
    }
}
