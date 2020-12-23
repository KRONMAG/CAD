using System.Collections.Generic;
using System.Linq;
using CodeContracts;

namespace CAD.DomainModel.Schema
{
    /// <summary>
    /// Матрица с маркированными строками и столбцами
    /// </summary>
    /// <typeparam name="TRowLabel">Тип меток строк матрицы</typeparam>
    /// <typeparam name="TColumnLabel">Тип меток столбцов матрицы</typeparam>
    /// <typeparam name="TElement">Тип элементов матрицы</typeparam>
    public class LabeledMatrix<TRowLabel, TColumnLabel, TElement>
    {
        /// <summary>
        /// Элементы матрицы
        /// </summary>
        private Dictionary<(TRowLabel, TColumnLabel), TElement> _elements;

        /// <summary>
        /// Метки строк матрицы
        /// </summary>
        public IReadOnlyList<TRowLabel> RowLabels { get; }

        /// <summary>
        /// Метки столбцов матрицы
        /// </summary>
        public IReadOnlyList<TColumnLabel> ColumnLabels { get; }

        /// <summary>
        /// Обращение к элементу матрицы на основе значений меток строки и столбца
        /// </summary>
        /// <param name="rowLabel">Метка строки матрицы</param>
        /// <param name="columnLabel">Метка столбца матрицы</param>
        /// <returns>Элемент матрицы, стоящий на пересечении указанных строки и столбца</returns>
        public TElement this[TRowLabel rowLabel, TColumnLabel columnLabel]
        {
            get => _elements[(rowLabel, columnLabel)];
            set => _elements[(rowLabel, columnLabel)] = value;
        }

        /// <summary>
        /// Создание экезмпляра класса
        /// </summary>
        /// <param name="rowLabels">Метки строк матрицы</param>
        /// <param name="columnLabels">Метки столбцов матрицы</param>
        public LabeledMatrix(IReadOnlyList<TRowLabel> rowLabels, IReadOnlyList<TColumnLabel> columnLabels)
        {
            Requires.NotNull(rowLabels, nameof(rowLabels));
            Requires.NotNull(columnLabels, nameof(columnLabels));
            Requires.True
            (
                rowLabels.Distinct().Count() == rowLabels.Count,
                "Список меток строк матрицы не должен содержать дубликатов"
            );
            Requires.True
            (
                columnLabels.Distinct().Count() == columnLabels.Count(),
                "Список меток столбцов матрицы не должен содержить дубликатов"
            );
            

            RowLabels = rowLabels;
            ColumnLabels = columnLabels;

            _elements = new Dictionary<(TRowLabel, TColumnLabel), TElement>();

            foreach (var rowLabel in rowLabels)
                foreach (var columnLabel in columnLabels)
                    _elements[(rowLabel, columnLabel)] = default;
        }

        /// <summary>
        /// Преобразование маркированной матрицы в двумерный массив
        /// </summary>
        /// <param name="matrix">Маркированная матрица</param>
        public static implicit operator TElement[,](LabeledMatrix<TRowLabel, TColumnLabel, TElement> matrix)
        {
            Requires.NotNull(matrix, nameof(matrix));

            var result = new TElement[matrix.RowLabels.Count, matrix.ColumnLabels.Count];
            for (var i = 0; i < matrix.RowLabels.Count; i++)
                for (var j = 0; j < matrix.ColumnLabels.Count; j++)
                    result[i, j] = matrix[matrix.RowLabels[i], matrix.ColumnLabels[j]];

            return result;
        }
    }
}