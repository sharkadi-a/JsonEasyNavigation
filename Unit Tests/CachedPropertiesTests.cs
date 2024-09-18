using System.Text.Json;
using Shouldly;

namespace JsonEasyNavigation.Tests;

public class CachedPropertiesTests
{
    [Fact]
    public void WithCachedProperties_ShouldSucceed()
    {
        const string json = """{ "item3": 3, "item1": 1, "item4": 4}""";
            
        var jsonDocument = JsonDocument.Parse(json);
        var nav          = jsonDocument.ToNavigation().WithCachedProperties();

        var item = nav["item1"];
        item.Exist.ShouldBeTrue();
        item.Name.ShouldBe("item1");
        item.GetInt32OrDefault().ShouldBe(1);
    }

    private const string Json = """{ "item3": 3, "item1": 1, "item4": { "item42" : 42, "item41": 41 } }""";

    [Fact]
    public void WithCachedPropertiesAndDescendants_ShouldSucceed()
    {
        var jsonDocument = JsonDocument.Parse(Json);
        var nav          = jsonDocument.ToNavigation().WithCachedProperties();

        var item = nav["item4"];
        item.Exist.ShouldBeTrue();
        item.Name.ShouldBe("item4");
        item.JsonElement.ValueKind.ShouldBe(JsonValueKind.Object);

        var innerItem = item["item41"];
        innerItem.Exist.ShouldBeTrue();
        innerItem.Name.ShouldBe("item41");
        innerItem.GetInt32OrDefault().ShouldBe(41);
    }

    [Fact]
    public void WithoutCachedProperties_ShouldSucceed()
    {
        var jsonDocument = JsonDocument.Parse(Json);
        var nav          = jsonDocument.ToNavigation().WithoutCachedProperties();

        var item = nav["item4"];
        item.Exist.ShouldBeTrue();
        item.Name.ShouldBe("item4");
        item.JsonElement.ValueKind.ShouldBe(JsonValueKind.Object);

        var innerItem = item["item41"];
        innerItem.Exist.ShouldBeTrue();
        innerItem.Name.ShouldBe("item41");
        innerItem.GetInt32OrDefault().ShouldBe(41);
    }
        
    [Fact]
    public void WithThenWithoutCachedProperties_ShouldSucceed()
    {
        var jsonDocument = JsonDocument.Parse(Json);
        var nav          = jsonDocument.ToNavigation().WithCachedProperties().WithoutCachedProperties();

        var item = nav["item4"];
        item.Exist.ShouldBeTrue();
        item.Name.ShouldBe("item4");
        item.JsonElement.ValueKind.ShouldBe(JsonValueKind.Object);

        var innerItem = item["item41"];
        innerItem.Exist.ShouldBeTrue();
        innerItem.Name.ShouldBe("item41");
        innerItem.GetInt32OrDefault().ShouldBe(41);
    }
}