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
    public class LoadSchemaPresenter : BasePresenter<ILoadSchemaView>
    {
        /// <summary>
        /// Создание экземпляра класса
        /// </summary>
        /// <param name="controller">Контроллер приложения</param>
        /// <param name="view">Представление</param>
        public LoadSchemaPresenter(ApplicationController controller, ILoadSchemaView view) :
            base(controller, view) =>
            view.LoadSchema += LoadSchema;

        /// <summary>
        /// Обработчик события запроса загрузки схемы соединений из файла
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие</param>
        /// <param name="args">Параметры события</param>
        private void LoadSchema(object sender, LoadSchemaEventArgs args)
        {
            Requires.NotNull(sender, nameof(sender));
            Requires.NotNull(args, nameof(args));

            if (string.IsNullOrWhiteSpace(args.SchemaFilePath))
                view.MessageDialog("Ошибка", "Не указан путь к файлу со списком соединений");
            else if (!File.Exists(args.SchemaFilePath))
                view.MessageDialog("Ошибка", "Не найден файл списка соединений по указанному пути");
            else if (string.IsNullOrWhiteSpace(args.E0Prefix))
                view.MessageDialog("Ошибка", "Не задан префикс элемента e0");
            else
            {
                Schema schema = null;

                try
                {
                    schema = new SchemaParser().Parse(File.ReadAllLines(args.SchemaFilePath), args.E0Prefix);
                }
                catch
                {
                    view.MessageDialog
                    (
                        "Ошибка",
                        "Не удалось разобрать содержимое указанного файла списка соединений"
                    );
                }
                finally
                {
                    view.StopAnimation();
                }

                if (schema != null)
                {
                    controller.RaiseEvent<SchemaLoadedEvent, Schema>(this, schema);
                    view.Close();
                }
            }
        }
    }
}