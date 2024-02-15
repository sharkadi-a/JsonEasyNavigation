using System.Text.Json;
using Shouldly;

namespace JsonEasyNavigation.Tests;

public class NumberTests
{
    [Fact]
    public void WhenCorrectNumber_ShouldSucceed()
    {
        const string json = """{ "item": 42 }""";
            
        var jsonDocument = JsonDocument.Parse(json);
        var nav          = jsonDocument.ToNavigation().WithCachedProperties();

        var item = nav["item"];
        item.Exist.ShouldBeTrue();
        item.GetInt16OrDefault().ShouldBe((short)42);
        item.GetInt32OrDefault().ShouldBe(42);
        item.GetInt64OrDefault().ShouldBe(42);
        item.GetUInt16OrDefault().ShouldBe((ushort)42);
        item.GetUInt32OrDefault().ShouldBe((uint)42);
        item.GetUInt64OrDefault().ShouldBe((ulong)42);
        item.GetByteOrDefault().ShouldBe((byte)42);
        item.GetDecimalOrDefault().ShouldBe(42);
        item.GetDoubleOrDefault().ShouldBe(42);
        item.GetSingleOrDefault().ShouldBe(42);
            
        item.GetValueOrDefault<int>().ShouldBe(42);
        item.GetValueOrDefault<short>().ShouldBe((short)42);
        item.GetValueOrDefault<long>().ShouldBe(42);
        item.GetValueOrDefault<uint>().ShouldBe((uint)42);
        item.GetValueOrDefault<ushort>().ShouldBe((ushort)42);
        item.GetValueOrDefault<ulong>().ShouldBe((ulong)42);
        item.GetValueOrDefault<byte>().ShouldBe((byte)42);
        item.GetValueOrDefault<decimal>().ShouldBe(42);
        item.GetValueOrDefault<double>().ShouldBe(42);
        item.GetValueOrDefault<float>().ShouldBe(42);
    }
        
    [Fact]
    public void WhenSignedNumberUsed_ShouldSucceed()
    {
        const string json = """{ "item": -15 }""";

        var jsonDocument = JsonDocument.Parse(json);
        var nav          = jsonDocument.ToNavigation().WithCachedProperties();
        var item         = nav["item"];
        item.Exist.ShouldBeTrue();
        item.GetInt16OrDefault().ShouldBe((short)-15);
        item.GetInt32OrDefault().ShouldBe(-15);
        item.GetInt64OrDefault().ShouldBe(-15);
        item.GetDecimalOrDefault().ShouldBe(-15);
        item.GetDoubleOrDefault().ShouldBe(-15);
        item.GetSingleOrDefault().ShouldBe(-15);
        item.GetSByteOrDefault().ShouldBe((sbyte)-15);
            
        item.GetValueOrDefault<short>().ShouldBe((short)-15);
        item.GetValueOrDefault<int>().ShouldBe(-15);
        item.GetValueOrDefault<long>().ShouldBe(-15);
        item.GetValueOrDefault<decimal>().ShouldBe(-15);
        item.GetValueOrDefault<double>().ShouldBe(-15);
        item.GetValueOrDefault<float>().ShouldBe(-15);
        item.GetValueOrDefault<sbyte>().ShouldBe((sbyte)-15);
    }
        
    [Fact]
    public void WhenUnsignedNumbersRequired_ButSignedUsed_ShouldFail()
    {
        const string json = """{ "item": -1 }""";

        var jsonDocument = JsonDocument.Parse(json);
        var nav          = jsonDocument.ToNavigation().WithCachedProperties();

        var item = nav["item"];
        item.GetUInt16OrDefault().ShouldBe(default);
        item.GetUInt32OrDefault().ShouldBe(default);
        item.GetUInt64OrDefault().ShouldBe(default);
        item.GetByteOrDefault().ShouldBe(default);
    }

    [Theory]
    [InlineData("""{ "item": {} }""")]
    [InlineData("""{ "item": "" }""")]
    [InlineData("""{ "item": [] }""")]
    [InlineData("""{ "item": null }""")]
    [InlineData("""{ "item": true }""")]
    [InlineData("""{ "item": false }""")]
    public void WhenValueIsNotNumber_ShouldFail(string json)
    {
        var jsonDocument = JsonDocument.Parse(json);
        var nav          = jsonDocument.ToNavigation().WithCachedProperties();
            
        var item = nav["item"];
        item.Exist.ShouldBeTrue();
        item.GetInt16OrDefault().ShouldBe(default);
        item.GetInt32OrDefault().ShouldBe(default);
        item.GetInt64OrDefault().ShouldBe(default);
        item.GetUInt16OrDefault().ShouldBe(default);
        item.GetUInt32OrDefault().ShouldBe(default);
        item.GetUInt64OrDefault().ShouldBe(default);
        item.GetByteOrDefault().ShouldBe(default);
        item.GetSByteOrDefault().ShouldBe(default);
        item.GetDecimalOrDefault().ShouldBe(default);
        item.GetDoubleOrDefault().ShouldBe(default);
        item.GetSingleOrDefault().ShouldBe(default);
            
        item.TryGetValue(out short _).ShouldBeFalse();
        item.TryGetValue(out int _).ShouldBeFalse();
        item.TryGetValue(out long _).ShouldBeFalse();
        item.TryGetValue(out uint _).ShouldBeFalse();
        item.TryGetValue(out ushort _).ShouldBeFalse();
        item.TryGetValue(out ulong _).ShouldBeFalse();
        item.TryGetValue(out byte _).ShouldBeFalse();
        item.TryGetValue(out sbyte _).ShouldBeFalse();
        item.TryGetValue(out decimal _).ShouldBeFalse();
        item.TryGetValue(out float _).ShouldBeFalse();
        item.TryGetValue(out double _).ShouldBeFalse();
    }

    [Fact]
    public void WhenFloatsUsed_ShouldSucceed()
    {
        const string json         = """{ "item": 2021.1025 }""";
        var          jsonDocument = JsonDocument.Parse(json);
        var          nav          = jsonDocument.ToNavigation().WithCachedProperties();
            
        var item = nav["item"];
        item.Exist.ShouldBeTrue();
        item.GetDecimalOrDefault().ShouldBe(2021.1025M);
        item.GetDoubleOrDefault().ShouldBe(2021.1025D);
        item.GetSingleOrDefault().ShouldBe((float)2021.1025);
            
        item.GetValueOrDefault<decimal>().ShouldBe(2021.1025M);
        item.GetValueOrDefault<float>().ShouldBe((float)2021.1025M);
        item.GetValueOrDefault<double>().ShouldBe(2021.1025D);
    }

    [Fact]
    public void WhenValidByteUsed_ShouldSucceed()
    {
        const string json         = """{ "item": 255 }""";
        var          jsonDocument = JsonDocument.Parse(json);
        var          nav          = jsonDocument.ToNavigation().WithCachedProperties();
            
        var item = nav["item"];
        item.Exist.ShouldBeTrue();
        item.GetByteOrDefault().ShouldBe((byte)255);
        item.GetValueOrDefault<byte>().ShouldBe((byte)255);
    }
        
    [Fact]
    public void WhenInvalidByteValueUsed_ShouldSucceed()
    {
        const string json         = """{ "item": 256 }""";
        var          jsonDocument = JsonDocument.Parse(json);
        var          nav          = jsonDocument.ToNavigation().WithCachedProperties();
            
        var item = nav["item"];
        item.Exist.ShouldBeTrue();
        item.GetByteOrDefault().ShouldBe((byte)0);
        item.TryGetValue(out byte _).ShouldBeFalse();
    }
}