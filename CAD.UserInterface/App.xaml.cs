using System.Windows;
using CAD.Presentation.Common;
using CAD.Presentation.Presenters;
using CAD.Presentation.Views;

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
        public App() =>
            new ApplicationController()
                .Register<IMainView, MainWindow>()
                .Register<ISchemaView, SchemaWindow>()
                .Run<MainPresenter>();
    }
}