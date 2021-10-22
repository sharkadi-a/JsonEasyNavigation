namespace System.Text.Json
{
    internal static class PropertyPathVisitor
    {
        public static NavigationElement Visit(JsonElement jsonElement, ArraySegment<string> properties)
        {
            while (true)
            {
                if (properties.Count < 1)
                {
                    return new NavigationElement(jsonElement, false);
                }
                
                var property = properties[0];
                if (jsonElement.ValueKind != JsonValueKind.Object) return default;
                
                if (!jsonElement.TryGetProperty(property, out var result)) return default;

                jsonElement = result;
                properties = properties[1..];
            }
        }
    }
}