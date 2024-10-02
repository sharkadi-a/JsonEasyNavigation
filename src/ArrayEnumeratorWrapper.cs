using System.Collections;

namespace JsonEasyNavigation;

internal struct ArrayEnumeratorWrapper : IEnumerator<JsonNavigationElement>
{
    private JsonElement.ArrayEnumerator _enumerator;
        
    public ArrayEnumeratorWrapper(JsonElement jsonElement)
    {
        if (jsonElement.ValueKind != JsonValueKind.Array)
            throw new InvalidOperationException("JsonElement must be of 'Array' kind");
        _enumerator = jsonElement.EnumerateArray();
    }
        
    public bool MoveNext() => _enumerator.MoveNext();

    public void Reset() => _enumerator.Reset();

    public JsonNavigationElement Current => _enumerator.Current.ToNavigation();

    object IEnumerator.Current => Current;

    public void Dispose() => _enumerator.Dispose();
}