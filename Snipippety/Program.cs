using System;
using System.IO;
using System.Text;

namespace Snipippety
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Run(args);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                Environment.Exit(2);
            }
        }

        static void Run(string[] args)
        {
            if (args.Length != 3)
            {
                throw new ArgumentException($@"Please provide the following arguments when calling this script:

    snipippety <source-file> <snippet-dir> <destination-file>

e.g. like this:

    snipippety ./src/YourLittleBook.md ./src/examples ./temp/YourLittleBook.md
");
            }

            var sourceFilePath = args[0];
            var snippetDirectory = args[1];
            var destinationFilePath = args[2];

            Run(sourceFilePath, snippetDirectory, destinationFilePath);
        }

        static void Run(string sourceFilePath, string snippetDirectory, string destinationFilePath)
        {
            Console.Write($"{sourceFilePath} => {destinationFilePath} (snippets from {snippetDirectory})");

            try
            {
                var input = File.ReadAllText(sourceFilePath, Encoding.UTF8);
                var replacer = new Replacer();
                var output = replacer.Replace(input, new ReplacerContext(snippetDirectory));

                File.WriteAllText(destinationFilePath, output, Encoding.UTF8);

                Console.WriteLine("OK");
            }
            catch
            {
                Console.WriteLine("Not ok :(");
                throw;
            }
        }
    }
}
