using System.Collections.Generic;
using System.Linq;

namespace System.Text.Json
{
    public readonly partial struct JsonNavigationElement : IReadOnlyDictionary<string, JsonNavigationElement>
    {
        public JsonNavigationElement this[string property] =>
            JsonElement.TryGetProperty(property, out var p) ? new JsonNavigationElement(p, property, IsStablePropertyOrder) : default;

        public int Count
        {
            get
            {
                if (JsonElement.ValueKind == JsonValueKind.Array)
                {
                    return JsonElement.GetArrayLength();
                }
                
                if (JsonElement.ValueKind == JsonValueKind.Object)
                {
                    return JsonElement.EnumerateObject().Count();
                }

                return default;
            }
        }

        IEnumerator<KeyValuePair<string, JsonNavigationElement>> IEnumerable<KeyValuePair<string, JsonNavigationElement>>.GetEnumerator()
        {
            if (JsonElement.ValueKind != JsonValueKind.Object)
                return Enumerable.Empty<KeyValuePair<string, JsonNavigationElement>>().GetEnumerator();

            return new ObjectEnumeratorWrapper(JsonElement, IsStablePropertyOrder);
        }

        public bool ContainsKey(string key)
        {
            return this[key].Exist;
        }

        public bool TryGetValue(string key, out JsonNavigationElement value)
        {
            value = this[key];
            return value.Exist;
        }

        public IEnumerable<string> Keys => JsonElement.EnumerateObject().Select(x => x.Name);

        public IEnumerable<JsonNavigationElement> Values
        {
            get
            {
                var isStable = IsStablePropertyOrder;
                
                if (JsonElement.ValueKind == JsonValueKind.Array)
                {
                    return JsonElement.EnumerateArray().Select(x => new JsonNavigationElement(x, isStable));
                }

                if (JsonElement.ValueKind == JsonValueKind.Object)
                {
                    return JsonElement.EnumerateObject().Select(x => new JsonNavigationElement(x.Value, isStable));
                }
                
                return Enumerable.Empty<JsonNavigationElement>();
            }
        }
    }
}