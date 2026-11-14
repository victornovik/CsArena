using System.Net;

//var data = GetData();

//var beforeChange = TakeSnapshot(data);
//Change(data);
//var afterChange = TakeSnapshot(data);

//var mySportMessage = ToMySportAuditMessage(beforeChange, afterChange);
//var gosTechMessage = ToGosTechAuditMessage(beforeChange, afterChange);

var success = await new AuditHub().AuditAction([
    new MySportAuditMessage
    {
        PersonId = 1,
        Action = "Update",
        IsSuccess = true,
        IpAddress = IPAddress.Loopback,
        BeforeChangeState = "{}",
        AfterChangeState = "{}"
    },
    new GosTechAuditMessage { UserId = "user-1", Message = "Updated" }
]);

Console.WriteLine($"Has errors while writing audit: {!success}");

public interface IAuditable
{
    public Task<bool> DoAudit();
}

public class MySportAuditMessage : IAuditable
{
    public int PersonId { get; init; }
    public required string Action { get; init; }
    public bool IsSuccess { get; init; }
    public required IPAddress IpAddress { get; init; }
    public required string BeforeChangeState { get; init; }
    public required string AfterChangeState { get; init; }

    public Task<bool> DoAudit()
    {
        // convert to json
        // write to rabbit mq
        // return true/false if success
        return Task.FromResult(true);
    }
}

public class GosTechAuditMessage : IAuditable
{
    public required string UserId { get; init; }
    public required string Message { get; init; }

    public Task<bool> DoAudit()
    {
        // write to Kafka
        // return true/false if success
        return Task.FromResult(true);
    }
}

public class AuditHub
{
    public async Task<bool> AuditAction(IEnumerable<IAuditable> messages)
    {
        var results = await Task.WhenAll(messages.Select(msg => msg.DoAudit()));

        return results.All(ok => ok);
    }
}


//var reader = new CsvReader(@"data\countries.csv");
//var countries = reader.ReadCountriesInDictionary();

//foreach (var k in countries.Keys)
//{
//    Console.WriteLine($"{k}");
//    foreach (var c in countries[k])
//    {
//        Console.WriteLine($"{PopulationFormatter.FormatPopulation(c.Population),15}: {c.Name}");
//    }
//}


//var countries = reader.ReadCountries();

//var filteredCountriesMs = countries.Where(c => !c.Name.Contains(',')).OrderBy(c => c.Name);

//var filteredCountriesQs = 
//    from c in countries
//    where !c.Name.Contains(',')
//    orderby c.Name
//    select c;

//foreach (var c in filteredCountriesQs)
//{
//    Console.WriteLine($"{PopulationFormatter.FormatPopulation(c.Population),15}: {c.Name}");
//}


//var lilCountry = new Country("Lilliput", "LIL", "Nowhere", 2_000_000);
//var index = countries.FindIndex(c => c.Population < 2_000_000);
//countries.Insert(index, lilCountry);
//countries.RemoveAt(index);