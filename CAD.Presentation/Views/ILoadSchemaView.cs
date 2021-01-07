using System;
using CAD.Presentation.Common;
using CAD.Presentation.Views.EventArgs;

namespace CAD.Presentation.Views
{
    /// <summary>
    /// Описание представления загрузки схемы соединений
    /// </summary>
    public interface ILoadSchemaView : IView
    {
        /// <summary>
        /// Событие запроса загрузки схемы соединений из файла
        /// </summary>
        event EventHandler<LoadSchemaEventArgs> LoadSchema;
    }
}