using System.Text.Json;
using Shouldly;

namespace JsonEasyNavigation.Tests;

public class PropertyTests
{
    private const string Json  = """{ "item1" : "first", "item2": "second" }""";
    private const string Json2 = """{ "item1" : "first", "item2": "second", "item3": "third" }""";
    private static readonly string[] Expected = ["item1", "item2"];

    [Fact]
    public void JsonElementKind_ShouldBeObject()
    {
        var jsonDocument = JsonDocument.Parse(Json);
        var nav          = jsonDocument.ToNavigation();

        nav.JsonElement.ValueKind.ShouldBe(JsonValueKind.Object);
    }
        
    [Fact]
    public void WhenInvalidElementKind_ShouldFail()
    {
        const string json = """{ "item1" : "first" }""";

        var jsonDocument = JsonDocument.Parse(json);
        var nav          = jsonDocument.ToNavigation();

        var item = nav["item1"];
        item.Values.ShouldBeEmpty();
    }

    [Fact]
    public void WhenPropertyExist_ValueShouldNotBeNull()
    {
        const string json = """{ "item1" : "value" }""";

        var jsonDocument = JsonDocument.Parse(json);
        var nav          = jsonDocument.ToNavigation();

        var item = nav["item1"];
        item.Exist.ShouldBeTrue();
        item.IsNullValue.ShouldBeFalse();
    }
        
    [Fact]
    public void WhenPropertyIsNull_ShouldSucceed()
    {
        const string json = """{ "item1" : null }""";

        var jsonDocument = JsonDocument.Parse(json);
        var nav          = jsonDocument.ToNavigation();

        var item = nav["item1"];
        item.Exist.ShouldBeTrue();
        item.IsNullValue.ShouldBeTrue();
    }        
        
    [Fact]
    public void WhenPropertyIsEmptyObject_ShouldSucceed()
    {
        const string json = """{ "item1" : {} }""";

        var jsonDocument = JsonDocument.Parse(json);
        var nav          = jsonDocument.ToNavigation();

        var item = nav["item1"];
        item.Exist.ShouldBeTrue();
        item.IsNullValue.ShouldBeFalse();
        item.Count.ShouldBe(0);
    }

    [Fact]
    public void WhenPropertyDoesNotExist_ValueShouldBeNull()
    {
        const string json = """{ "item1" : null }""";

        var jsonDocument = JsonDocument.Parse(json);
        var nav          = jsonDocument.ToNavigation();

        var item = nav["item2"];
        item.Exist.ShouldBeFalse();
        item.IsNullValue.ShouldBeTrue();
    }

    [Fact]
    public void JsonPropertiesCount_ShouldSucceed()
    {
        var jsonDocument = JsonDocument.Parse(Json);
        var nav          = jsonDocument.ToNavigation();

        nav.Count.ShouldBe(2);
    }

    [Fact]
    public void JsonPropertiesNames_ShouldSucceed()
    {
        var jsonDocument = JsonDocument.Parse(Json);
        var nav          = jsonDocument.ToNavigation();

        nav.Keys.Count().ShouldBe(2);
        nav.Keys.ShouldBeSubsetOf(["item1", "item2"]);
    }

    [Fact]
    public void GetNonExistingProperty_ShouldFail()
    {
        var jsonDocument = JsonDocument.Parse(Json);
        var nav          = jsonDocument.ToNavigation();

        nav["item3"].Exist.ShouldBeFalse();
    }
        
    [Fact]
    public void GetNonExistingPropertyByIndex_ShouldFail()
    {
        var jsonDocument = JsonDocument.Parse(Json);
        var nav          = jsonDocument.ToNavigation();

        nav[nav.Count].Exist.ShouldBeFalse();
    }        
        
    [Fact]
    public void WhenIndexOutOfRange_ShouldFail()
    {
        var jsonDocument = JsonDocument.Parse(Json);
        var nav          = jsonDocument.ToNavigation();

        Should.Throw<ArgumentOutOfRangeException>(() => nav[-1].Exist);
    }

    [Fact]
    public void JsonPropertiesValues_ShouldSucceed()
    {
        var jsonDocument = JsonDocument.Parse(Json);
        var nav          = jsonDocument.ToNavigation();

        nav.Values.Count().ShouldBe(2);
        nav.Values.ShouldAllBe(x => x.Exist);
        nav.Values.ShouldAllBe(x => x.JsonElement.ValueKind == JsonValueKind.String);
        nav.Values.Select(x => x.GetStringOrEmpty()).ShouldBeSubsetOf(["first", "second"]);
    }

