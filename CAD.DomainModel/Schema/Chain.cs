using System.Collections.Generic;
using System.Linq;
using CodeContracts;

namespace CAD.DomainModel.Schema
{
    /// <summary>
    /// Цепь схемы соединений
    /// </summary>
    public class Chain
    {
        /// <summary>
        /// Наименование цепи
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Элементы цепи
        /// </summary>
        public IReadOnlyList<Element> Elements { get; }

        /// <summary>
        /// Создание экземпляра класса
        /// </summary>
        /// <param name="name">Наименование цепи</param>
        /// <param name="elements">Элементы цепи</param>
        public Chain(string name, IReadOnlyList<Element> elements)
        {
            Requires.NotNull(name, nameof(name));
            Requires.NullOrWithNoNullElements(elements, nameof(elements));
            Requires.True(!string.IsNullOrWhiteSpace(name), "Имя цепи не может быть пустым");
            Requires.True(elements.Count > 1, "Цепь должна соединять более одного элемента");
            Requires.True
            (
                elements.Distinct().Count() == elements.Count,
                "Список элементов цепи не должен содержать дубликатов"
            );

            Name = name;
            Elements = elements;
        }

        /// <summary>
        /// Получение текстового представления цепи
        /// </summary>
        /// <returns>Наименование цепи</returns>
        public override string ToString() =>
            Name;

        /// <summary>
        /// Вычисление хеш-кода цепи
        /// </summary>
        /// <returns>Хеш-код наименования цепи</returns>
        public override int GetHashCode() =>
            Name.GetHashCode();

        /// <summary>
        /// Сравнение текущего экземпляра класса с переданным объектом
        /// </summary>
        /// <param name="obj">Объект для сравнения</param>
        /// <returns>
        /// Возможны следующие ситуации:
        /// 1) если объект равен null, возвращается ложь
        /// 2) если объект представляет экземпляр класса Chain сравниваются наименования цепей:
        /// если они равны, возвращается истина, иначе - ложь
        /// 3) если объект не является экземляром класса Chain,
        /// для него вызывается метод Equals базового класса
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is Chain)
            {
                var chain = (Chain)obj;
                return chain.Name == this.Name;
            }
            return base.Equals(obj);
        }
    }
}