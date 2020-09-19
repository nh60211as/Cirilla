using System;
using System.Linq;
using System.IO;
using Cirilla.Core.Helpers;
using Cirilla.Core.Models;
using CsvHelper;

namespace GMDDiff
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to my second C# program!");

            string fileOrig = args[0];
            string fileCmp = args[1];

            GMD GMDOrig = new GMD(fileOrig);
            GMD GMDCmp = new GMD(fileCmp);

            Console.WriteLine("Verifying GMD content...");
            // check if all the keys in fileCmp is the same as fileOrig
            foreach (GMD_Entry entryCmp in GMDCmp.Entries.OfType<GMD_Entry>())
            {
                // there will be a faster way to compare
                // but I don't know how to write C# code so this should do
                GMD_Entry entryOrig = GMDOrig.Entries.OfType<GMD_Entry>().FirstOrDefault(x => x.Key == entryCmp.Key);

                if (entryOrig is null)
                {
                    Console.WriteLine("Key entry doesn't match. Abort.");
                    return;
                }
            }
            Console.WriteLine("Verification done.");


            string csvFileName = args[2];
            FileStream fs = new FileStream(csvFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            TextWriter tw = new StreamWriter(fs, ExEncoding.UTF8);
            CsvWriter csvWriter = new CsvWriter(tw);
            csvWriter = new CsvWriter(tw);
            csvWriter.Configuration.HasHeaderRecord = false;
            csvWriter.Configuration.Delimiter = ";";

            Console.WriteLine("Writing difference to " + csvFileName);
            foreach (GMD_Entry entryCmp in GMDCmp.Entries.OfType<GMD_Entry>())
            {
                GMD_Entry entryOrig = GMDOrig.Entries.OfType<GMD_Entry>().FirstOrDefault(x => x.Key == entryCmp.Key);

                if (entryCmp.Value != entryOrig.Value)
                {
                    csvWriter.WriteRecord(entryCmp);
                    csvWriter.NextRecord();
                }
            }
            Console.WriteLine("Now leave.");
        }
    }
}
