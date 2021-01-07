using System.Windows;
using CAD.Presentation.Common;
using CAD.Presentation.Presenters;
using CAD.Presentation.Views;
using CAD.UserInterface.LayoutElements;
using CAD.UserInterface.LoadSchema;
using CAD.UserInterface.ShowSchema;

namespace CAD.UserInterface
{
    /// <summary>
    /// Класс приложения
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Запуск приложения
        /// </summary>
        public App()
        {
            
            new ApplicationController()
                .RegisterSingleton<IShowSchemaView, ShowSchemaWindow>()
                .Register<ILoadSchemaView, LoadSchemaWindow>()
                .Register<ILayoutElementsView, LayoutElementsWindow>()
                .Run<ShowSchemaPresenter>();
        }
    }
}