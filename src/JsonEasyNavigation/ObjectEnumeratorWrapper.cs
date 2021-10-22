using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System.Text.Json
{
    internal class ObjectEnumeratorWrapper : IEnumerator<JsonNavigationElement>, IEnumerator<KeyValuePair<string, JsonNavigationElement>>
    {
        private IEnumerator<JsonProperty> _enumerator;

        public ObjectEnumeratorWrapper(JsonElement jsonElement, bool stable)
        {
            if (jsonElement.ValueKind != JsonValueKind.Object)
                throw new InvalidOperationException("JsonElement must be of 'Object' kind");
            
            var enumerator = jsonElement.EnumerateObject();
            _enumerator = stable ? enumerator.OrderBy(x => x.Name).GetEnumerator() : enumerator;
        }
        
        public bool MoveNext()
        {
            return _enumerator.MoveNext();
        }

        public void Reset()
        {
            _enumerator.Reset();
        }

        KeyValuePair<string, JsonNavigationElement> IEnumerator<KeyValuePair<string, JsonNavigationElement>>.Current =>
            new(_enumerator.Current.Name, Current);

        public JsonNavigationElement Current => _enumerator.Current.Value.ToNavigation();

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            _enumerator.Dispose();
        }
    }
}