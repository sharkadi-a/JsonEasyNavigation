using System.Collections;

namespace JsonEasyNavigation;

internal struct ObjectEnumeratorWrapper : IEnumerator<JsonNavigationElement>, IEnumerator<KeyValuePair<string, JsonNavigationElement>>
{
    private readonly JsonNavigationElement     _element;
    private readonly IEnumerator<JsonProperty> _enumerator;
    private          int                       counter;

    public ObjectEnumeratorWrapper(JsonNavigationElement element, bool stable)
    {
        _element = element;
        var jsonElement = element.JsonElement;
            
        if (jsonElement.ValueKind != JsonValueKind.Object)
            throw new InvalidOperationException("JsonElement must be of 'Object' kind");
            
        var enumerator = jsonElement.EnumerateObject();
        _enumerator = stable ? enumerator.OrderBy(x => x.Name).GetEnumerator() : enumerator;
        counter     = -1;
    }

    public ObjectEnumeratorWrapper(JsonNavigationElement element, IEnumerable<JsonProperty> jsonProperties)
    {
        _enumerator = jsonProperties.GetEnumerator();
        _element    = element;
        counter     = -1;
    }
        
    public bool MoveNext()
    {
        ++counter;
        return _enumerator.MoveNext();
    }

    public void Reset()
    {
        _enumerator.Reset();
        counter = -1;
    }

    KeyValuePair<string, JsonNavigationElement> IEnumerator<KeyValuePair<string, JsonNavigationElement>>.Current => new(_enumerator.Current.Name, Current);

    public JsonNavigationElement Current => new(_enumerator.Current,
                                                counter, _element.IsStablePropertyOrder, _element.HasCachedProperties);

    object IEnumerator.Current => Current;

    public void Dispose() => _enumerator.Dispose();
}