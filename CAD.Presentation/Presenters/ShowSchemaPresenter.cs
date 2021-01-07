using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeContracts;
using CsvHelper;
using CAD.Presentation.Common;
using CAD.Presentation.Views;
using CAD.DomainModel.Schema;
using CAD.DomainModel.Graph;
using CAD.Presentation.Presenters.Events;
using CAD.Presentation.Views.EventArgs;

namespace CAD.Presentation.Presenters
{
    /// <summary>
    /// Представитель представления отображения схемы соединений
    /// </summary>
    public class ShowSchemaPresenter : BasePresenter<IShowSchemaView>
    {
        /// <summary>
        /// Коммутационная схема
        /// </summary>
        private Schema _schema;

        /// <summary>
        /// Создание экземпляра класса
        /// </summary>
        /// <param name="controller">Контроллер приложения</param>
        /// <param name="view">Представление отображения схемы соединений</param>
        public ShowSchemaPresenter(ApplicationController controller, IShowSchemaView view) :
            base(controller, view)
        {
            view.GoToLayoutElementsView += GoToLayoutElementsView;
            view.GoToSchemaLoadView += GoToLoadSchemaView;
            view.SaveElementsDistribution += SaveElementsDistribution;

            controller.Subscribe<SchemaLoadedEvent, Schema>
            (
                this,
                new SchemaLoadedEvent(SchemaLoaded)
            );
            controller.Subscribe<LayoutResultAcceptedEvent, Schema>
            (
                this,
                new LayoutResultAcceptedEvent(LayoutResultsAccepted)
            );
        }

        /// <summary>
        /// Обработчик события окончания загрузки схемы:
        /// Передает в представление для отображения:
        /// 1) текстовое описание схемы
        /// 2) матрицу соединений
        /// 3) матрицу комплексов
        /// 4) взвешенный граф схемы
        /// 5) распределение элементов по узлам
        /// 6) количество межузловых соединений
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="schema">Загруженная схема соединений</param>
        private void SchemaLoaded(object sender, Schema schema)
        {
            _schema = schema;

            Task.Run(() =>
            {
                view.StartAnimation("Отображение схемы соединений");
                view.ShowSchemaText(schema.ToString());
                view.ShowMatrixOfComplexes(schema.MatrixOfComplexes);
                view.ShowMatrixOfConnections(schema.MatrixOfConnections);
                view.ShowWeightedSchemaGraph(new WeightedSchemaGraph(schema));
                view.ShowElementsDistribution(schema.Elements.OrderBy(element => element.NodeId).ToList());
                view.ShowInternodeConnectionsCount(schema.InternodeConnectionsCount);
                view.StopAnimation();
            });
        }

        /// <summary>
        /// Обработчик события принятия результатов выполнения алгоритма компоновки.
        /// Обновляет отображение:
        /// 1) взвешенного графа схемы
        /// 2) распределения элементов по узлам,
        /// 3) количества межузловых соединений
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="schema">Схема соединений со скомпонованными элементами</param>
        private void LayoutResultsAccepted(object sender, Schema schema)
        {
            Requires.NotNull(schema, nameof(schema));

            if (_schema != null && _schema == schema)
                Task.Run(() =>
                {
                    view.StartAnimation("Обновление схемы соединений");
                    view.ShowWeightedSchemaGraph(new WeightedSchemaGraph(schema));
                    view.ShowElementsDistribution(schema.Elements.OrderBy(element => element.NodeId).ToList());
                    view.ShowInternodeConnectionsCount(schema.InternodeConnectionsCount);
                    view.StopAnimation();
                });
        }

        /// <summary>
        /// Обработчик запроса перехода к представлению загрузки схемы соединений
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void GoToLoadSchemaView(object sender, EventArgs e) =>
            controller.Run<LoadSchemaPresenter>();

        /// <summary>
        /// Обработчик события запроса перехода к представлению компоновки элементов схемы в узлы
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void GoToLayoutElementsView(object sender, EventArgs e)
        {
            if (_schema == null)
                view.ShowMessageDialog
                (
                    "Ошибка",
                    "Загрузите схему соединений для запуска алгоритма компоновки"
                );
            else
                controller.Run<LayoutElementsPresenter, Schema>(_schema);
        }

        /// <summary>
        /// Обработчик события запроса сохранения данных компоновки.
        /// Если схема загружена, записывает данные компоновки в указанный CSV-файл
        /// со столбцами "Элемент", "Номер узла"
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Параметры события</param>
        private void SaveElementsDistribution(object sender, SaveElementsDistributionEventArgs e)
        {
            Requires.NotNull(e, nameof(e));

            if (_schema == null)
                view.ShowMessageDialog
                (
                    "Ошибка",
                    "Сохранение данных компоновки невозможно: не загружена схема соединений"
                );
            else if (string.IsNullOrWhiteSpace(e.FilePath))
                view.ShowMessageDialog
                (
                    "Ошибка",
                    "Указан пустой путь к файлу для сохранения данных компоновки"
                );
            else
            {
                Task.Run(() =>
                {
                    view.StartAnimation("Сохранение данных компоновки");
                    try
                    {
                        using (var streamWriter = new StreamWriter(e.FilePath, false, Encoding.UTF8))
                        using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                        {

                            csvWriter.Configuration.Delimiter = ";";
                            csvWriter.Configuration.HasHeaderRecord = false;
                            csvWriter.WriteField("Элемент");
                            csvWriter.WriteField("Номер узла");
                            csvWriter.NextRecord();
                            csvWriter.WriteRecords(_schema.Elements.OrderBy(element => element.NodeId));
                        }
                        view.ShowMessageDialog
                        (
                            "Успех",
                            "Данные компоновки успешно записаны в CSV-файл"
                        );
                    }
                    catch
                    {
                        view.ShowMessageDialog
                        (
                            "Ошибка",
                            "В ходе сохранения данных компоновки возникла ошибка"
                        );
                    }
                    finally
                    {
                        view.StopAnimation();
                    }
                });
            }
        }
    }
}