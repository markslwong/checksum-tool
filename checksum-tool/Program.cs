using System;
using System.IO;
using System.Linq;


namespace ChecksumTool
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Run(args);

            Console.ReadKey();
        }

        private static void Run(string[] args)
        {
            if (args.Length != 4)
            {
                ArgumentError("Invalid number of arguments.");
                return;
            }

            var targetDirectory = args[0];
            var outputFile      = args[1];
            var outputNamespace = args[2];
            var outputClass     = args[3];

            if (!Directory.Exists(targetDirectory))
            {
                ArgumentError("Target directory does not exist.");
                return;
            }

            if (outputNamespace.Any(char.IsWhiteSpace))
            {
                ArgumentError("Output namespace cannot have any whitespace characters.");
                return;
            }

            if (outputClass.Any(char.IsWhiteSpace))
            {
                ArgumentError("Output class cannot have any whitespace characters.");
                return;
            }
            
            var outputNamespaceSegments = outputNamespace.Split('.');

            if (outputNamespaceSegments.Any(x => x.Length == 0 || !char.IsLetter(x[0]) || !x.All(char.IsLetterOrDigit)))
            {
                ArgumentError("Invalid namespace name (must start with letter, and only contain alpha-numeric characters.");
                return;
            }

            if (outputClass.Length == 0 || !char.IsLetter(outputClass[1]) || !outputClass.All(char.IsLetterOrDigit))
            {
                ArgumentError("Invalid class name (must start with letter, and only contain alpha-numeric characters.");
                return;
            }
            
            try
            {
                ChecksumLogic.Run(new DirectoryInfo(targetDirectory), outputFile, outputNamespace, outputClass);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown error: " + e);
            }
        }

        private static void ArgumentError(string description)
        {
            Console.WriteLine("ChecksumTool.exe [target-directory] [output-file] [output-namespace] [output-class]");
            Console.WriteLine();
            Console.WriteLine(description);
        }
    }
}
