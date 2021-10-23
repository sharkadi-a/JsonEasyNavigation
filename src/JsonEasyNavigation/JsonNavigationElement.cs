using System;
using System.Collections.Generic;
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
    public readonly partial struct JsonNavigationElement
    {
        internal bool IsStablePropertyOrder { get; }

        internal bool CachedProperties => _properties != default;
        
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
        /// Returns wrapped <see cref="JsonElement"/> as a string or default value, if this element is not a string.
        /// This method should never throw an exception.
        /// </summary>
        public string GetStringOrDefault()
        {
            return JsonElement.ValueKind == JsonValueKind.String ? JsonElement.ToString() : default;
        }

        public string GetStringOrEmpty() => GetStringOrDefault() ?? string.Empty;

        public int GetInt32OrDefault()
        {
            if (JsonElement.ValueKind != JsonValueKind.Number) return default;
            return JsonElement.TryGetInt32(out var value) ? value : default;
        }        
        
        public long GetInt64OrDefault()
        {
            if (JsonElement.ValueKind != JsonValueKind.Number) return default;
            return JsonElement.TryGetInt64(out var value) ? value : default;
        }        
        
        public short GetInt16OrDefault()
        {
            if (JsonElement.ValueKind != JsonValueKind.Number) return default;
            return JsonElement.TryGetInt16(out var value) ? value : default;
        }

        public IEnumerable<int> GetEnumerableOfInt32OrEmpty()
        {
            foreach (var nav in this)
            {
                if (nav.JsonElement.TryGetInt32(out var value))
                {
                    yield return value;
                }
            }
        }
    }
}