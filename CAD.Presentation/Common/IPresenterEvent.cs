using System;

namespace CAD.Presentation.Common
{
    public interface IPresenterEvent<T>
    {
        EventHandler<T> Handler { get; }
    }
}