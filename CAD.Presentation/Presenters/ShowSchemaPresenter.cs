using System;
using System.Threading.Tasks;
using CodeContracts;
using CsvHelper;
using CAD.Presentation.Common;
using CAD.Presentation.Views;
using CAD.DomainModel.Schema;
using CAD.DomainModel.Graph;
using CAD.Presentation.Presenters.Events;
using CAD.Presentation.Views.EventArgs;
using System.IO;
using System.Text;
using System.Globalization;

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
        /// <param name="view">Представление</param>
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
        /// 6) количество межзловых соединений
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
                view.ShowElementsDistribution(schema.ElementsDistribution);
                view.ShowInternodeConnectionsCount(schema.InternodeConnectionsCount);
                view.StopAnimation();
            });
        }

        /// <summary>
        /// Обработчик события принятия результатов выполнения алгоритма компоновки
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="schema">Схема соединений со скомпонованными элементами</param>
        private void LayoutResultsAccepted(object sender, Schema schema)
        {
            Requires.NotNull(sender, nameof(sender));
            Requires.NotNull(schema, nameof(schema));

            if (_schema != null && _schema == schema)
                Task.Run(() =>
                {
                    view.StartAnimation("Обновление схемы соединений");
                    view.ShowWeightedSchemaGraph(new WeightedSchemaGraph(schema));
                    view.ShowElementsDistribution(schema.ElementsDistribution);
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

        private void SaveElementsDistribution(object sender, SaveElementsDistributionEventArgs args)
        {

        }
    }
}