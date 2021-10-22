using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System.Text.Json
{
    public readonly partial struct JsonNavigationElement : IReadOnlyList<JsonNavigationElement>
    {
        public JsonNavigationElement this[int index]
        {
            get
            {
                if (JsonElement.ValueKind == JsonValueKind.Array)
                {
                    var len = JsonElement.GetArrayLength();
                    return index < len ? new JsonNavigationElement(JsonElement[index], index, IsStablePropertyOrder) : default;
                }

                if (JsonElement.ValueKind == JsonValueKind.Object)
                {
                    var c = 0;
                    IEnumerable<JsonProperty> enumerator = IsStablePropertyOrder
                        ? JsonElement.EnumerateObject().OrderBy(x => x.Name)
                        : JsonElement.EnumerateObject();
                    
                    foreach (var property in enumerator)
                    {
                        if (c == index) return new JsonNavigationElement(property.Value, property.Name, c, IsStablePropertyOrder);
                        c++;
                    }
                }

                return default;
            }
        }
        
        public IEnumerator<JsonNavigationElement> GetEnumerator()
        {
            if (JsonElement.ValueKind == JsonValueKind.Array)
            {
                return new ArrayEnumeratorWrapper(JsonElement);
            }

            if (JsonElement.ValueKind == JsonValueKind.Object)
            {
                return new ObjectEnumeratorWrapper(JsonElement, IsStablePropertyOrder);
            }

            return Enumerable.Empty<JsonNavigationElement>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}