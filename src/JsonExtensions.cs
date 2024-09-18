namespace JsonEasyNavigation;

/// <summary>
/// Various <see cref="JsonDocument"/>, <see cref="JsonElement"/> and <see cref="JsonNavigationElement"/> extensions
/// and utilities.
/// </summary>
public static class JsonExtensions
{
    /// <summary>
    /// Makes the <see cref="JsonNavigationElement"/> and all it's descendants to have a stable order of the
    /// properties in object-kind elements.
    /// </summary>
    public static JsonNavigationElement WithStablePropertyOrder(this JsonNavigationElement jsonNavigationElement) => jsonNavigationElement.IsStablePropertyOrder
                                                                                                                         ? jsonNavigationElement
                                                                                                                         : new(jsonNavigationElement.JsonElement, true, jsonNavigationElement.HasCachedProperties);

    /// <summary>
    /// Makes the <see cref="JsonNavigationElement"/> and all it's descendants to have an unstable order (well,
    /// may be) of the properties in object-kind elements. This does not imply that elements will definitely have
    /// random order, but the order is not guaranteed.
    /// </summary>
    public static JsonNavigationElement WithoutStablePropertyOrder(this JsonNavigationElement jsonNavigationElement) => jsonNavigationElement.IsStablePropertyOrder
                                                                                                                            ? new(jsonNavigationElement.JsonElement, false, jsonNavigationElement.HasCachedProperties)
                                                                                                                            : jsonNavigationElement;

    /// <summary>
    /// Makes the <see cref="JsonNavigationElement"/> and all it's descendants to have properties cached in an array.
    /// May be useful if <see cref="JsonNavigationElement"/> is being used multiple time to access it's <see cref="JsonElement"/>
    /// properties. Allocates memory in heap (just once, when enumeration is executed).
    /// </summary>
    public static JsonNavigationElement WithCachedProperties(this JsonNavigationElement jsonNavigationElement) => jsonNavigationElement.HasCachedProperties
                                                                                                                      ? jsonNavigationElement
                                                                                                                      : new(jsonNavigationElement.JsonElement,
                                                                                                                            jsonNavigationElement.IsStablePropertyOrder, true);

    /// <summary>
    /// Makes the <see cref="JsonNavigationElement"/> and all it's descendants to have all properties not to
    /// be cached. 
    /// </summary>
    /// <param name="jsonNavigationElement"></param>
    /// <returns></returns>
    public static JsonNavigationElement WithoutCachedProperties(this JsonNavigationElement jsonNavigationElement) => jsonNavigationElement.HasCachedProperties
                                                                                                                         ? new(jsonNavigationElement.JsonElement,
                                                                                                                               jsonNavigationElement.IsStablePropertyOrder, false)
                                                                                                                         : jsonNavigationElement;

    /// <summary>
    /// Creates a <see cref="JsonNavigationElement"/> wrapper from <see cref="JsonElement"/>, which allows to
    /// navigate through it's properties and array items like using dictionary or list.
    /// </summary>
    public static JsonNavigationElement ToNavigation(this JsonElement jsonElement) => new(jsonElement, false, false);

    /// <summary>
    /// Creates a <see cref="JsonNavigationElement"/> wrapper from <see cref="JsonDocument"/>, which allows to
    /// navigate through it's properties and array items like using dictionary or list.
    /// </summary>
    public static JsonNavigationElement ToNavigation(this JsonDocument jsonDocument) => new(jsonDocument.RootElement, false, false);

    /// <summary>
    /// Allows to navigate in-depth through <see cref="JsonElement"/> properties by their names.
    /// </summary>
    public static JsonNavigationElement Property(this JsonElement jsonElement, params string[] propertyPath) => PropertyPathVisitor.Visit(jsonElement, new(propertyPath));

    /// <summary>
    /// Allows to navigate in-depth through <see cref="JsonDocument"/> properties by their names.
    /// </summary>
    public static JsonNavigationElement Property(this JsonDocument jsonDocument, params string[] propertyPath) => jsonDocument.RootElement.Property(propertyPath);

    /// <summary>
    /// Apply selector to all the <see cref="JsonDocument"/> one-by-one and return an enumerable with
    /// resulting <see cref="JsonNavigationElement"/>.
    /// </summary>
    public static IEnumerable<JsonNavigationElement> NavigateSelect(this IEnumerable<JsonDocument> jsonDocuments,
                                                                    Func<JsonNavigationElement, JsonNavigationElement> selector) => jsonDocuments.Select(x => x.RootElement).NavigateSelect(selector);

    /// <summary>
    /// Apply selector to all the <see cref="JsonElement"/> one-by-one and return an enumerable with
    /// resulting <see cref="JsonNavigationElement"/>.
    /// </summary>
    public static IEnumerable<JsonNavigationElement> NavigateSelect(this IEnumerable<JsonElement> jsonElements,
                                                                    Func<JsonNavigationElement, JsonNavigationElement> selector) => jsonElements.Select(element => selector(element.ToNavigation()));
}