using System.IO;
using CodeContracts;
using CAD.DomainModel.Schema;
using CAD.Presentation.Common;
using CAD.Presentation.Views;
using CAD.Presentation.Views.EventArgs;
using CAD.Presentation.Presenters.Events;

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
            Requires.NotNull(args, nameof(args));

            if (string.IsNullOrWhiteSpace(args.SchemaFilePath))
                view.ShowMessageDialog("Ошибка", "Не указан путь к файлу со списком соединений");
            else if (!File.Exists(args.SchemaFilePath))
                view.ShowMessageDialog("Ошибка", "Не найден файл списка соединений по указанному пути");
            else if (string.IsNullOrWhiteSpace(args.E0Prefix))
                view.ShowMessageDialog("Ошибка", "Не задан префикс элемента e0");
            else
            {
                try
                {
                    var schemaWasParsed = SchemaParser.TryParse
                    (
                        File.ReadAllLines(args.SchemaFilePath),
                        args.Format,
                        args.E0Prefix,
                        out Schema schema
                    );

                    if (schemaWasParsed)
                    {
                        controller.RaiseEvent<SchemaLoadedEvent, Schema>(this, schema);
                        view.Close();
                    }
                    else
                    {
                        view.ShowMessageDialog
                        (
                            "Ошибка",
                            "Указанная схема соединений не содержит цепей с разными элементами"
                        );
                    }
                }
                catch
                {
                    view.ShowMessageDialog
                    (
                        "Ошибка",
                        "Не удалось разобрать содержимое указанного файла списка соединений"
                    );
                }
                finally
                {
                    view.StopAnimation();
                }
            }
        }
    }
}