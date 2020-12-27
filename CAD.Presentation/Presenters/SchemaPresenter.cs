using System;
using System.Linq;
using CodeContracts;
using CAD.Presentation.Common;
using CAD.Presentation.Views;
using CAD.DomainModel.Schema;
using CAD.DomainModel.Graph;
using CAD.Presentation.Views.EventArgs;

namespace CAD.Presentation.Presenters
{
    /// <summary>
    /// Представитель представления отображения схемы соединений
    /// </summary>
    public class SchemaPresenter : BasePresenter<ISchemaView, Schema>
    {
        /// <summary>
        /// Создание экземпляра класса
        /// </summary>
        /// <param name="controller">Контроллер приложения</param>
        /// <param name="view">Представление</param>
        public SchemaPresenter(ApplicationController controller, ISchemaView view) :
            base(controller, view)
        {
            view.LayoutSchemaElements += ArrangeElements;
        }

        /// <summary>
        /// Обработчик события запроса компоновки элементов
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="args">Параметры события</param>
        private void ArrangeElements(object sender, LayoutSchemaElementEventArgs args)
        {
            Requires.NotNull(sender, nameof(sender));
            Requires.NotNull(args, nameof(args));

            if (args.NodesCount < 1)
                view.MessageDialog("Ошибка", "Количество узлов должно быть больше нуля");
            else if (args.NodesCount > parameter.Elements.Count)
                view.MessageDialog("Ошибка", "Количество узлов не может быть больше числа элементов");
            else
            {
                var random = new Random();
                var currentNodeId = 1;
                parameter.Elements
                    .OrderBy(element => random.Next())
                    .ToList()
                    .ForEach(element =>
                    {
                        element.NodeId = currentNodeId;
                        currentNodeId = currentNodeId == args.NodesCount ? 1 : currentNodeId += 1;
                    });
                view.ShowWeightedSchemaGraph(new WeightedSchemaGraph(parameter));
                view.ShowInternodeConnectionsCount(parameter.GetInternodeConnectionsCount());
            }
        }

        /// <summary>
        /// Показ представления, вызов методов представления для отображения схемы соединений
        /// </summary>
        /// <param name="schema">Схема соединений</param>
        public override void Run(Schema schema)
        {
            Requires.NotNull(schema, nameof(schema));

            base.Run(schema);

            view.StartAnimation("Отображение схемы соединений");

            Worker.Run
            (
                view,
                () =>
                {
                    view.ShowSchemaText(schema.ToString());
                    view.ShowMatrixOfComplexes(schema.MatrixOfComplexes);
                    view.ShowMatrixOfConnections(schema.MatrixOfConnections);
                    view.ShowWeightedSchemaGraph(new WeightedSchemaGraph(schema));
                    view.ShowInternodeConnectionsCount(schema.GetInternodeConnectionsCount());
                    return true;
                },
                _ =>
                {
                    view.StopAnimation();
                },
                _ =>
                {

                }
            );
        }
    }
}