    [Fact]
    public void JsonPropertiesTryGetValue_ShouldSucceed()
    {
        var jsonDocument = JsonDocument.Parse(Json2);
        var nav          = jsonDocument.ToNavigation();

        foreach (var navKey in nav.Keys)
        {
            nav.TryGetValue(navKey, out var item).ShouldBeTrue();
            nav.Values.ShouldContain(item);
        }
    }
        
    [Fact]
    public void AsDictionary_ShouldSucceed()
    {
        var jsonDocument = JsonDocument.Parse(Json2);
        var nav          = jsonDocument.ToNavigation();

        var dict = (IReadOnlyDictionary<string, JsonNavigationElement>) nav;
        foreach (var pairs in dict)
        {
            nav[pairs.Key].Exist.ShouldBeTrue();
            nav.Keys.ShouldContain(pairs.Key);
            nav.Values.ShouldContain(pairs.Value);
        }
    }   
        
    [Fact]
    public void AsList_ShouldSucceed()
    {
        var jsonDocument = JsonDocument.Parse(Json2);
        var nav          = jsonDocument.ToNavigation();

        var list = (IReadOnlyList<JsonNavigationElement>) nav;
        foreach (var elem in list)
        {
            elem.Exist.ShouldBeTrue();
            nav.Keys.ShouldContain(elem.Name);
            nav[elem.Name].Exist.ShouldBeTrue();
        }
    }

    [Fact]
    public void WhenHierarchyLevel0_ShouldExist()
    {
        const string json = """{ "item1" : 123, "item2": "item2_value" }""";

        var jsonDocument = JsonDocument.Parse(json);
        var nav          = jsonDocument.ToNavigation();
            
        var first = nav["item1"];
        first.Exist.ShouldBeTrue();
        first.GetInt32OrDefault().ShouldBe(123);

        var second = nav["item2"];
        second.Exist.ShouldBeTrue();
        second.GetStringOrEmpty().ShouldBe("item2_value");
    }

    [Fact]
    public void WhenHierarchyLevel1_ShouldExist()
    {
        const string json = """{ "item1" : { "item2" : 123 } }""";

        var jsonDocument = JsonDocument.Parse(json);
        var nav          = jsonDocument.ToNavigation();

        var item = nav["item1"]["item2"]; 
        item.Exist.ShouldBeTrue();
        item.GetInt32OrDefault().ShouldBe(123);
    }
        
    [Fact]
    public void WhenHierarchyLevel2_ShouldExist()
    {
        const string json = """{ "item1" : { "item2" : { "item3" : "test-string" } } }""";

        var jsonDocument = JsonDocument.Parse(json);
        var nav          = jsonDocument.ToNavigation();

        var item = nav["item1"]["item2"]["item3"];
        item.Exist.ShouldBeTrue();
        item.GetStringOrEmpty().ShouldBe("test-string");
    }

    [Fact]
    public void WhenHierarchyLevel0_ShouldNotExist()
    {
        const string json = """{ "item1" : 123}""";

        var jsonDocument = JsonDocument.Parse(json);
        var nav          = jsonDocument.ToNavigation();

        var item = nav["item2"];
        item.Exist.ShouldBeFalse();
    }
        
    [Fact]
    public void WhenHierarchyLevel1_ShouldNotExist()
    {
        const string json = """{ "item1" : { "item2" : 123 } }""";

        var jsonDocument = JsonDocument.Parse(json);
        var nav          = jsonDocument.ToNavigation();

        var item = nav["item1"]["item3"]; 
        item.Exist.ShouldBeFalse();
    }

    [Fact]
    public void EnumerateObject_ShouldEnumerateProperties()
    {
        var jsonDocument = JsonDocument.Parse(Json);
        var nav          = jsonDocument.ToNavigation();

        var list = new List<string>();
        foreach (var item in nav)
        {
            list.Add(item.Name);
        }
        list.ShouldBe(Expected);
    }

    [Fact]
    public void EnumerateObject_CachedProperties_ShouldEnumerateProperties()
    {
        var jsonDocument = JsonDocument.Parse(Json);
        var nav          = jsonDocument.ToNavigation().WithCachedProperties();

        var list = new List<string>();
        foreach (var item in nav)
        {
            list.Add(item.Name);
        }
        list.ShouldBe(Expected);
    }
}