using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TukanoTask {
    class Program {
        static void Main(string[] args) {
            List<string> przelewy = new List<string>();
            GetDataFromInputFile("input.txt", ref przelewy);
            Result(ref przelewy);
        }

        static void Result(ref List<string> transfers) {
            List<string> slownikImion = new List<string>();
            List<string> result = new List<string>();

            GetNames("wykaz_imion_meskich.csv", ref slownikImion);
            GetNames("wykaz_imion_zenskich.csv", ref slownikImion);

            foreach (var record in transfers) {
                string[] recordItems = record.Split(' ', ',', '.');

                string imiona = "";
                string nazwisko = "";
                string adres = "";

                foreach (var word in recordItems) {
                    string imie = slownikImion.Where(q => q.ToUpper() == (word.ToUpper())).FirstOrDefault();
                    if (!string.IsNullOrEmpty(imie))
                        if (string.IsNullOrEmpty(adres))
                            imiona += word + ' ';
                        else
                            adres += word + ' ';
                    else if (string.IsNullOrEmpty(nazwisko))
                        nazwisko = word;
                    else if (Regex.IsMatch(word, "^[0-9]{2}-[0-9]{3}$"))
                        adres += word + ' ';
                    else if (word.Any(char.IsDigit)) {
                        adres += word;
                        break;
                    }
                    else if (string.IsNullOrEmpty(word))
                        continue;
                    else if (word.All(char.IsLetterOrDigit))
                        adres += word + ' ';
                }

                if (!string.IsNullOrEmpty(imiona) && !string.IsNullOrEmpty(nazwisko) && !string.IsNullOrEmpty(adres)) {
                    string resultRow = $"IMIONA: {imiona}NAZWISKO: {nazwisko} ADRES: {adres}";
                    result.Add(resultRow);
                    Console.WriteLine(resultRow);
                }
            }

            System.IO.File.WriteAllLines("result.txt", result);
            //SaveDataToFile("result.txt", ref result);
        }

        static void GetDataFromInputFile(string fileName, ref List<string> transfers) {
            using (var streamReader = new StreamReader(fileName)) {
                while (!streamReader.EndOfStream) {
                    transfers.Add(streamReader.ReadLine());
                }
            }
        }

        static void GetNames(string fileName, ref List<string> names) {
            using (var streamReader = new StreamReader(fileName)) {
                while (!streamReader.EndOfStream) {
                    names.Add(streamReader.ReadLine().Split(',')[0]);
                }
            }
        }

        static void SaveDataToFile(string fileName, ref List<string> result) {
            using (TextWriter tw = new StreamWriter(fileName)) {
                foreach (string row in result) {
                    tw.WriteLine(row);
                }
            }
        }

    }
}

