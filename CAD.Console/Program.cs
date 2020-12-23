using System.Linq;
using System.Collections.Concurrent;
using System.IO;
using CAD.DomainModel.LayoutAlgorithm;
using CAD.DomainModel.Schema;
using System.Diagnostics;
using System.Text;

namespace CAD.Console
{
    public class Program
    {
        public static void Main()
        {
            var parser = new SchemaParser();
            var schema = parser.Parse(File.ReadAllLines(@"C:\Users\vovan\source\repos\CAD\Schemas\allegro_2.net"), "SNP");
            var algorithm = new GeneticAlgorithm();

            var args = new GeneticAlgorithmArgs
            (
                schema: schema,
                nodesCount: 2,
                generationsCount: 100000,
                populationSize: 50,
                parentSelection: ParentSelectionType.Outbreeding,
                selection: SelectionType.Elitism
            );

            algorithm.GenerationCompleted += (s, e) =>
                System.Console.WriteLine($"{e.GenerationNumber} {e.MinConnectionsCount}"); ;
            algorithm.Run(args);

            System.Console.ReadLine();
        }

        public static void WriteGeneticAlgorithmStatistics(string schemaFilePath, int nodesCount, int populationSize, int generationsCount)
        {
            var parser = new SchemaParser();
            var schema = parser.Parse(File.ReadAllLines(schemaFilePath), "SNP");
            var parentSelections = new[]
            {
                ParentSelectionType.Inbreeding,
                ParentSelectionType.Outbreeding,
                ParentSelectionType.Panmixia
            };
            var selections = new[]
            {
                SelectionType.Elitism,
                SelectionType.Tournament
            };

            var results = new ConcurrentBag<(ParentSelectionType, SelectionType, GeneticAlgorithmResult)>();

            foreach (var parentSelection in parentSelections)
                foreach (var selection in selections)
                {
                    var algorithm = new GeneticAlgorithm();
                    algorithm.GenerationCompleted += (_, result) =>
                        results.Add((parentSelection, selection, result));

                    var args = new GeneticAlgorithmArgs
                    (
                        schema: schema,
                        nodesCount: nodesCount,
                        generationsCount: generationsCount,
                        populationSize: populationSize,
                        parentSelection,
                        selection
                    );

                    for (var i = 1; i < 11; i++)
                    {
                        System.Console.WriteLine($"{parentSelection} {selection} {i}");
                        algorithm.Run(args);
                    }
                }

            var outputFile = "out.csv";
            var separator = ";";

            var header = string.Join
            (
                separator,
                new[]
                {
                    "Операторы",
                    "Номер поколения",
                    "Приспособленность наилучшей особи",
                    "Средняя приспособленность популяции",
                    "Приспособленность наихудшей особи"
                }
            );

            File.WriteAllText(outputFile, header);

            var content = results
                .GroupBy(result => (result.Item1, result.Item2, result.Item3.GenerationNumber))
                .Select
                (
                    group =>
                    (
                        group.Key.Item1,
                        group.Key.Item2,
                        group.Key.Item3,
                        group.Average(item => item.Item3.MinConnectionsCount),
                        group.Average(item => item.Item3.AverageConnectionsCount),
                        group.Average(item => item.Item3.MaxConnectionsCount)
                   )
                )
                .Select(result =>
                {
                    var parentSelectionName = result.Item1 switch
                    {
                        ParentSelectionType.Inbreeding => "Инбридинг",
                        ParentSelectionType.Outbreeding => "Аутбридинг",
                        ParentSelectionType.Panmixia => "Панмиксия"
                    };

                    var selectionName = result.Item2 switch
                    {
                        SelectionType.Elitism => "элитарная селекция",
                        SelectionType.Tournament => "турнирная селекция"
                    };

                    return string.Join
                    (
                        separator,
                        new[]
                        {
                            $"{parentSelectionName}, {selectionName}",
                            result.Item3.ToString(),
                            result.Item4.ToString(),
                            result.Item5.ToString(),
                            result.Item6.ToString()
                        }
                    );
                });

            File.AppendAllLines($"out {new FileInfo(schemaFilePath).Name}.csv", content);
        }

        public static void WriteTimeStatistics()
        {
            var parser = new SchemaParser();
            var schema = parser.Parse(File.ReadAllLines(@"C:\Users\vovan\source\repos\CAD\Schemas\shema1_cl90.net"), "SNP");

            var results = new ConcurrentBag<(int, double, double)>();

            for (var i = 10; i <= 250; i += 10)
            {
                System.Console.WriteLine($"{i}");

                var algorithm = new GeneticAlgorithm();

                var args = new GeneticAlgorithmArgs
                (
                    schema: schema,
                    nodesCount: 5,
                    generationsCount: 100,
                    populationSize: i,
                    parentSelection: ParentSelectionType.Outbreeding,
                    selection: SelectionType.Elitism
                );

                var seconds = 0d;
                var bestResult = 0d;

                for (var j = 1; j < 11; j++)
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var result = algorithm.Run(args);
                    stopwatch.Stop();
                    seconds += stopwatch.ElapsedMilliseconds;
                    bestResult += result.MinConnectionsCount;
                }

                seconds = seconds / 10;
                bestResult = bestResult / 10;

                results.Add((i, seconds, bestResult));
            }

            var outputFile = "out.csv";
            var separator = ";";
            var encoding = Encoding.UTF8;

            var header = string.Join
            (
                separator,
                new[]
                {
                    "Количество особей",
                    "Затраченное время (мс)",
                    "Приспособленность наилучшей особи\n",
                }
            );

            File.WriteAllText(outputFile, header, encoding);

            var content = results
                .Select(result => $"{result.Item1};{result.Item2};{result.Item3}");

            File.AppendAllLines(outputFile, content, encoding);
        }
    }
}