using System.Text.Json;
using Shouldly;
using Xunit;

namespace JsonEasyNavigation.Tests
{
    public class SimpleArrayTests
    {
        [Fact]
        public void WhenArrayLevel0_ShouldSucceed()
        {
            var json = @"[ ""item1"", ""item2"" ]";

            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.ToNavigation();

            var item1 = nav[0];
            var item2 = nav[1];
            
            item1.Exist.ShouldBeTrue();
            item2.Exist.ShouldBeTrue();
            item1.GetStringOrEmpty().ShouldBe("item1");
            item2.GetStringOrEmpty().ShouldBe("item2");
        }
    }
}