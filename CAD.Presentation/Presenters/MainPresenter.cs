using System.IO;
using CodeContracts;
using CAD.DomainModel.Schema;
using CAD.Presentation.Common;
using CAD.Presentation.Views;
using CAD.Presentation.Views.EventArgs;

namespace CAD.Presentation.Presenters
{
    /// <summary>
    /// Представитель представления главного меню программы
    /// </summary>
    public class MainPresenter : BasePresenter<IMainView>
    {
        /// <summary>
        /// Создание экземпляра класса
        /// </summary>
        /// <param name="controller">Контроллер приложения</param>
        /// <param name="view">Представление</param>
        public MainPresenter(ApplicationController controller, IMainView view) :
            base(controller, view) =>
            view.LoadSchema += OpenSchemaFile;

        /// <summary>
        /// Обработчик события запроса загрузки схемы соединений из файла
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие</param>
        /// <param name="args">Параметры события</param>
        private void OpenSchemaFile(object sender, LoadSchemaEventArgs args)
        {
            Requires.NotNull(sender, nameof(sender));
            Requires.NotNull(args, nameof(args));

            if (string.IsNullOrWhiteSpace(args.SchemaFilePath))
                view.MessageDialog("Ошибка", "Не указан путь к файлу со списком соединений");
            else if (!File.Exists(args.SchemaFilePath))
                view.MessageDialog("Ошибка", "Не найден файл со списком соединений по уканному пути");
            else if (string.IsNullOrWhiteSpace(args.E0Prefix))
                view.MessageDialog("Ошибка", "Не задан префикс элемента e0");
            else
            {
                view.StartAnimation("Чтение и обработка файла со списком соединений");
                Worker.Run
                (
                    view,
                    () => new SchemaParser().Parse(File.ReadAllLines(args.SchemaFilePath), args.E0Prefix),
                    schema =>
                    {
                        view.StopAnimation();
                        controller.Run<SchemaPresenter, Schema>(schema);
                    },
                    _ =>
                    {
                        view.StopAnimation();
                        view.MessageDialog
                        (
                            "Ошибка",
                            "В ходе чтения и обработки файла с описанием схемы соединений произошла ошибка"
                        );
                    }
                );
            }
        }
    }
}