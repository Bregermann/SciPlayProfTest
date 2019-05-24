using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class Generator
{
    private const int Population = 1_000_000;
    private const int YearStart = 1900;
    private const int YearEnd = 2000;
    private const string InputFilePath = "Assets/GeneratedData/Input.json";
    private const string OutputFilePath = "Assets/GeneratedData/Output.json";

    [MenuItem("Tools/Generate Input")]
    public static void GenerateInputData()
    {
        var people = new List<Person>();
        for (int i = 0; i < Population; i++)
        {
            var birthYear = Random.Range(YearStart, YearEnd);
            var endYear = Random.Range(birthYear, YearEnd + 1);
            people.Add(new Person(birthYear, endYear));
        }

        var json = JsonConvert.SerializeObject(people, Formatting.Indented);
        File.WriteAllText(InputFilePath, json);

        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/Generate Output")]
    public static void GenerateOutputData()
    {
        string json;
        try
        {
            json = File.ReadAllText(InputFilePath);
        }
        catch (FileNotFoundException)
        {
            GenerateInputData();
            json = File.ReadAllText(InputFilePath);
        }

        var populationByYear = new Dictionary<int, int>();
        for (int i = YearStart; i <= YearEnd; i++)
        {
            populationByYear.Add(i, 0);
        }

        var people = JsonConvert.DeserializeObject<List<Person>>(json);
        foreach (Person p in people)
        {
            for (int i = p.BirthYear; i <= p.EndYear; i++)
            {
                populationByYear[i]++;
            }
        }

        using (StreamWriter sw = File.CreateText(OutputFilePath))
        {
            foreach (int year in populationByYear.Keys)
            {
                sw.WriteLine($"{year} {populationByYear[year]}");
            }
            sw.WriteLine("");
            sw.WriteLine($"Max Population Year: {populationByYear.Aggregate((o, n) => o.Value > n.Value ? o : n).Key}");
            sw.WriteLine($"Min Population Year: {populationByYear.Aggregate((o, n) => o.Value > n.Value ? n : o).Key}");
            sw.WriteLine($"Average Population: {populationByYear.Values.Average()}");
            sw.WriteLine("");
            sw.WriteLine($"Max Life Expectancy: {people.Select(p => p.Age()).Max()}");
            sw.WriteLine($"Min Life Expectancy: {people.Select(p => p.Age()).Min()}");
            sw.WriteLine($"Average Life Expectancy: {people.Select(p => p.Age()).Average()}");
        }

        AssetDatabase.Refresh();
    }
}