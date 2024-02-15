namespace JsonEasyNavigation
{
    internal static class PropertyPathVisitor
    {
        public static JsonNavigationElement Visit(JsonElement jsonElement, ArraySegment<string> properties)
        {
            while (true)
            {
                if (properties.Count < 1)
                {
                    return new JsonNavigationElement(jsonElement, false, false);
                }
                
                var property = properties.First();
                if (jsonElement.ValueKind != JsonValueKind.Object) return default;
                
                if (!jsonElement.TryGetProperty(property, out var result)) return default;

                jsonElement = result;

                var offset = properties.Offset + 1;
                if (offset > properties.Array.Length) break;
                
                properties = new ArraySegment<string>(properties.Array, 
                    offset,
                    properties.Count - 1);
            }

            return default;
        }
    }
}