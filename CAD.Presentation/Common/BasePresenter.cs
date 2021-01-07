using CodeContracts;

namespace CAD.Presentation.Common
{
    /// <summary>
    /// Базовый класс представителя
    /// </summary>
    /// <typeparam name="T">Интерфейс представления</typeparam>
    public abstract class BasePresenter<T> : IPresenter where T: class, IView
    {
        /// <summary>
        /// Контроллер приложения
        /// </summary>
        protected readonly ApplicationController controller;

        /// <summary>
        /// Представление
        /// </summary>
        protected readonly T view;

        /// <summary>
        /// Создание представителя
        /// </summary>
        /// <param name="controller">Контроллер приложения</param>
        /// <param name="view">Представление</param>
        public BasePresenter(ApplicationController controller, T view)
        {
            Requires.NotNull(controller, nameof(controller));
            Requires.NotNull(view, nameof(view));

            this.controller = controller;
            this.view = view;
        }

        /// <summary>
        /// Запуск представителя: показ представления
        /// </summary>
        public virtual void Run() =>
            view.Show();

        /// <summary>
        /// Отписка представителя от всех событий
        /// </summary>
        ~BasePresenter() =>
            controller.Unsubscribe(this);
    }

    /// <summary>
    /// Базовый класс параметризованного представителя
    /// </summary>
    /// <typeparam name="T">Интерфейс представления</typeparam>
    /// <typeparam name="U">Тип параметра, передающегося представителю при его запуске</typeparam>
    public abstract class BasePresenter<T, U>: IPresenter<U> where T: class, IView
    {
        /// <summary>
        /// Контроллер приложения
        /// </summary>
        protected readonly ApplicationController controller;

        /// <summary>
        /// Представление
        /// </summary>
        protected readonly T view;

        /// <summary>
        /// Параметр представителя
        /// </summary>
        protected U parameter;

        /// <summary>
        /// Создание параметризованного представителя
        /// </summary>
        /// <param name="controller">Контроллер приложения</param>
        /// <param name="view">Представление</param>
        public BasePresenter(ApplicationController controller, T view)
        {
            Requires.NotNull(controller, nameof(controller));
            Requires.NotNull(view, nameof(view));

            this.controller = controller;
            this.view = view;
        }

        /// <summary>
        /// Запуск представителя с параметром: показ представления
        /// </summary>
        /// <param name="parameter">Параметр</param>
        public virtual void Run(U parameter)
        {
            this.parameter = parameter;
            view.Show();
        }

        /// <summary>
        /// Отписка представителя от всех событий
        /// </summary>
        ~BasePresenter() =>
            controller.Unsubscribe(this);
    }
}