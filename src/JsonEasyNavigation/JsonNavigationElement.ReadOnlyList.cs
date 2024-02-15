using System.Collections;

namespace JsonEasyNavigation
{
    public readonly partial struct JsonNavigationElement : IReadOnlyList<JsonNavigationElement>
    {
        /// <summary>
        /// Returns a <see cref="JsonNavigationElement"/> of the <see cref="JsonElement"/> at required position. This
        /// index works for both arrays and objects (the latter returns properties). Keep in mind, that the order of
        /// properties in an object is not guaranteed. You should use <see cref="JsonExtensions.WithStablePropertyOrder"/>
        /// to persist the order of properties (sorted by their names).
        /// </summary>
        /// <example>
        /// Say, there is JSON document:<br/>
        /// {<br/>
        ///     "item1": 1,<br/>
        ///     "item2": 2<br/>
        /// }<br/>
        /// And a navigation object:<br/>
        /// var nav = JsonDocument.Parse(...).ToNavigation();<br/>
        /// <br/>
        /// We can get property using the indexer of <see cref="JsonNavigationElement"/>:<br/>
        /// var number = nav[0].TryGetInt32OrDefault();<br/>
        /// The number should be 1.<br/>
        /// <br/>
        /// To be sure, that properties have predicted order, we can use method <see cref="JsonExtensions.WithStablePropertyOrder"/>: <br/>
        /// var number = nav.WithStablePropertyOrder()[0];<br/>
        /// The number value is always be 1.<br/> 
        /// <br/>
        /// Now, let's say we have an array:<br/>
        /// [ 1, 2 ]<br/>
        /// Make a navigation object:<br/>
        /// var nav = JsonDocument.Parse(...).ToNavigation();<br/>
        /// <br/>
        /// We can access the first element the same way:<br/>
        /// var number = nav[0].TryGetInt32OrDefault();<br/>
        /// The value should be 1.
        /// </example>
        public JsonNavigationElement this[int index]
        {
            get
            {
                if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
                
                if (JsonElement.ValueKind == JsonValueKind.Array)
                {
                    var len = JsonElement.GetArrayLength();
                    return index < len
                        ? new JsonNavigationElement(JsonElement[index], index, IsStablePropertyOrder,
                            HasCachedProperties)
                        : default;
                }

                if (JsonElement.ValueKind == JsonValueKind.Object)
                {
                    if (HasCachedProperties)
                    {
                        if (_properties.Value.Length == 0) return default;
                        if (index >= 0 && index < _properties.Value.Length)
                        {
                            var property = _properties.Value[index];
                            return new JsonNavigationElement(property.Value, property.Name, IsStablePropertyOrder,
                                HasCachedProperties);
                        }
                    }
                    
                    var c = 0;
                    IEnumerable<JsonProperty> enumerator = IsStablePropertyOrder
                        ? JsonElement.EnumerateObject().OrderBy(x => x.Name)
                        : (IEnumerable<JsonProperty>)JsonElement.EnumerateObject();
                    
                    foreach (var property in enumerator)
                    {
                        if (c == index)
                            return new JsonNavigationElement(property.Value, property.Name, c, IsStablePropertyOrder,
                                HasCachedProperties);
                        c++;
                    }
                }

                return default;
            }
        }
        
        /// <inheritdoc/>
        public IEnumerator<JsonNavigationElement> GetEnumerator()
        {
            if (JsonElement.ValueKind == JsonValueKind.Array)
            {
                return new ArrayEnumeratorWrapper(JsonElement);
            }

            if (JsonElement.ValueKind == JsonValueKind.Object)
            {
                return HasCachedProperties
                    ? new ObjectEnumeratorWrapper(this, _properties.Value)
                    : new ObjectEnumeratorWrapper(this, IsStablePropertyOrder);
            }

            return Enumerable.Empty<JsonNavigationElement>().GetEnumerator();
        }
        
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}