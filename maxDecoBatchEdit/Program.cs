using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cirilla.Core.Helpers;
using Cirilla.Core.Models;
using CsvHelper;

namespace maxDecoBatchEdit
{
    class Program
    {
        static readonly string[] languageSuffix = {"ara","chS","chT","eng","fre",
                                            "ger","ita","jpn","kor","pol",
                                            "ptB","rus","spa"};
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to my first C# program!");
            Console.WriteLine("...");
            Console.WriteLine("...");
            Console.WriteLine("Today I will edit item_<language>.gmd with the patch csv file.");

            string itemFileName = args[0];
            FileAttributes attr = File.GetAttributes(itemFileName);
            if (attr.HasFlag(FileAttributes.Directory)) // if the path is a directory
            {
                foreach (string suffix in languageSuffix)
                {
                    string inGmdFileName = itemFileName + "/" + "item_" + suffix + ".gmd";
                    string inGmdFileNameBak = inGmdFileName + ".bak";
                    string csvFileName = itemFileName + "/" + "item_" + suffix + "_remain.csv";
                    if(!File.Exists(inGmdFileName) || !File.Exists(csvFileName))
                    {
                        Console.WriteLine("Skipping language " + suffix + " because its item.gmd or item_remain.csv does not exist.");
                        continue;
                    }

                    // make a back up first
                    Console.WriteLine("Backed up " + inGmdFileName + " to " + inGmdFileNameBak);
                    System.IO.File.Copy(inGmdFileName, inGmdFileNameBak, true);

                    // read the backup file and write it to the original file name
                    Console.WriteLine("Reading " + inGmdFileName);
                    ReadGmdAndApplyCsvAndSave(inGmdFileNameBak, csvFileName, inGmdFileName);
                    Console.WriteLine("Writing " + inGmdFileName);
                }
            }
            else
            {
                // TODO
                Console.WriteLine("The input argument must be a file folder.");
            }

            Console.WriteLine("Thank you for joining my TEDx Talks.");
        }

        static void ReadGmdAndApplyCsvAndSave(string inGmdFileName, string csvFileName, string outGmdFileName)
        {
            GMD gmdContent = new GMD(inGmdFileName);
            ApplyCsv(gmdContent, csvFileName);
            gmdContent.Save(outGmdFileName);
        }

        // Copied from Cirilla.ViewModels
        static void ApplyCsv(GMD gmdContent, string csvFileName)
        {

            List<StringKeyValuePair> values;

            using (FileStream fs = new FileStream(csvFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (TextReader tr = new StreamReader(fs, ExEncoding.UTF8))
            using (CsvReader csv = new CsvReader(tr))
            {
                csv.Configuration.HasHeaderRecord = false;
                csv.Configuration.Delimiter = ";";

                values = csv.GetRecords<StringKeyValuePair>().ToList();
            }

            foreach (StringKeyValuePair newEntry in values)
            {
                // If key matches, add
                GMD_Entry entry = gmdContent.Entries.OfType<GMD_Entry>().FirstOrDefault(x => x.Key == newEntry.Key);

                if (entry != null)
                    entry.Value = newEntry.Value;
            }
        }

        // Copied from Cirilla.Models.StringKeyValuePair
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
