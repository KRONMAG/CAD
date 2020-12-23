using CodeContracts;

namespace CAD.Presentation.Views.EventArgs
{
    /// <summary>
    /// Параметры события запроса загрузки схемы соединений из файла
    /// </summary>
    public class LoadSchemaEventArgs
    {
        /// <summary>
        /// Путь к файлу с описанием схемы
        /// </summary>
        public string SchemaFilePath { get; }

        /// <summary>
        /// Префикс элемента e0
        /// </summary>
        public string E0Prefix { get; }

        /// <summary>
        /// Создание экземпляра класса
        /// </summary>
        /// <param name="schemaFilePath">Путь к файлу с описанием схемы</param>
        /// <param name="e0Prefix">Префикс элемента e0</param>
        public LoadSchemaEventArgs(string schemaFilePath, string e0Prefix)
        {
            Requires.NotNull(schemaFilePath, nameof(schemaFilePath));
            Requires.NotNull(e0Prefix, nameof(e0Prefix));

            SchemaFilePath = schemaFilePath;
            E0Prefix = e0Prefix;
        }
    }
}