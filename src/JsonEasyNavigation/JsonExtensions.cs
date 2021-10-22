using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Text.Json
{
    /// <summary>
    /// Various <see cref="JsonDocument"/>, <see cref="JsonElement"/> and <see cref="JsonNavigationElement"/> extensions
    /// and utilities.
    /// </summary>
    public static class JsonExtensions
    {
        /// <summary>
        /// Makes this <see cref="JsonNavigationElement"/> and all it's descendants to have a stable order of the
        /// properties in object-kind elements.
        /// </summary>
        public static JsonNavigationElement WithStablePropertyOrder(this JsonNavigationElement jsonNavigationElement)
        {
            return jsonNavigationElement.IsStablePropertyOrder
                ? jsonNavigationElement
                : new JsonNavigationElement(jsonNavigationElement.JsonElement, true);
        }

        /// <summary>
        /// Makes this <see cref="JsonNavigationElement"/> and all it's descendants to have an unstable order of the 
        /// properties in object-kind elements. This does not imply that elements will definitely have random order, 
        /// but the order is not guaranteed.
        /// </summary>
        public static JsonNavigationElement WithoutStablePropertyOrder(this JsonNavigationElement jsonNavigationElement)
        {
            return jsonNavigationElement.IsStablePropertyOrder
                ? new JsonNavigationElement(jsonNavigationElement.JsonElement, false)
                : jsonNavigationElement;
        }
        
        /// <summary>
        /// Creates a <see cref="JsonNavigationElement"/> wrapper from <see cref="JsonElement"/>, which allows to
        /// navigate through it's properties and array items like using dictionary or list.
        /// </summary>
        public static JsonNavigationElement ToNavigation(this JsonElement jsonElement)
        {
            return new JsonNavigationElement(jsonElement, false);
        }
        
        /// <summary>
        /// Creates a <see cref="JsonNavigationElement"/> wrapper from <see cref="JsonDocument"/>, which allows to
        /// navigate through it's properties and array items like using dictionary or list.
        /// </summary>
        public static JsonNavigationElement ToNavigation(this JsonDocument jsonDocument)
        {
            return new JsonNavigationElement(jsonDocument.RootElement, false);
        }

        /// <summary>
        /// Allows to navigate in-depth through <see cref="JsonElement"/> properties by their names.
        /// </summary>
        public static JsonNavigationElement Property(this JsonElement jsonElement, params string[] propertyPath)
        {
            return PropertyPathVisitor.Visit(jsonElement, new ArraySegment<string>(propertyPath));
        }
        
        /// <summary>
        /// Allows to navigate in-depth through <see cref="JsonDocument"/> properties by their names.
        /// </summary>
        public static JsonNavigationElement Property(this JsonDocument jsonDocument, params string[] propertyPath)
        {
            return jsonDocument.RootElement.Property(propertyPath);
        }

        /// <summary>
        /// Apply selector to all the <see cref="JsonDocument"/> one-by-one and return an enumerable with
        /// resulting <see cref="JsonNavigationElement"/>.
        /// </summary>
        public static IEnumerable<JsonNavigationElement> NavigateSelect(this IEnumerable<JsonDocument> jsonDocuments,
            Func<JsonNavigationElement, JsonNavigationElement> selector)
        {
            return jsonDocuments.Select(x => x.RootElement).NavigateSelect(selector);
        }
        
        /// <summary>
        /// Apply selector to all the <see cref="JsonElement"/> one-by-one and return an enumerable with
        /// resulting <see cref="JsonNavigationElement"/>.
        /// </summary>
        public static IEnumerable<JsonNavigationElement> NavigateSelect(this IEnumerable<JsonElement> jsonElements,
            Func<JsonNavigationElement, JsonNavigationElement> selector)
        {
            return jsonElements.Select(element => selector(element.ToNavigation()));
        }
    }
}