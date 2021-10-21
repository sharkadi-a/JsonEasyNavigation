using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System.Text.Json
{
    public readonly partial struct NavigationElement : IReadOnlyList<NavigationElement>
    {
        public NavigationElement this[string property] =>
            JsonElement.TryGetProperty(property, out var p) ? new NavigationElement(p, _isStablePropertyOrder) : default;

        public NavigationElement this[int index]
        {
            get
            {
                if (JsonElement.ValueKind == JsonValueKind.Array)
                {
                    var len = JsonElement.GetArrayLength();
                    return index < len ? new NavigationElement(JsonElement[index], _isStablePropertyOrder) : default;
                }

                if (JsonElement.ValueKind == JsonValueKind.Object)
                {
                    var c = 0;
                    foreach (var property in JsonElement.EnumerateObject())
                    {
                        if (c == index) return new NavigationElement(property.Value, _isStablePropertyOrder);
                        c++;
                    }
                }

                return default;
            }
        }
        
        public IEnumerator<NavigationElement> GetEnumerator()
        {
            if (JsonElement.ValueKind == JsonValueKind.Array)
            {
                return new ArrayEnumeratorWrapper(JsonElement);
            }

            if (JsonElement.ValueKind == JsonValueKind.Object)
            {
                return new ObjectEnumeratorWrapper(JsonElement, _isStablePropertyOrder);
            }

            return Enumerable.Empty<NavigationElement>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}