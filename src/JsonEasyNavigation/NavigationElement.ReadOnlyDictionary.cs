using System.Collections.Generic;
using System.Linq;

namespace System.Text.Json
{
    public readonly partial struct NavigationElement : IReadOnlyDictionary<string, NavigationElement>
    {
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

        IEnumerator<KeyValuePair<string, NavigationElement>> IEnumerable<KeyValuePair<string, NavigationElement>>.GetEnumerator()
        {
            if (JsonElement.ValueKind != JsonValueKind.Object)
                return Enumerable.Empty<KeyValuePair<string, NavigationElement>>().GetEnumerator();

            return new ObjectEnumeratorWrapper(JsonElement, _isStablePropertyOrder);
        }

        public bool ContainsKey(string key)
        {
            return this[key].Exist;
        }

        public bool TryGetValue(string key, out NavigationElement value)
        {
            value = this[key];
            return value.Exist;
        }

        public IEnumerable<string> Keys => JsonElement.EnumerateObject().Select(x => x.Name);

        public IEnumerable<NavigationElement> Values
        {
            get
            {
                var isStable = _isStablePropertyOrder;
                
                if (JsonElement.ValueKind == JsonValueKind.Array)
                {
                    return JsonElement.EnumerateArray().Select(x => new NavigationElement(x, isStable));
                }

                if (JsonElement.ValueKind == JsonValueKind.Object)
                {
                    return JsonElement.EnumerateObject().Select(x => new NavigationElement(x.Value, isStable));
                }
                
                return Enumerable.Empty<NavigationElement>();
            }
        }
    }
}