using System.Collections.Generic;

namespace System.Text.Json
{
    /// <summary>
    /// Represents a wrapper around <see cref="JsonElement"/>, which allows to navigate it's
    /// properties or array items like in a dictionary or a list. <see cref="JsonNavigationElement"/>
    /// is read-only and immutable.
    /// </summary>
    public readonly partial struct JsonNavigationElement
    {
        internal bool IsStablePropertyOrder { get; }
        
        /// <summary>
        /// Does <see cref="JsonElement"/> exists in JSON structure? This allows to check if JSON element being searched exist
        /// or not.
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
        /// Returns true, if <see cref="JsonElement"/> has a name (i.e. being a property).
        /// </summary>
        public bool HasName { get; }
        
        /// <summary>
        /// The name of <see cref="JsonElement"/>.
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Returns true, if <see cref="JsonElement"/> has an index in array (i.e. being an array item).
        /// </summary>
        public bool HasIndex { get; }
        
        /// <summary>
        /// Return index of <see cref="JsonElement"/> in a array OR index of a property in a object. Not all properties have index,
        /// ONLY those, which were accessed via the int indexer.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Returns true if <see cref="JsonElement"/> can be enumerated as an array or an object.
        /// </summary>
        public bool IsEnumerable => JsonElement.ValueKind is JsonValueKind.Array or JsonValueKind.Object;
        
        /// <summary>
        /// An actual <see cref="JsonElement"/> being wrapped around.
        /// </summary>
        public JsonElement JsonElement { get; }

        private JsonNavigationElement(JsonElement jsonElement, string name, int index, bool isStablePropertyOrder) : this(
            jsonElement, isStablePropertyOrder)
        {
            Name = name;
            HasName = true;
            Index = index;
            HasIndex = true;
        }

        private JsonNavigationElement(JsonElement jsonElement, string name, bool isStablePropertyOrder) : this(
            jsonElement, isStablePropertyOrder)
        {
            Name = name;
            HasName = true;
        }

        private JsonNavigationElement(JsonElement jsonElement, int index, bool isStablePropertyOrder) : this(
            jsonElement, isStablePropertyOrder)
        {
            Index = index;
            HasIndex = true;
        }
        
        internal JsonNavigationElement(JsonElement jsonElement, bool isStablePropertyOrder)
        {
            IsStablePropertyOrder = isStablePropertyOrder;
            JsonElement = jsonElement;
            Exist = !default(JsonElement).Equals(jsonElement);
            Name = null;
            Index = 0;
            HasIndex = false;
            HasName = false;
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
            return JsonElement.TryGetInt32(out var value) ? value : default;
        }        
        
        public long GetInt64OrDefault()
        {
            return JsonElement.TryGetInt64(out var value) ? value : default;
        }        
        
        public short GetInt16OrDefault()
        {
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