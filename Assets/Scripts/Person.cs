using Newtonsoft.Json;

public struct Person
{
    [JsonProperty]
    public int BirthYear { get; set; }

    [JsonProperty]
    public int EndYear { get; set; }

    [JsonConstructor]
    public Person(int birthYear, int endYear)
    {
        BirthYear = birthYear;
        EndYear = endYear;
    }

    public int Age()
    {
        return EndYear - BirthYear;
    }
}