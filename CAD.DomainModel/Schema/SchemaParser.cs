using System;
using System.Collections.Generic;
using System.Linq;
using CodeContracts;

namespace CAD.DomainModel.Schema
{
    /// <summary>
    /// Парсер схем соединений
    /// </summary>
    public class SchemaParser
    {
        /// <summary>
        /// Разбор текстового описания схемы соединений
        /// </summary>
        /// <param name="lines">Текстовое описание схемы</param>
        /// <param name="e0Prefix">Префикс элемента e0</param>
        /// <returns>Прочитанная схема соединений</returns>
        public Schema Parse(IReadOnlyList<string> lines, string e0Prefix)
        {
            Requires.NullOrWithNoNullElements(lines, nameof(lines));
            Requires.NotNull(e0Prefix, nameof(e0Prefix));
            Requires.True(lines.Count > 0, "Описание схемы не может быть пустым");
            Requires.True(!string.IsNullOrWhiteSpace(e0Prefix), "Префикс элемента e0 не может быть пустым");

            lines = lines
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Trim('\t', ' '))
                .ToList();

            if (lines.Any(line => line == "$NETS"))
                return ParseAllegro(lines, e0Prefix);
            return ParseCalay(lines, e0Prefix);
        }

        /// <summary>
        /// Парсинг схемы соединений, представленной в формате Allegro
        /// </summary>
        /// <param name="lines">Текстовое описание схемы</param>
        /// <param name="e0Prefix">Префикс элемента e0</param>
        /// <returns>Схема соединений</returns>
        private Schema ParseAllegro(IReadOnlyList<string> lines, string e0Prefix)
        {
            var chains = new List<Chain>();
            lines = lines
                .SkipWhile(line => line != "$NETS")
                .Where(line => line != "$NETS" && line != "$END")
                .ToList();
            var index = 0;
            while (index < lines.Count)
            {
                var line = lines[index];
                while (line.EndsWith(","))
                {
                    index++;
                    line = line + lines[index];
                }
                var items = line.Split
                (
                    separator: new[] { ';', ' ', '\t', ',' },
                    options: StringSplitOptions.RemoveEmptyEntries
                );
                if (!items.Skip(1).Any(item => item.StartsWith(e0Prefix)))
                {
                    var elements = items
                        .Skip(1)
                        .Select(element =>
                            new Element(new string(element.TakeWhile(symbol => symbol != '.').ToArray())))
                        .Distinct()
                        .ToList();
                    if (elements.Count > 1)
                        chains.Add(new Chain(items[0], elements));
                }
                index++;
            }

            return new Schema(chains);
        }

        /// <summary>
        /// Парсинг схемы соединений, представленной в формтае Calay
        /// </summary>
        /// <param name="lines">Текстовое описание схемы</param>
        /// <param name="e0Prefix">Префикс элемента e0</param>
        /// <returns>Схема соединений</returns>
        private Schema ParseCalay(IReadOnlyList<string> lines, string e0Prefix)
        {
            var chains = new List<Chain>();
            var index = 0;
            while (index < lines.Count)
            {
                var line = lines[index];
                while (!line.EndsWith(";"))
                {
                    index++;
                    line = line + lines[index];
                }
                var items = line.Split
                (
                    new[] { ' ', '\t', ';', ',' },
                    StringSplitOptions.RemoveEmptyEntries
                );
                if (!items.Skip(1).Any(item => item.StartsWith(e0Prefix)))
                {
                    var elements = items
                        .Skip(1)
                        .Select(item =>
                            new Element(new string(item.TakeWhile(symbol => symbol != '(').ToArray())))
                        .Distinct()
                        .ToList();
                    if (elements.Count > 1)
                        chains.Add(new Chain(items[0], elements));
                }
                index++;
            }
            return new Schema(chains);
        }
    }
}