using System.Text.Json;
using Shouldly;

namespace JsonEasyNavigation.Tests;

public class StablePropertyOrderTests
{
    private const string Json = """{ "item3": 3, "item1": 1, "item4": 4}""";
    private const string Json2 = """{ "item11" : 11, "item10": { "item21" : 21, "item20" : 20, "item22" : 22 } }""";

    [Fact]
    public void WithStablePropertyOrder_ShouldBeSorted()
    {
        var jsonDocument = JsonDocument.Parse(Json);
        var nav          = jsonDocument.ToNavigation().WithStablePropertyOrder()[0];
            
        nav.Exist.ShouldBeTrue();
        nav.HasName.ShouldBeTrue();
        nav.Name.ShouldBe("item1");
    }
        
    [Fact]
    public void WithStablePropertyOrderMultipleAccess_ShouldBeSorted()
    {
        var jsonDocument = JsonDocument.Parse(Json);
        var nav          = jsonDocument.ToNavigation().WithStablePropertyOrder();

        var nav1 = nav[0];
        var nav2 = nav[1];
        var nav3 = nav[2];
            
        nav1.Exist.ShouldBeTrue();
        nav1.HasName.ShouldBeTrue();
        nav1.Name.ShouldBe("item1");
            
        nav2.Exist.ShouldBeTrue();
        nav2.HasName.ShouldBeTrue();
        nav2.Name.ShouldBe("item3");

        nav3.Exist.ShouldBeTrue();
        nav3.HasName.ShouldBeTrue();
        nav3.Name.ShouldBe("item4");
    }

    [Fact]
    public void WithStableDescendants_ShouldAlsoBeSorted()
    {
        var jsonDocument = JsonDocument.Parse(Json2);
        var nav          = jsonDocument.ToNavigation().WithStablePropertyOrder();
            
        var first      = nav[0];
        var innerFirst = first[0];
            
        first.Exist.ShouldBeTrue();
        first.HasName.ShouldBeTrue();
        first.Name.ShouldBe("item10");
        first.JsonElement.ValueKind.ShouldBe(JsonValueKind.Object);
            
        innerFirst.Exist.ShouldBeTrue();
        innerFirst.HasName.ShouldBeTrue();
        innerFirst.Name.ShouldBe("item20");
        innerFirst.GetInt32OrDefault().ShouldBe(20);
    }

    [Fact]
    public void WithStableDescendantsAndSameObject_ShouldAlsoBeSorted()
    {
        var jsonDocument = JsonDocument.Parse(Json2);
        var nav          = jsonDocument.ToNavigation().WithStablePropertyOrder();

        var item = nav[0];
        item.Exist.ShouldBeTrue();
        item.HasName.ShouldBeTrue();
        item.Name.ShouldBe("item10");
            
        item[0].GetInt32OrDefault().ShouldBe(20);
        item[1].GetInt32OrDefault().ShouldBe(21);
        item[2].GetInt32OrDefault().ShouldBe(22);
    }

    [Fact]
    public void WithoutStableDescendants_ShouldSucceed()
    {
        var jsonDocument = JsonDocument.Parse(Json);
        var nav          = jsonDocument.ToNavigation().WithoutStablePropertyOrder();

        var item = nav["item4"];
        item.Exist.ShouldBeTrue();
        item.Name.ShouldBe("item4");
        item.GetInt32OrDefault().ShouldBe(4);
    }
        
    [Fact]
    public void WithThenWithoutStableDescendants_ShouldSucceed()
    {
        var jsonDocument = JsonDocument.Parse(Json);
        var nav          = jsonDocument.ToNavigation().WithStablePropertyOrder().WithoutStablePropertyOrder();

        var item = nav["item3"];
        item.Exist.ShouldBeTrue();
        item.Name.ShouldBe("item3");
        item.GetInt32OrDefault().ShouldBe(3);
    }
}