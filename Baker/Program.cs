using System.Text;

namespace blah
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string file = args[0];

            string[] lines = File.ReadAllLines(file);

            string[] labels = lines[0].Split(',');

            StringBuilder output = new StringBuilder();

            output.Append("{");
            output.Append("\n  \"Buildings\":[");

            bool firstPart = true;
            bool firstConverter = true;
            for (int i = 1; i < lines.Length; i++)
            {
                string[] values = lines[i].Split(",");

                if (values[0] != "")
                {
                    if (!firstPart)
                    {
                        output.Append("\n      ]");
                        output.Append("\n    },");
                    }
                    firstPart = false;
                    firstConverter = true;

                    output.Append("\n    {");
                    output.Append($"\n      \"Name\":\"{values[0]}\",");
                    output.Append("\n      \"Conversions\":[");
                    continue;
                }

                if (!firstConverter)
                {
                    output.Append(",");
                }
                firstConverter = false;

                output.Append($"\n        {{\n          \"Name\":\"{values[1]}\",\n          \"Amounts\":{{{string.Join(",", values.Select((s, i) => $"\n            \"{labels[i]}\":{s}").Skip(2).Where(s => !s.EndsWith(":")))}\n          }}\n        }}");
            }

            if (!firstPart)
            {
                output.Append("\n      ]");
                output.Append("\n    }");
            }
            output.Append("\n  ]");
            output.Append("\n}");

            File.WriteAllText(Path.ChangeExtension(file, ".json"), output.ToString());
        }
    }
}
