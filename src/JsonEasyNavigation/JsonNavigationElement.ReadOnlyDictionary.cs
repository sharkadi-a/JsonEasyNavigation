namespace JsonEasyNavigation
{
    public readonly partial struct JsonNavigationElement : IReadOnlyDictionary<string, JsonNavigationElement>
    {
        /// <summary>
        /// Returns a property of <see cref="JsonElement"/> having a given name. If there is no such property or
        /// <see cref="JsonElement"/> is not an object at all, and empty <see cref="JsonNavigationElement"/>
        /// will be returned (and it's Exist property will be false).
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
        /// var number = nav["item1"].TryGetInt32OrDefault();<br/>
        /// The number is 1, as expected.
        /// </example>
        public JsonNavigationElement this[string property]
        {
            get
            {
                if (JsonElement.ValueKind != JsonValueKind.Object)
                {
                    return default;
                }
                
                return JsonElement.TryGetProperty(property, out var p)
                    ? new JsonNavigationElement(p, property, IsStablePropertyOrder, HasCachedProperties)
                    : default;
            }
        }

        /// <summary>
        /// A total number of array items or property count in the <see cref="JsonElement"/> or 0 for other kind of elements.
        /// </summary>
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
                    return HasCachedProperties ? _properties.Value.Length : JsonElement.EnumerateObject().Count();
                }

                return default;
            }
        }

        /// <inheritdoc/>
        IEnumerator<KeyValuePair<string, JsonNavigationElement>> IEnumerable<KeyValuePair<string, JsonNavigationElement>>.GetEnumerator()
        {
            if (JsonElement.ValueKind != JsonValueKind.Object)
                return Enumerable.Empty<KeyValuePair<string, JsonNavigationElement>>().GetEnumerator();

            return HasCachedProperties
                ? new ObjectEnumeratorWrapper(this, _properties.Value)
                : new ObjectEnumeratorWrapper(this, IsStablePropertyOrder);
        }

        /// <summary>
        /// Returns true if the <see cref="JsonElement"/> is an object and contains a property with a give name.
        /// </summary>
        public bool ContainsKey(string key)
        {
            return this[key].Exist;
        }
        
        /// <summary>
        /// Returns true and a <see cref="JsonNavigationElement"/> if the <see cref="JsonElement"/> contains a property
        /// of a given name.
        /// </summary>
        public bool TryGetValue(string key, out JsonNavigationElement value)
        {
            value = this[key];
            return value.Exist;
        }

        /// <summary>
        /// Returns all property names of the <see cref="JsonElement"/>.
        /// </summary>
        public IEnumerable<string> Keys
        {
            get
            {
                if (JsonElement.ValueKind != JsonValueKind.Object)
                {
                    return Enumerable.Empty<string>();
                }
                
                return HasCachedProperties
                    ? _properties.Value.Select(x => x.Name)
                    : JsonElement.EnumerateObject().Select(x => x.Name);
            }
        }

        /// <summary>
        /// Returns all properties or array items of the <see cref="JsonElement"/> or empty enumerable.
        /// </summary>
        public IEnumerable<JsonNavigationElement> Values
        {
            get
            {
                var isStable = IsStablePropertyOrder;
                var preCache = HasCachedProperties;
                
                if (JsonElement.ValueKind == JsonValueKind.Array)
                {
                    return JsonElement.EnumerateArray().Select(x => new JsonNavigationElement(x, isStable, preCache));
                }

                if (JsonElement.ValueKind == JsonValueKind.Object)
                {
                    IEnumerable<JsonProperty> properties = HasCachedProperties
                        ? _properties.Value
                        : (IEnumerable<JsonProperty>)JsonElement.EnumerateObject();
                    return properties.Select(x => new JsonNavigationElement(x.Value, isStable, preCache));
                }
                
                return Enumerable.Empty<JsonNavigationElement>();
            }
        }
    }
}