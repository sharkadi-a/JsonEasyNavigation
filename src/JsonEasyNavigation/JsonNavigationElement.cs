using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;

namespace JsonEasyNavigation
{
    /// <summary>
    /// Represents a wrapper around <see cref="JsonElement"/>, which allows to navigate it's
    /// properties or array items like in a dictionary or a list. <see cref="JsonNavigationElement"/>
    /// is read-only and immutable.
    /// </summary>
    public readonly partial struct JsonNavigationElement : IEquatable<JsonNavigationElement>
    {
        internal bool IsStablePropertyOrder { get; }

        internal bool HasCachedProperties => _properties != default;

        /// <summary>
        /// Returns true if the <see cref="JsonElement"/> is null. The element can exist (i.e. <see cref="Exist"/>
        /// is true) but still be null.
        /// </summary>
        public bool IsNullValue => JsonElement.ValueKind == JsonValueKind.Null || !Exist;
        
        /// <summary>
        /// Does <see cref="JsonElement"/> exists in JSON structure? This allows to check if JSON element being
        /// searched exist or not.
        /// </summary>
        /// <example>
        /// Say, there is JSON document:<br/>
        /// {<br/>
        ///     "item1": 1,<br/>
        ///     "item2": 2<br/>
        /// }<br/>
        /// <br/>
        /// And a navigation object:<br/>
        /// var nav = JsonDocument.Parse(...).ToNavigation();<br/>
        /// <br/>
        /// You can check if element you want exist:<br/>
        /// nav["item3"].Exist<br/>
        /// <br/>
        /// This will return false, because there is no property with name "item3".<br/>
        /// But this code will return true:<br/>
        /// nav["item1"].Exist;<br/>
        /// </example>
        public bool Exist { get; }
        
        /// <summary>
        /// Returns true, if the <see cref="JsonElement"/> has a name (i.e. being a property).
        /// </summary>
        public bool HasName { get; }
        
        /// <summary>
        /// The name of the <see cref="JsonElement"/>.
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Returns true, if the <see cref="JsonElement"/> has an index in array (i.e. being an array item).
        /// </summary>
        public bool HasIndex { get; }
        
        /// <summary>
        /// Return index of the <see cref="JsonElement"/> in a array OR index of a property in a object. Not all properties have index,
        /// ONLY those, which were accessed via the int indexer.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Returns true if the <see cref="JsonElement"/> can be enumerated as an array or an object.
        /// </summary>
        public bool IsEnumerable => JsonElement.ValueKind is JsonValueKind.Array or JsonValueKind.Object;
        
        /// <summary>
        /// An actual <see cref="JsonElement"/> being wrapped around.
        /// </summary>
        public JsonElement JsonElement { get; }

        private readonly Lazy<JsonProperty[]> _properties;

        private JsonNavigationElement(JsonElement jsonElement, string name, int index, bool isStablePropertyOrder, bool precacheProperties) : this(
            jsonElement, isStablePropertyOrder, precacheProperties)
        {
            Name = name;
            HasName = true;
            Index = index;
            HasIndex = true;
        }

        private JsonNavigationElement(JsonElement jsonElement, string name, bool isStablePropertyOrder, bool precacheProperties) : this(
            jsonElement, isStablePropertyOrder, precacheProperties)
        {
            Name = name;
            HasName = true;
        }

        private JsonNavigationElement(JsonElement jsonElement, int index, bool isStablePropertyOrder, bool precacheProperties) : this(
            jsonElement, isStablePropertyOrder, precacheProperties)
        {
            Index = index;
            HasIndex = true;
        }

        internal JsonNavigationElement(JsonProperty jsonProperty, int index, bool isStablePropertyOrder,
            bool cacheProperties) : this(jsonProperty.Value, jsonProperty.Name, index, isStablePropertyOrder,
            cacheProperties)
        {

        }

        internal JsonNavigationElement(JsonElement jsonElement, bool isStablePropertyOrder, bool cacheProperties)
        {
            IsStablePropertyOrder = isStablePropertyOrder;
            JsonElement = jsonElement;
            Exist = !default(JsonElement).Equals(jsonElement);
            Name = null;
            Index = 0;
            HasIndex = false;
            HasName = false;

            _properties = cacheProperties
                ? new Lazy<JsonProperty[]>(() => isStablePropertyOrder
                    ? jsonElement.EnumerateObject().OrderBy(x => x.Name).ToArray()
                    : jsonElement.EnumerateObject().ToArray(), LazyThreadSafetyMode.ExecutionAndPublication)
                : default;
        }

        /// <summary>
        /// Returns wrapped <see cref="JsonElement"/> as a <see cref="string"/> or it's default value, if this element is not a string.
        /// This method should never throw an exception. May allocate is the heap.
        /// </summary>
        public string GetStringOrDefault()
        {
            return JsonElement.ValueKind == JsonValueKind.String ? JsonElement.ToString() : default;
        }

        /// <summary>
        /// Returns wrapped <see cref="JsonElement"/> as a <see cref="string"/>  or <see cref="string.Empty"/>, if this element is not a string.
        /// This method should never throw an exception. May allocate is the heap.
        /// </summary>
        public string GetStringOrEmpty() => GetStringOrDefault() ?? string.Empty;

        /// <summary>
        /// Returns wrapped <see cref="JsonElement"/> as an <see cref="int"/> or it's default value.
        /// </summary>
        public int GetInt32OrDefault()
        {
            if (JsonElement.ValueKind != JsonValueKind.Number) return default;
            return JsonElement.TryGetInt32(out var value) ? value : default;
        }        
        
        /// <summary>
        /// Returns wrapped <see cref="JsonElement"/> as an <see cref="long"/> or it's default value.
        /// </summary>
        public long GetInt64OrDefault()
        {
            if (JsonElement.ValueKind != JsonValueKind.Number) return default;
            return JsonElement.TryGetInt64(out var value) ? value : default;
        }        
        
        /// <summary>
        /// Returns wrapped <see cref="JsonElement"/> as an <see cref="short"/> or it's default value.
        /// </summary>
        public short GetInt16OrDefault()
        {
            if (JsonElement.ValueKind != JsonValueKind.Number) return default;
            return JsonElement.TryGetInt16(out var value) ? value : default;
        }

        /// <summary>
        /// Returns wrapped <see cref="JsonElement"/> as an <see cref="uint"/> or it's default value.
        /// </summary>
        public uint GetUInt32OrDefault()
        {
            if (JsonElement.ValueKind != JsonValueKind.Number) return default;
            return JsonElement.TryGetUInt32(out var value) ? value : default;
        }        
        
        /// <summary>
        /// Returns wrapped <see cref="JsonElement"/> as an <see cref="ulong"/> or it's default value.
        /// </summary>
        public ulong GetUInt64OrDefault()
        {
            if (JsonElement.ValueKind != JsonValueKind.Number) return default;
            return JsonElement.TryGetUInt64(out var value) ? value : default;
        }        
        
        /// <summary>
        /// Returns wrapped <see cref="JsonElement"/> as an <see cref="ushort"/> or it's default value.
        /// </summary>
        public ushort GetUInt16OrDefault()
        {
            if (JsonElement.ValueKind != JsonValueKind.Number) return default;
            return JsonElement.TryGetUInt16(out var value) ? value : default;
        }

        /// <summary>
        /// Returns wrapped <see cref="JsonElement"/> as a <see cref="decimal"/> or it's default value.
        /// </summary>
        public decimal GetDecimalOrDefault()
        {
            if (JsonElement.ValueKind != JsonValueKind.Number) return default;
            return JsonElement.TryGetDecimal(out var value) ? value : default;
        }
        
        /// <summary>
        /// Returns wrapped <see cref="JsonElement"/> as a <see cref="double"/> or it's default value.
        /// </summary>
        public double GetDoubleOrDefault()
        {
            if (JsonElement.ValueKind != JsonValueKind.Number) return default;
            return JsonElement.TryGetDouble(out var value) ? value : default;
        }
        
        /// <summary>
        /// Returns wrapped <see cref="JsonElement"/> as a <see cref="byte"/> or it's default value.
        /// </summary>
        public byte GetByteOrDefault()
        {
            if (JsonElement.ValueKind != JsonValueKind.Number) return default;
            return JsonElement.TryGetByte(out var value) ? value : default;
        }

        /// <summary>
        /// Returns wrapped <see cref="JsonElement"/> as a <see cref="sbyte"/> or it's default value.
        /// </summary>
        public sbyte GetSByteOrDefault()
        {
            if (JsonElement.ValueKind != JsonValueKind.Number) return default;
            return JsonElement.TryGetSByte(out var value) ? value : default;
        }

        /// <summary>
        /// Returns wrapped <see cref="JsonElement"/> as a <see cref="float"/> or it's default value.
        /// </summary>
        public float GetSingleOrDefault()
        {
            if (JsonElement.ValueKind != JsonValueKind.Number) return default;
            return JsonElement.TryGetSingle(out var value) ? value : default;
        }

        /// <summary>
        /// Returns wrapped <see cref="JsonElement"/> as a <see cref="Guid"/> or it's default value.
        /// </summary>
        public Guid GetGuidOrDefault()
        {
            if (JsonElement.ValueKind != JsonValueKind.String) return default;
            return JsonElement.TryGetGuid(out var value) ? value : default;
        }

        /// <summary>
        /// Returns wrapped <see cref="JsonElement"/> as a <see cref="DateTime"/> or it's default value.
        /// </summary>
        public DateTime GetDateTimeOrDefault()
        {
            if (JsonElement.ValueKind != JsonValueKind.String) return default;
            return JsonElement.TryGetDateTime(out var value) ? value : default;
        }

        /// <summary>
        /// Returns wrapped <see cref="JsonElement"/> as an <see cref="DateTimeOffset"/> or it's default value.
        /// </summary>
        public DateTimeOffset GetDateTimeOffsetOrDefault()
        {
            if (JsonElement.ValueKind != JsonValueKind.String) return default;
            return JsonElement.TryGetDateTimeOffset(out var value) ? value : default;
        }

        /// <summary>
        /// Returns wrapped <see cref="JsonElement"/> as an array of <see cref="byte"/> or it's default value.
        /// </summary>
        public byte[] GetBytesFromBase64OrDefault()
        {
            if (JsonElement.ValueKind != JsonValueKind.String) return default;
            return JsonElement.TryGetBytesFromBase64(out var value) ? value : Array.Empty<byte>();
        }

        /// <summary>
        /// Returns wrapped <see cref="JsonElement"/> as a <see cref="Stream"/> or it's default value.
        /// </summary>
        public Stream GetStreamFromBase64OrDefault()
        {
            return JsonElement.ValueKind != JsonValueKind.String
                ? Stream.Null
                : new MemoryStream(GetBytesFromBase64OrDefault());
        }

        /// <summary>
        /// Returns wrapped <see cref="JsonElement"/> as a <see cref="bool"/> or it's default value.
        /// </summary>
        public bool GetBooleanOrDefault()
        {
            return JsonElement.ValueKind == JsonValueKind.True;
        }

        /// <summary>
        /// Tries to get <see cref="JsonElement"/> value as of type <typeparam name="T"/> or returns false, if this fails.
        /// </summary>
        public bool TryGetValue<T>(out T value)
        {
            return PrimitiveValueExtractor.TryGetValue(JsonElement, out value);
        }

        /// <summary>
        /// Tries to get <see cref="JsonElement"/> value as of type <typeparam name="T"/> or returns it's default value,
        /// if this fails.
        /// </summary>
        public T GetValueOrDefault<T>()
        {
            return PrimitiveValueExtractor.TryGetValue(JsonElement, out T value) ? value : default;
        }
        
        /// <inheritdoc/>
        public bool Equals(JsonNavigationElement other)
        {
            return JsonElement.Equals(other.JsonElement);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is JsonNavigationElement other && Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return JsonElement.GetHashCode();
        }
    }
}