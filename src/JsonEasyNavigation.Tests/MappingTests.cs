using System.Text.Json;
using Shouldly;
using Xunit;

namespace JsonEasyNavigation.Tests
{
    public class MappingTests
    {
        private class SimpleObject
        {
            public int item1 { get; set; }
        }

        private class HierarchyObject
        {
            public class InnerObject
            {
                public decimal inner { get; set; }
            }

            public int item1 { get; set; }
            public string item2 { get; set; }
            public InnerObject item3 { get; set; }
        }        
        
        private class HierarchyObjectWithArray
        {
            public class InnerObject
            {
                public decimal item1 { get; set; }
                public string item2 { get; set; }
                public InnerObject[] innerArray { get; set; }
            }

            public InnerObject[] array { get; set; }
        }

        [Fact]
        public void SimpleObjectMapping_ShouldSucceed()
        {
            var json = @"{""item1"": 2021}";

            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.ToNavigation();

            var obj = nav.Map<SimpleObject>();
            obj.ShouldNotBeNull();
            obj.item1.ShouldBe(2021);
        }        
        
        [Fact]
        public void HierarchyObjectMapping_ShouldSucceed()
        {
            var json = @"{""item1"": 2021, ""item2"": ""string"", ""item3"": {""inner"": 123.321} }";

            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.ToNavigation();

            var obj = nav.Map<HierarchyObject>();
            obj.ShouldNotBeNull();
            obj.item1.ShouldBe(2021);
            obj.item2.ShouldBe("string");
            obj.item3.ShouldNotBeNull();
            obj.item3.inner.ShouldBe(123.321M);
        }        
        
        [Fact]
        public void HierarchySubObjectMapping_ShouldSucceed()
        {
            var json = @"{""item1"": 2021, ""item2"": ""string"", ""item3"": {""inner"": 123.321} }";

            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.ToNavigation();

            var obj = nav["item3"].Map<HierarchyObject.InnerObject>();
            obj.ShouldNotBeNull();
            obj.inner.ShouldBe(123.321M);
        }
        
        [Fact]
        public void HierarchyWithArrayMapping_ShouldSucceed()
        {
            var json = @"{ ""array"": [ 
                            { ""item1"": 123.321, ""item2"": ""string"", 
                            ""innerArray"": [ 
                            { ""item1"": 999.111, ""item2"": ""anotherString"", ""innerArray"" : [] }
                         ]} ] }";

            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.ToNavigation();

            var obj = nav.Map<HierarchyObjectWithArray>();
            obj.ShouldNotBeNull();
            obj.array.ShouldNotBeEmpty();
            foreach (var o in obj.array)
            {
                o.item1.ShouldBe(123.321M);
                o.item2.ShouldBe("string");
                o.innerArray.ShouldNotBeEmpty();
                foreach (var innerObject in o.innerArray)
                {
                    innerObject.item1.ShouldBe(999.111M);
                    innerObject.item2.ShouldBe("anotherString");
                    innerObject.innerArray.ShouldBeEmpty();
                }
            }
        }
    }
}