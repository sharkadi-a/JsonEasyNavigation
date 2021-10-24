using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Shouldly;
using Xunit;

namespace JsonEasyNavigation.Tests
{
    public class PropertyTests
    {
        [Fact]
        public void JsonElementKind_ShouldBeObject()
        {
            var json = @"{ ""item1"" : ""first"", ""item2"": ""second"" }";

            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.ToNavigation();

            nav.JsonElement.ValueKind.ShouldBe(JsonValueKind.Object);
        }
        
        [Fact]
        public void WhenInvalidElementKind_ShouldFail()
        {
            var json = @"{ ""item1"" : ""first"" }";

            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.ToNavigation();

            var item = nav["item1"];
            item.Values.ShouldBeEmpty();
        }

        [Fact]
        public void JsonPropertiesCount_ShouldSucceed()
        {
            var json = @"{ ""item1"" : ""first"", ""item2"": ""second"" }";

            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.ToNavigation();

            nav.Count.ShouldBe(2);
        }

        [Fact]
        public void JsonPropertiesNames_ShouldSucceed()
        {
            var json = @"{ ""item1"" : ""first"", ""item2"": ""second"" }";

            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.ToNavigation();

            nav.Keys.Count().ShouldBe(2);
            nav.Keys.ShouldBeSubsetOf(new[] {"item1", "item2"});
        }

        [Fact]
        public void GetNonExistingProperty_ShouldFail()
        {
            var json = @"{ ""item1"" : ""first"", ""item2"": ""second"" }";

            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.ToNavigation();

            nav["item3"].Exist.ShouldBeFalse();
        }
        
        [Fact]
        public void GetNonExistingPropertyByIndex_ShouldFail()
        {
            var json = @"{ ""item1"" : ""first"", ""item2"": ""second"" }";

            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.ToNavigation();

            nav[nav.Count].Exist.ShouldBeFalse();
        }        
        
        [Fact]
        public void WhenIndexOutOfRange_ShouldFail()
        {
            var json = @"{ ""item1"" : ""first"", ""item2"": ""second"" }";

            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.ToNavigation();

            Should.Throw<ArgumentOutOfRangeException>(() => nav[-1].Exist);
        }

        [Fact]
        public void JsonPropertiesValues_ShouldSucceed()
        {
            var json = @"{ ""item1"" : ""first"", ""item2"": ""second"" }";

            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.ToNavigation();

            nav.Values.Count().ShouldBe(2);
            nav.Values.ShouldAllBe(x => x.Exist);
            nav.Values.ShouldAllBe(x => x.JsonElement.ValueKind == JsonValueKind.String);
            nav.Values.Select(x => x.GetStringOrEmpty()).ShouldBeSubsetOf(new[] {"first", "second"});
        }

        [Fact]
        public void JsonPropertiesTryGetValue_ShouldSucceed()
        {
            var json = @"{ ""item1"" : ""first"", ""item2"": ""second"", ""item3"": ""third"" }";

            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.ToNavigation();

            foreach (var navKey in nav.Keys)
            {
                nav.TryGetValue(navKey, out var item).ShouldBeTrue();
                nav.Values.ShouldContain(item);
            }
        }
        
        [Fact]
        public void AsDictionary_ShouldSucceed()
        {
            var json = @"{ ""item1"" : ""first"", ""item2"": ""second"", ""item3"": ""third"" }";

            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.ToNavigation();

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
            var json = @"{ ""item1"" : ""first"", ""item2"": ""second"", ""item3"": ""third"" }";

            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.ToNavigation();

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
            var json = @"{ ""item1"" : 123, ""item2"": ""item2_value"" }";

            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.ToNavigation();
            
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
            var json = @"{ ""item1"" : { ""item2"" : 123 } }";

            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.ToNavigation();

            var item = nav["item1"]["item2"]; 
            item.Exist.ShouldBeTrue();
            item.GetInt32OrDefault().ShouldBe(123);
        }
        
        [Fact]
        public void WhenHierarchyLevel2_ShouldExist()
        {
            var json = @"{ ""item1"" : { ""item2"" : { ""item3"" : ""test-string"" } } }";

            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.ToNavigation();

            var item = nav["item1"]["item2"]["item3"];
            item.Exist.ShouldBeTrue();
            item.GetStringOrEmpty().ShouldBe("test-string");
        }

        [Fact]
        public void WhenHierarchyLevel0_ShouldNotExist()
        {
            var json = @"{ ""item1"" : 123}";

            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.ToNavigation();

            var item = nav["item2"];
            item.Exist.ShouldBeFalse();
        }
        
        [Fact]
        public void WhenHierarchyLevel1_ShouldNotExist()
        {
            var json = @"{ ""item1"" : { ""item2"" : 123 } }";

            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.ToNavigation();

            var item = nav["item1"]["item3"]; 
            item.Exist.ShouldBeFalse();
        }
    }
}