using System.Text.Json;
using Shouldly;

namespace JsonEasyNavigation.Tests;

public class MappingTests
{
    [Serializable]
    public class SimpleObject
    {
        public int Item1 { get; set; }
    }

    [Serializable]
    public class HierarchyObject
    {
        [Serializable]
        public class InnerObject
        {
            public decimal Inner { get; set; }
        }

        public int          Item1 { get; set; }
        public string?      Item2 { get; set; }
        public InnerObject? Item3 { get; set; }
    }

    [Serializable]
    public class HierarchyObjectWithArray
    {
        [Serializable]
        public class InnerObject
        {
            public decimal        Item1      { get; set; }
            public string?        Item2      { get; set; }
            public InnerObject[]? InnerArray { get; set; }
        }

        public InnerObject[]? Array { get; set; }
    }

    [Fact]
    public void SimpleObjectMapping_ShouldSucceed()
    {
        const string json = """{"item1": 2021}""";

        var jsonDocument = JsonDocument.Parse(json);
        var nav          = jsonDocument.ToNavigation();

        var obj = nav.Map<SimpleObject>();
        obj.ShouldNotBeNull();
        obj.Item1.ShouldBe(2021);
    }

    private const string Json = """{"item1": 2021, "item2": "string", "item3": {"inner": 123.321} }""";

    [Fact]
    public void HierarchyObjectMapping_ShouldSucceed()
    {
        var jsonDocument = JsonDocument.Parse(Json);
        var nav          = jsonDocument.ToNavigation();

        var obj = nav.Map<HierarchyObject>();
        obj.ShouldNotBeNull();
        obj.Item1.ShouldBe(2021);
        obj.Item2.ShouldBe("string");
        obj.Item3.ShouldNotBeNull();
        obj.Item3.Inner.ShouldBe(123.321M);
    }        
        
    [Fact]
    public void HierarchySubObjectMapping_ShouldSucceed()
    {
        var jsonDocument = JsonDocument.Parse(Json);
        var nav          = jsonDocument.ToNavigation();

        var obj = nav["item3"].Map<HierarchyObject.InnerObject>();
        obj.ShouldNotBeNull();
        obj.Inner.ShouldBe(123.321M);
    }
        
    [Fact]
    public void HierarchyWithArrayMapping_ShouldSucceed()
    {
        const string json = """
                            { "array": [ 
                                                        { "item1": 123.321, "item2": "string", 
                                                        "innerArray": [ 
                                                        { "item1": 999.111, "item2": "anotherString", "innerArray" : [] }
                                                     ]} ] }
                            """;

        var jsonDocument = JsonDocument.Parse(json);
        var nav          = jsonDocument.ToNavigation();

        var obj = nav.Map<HierarchyObjectWithArray>();
        obj.ShouldNotBeNull();
        obj.Array.ShouldNotBeEmpty();
        foreach (var o in obj.Array)
        {
            o.Item1.ShouldBe(123.321M);
            o.Item2.ShouldBe("string");
            o.InnerArray.ShouldNotBeEmpty();
            foreach (var innerObject in o.InnerArray)
            {
                innerObject.Item1.ShouldBe(999.111M);
                innerObject.Item2.ShouldBe("anotherString");
                innerObject.InnerArray.ShouldBeEmpty();
            }
        }
    }
}