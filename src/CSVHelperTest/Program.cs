using Cirilla.Core.Helpers;
using Cirilla.Core.Models;
using CsvHelper;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CSVHelperTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string testFileName = "E:/Monster Hunter World MODs/WorldChunkTool-2020-07-09-G5/chunk_combined/common/text/vfont/skill_eng.gmd";
            GMD gmd = new GMD(testFileName);

            string csvFileName = "skill.csv";
            using (FileStream fs = new FileStream(csvFileName, FileMode.Create))
            using (TextWriter tw = new StreamWriter(fs, ExEncoding.UTF8))
            using (CsvWriter writer = new CsvWriter(tw, CultureInfo.InvariantCulture))
            {
                writer.Configuration.Delimiter = ";";

                foreach (var entry in gmd.Entries.OfType<GMD_Entry>())
                {
                    writer.WriteRecord(new StringKeyValuePair(entry.Key, entry.Value));
                    writer.NextRecord();
                }
            }
        }

        public class StringKeyValuePair
        {
            public string Key { get; }
            public string Value { get; }

            public StringKeyValuePair(string key, string value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}
