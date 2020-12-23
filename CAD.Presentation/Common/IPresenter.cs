namespace CAD.Presentation.Common
{
    /// <summary>
    /// Описание функционала представителя
    /// </summary>
    public interface IPresenter
    {
        /// <summary>
        /// Запуск представителя
        /// </summary>
        void Run();
    }

    /// <summary>
    /// Описание функционала параметризованного представителя
    /// </summary>
    /// <typeparam name="T">Тип параметра представителя</typeparam>
    public interface IPresenter<T>
    {
        /// <summary>
        /// Запуск представителя с параметром
        /// </summary>
        /// <param name="parameter">Параметр, передаваемый представителю</param>
        void Run(T parameter);
    }
}