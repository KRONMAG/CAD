using System;
using CAD.Presentation.Common;
using CAD.Presentation.Views;
using CAD.DomainModel.Schema;
using CAD.DomainModel.Graph;

namespace CAD.Presentation.Presenters
{
    /// <summary>
    /// Представитель представления отображения схемы соединений
    /// </summary>
    public class ShowSchemaPresenter : BasePresenter<IShowSchemaView>
    {
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
            controller.Subscribe<SchemaLoadedEvent, Schema>
            (
                this,
                new SchemaLoadedEvent(SchemaLoaded)
            );
        }

        private void SchemaLoaded(object sender, Schema schema)
        {
            _schema = schema;
            view.StartAnimation("Отображение схемы соединений");
            view.ShowSchemaText(schema.ToString());
            view.ShowMatrixOfComplexes(schema.MatrixOfComplexes);
            view.ShowMatrixOfConnections(schema.MatrixOfConnections);
            view.ShowWeightedSchemaGraph(new WeightedSchemaGraph(schema));
            view.ShowElementsDistribution(schema.ElementsDistribution);
            view.ShowInternodeConnectionsCount(schema.InternodeConnectionsCount);
            view.StopAnimation();
        }

        private void GoToLoadSchemaView(object sender, EventArgs e) =>
            controller.Run<LoadSchemaPresenter>();

        private void GoToLayoutElementsView(object sender, EventArgs e)
        {
            if (_schema == null)
                view.MessageDialog
                (
                    "Ошибка",
                    "Компоновка элементов в узлы невозможна: не загружена схема соединений"
                );
            else
                controller.Run<LayoutAlgoritmPresenter, Schema>(_schema);
        }
    }
}