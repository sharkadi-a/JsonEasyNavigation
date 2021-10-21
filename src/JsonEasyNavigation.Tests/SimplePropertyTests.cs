using System;
using System.Text.Json;
using Shouldly;
using Xunit;

namespace JsonEasyNavigation.Tests
{
    public class SimplePropertyTests
    {
        [Fact]
        public void WhenHierarchyLevel0_ShouldExist()
        {
            var json = @"{ ""item1"" : 123 }";

            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.ToNavigation();

            var item = nav["item1"];
            item.Exist.ShouldBeTrue();
            item.GetInt32OrDefault().ShouldBe(123);
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