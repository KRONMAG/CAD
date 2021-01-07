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

        /// <summary>
        /// Подписчики и подписки на события
        /// </summary>
        private readonly Dictionary<object, List<object>> _subscribers;

        /// <summary>
        /// Создания контроллера приложения: инициализация IoC-контейнера,
        /// инициализация словаря подписчиков и подписок на события
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

        /// <summary>
        /// Подписка объекта на событие
        /// </summary>
        /// <param name="subscriber">Объект, подписчик на событие</param>
        /// <param name="event">
        /// Событие, на которое необходимо подписаться, содержащее обоработчик
        /// </param>
        public void Subscribe<T, U>(object subscriber, T @event) where T: class, IEvent<U>
        {
            Requires.NotNull(subscriber, nameof(subscriber));
            Requires.NotNull(@event, nameof(@event));

            if (!_subscribers.ContainsKey(subscriber))
                _subscribers.Add(subscriber, new List<object>());

            _subscribers[subscriber].Add(@event);
        }

        /// <summary>
        /// Отписка объкта от всех событий
        /// </summary>
        /// <param name="subscriber">Объект</param>
        public void Unsubscribe(object subscriber)
        {
            Requires.NotNull(subscriber, nameof(subscriber));

            _subscribers.Remove(subscriber);
        }

        /// <summary>
        /// Генерация события заданного типа: вызов всех его обработчиков
        /// с передачей в них параметра события
        /// </summary>
        /// <typeparam name="T">Тип события</typeparam>
        /// <typeparam name="U">Тип параметров события</typeparam>
        /// <param name="sender">Источник события</param>
        /// <param name="args">Параметры события</param>
        public void RaiseEvent<T, U>(object sender, U args) where T: class, IEvent<U>
        {
            Requires.NotNull(sender, nameof(sender));

            _subscribers.Values
                .SelectMany(handlers => handlers)
                .OfType<T>()
                .ToList()
                .ForEach(handler => handler.Handler.BeginInvoke(sender, args, null, null));
        }
    }
}