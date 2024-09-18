using System.Text;
using System.Text.Json;
using Shouldly;

namespace JsonEasyNavigation.Tests;

public class OtherTypesTests
{
    [Theory]
    [InlineData("""{ "item": true }""",  JsonValueKind.True,  true)]
    [InlineData("""{ "item": false }""", JsonValueKind.False, false)]
    public void WhenValueIsBoolean_ShouldSucceed(string json, JsonValueKind kind, bool expected)
    {
        var jsonDocument = JsonDocument.Parse(json);

        var nav = jsonDocument.ToNavigation();

        var item = nav["item"];
        item.Exist.ShouldBeTrue();
        item.JsonElement.ValueKind.ShouldBe(kind);
        item.GetBooleanOrDefault().ShouldBe(expected);
        item.TryGetValue(out bool value).ShouldBeTrue();
        value.ShouldBe(expected);
    }

    [Theory]
    [InlineData("""{ "item": null }""")]
    [InlineData("""{ "item": {} }""")]
    [InlineData("""{ "item": [] }""")]
    [InlineData("""{ "item": "string" }""")]
    [InlineData("""{ "item": 0 }""")]
    [InlineData("""{ "item": 0.1 }""")]
    public void WhenValueIsInvalidBoolean_ShouldFail(string json)
    {
        var jsonDocument = JsonDocument.Parse(json);
            
        var nav = jsonDocument.ToNavigation();

        var item = nav["item"];
        item.Exist.ShouldBeTrue();
        item.JsonElement.ValueKind.ShouldNotBe(JsonValueKind.True);
        item.JsonElement.ValueKind.ShouldNotBe(JsonValueKind.False);
        item.GetBooleanOrDefault().ShouldBeFalse();
        item.TryGetValue(out bool _).ShouldBeFalse();
    }

    [Fact]
    public void WhenValueIsDateTime_ShouldSucceed()
    {
        const string json         = """{ "item": "2021-10-25T12:30:30" }""";
        var          jsonDocument = JsonDocument.Parse(json);

        var nav = jsonDocument.ToNavigation();

        var item = nav["item"];
        item.Exist.ShouldBeTrue();
        item.GetDateTimeOrDefault().ShouldBe(new DateTime(2021,              10, 25, 12, 30, 30));
        item.GetValueOrDefault<DateTime>().ShouldBe(new DateTime(2021,       10, 25, 12, 30, 30));
        item.GetDateTimeOffsetOrDefault().ShouldBe(new DateTime(2021,        10, 25, 12, 30, 30));
        item.GetValueOrDefault<DateTimeOffset>().ShouldBe(new DateTime(2021, 10, 25, 12, 30, 30));
    }
        
    [Fact]
    public void WhenValueIsDateTimeOffset_ShouldSucceed()
    {
        const string json         = """{ "item": "2021-10-25T12:30:30+03:00" }""";
        var          jsonDocument = JsonDocument.Parse(json);

        var nav = jsonDocument.ToNavigation();

        var item = nav["item"];
        item.Exist.ShouldBeTrue();
        var offset = new DateTimeOffset(new DateTime(2021, 10, 25, 12, 30, 30, DateTimeKind.Unspecified), TimeSpan.FromHours(3));
        item.GetDateTimeOffsetOrDefault().ShouldBe(offset);
        item.GetValueOrDefault<DateTimeOffset>().ShouldBe(offset);
    }        
        
    [Theory]
    [InlineData("""{ "item": null }""")]
    [InlineData("""{ "item": {} }""")]
    [InlineData("""{ "item": "string" }""")]
    [InlineData("""{ "item": 0 }""")]
    [InlineData("""{ "item": 0.1 }""")]
    [InlineData("""{ "item": true }""")]
    [InlineData("""{ "item": false }""")]
    [InlineData("""{ "item": [] }""")]
    public void WhenValueIsInvalidDateTime_ShouldSucceed(string json)
    {
        var jsonDocument = JsonDocument.Parse(json);
        var nav = jsonDocument.ToNavigation();

        var item = nav["item"];
        item.Exist.ShouldBeTrue();
        item.GetDateTimeOffsetOrDefault().ShouldBe(default);
        item.TryGetValue(out DateTimeOffset _).ShouldBe(default);
    }

    [Fact]
    public void WhenValueIsGuid_ShouldSucceed()
    {
        var guid         = Guid.NewGuid();
        var json         = $$"""{ "item": "{{guid.ToString()}}" }""";
        var jsonDocument = JsonDocument.Parse(json);

        var nav = jsonDocument.ToNavigation();

        var item = nav["item"];
        item.Exist.ShouldBeTrue();
        item.GetGuidOrDefault().ShouldBe(guid);
        item.GetValueOrDefault<Guid>().ShouldBe(guid);
    }        
        
