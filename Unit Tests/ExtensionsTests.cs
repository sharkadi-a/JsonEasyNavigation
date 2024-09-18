using System.Text.Json;
using Shouldly;

namespace JsonEasyNavigation.Tests;

public class ExtensionsTests
{
    [Fact]
    public void UsingPath_ShouldFindProperty()
    {
        const string json = """{ "item1" : { "item2" : { "item3" : "test-string" } } }""";
            
        var jsonDocument = JsonDocument.Parse(json);
        var nav          = jsonDocument.RootElement.Property("item1", "item2", "item3");

        nav.Exist.ShouldBeTrue();
        nav.GetStringOrDefault().ShouldBe("test-string");
    }        
        
    [Fact]
    public void UsingPath_ShouldNotFindProperty()
    {
        const string json = """{ "item1" : { "item2" : { "item3" : "test-string" } } }""";
            
        var jsonDocument = JsonDocument.Parse(json);
        var nav          = jsonDocument.RootElement.Property("item1", "item2", "item3", "item4");

        nav.Exist.ShouldBeFalse();
    }
        
    [Fact]
    public void UsingPathAndGetProperty_ShouldFindProperty()
    {
        const string json = """{ "item1" : { "item2" : { "item3" : { "item4" : 100 } } } }""";

        var jsonDocument = JsonDocument.Parse(json);
        var nav          = jsonDocument.RootElement.Property("item1", "item2", "item3")["item4"];

        nav.Exist.ShouldBeTrue();
        nav.GetInt32OrDefault().ShouldBe(100);
    }

    [Fact]
    public void SelectFromManyJsonSources_SumShouldBeCorrect()
    {
        const string json1 = """{ "item3": 3, "item1": 10, "item4": 4}""";
        const string json2 = """{ "item3": 3, "item1": 15, "item4": 4}""";
        const string json3 = """{ "item3": 3, "item1": 99, "item4": 4}""";

        var jsonDocuments = new[]
        {
            JsonDocument.Parse(json1),
            JsonDocument.Parse(json2),
            JsonDocument.Parse(json3),
        };

        var sum = jsonDocuments
                  .NavigateSelect(x => x["item1"])
                  .Select(x => x.GetInt32OrDefault())
                  .Sum();

        sum.ShouldBe(10 + 15 + 99);
    }
}