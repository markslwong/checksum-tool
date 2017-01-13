using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;


namespace ChecksumTool
{
    public static class ChecksumLogic
    {
        public static void Run(DirectoryInfo targetDirectory, string outputFile, string outputNamespace, string outputClass)
        {
            var files = ListFilesRecusive(targetDirectory);
            var hashes = new List<string>();

            using (var algorithm = SHA256.Create())
            {
                foreach (var file in files)
                {
                    var bytes = File.ReadAllBytes(Path.Combine(targetDirectory.FullName, file));

                    var hash = algorithm.ComputeHash(bytes);

                    hashes.Add(Convert.ToBase64String(hash));
                }
            }

            using (var fileStream = File.OpenWrite(outputFile))
            using (var textWriter = new StreamWriter(fileStream))
            {
                textWriter.WriteLine("using System;");
                textWriter.WriteLine("using System.Collections.Generic;");
                textWriter.WriteLine();
                textWriter.WriteLine();
                textWriter.WriteLine("namespace {0}", outputNamespace);
                textWriter.WriteLine("{");
                textWriter.WriteLine("    public static class {0}", outputClass);
                textWriter.WriteLine("    {");
                textWriter.WriteLine("        public static readonly IDictionary<string, string> Hashes = new Dictionary<string, string>");
                textWriter.WriteLine("        {");

                for (var i = 0; i < files.Count(); ++i)
                {
                    var file = files[i];
                    var hash = hashes[i];

                    textWriter.WriteLine("            {{ @\"{0}\",", file);
                    textWriter.WriteLine("                @\"{0}\" }},", hash);
                }

                textWriter.WriteLine("        };");
                textWriter.WriteLine("    }");
                textWriter.WriteLine("}");

                textWriter.Close();
                fileStream.Close();

                Console.WriteLine("Checksum Complete");
            }
        }

        private static IList<string> ListFilesRecusive(DirectoryInfo directory)
        {
            var results = new List<string>();

            foreach (var file in directory.GetFiles())
            {
                results.Add(file.Name);
            }

            foreach (var subDirectory in directory.GetDirectories())
            {
                var subFiles = ListFilesRecusive(subDirectory);

                foreach (var subFile in subFiles)
                {
                    results.Add(Path.Combine(subDirectory.Name, subFile));
                }
            }

            return results;
        }
    }
}
