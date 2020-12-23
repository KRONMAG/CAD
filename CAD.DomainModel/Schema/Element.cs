using CodeContracts;

namespace CAD.DomainModel.Schema
{
    /// <summary>
    /// Элемент схемы соединений
    /// </summary>
    public class Element
    {
        /// <summary>
        /// Номер узла, в котором расположен элемент
        /// </summary>
        private int _nodeId;

        /// <summary>
        /// Наименование элемента
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Номер узла, в котором размещен элемент
        /// </summary>
        public int NodeId
        {
            get => _nodeId;
            set
            {
                Requires.InRange
                (
                    value > 0,
                    nameof(value),
                    "Номер узла должен быть положительным числом"
                );

                _nodeId = value;
            }
        }

        /// <summary>
        /// Создание экземпляра класса
        /// </summary>
        /// <param name="name">Наименование элемента</param>
        public Element(string name)
        {
            Requires.NotNull(name, nameof(name));
            Requires.NotNullOrEmpty(name, nameof(name), "Имя элемента не может быть пустым");

            Name = name;
            _nodeId = 1;
        }

        /// <summary>
        /// Получение строкового представления элемента
        /// </summary>
        /// <returns>Наименование элемента</returns>
        public override string ToString() =>
            Name;

        /// <summary>
        /// Вычисление хеш-кода элемента
        /// </summary>
        /// <returns>Хеш-код наименования элемента</returns>
        public override int GetHashCode() =>
            Name.GetHashCode();

        /// <summary>
        /// Сравнение текущего экземпляра класса с переданным объектом
        /// </summary>
        /// <param name="obj">Объект для сравнения</param>
        /// <returns>
        /// Возможны следующие ситуации:
        /// 1) если объект равен null, возвращается ложь
        /// 2) если объект представляет экземпляр класса Element сравниваются наименования элементов:
        /// если они равны, возвращается истина, иначе - ложь
        /// 3) если объект не является экземляром класса Element,
        /// для него вызывается метод Equals базового класса
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is Element)
            {
                var element = (Element)obj;
                return element.Name == this.Name;
            }
            return base.Equals(obj);
        }
    }
}