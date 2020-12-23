using System;
using CAD.Presentation.Common;
using CAD.Presentation.Views.EventArgs;

namespace CAD.Presentation.Views
{
    /// <summary>
    /// Представление главного меню программы
    /// </summary>
    public interface IMainView : IView
    {
        /// <summary>
        /// Событие запроса загрузки схемы соединений из файла
        /// </summary>
        event EventHandler<LoadSchemaEventArgs> LoadSchema;
    }
}