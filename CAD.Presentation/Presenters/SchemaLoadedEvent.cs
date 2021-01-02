using System;
using CAD.DomainModel.Schema;
using CAD.Presentation.Common;
using CodeContracts;

namespace CAD.Presentation.Presenters
{
    public class SchemaLoadedEvent : IPresenterEvent<Schema>
    {
        public EventHandler<Schema> Handler { get; }

        public SchemaLoadedEvent(EventHandler<Schema> handler)
        {
            Requires.NotNull(handler, nameof(handler));

            Handler = handler;
        }
    }
}