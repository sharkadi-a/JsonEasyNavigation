﻿using System.Text.Json;
using Shouldly;
using Xunit;

namespace JsonEasyNavigation.Tests
{
    public class NumberTests
    {
        [Fact]
        public void WhenCorrectNumber_ShouldSucceed()
        {
            var json = @"{ ""item"": 42 }";
            
            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.ToNavigation().WithCachedProperties();

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
        }
        
        [Fact]
        public void WhenSignedNumberUsed_ShouldSucceed()
        {
            var json = @"{ ""item"": -15 }";

            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.ToNavigation().WithCachedProperties();
            var item = nav["item"];
            item.Exist.ShouldBeTrue();
            item.GetInt16OrDefault().ShouldBe((short)-15);
            item.GetInt32OrDefault().ShouldBe(-15);
            item.GetInt64OrDefault().ShouldBe(-15);
            item.GetDecimalOrDefault().ShouldBe(-15);
            item.GetDoubleOrDefault().ShouldBe(-15);
            item.GetSByteOrDefault().ShouldBe((sbyte)-15);
        }
        
        [Fact]
        public void WhenUnsignedNumbersRequired_ButSignedUsed_ShouldFail()
        {
            var json = @"{ ""item"": -1 }";

            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.ToNavigation().WithCachedProperties();

            var item = nav["item"];
            item.GetUInt16OrDefault().ShouldBe(default);
            item.GetUInt32OrDefault().ShouldBe(default);
            item.GetUInt64OrDefault().ShouldBe(default);
            item.GetByteOrDefault().ShouldBe(default);
        }

        [Theory]
        [InlineData(@"{ ""item"": {} }")]
        [InlineData(@"{ ""item"": """" }")]
        [InlineData(@"{ ""item"": [] }")]
        [InlineData(@"{ ""item"": null }")]
        public void WhenValueIsNotNumber_ShouldFail(string json)
        {
            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.ToNavigation().WithCachedProperties();
            
            var item = nav["item"];
            item.Exist.ShouldBeTrue();
            item.GetInt16OrDefault().ShouldBe(default);
            item.GetInt32OrDefault().ShouldBe(default);
            item.GetInt64OrDefault().ShouldBe(default);
            item.GetUInt16OrDefault().ShouldBe(default);
            item.GetUInt32OrDefault().ShouldBe(default);
            item.GetUInt64OrDefault().ShouldBe(default);
            item.GetByteOrDefault().ShouldBe(default);
            item.GetDecimalOrDefault().ShouldBe(default);
            item.GetDoubleOrDefault().ShouldBe(default);
        }

        [Fact]
        public void WhenFloatsUsed_ShouldSucceed()
        {
            var json = @"{ ""item"": 2021.1025 }";
            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.ToNavigation().WithCachedProperties();
            
            var item = nav["item"];
            item.Exist.ShouldBeTrue();
            item.GetDecimalOrDefault().ShouldBe(2021.1025M);
            item.GetDoubleOrDefault().ShouldBe(2021.1025D);
            item.GetSingleOrDefault().ShouldBe((float)2021.1025);
        }

        [Fact]
        public void WhenValidByteUsed_ShouldSucceed()
        {
            var json = @"{ ""item"": 255 }";
            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.ToNavigation().WithCachedProperties();
            
            var item = nav["item"];
            item.Exist.ShouldBeTrue();
            item.GetByteOrDefault().ShouldBe((byte)255);
        }
        
        [Fact]
        public void WhenInvalidByteValueUsed_ShouldSucceed()
        {
            var json = @"{ ""item"": 256 }";
            var jsonDocument = JsonDocument.Parse(json);
            var nav = jsonDocument.ToNavigation().WithCachedProperties();
            
            var item = nav["item"];
            item.Exist.ShouldBeTrue();
            item.GetByteOrDefault().ShouldBe((byte)0);
        }
    }
}