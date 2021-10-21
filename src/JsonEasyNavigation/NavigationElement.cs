using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System.Text.Json
{
    public readonly partial struct NavigationElement
    {
        private readonly bool _isStablePropertyOrder;
        public bool Exist { get; }

        public bool IsEnumerable => JsonElement.ValueKind is JsonValueKind.Array or JsonValueKind.Object;
        
        public JsonElement JsonElement { get; }
        
        internal NavigationElement(JsonElement jsonElement, bool isStablePropertyOrder)
        {
            _isStablePropertyOrder = isStablePropertyOrder;
            JsonElement = jsonElement;
            Exist = !default(JsonElement).Equals(jsonElement);
        }

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