    [Theory]
    [InlineData("""{ "item": null }""")]
    [InlineData("""{ "item": {} }""")]
    [InlineData("""{ "item": "string" }""")]
    [InlineData("""{ "item": 0 }""")]
    [InlineData("""{ "item": 0.1 }""")]
    [InlineData("""{ "item": true }""")]
    [InlineData("""{ "item": false }""")]
    [InlineData("""{ "item": [] }""")]
    public void WhenValueIsNotGuid_ShouldFail(string json)
    {
        var jsonDocument = JsonDocument.Parse(json);
        var nav = jsonDocument.ToNavigation();

        var item = nav["item"];
        item.Exist.ShouldBeTrue();
        item.GetGuidOrDefault().ShouldBe(default);
        item.GetValueOrDefault<Guid>().ShouldBe(default);
    }

    [Theory]
    [InlineData("""{ "item": "string" }""")]
    [InlineData("""{ "item": "\u0073\u0074\u0072\u0069\u006e\u0067" }""")]
    public void WhenValueIsString_ShouldSucceed(string json)
    {
        var jsonDocument = JsonDocument.Parse(json);
        var nav = jsonDocument.ToNavigation();

        var item = nav["item"];
        item.Exist.ShouldBeTrue();
        item.JsonElement.ValueKind.ShouldBe(JsonValueKind.String);
        item.GetStringOrEmpty().ShouldBe("string");
        item.GetValueOrDefault<string>().ShouldBe("string");
    }
        
    [Theory]
    [InlineData("""{ "item": null }""")]
    [InlineData("""{ "item": {} }""")]
    [InlineData("""{ "item": 0 }""")]
    [InlineData("""{ "item": 0.1 }""")]
    [InlineData("""{ "item": true }""")]
    [InlineData("""{ "item": false }""")]
    [InlineData("""{ "item": [] }""")]
    public void WhenValueIsString_ShouldFail(string json)
    {
        var jsonDocument = JsonDocument.Parse(json);
        var nav = jsonDocument.ToNavigation();

        var item = nav["item"];
        item.Exist.ShouldBeTrue();
        item.JsonElement.ValueKind.ShouldNotBe(JsonValueKind.String);
        item.GetStringOrDefault().ShouldBe(default);
        item.GetStringOrEmpty().ShouldBe(string.Empty);
        item.TryGetValue(out string _).ShouldBeFalse();
    }

    [Fact]
    public void WhenValueIsBase64_ShouldSucceed()
    {
        const string json         = """{ "item": "SGVsbG8sIHdvcmxkIQ==" }""";
        var          jsonDocument = JsonDocument.Parse(json);

        var nav = jsonDocument.ToNavigation();

        var item = nav["item"];
        item.Exist.ShouldBeTrue();
            
        var bytes = item.GetBytesFromBase64OrDefault();
        Encoding.UTF8.GetString(bytes).ShouldBe("Hello, world!");

        bytes = item.GetValueOrDefault<byte[]>();
        Encoding.UTF8.GetString(bytes).ShouldBe("Hello, world!");

        using var stream = item.GetStreamFromBase64OrDefault();
        stream.ReadExactly(bytes, 0, (int)stream.Length);
        Encoding.UTF8.GetString(bytes).ShouldBe("Hello, world!");

        using var stream2 = item.GetValueOrDefault<Stream>();
        stream2.ReadExactly(bytes, 0, (int)stream2.Length);
        Encoding.UTF8.GetString(bytes).ShouldBe("Hello, world!");
    }

    [Theory]
    [InlineData("""{ "item": null }""")]
    [InlineData("""{ "item": {} }""")]
    [InlineData("""{ "item": 0 }""")]
    [InlineData("""{ "item": 0.1 }""")]
    [InlineData("""{ "item": true }""")]
    [InlineData("""{ "item": false }""")]
    [InlineData("""{ "item": [] }""")]
    public void WhenValueIsNotBase64_ShouldFail(string json)
    {
        var jsonDocument = JsonDocument.Parse(json);
        var nav = jsonDocument.ToNavigation();

        var item = nav["item"];
        item.Exist.ShouldBeTrue();
        item.GetBytesFromBase64OrDefault().ShouldBe(default);
        item.TryGetValue(out byte[] _).ShouldBeFalse();
    }
}