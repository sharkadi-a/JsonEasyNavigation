using System.Text.Json;
using Shouldly;

namespace JsonEasyNavigation.Tests;

public class ArrayTests
{
    [Fact]
    public void GetArrayItem_ShouldSucceed()
    {
        const string json = """[ "item1", "item2" ]""";

        var jsonDocument = JsonDocument.Parse(json);
        var nav          = jsonDocument.ToNavigation();

        var item1 = nav[0];
        var item2 = nav[1];
            
        nav.JsonElement.ValueKind.ShouldBe(JsonValueKind.Array);
        nav.Count.ShouldBe(2);
            
        item1.Exist.ShouldBeTrue();
        item1.Index.ShouldBe(0);
        item1.GetStringOrEmpty().ShouldBe("item1");

        item2.Exist.ShouldBeTrue();
        item2.Index.ShouldBe(1);
        item2.GetStringOrEmpty().ShouldBe("item2");
    }

    [Fact]
    public void WhenEmptyArray_ShouldSucceed()
    {
        const string json = "[ ]";

        var jsonDocument = JsonDocument.Parse(json);
        var nav          = jsonDocument.ToNavigation();
            
        nav.Exist.ShouldBeTrue();
        nav.IsNullValue.ShouldBeFalse();
        nav.Count.ShouldBe(0);
    }

    [Fact]
    public void ArrayCount_ShouldSucceed()
    {
        const string json = """[ "item1", "item2", "item3", "item4" ]""";

        var jsonDocument = JsonDocument.Parse(json);
        var nav          = jsonDocument.ToNavigation();
            
        nav.Count.ShouldBe(4);
    }
        
    [Fact]
    public void GetNonExistingArrayItem_ShouldSucceed()
    {
        const string json = """[ "item1", "item2" ]""";

        var jsonDocument = JsonDocument.Parse(json);
        var nav          = jsonDocument.ToNavigation();

        nav[nav.Count].Exist.ShouldBeFalse();
    }
        
    [Fact]
    public void WhenIndexOutOfRange_ShouldThrow()
    {
        const string json = """[ "item1", "item2" ]""";

        var jsonDocument = JsonDocument.Parse(json);
        var nav          = jsonDocument.ToNavigation();

        Should.Throw<ArgumentOutOfRangeException>(() => nav[-1].Exist);
    }
        
    [Fact]
    public void WhenArrayInProperty_ShouldSucceed()
    {
        const string json = """{ "item1": 1, "item2": 2, "item3":  [ "item31", "item32", "item33" ] }""";
            
        var jsonDocument = JsonDocument.Parse(json);
        var nav          = jsonDocument.ToNavigation();

        var item = nav["item3"];
        item.Exist.ShouldBeTrue();
        item.Name.ShouldBe("item3");
        item.JsonElement.ValueKind.ShouldBe(JsonValueKind.Array);
        item.Count.ShouldBe(3);
            
        item[0].GetStringOrEmpty().ShouldBe("item31");
        item[1].GetStringOrEmpty().ShouldBe("item32");
        item[2].GetStringOrEmpty().ShouldBe("item33");
    }

    [Fact]
    public void WhenArrayInArray_ShouldSucceed()
    {
        const string json = """[ "item1", "item2", [ "item31", "item32" ] ]""";

        var jsonDocument = JsonDocument.Parse(json);
        var nav          = jsonDocument.ToNavigation();

        var item = nav[2];
        item.Exist.ShouldBeTrue();
        item.JsonElement.ValueKind.ShouldBe(JsonValueKind.Array);
        item.Count.ShouldBe(2);
            
        var inner = item[1];
        inner.Exist.ShouldBeTrue();
        inner.GetStringOrEmpty().ShouldBe("item32");
    }

    private const string Json = """[ "item1", "item2", "item3" ]""";

    [Fact]
    public void WhenEnumeratingKey_ShouldBeEmpty()
    {
        var jsonDocument = JsonDocument.Parse(Json);
        var nav          = jsonDocument.ToNavigation();

        nav.Keys.ShouldBeEmpty();
    }

    [Fact]
    public void EnumerateArray_ShouldEnumerateItems()
    {


        var jsonDocument = JsonDocument.Parse(Json);
        var nav          = jsonDocument.ToNavigation();
            
        var list = new List<string>();
        foreach (var item in nav)
            list.Add(item.GetStringOrEmpty());
        list.ShouldBe(["item1", "item2", "item3"]);
    }
}