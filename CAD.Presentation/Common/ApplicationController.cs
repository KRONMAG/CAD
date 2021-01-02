using System.Collections.Generic;
using System.Linq;
using CodeContracts;
using Unity;

namespace CAD.Presentation.Common
{
    /// <summary>
    /// Контроллер приложения, представляющий централизованный контроль
    /// над представлениями, представителями и их зависимостями
    /// </summary>
    public class ApplicationController
    {
        /// <summary>
        /// Контейнер инверсии управления
        /// </summary>
        private readonly UnityContainer _container;

        private readonly Dictionary<object, List<object>> _subscribers;

        /// <summary>
        /// Создания контроллера приложения: инициализация IoC-контейнера
        /// </summary>
        public ApplicationController()
        {
            _container = new UnityContainer();
            _container.RegisterInstance(this);
            _subscribers = new Dictionary<object, List<object>>();
        }

        /// <summary>
        /// Регистрация зависимости в форме одиночки
        /// </summary>
        /// <typeparam name="T">Класс зависимости</typeparam>
        /// <returns>Контроллер приложения</returns>
        public ApplicationController RegisterSingleton<T>() where T : class
        {
            _container.RegisterSingleton<T>();
            return this;
        }

        /// <summary>
        /// Регистрация зависимости в форме одиночки для интерфейса и класса его реализующего
        /// или базового класса и класса-наследника
        /// </summary>
        /// <typeparam name="T">Интерфейс (родительский класс)</typeparam>
        /// <typeparam name="U">Класс, реализующий интерфейс (класс-наследник)</typeparam>
        /// <returns>Контроллер приложения</returns>
        public ApplicationController RegisterSingleton<T, U>() where U : class, T
        {
            _container.RegisterSingleton<T, U>();
            return this;
        }

        /// <summary>
        /// Регистрация зависимости класса
        /// </summary>
        /// <typeparam name="T">Класс зависимости</typeparam>
        /// <returns>Контроллер приложения</returns>
        public ApplicationController Register<T>() where T : class
        {
            _container.RegisterType<T>();
            return this;
        }

        /// <summary>
        /// Регистрация зависимости в виде интерфейса и реализующего его класса или 
        /// базового класса и класса-наследника
        /// </summary>
        /// <typeparam name="T">Интерфейс (родительский класс)</typeparam>
        /// <typeparam name="U">Класс, реализующий интерфейс (класс-потомок)</typeparam>
        /// <returns>Контроллер приложения</returns>
        public ApplicationController Register<T, U>() where U : class, T
        {
            _container.RegisterType<T, U>();
            return this;
        }

        /// <summary>
        /// Регистрация зависимости представителя с созданием
        /// его экземпляра и вызовом метода запуска
        /// </summary>
        /// <typeparam name="T">Тип представителя</typeparam>
        public void Run<T>() where T : class, IPresenter
        {
            if (!_container.IsRegistered<T>())
                _container.RegisterType<T>();
            _container.Resolve<T>().Run();
        }

        /// <summary>
        /// Регистрация зависимости параметризованного представителя
        /// с созданием  его экземпляра и вызовом процедуры его запуска
        /// </summary>
        /// <typeparam name="T">Тип представителя</typeparam>
        /// <typeparam name="U">Тип параметра представителя</typeparam>
        /// <param name="parameter">Параметр представителя</param>
        public void Run<T, U>(U parameter) where T : class, IPresenter<U>
        {
            if (!_container.IsRegistered<T>())
                _container.RegisterType<T>();
            _container.Resolve<T>().Run(parameter);
        }

        #region EventBus

        private void Subscribe(object subscriber, object @event)
        {
            if (!_subscribers.ContainsKey(subscriber))
                _subscribers.Add(subscriber, new List<object>());

            _subscribers[subscriber].Add(@event);
        }

        public void Subscribe<T, U>(IPresenter subscriber, T @event)
            where T : class, IPresenterEvent<U>
        {
            Requires.NotNull(subscriber, nameof(subscriber));
            Requires.NotNull(@event, nameof(@event));

            Subscribe(subscriber, @event);
        }

        public void Subscribe<T, U, V>(IPresenter<V> subscriber, T @event)
            where T : class, IPresenterEvent<U>
        {
            Requires.NotNull(subscriber, nameof(subscriber));
            Requires.NotNull(@event, nameof(@event));

            Subscribe(subscriber, @event);
        }

        public void Unsubscribe(IPresenter subscriber)
        {
            Requires.NotNull(subscriber, nameof(subscriber));

            _subscribers.Remove(subscriber);
        }

        public void Unsubscribe<T>(IPresenter<T> subscriber)
        {
            Requires.NotNull(subscriber, nameof(subscriber));

            _subscribers.Remove(subscriber);
        }

        private void RaiseEvent<T, U>(object sender, U args)
            where T: class, IPresenterEvent<U> =>
            _subscribers.Values
                .SelectMany(handlers => handlers)
                .OfType<T>()
                .ToList()
                .ForEach(handler => handler.Handler.BeginInvoke(sender, args, null, null));

        public void RaiseEvent<T, U>(IPresenter sender, U args)
            where T: class, IPresenterEvent<U>
        {
            Requires.NotNull(sender, nameof(sender));

            RaiseEvent<T, U>((object)sender, args);
        }

        public void RaiseEvent<T, U, V>(IPresenter<V> sender, U args)
            where T: class, IPresenterEvent<U>
        {
            Requires.NotNull(sender, nameof(sender));

            RaiseEvent<T, U>(sender, args);
        }

        #endregion
    }
}