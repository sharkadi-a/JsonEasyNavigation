using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Text.Json
{
    public static class JsonExtensions
    {
        public static NavigationElement WithStablePropertyOrder(this NavigationElement navigationElement)
        {
            return new NavigationElement(navigationElement.JsonElement, true);
        }
        
        public static NavigationElement ToNavigation(this JsonElement jsonElement)
        {
            return new NavigationElement(jsonElement, false);
        }
        
        public static NavigationElement ToNavigation(this JsonDocument jsonDocument)
        {
            return new NavigationElement(jsonDocument.RootElement, false);
        }

        public static NavigationElement Property(this JsonElement jsonElement, params string[] propertyPath)
        {
            return PropertyPathVisitor.Visit(jsonElement, new ArraySegment<string>(propertyPath));
        }

        public static IEnumerable<NavigationElement> ToNavigation(this IEnumerable<JsonElement> jsonElements,
            Func<NavigationElement, NavigationElement> selector)
        {
            return jsonElements.Select(element => selector(element.ToNavigation()));
        }
    }
}