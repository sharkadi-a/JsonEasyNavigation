using System.Text.Json;
using Shouldly;

namespace JsonEasyNavigation.Tests;

public class StablePropertyOrderAndCachedPropertiesTests
{
    private const string Json  = """{ "item3": 3, "item1": { "item12" : 12, "item10" : 10, "item11" : 11 }, "item4": 4, "item0" : 0 }""";
    private const string Json2 = """{ "item3": 3, "item1": 1, "item40": 40, "item0" : 0 }""";
    private const string Json3 = """{ "item3": 3, "item1": 1, "item4": 4, "item0" : 0 }""";

    [Fact]
    public void WithBoth_PropertiesShouldBeOrdered()
    {
        var jsonDocument = JsonDocument.Parse(Json3);
        var nav          = jsonDocument.ToNavigation().WithCachedProperties().WithStablePropertyOrder();

        var first = nav[0];
        first.Exist.ShouldBeTrue();
        first.Name.ShouldBe("item0");
        first.GetInt32OrDefault().ShouldBe(0);

        nav[1].GetInt32OrDefault().ShouldBe(1);
        nav[2].GetInt32OrDefault().ShouldBe(3);
        nav[3].GetInt32OrDefault().ShouldBe(4);
    }

    [Fact]
    public void WithBothAndDescendants_PropertiesShouldBeOrdered()
    {
        var jsonDocument = JsonDocument.Parse(Json);
        var nav          = jsonDocument.ToNavigation().WithCachedProperties().WithStablePropertyOrder();

        var first = nav[1];
        first.Exist.ShouldBeTrue();
        first.Name.ShouldBe("item1");
        first.JsonElement.ValueKind.ShouldBe(JsonValueKind.Object);

        var inner = first[2];
        inner.Exist.ShouldBeTrue();
        inner.Name.ShouldBe("item12");
        inner.GetInt32OrDefault().ShouldBe(12);
    } 
        
    [Fact]
    public void WithBothAndDescendantsAndSameObject_PropertiesShouldBeOrdered()
    {
        var jsonDocument = JsonDocument.Parse(Json);
        var nav          = jsonDocument.ToNavigation().WithCachedProperties().WithStablePropertyOrder();

        var item = nav[1];
        item.Exist.ShouldBeTrue();
        item.Name.ShouldBe("item1");
        item.JsonElement.ValueKind.ShouldBe(JsonValueKind.Object);

        item[0].GetInt32OrDefault().ShouldBe(10);
        item[1].GetInt32OrDefault().ShouldBe(11);
        item[2].GetInt32OrDefault().ShouldBe(12);
    }
        
    [Fact]
    public void WithBothAndDescendantsAndSameObject_ThenWithoutCachedProperties_PropertiesShouldBeOrdered()
    {
        var jsonDocument = JsonDocument.Parse(Json);
        var nav = jsonDocument.ToNavigation()
                              .WithCachedProperties()
                              .WithStablePropertyOrder()
                              .WithoutCachedProperties();

        var item = nav[1];
        item.Exist.ShouldBeTrue();
        item.Name.ShouldBe("item1");
        item.JsonElement.ValueKind.ShouldBe(JsonValueKind.Object);

        item[0].GetInt32OrDefault().ShouldBe(10);
        item[1].GetInt32OrDefault().ShouldBe(11);
        item[2].GetInt32OrDefault().ShouldBe(12);
    }
        
    [Fact]
    public void WithBoth_AndThenWithoutBoth_PropertiesShouldBeOrdered()
    {
        var jsonDocument = JsonDocument.Parse(Json2);
        var nav = jsonDocument.ToNavigation()
                              .WithCachedProperties()
                              .WithStablePropertyOrder()
                              .WithoutCachedProperties()
                              .WithoutStablePropertyOrder();

        nav["item0"].GetInt32OrDefault().ShouldBe(0);
        nav["item1"].GetInt32OrDefault().ShouldBe(1);
        nav["item3"].GetInt32OrDefault().ShouldBe(3);
        nav["item40"].GetInt32OrDefault().ShouldBe(40);
    }  
}