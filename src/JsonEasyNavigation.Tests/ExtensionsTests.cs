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
    }
}