using System.Linq;
using System.Text.Json;
using Shouldly;
using Xunit;

namespace JsonEasyNavigation.Tests
{
    public class ExtensionsTests
    {
        [Fact]
        public void UsingPath_ShouldFindProperty()
        {
            var json = @"{ ""item1"" : { ""item2"" : { ""item3"" : ""test-string"" } } }";
            
            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.RootElement.Property("item1", "item2", "item3");

            nav.Exist.ShouldBeTrue();
            nav.GetStringOrDefault().ShouldBe("test-string");
        }        
        
        [Fact]
        public void UsingPath_ShouldNotFindProperty()
        {
            var json = @"{ ""item1"" : { ""item2"" : { ""item3"" : ""test-string"" } } }";
            
            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.RootElement.Property("item1", "item2", "item3", "item4");

            nav.Exist.ShouldBeFalse();
        }
        
        [Fact]
        public void UsingPathAndGetProperty_ShouldFindProperty()
        {
            var json = @"{ ""item1"" : { ""item2"" : { ""item3"" : { ""item4"" : 100 } } } }";

            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.RootElement.Property("item1", "item2", "item3")["item4"];

            nav.Exist.ShouldBeTrue();
            nav.GetInt32OrDefault().ShouldBe(100);
        }

        [Fact]
        public void WithStablePropertyOrder_ShouldBeSorted()
        {
            var json = @"{ ""item3"": 3, ""item1"": 1, ""item4"": 4}";
            
            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.ToNavigation().WithStablePropertyOrder()[0];
            
            nav.Exist.ShouldBeTrue();
            nav.HasName.ShouldBeTrue();
            nav.Name.ShouldBe("item1");
        }

        [Fact]
        public void SelectFromManyJsonSources_SumShouldBeCorrect()
        {
            var json1 = @"{ ""item3"": 3, ""item1"": 10, ""item4"": 4}";
            var json2 = @"{ ""item3"": 3, ""item1"": 15, ""item4"": 4}";
            var json3 = @"{ ""item3"": 3, ""item1"": 99, ""item4"": 4}";

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
}