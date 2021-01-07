using CodeContracts;

namespace CAD.Presentation.Views.EventArgs
{
    /// <summary>
    /// Событие запроса сохранения данных распределения элементов по узлам
    /// </summary>
    public class SaveElementsDistributionEventArgs
    {
        /// <summary>
        /// Путь к файлу, в который необходимо записать данные
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Создание экземпляра класса
        /// </summary>
        /// <param name="filePath">Путь к файлу, в который необходимо записать данные</param>
        public SaveElementsDistributionEventArgs(string filePath)
        {
            Requires.NotNull(filePath, nameof(filePath));

            FilePath = filePath;
        }
    }
}