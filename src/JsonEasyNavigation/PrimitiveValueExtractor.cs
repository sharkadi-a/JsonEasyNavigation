using System;
using System.Text.Json;

namespace JsonEasyNavigation
{
    internal static class PrimitiveValueExtractor
    {
        public static bool TryGetValue<T>(JsonElement element, out T value)
        {
            var type = typeof(T);
            value = default;
            var isSuccess = false;

            if (type == typeof(int))
            {
                isSuccess = element.TryGetInt32(out var integer32);
                value = BoxingSafeConverter<int, T>.Instance.Convert(integer32);
            }

            return isSuccess;
        }
    }
}