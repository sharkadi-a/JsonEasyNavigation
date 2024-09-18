namespace JsonEasyNavigation;

internal static class PrimitiveValueExtractor
{
    public static bool TryGetValue<T>(JsonElement element, out T value)
    {
        var type = typeof(T);
        value = default;
        var isSuccess = false;

        if (type == typeof(int) && element.ValueKind == JsonValueKind.Number)
        {
            isSuccess = element.TryGetInt32(out var v);
            if (isSuccess) value = BoxingSafeConverter<int, T>.Instance.Convert(v);
        }
        else if (type == typeof(short) && element.ValueKind == JsonValueKind.Number)
        {
            isSuccess = element.TryGetInt16(out var v);
            if (isSuccess) value = BoxingSafeConverter<short, T>.Instance.Convert(v);
        }
        else if (type == typeof(long) && element.ValueKind == JsonValueKind.Number)
        {
            isSuccess = element.TryGetInt64(out var v);
            if (isSuccess) value = BoxingSafeConverter<long, T>.Instance.Convert(v);
        }
        else if (type == typeof(uint) && element.ValueKind == JsonValueKind.Number)
        {
            isSuccess = element.TryGetUInt32(out var v);
            if (isSuccess) value = BoxingSafeConverter<uint, T>.Instance.Convert(v);
        }
        else if (type == typeof(ushort) && element.ValueKind == JsonValueKind.Number)
        {
            isSuccess = element.TryGetUInt16(out var v);
            if (isSuccess) value = BoxingSafeConverter<ushort, T>.Instance.Convert(v);
        }
        else if (type == typeof(ulong) && element.ValueKind == JsonValueKind.Number)
        {
            isSuccess = element.TryGetUInt64(out var v);
            if (isSuccess) value = BoxingSafeConverter<ulong, T>.Instance.Convert(v);
        }
        else if (type == typeof(byte) && element.ValueKind == JsonValueKind.Number)
        {
            isSuccess = element.TryGetByte(out var v);
            if (isSuccess) value = BoxingSafeConverter<byte, T>.Instance.Convert(v);
        }
        else if (type == typeof(sbyte) && element.ValueKind == JsonValueKind.Number)
        {
            isSuccess = element.TryGetSByte(out var v);
            if (isSuccess) value = BoxingSafeConverter<sbyte, T>.Instance.Convert(v);
        }
        else if (type == typeof(decimal) && element.ValueKind == JsonValueKind.Number)
        {
            isSuccess = element.TryGetDecimal(out var v);
            if (isSuccess) value = BoxingSafeConverter<decimal, T>.Instance.Convert(v);
        }
        else if (type == typeof(float) && element.ValueKind == JsonValueKind.Number)
        {
            isSuccess = element.TryGetSingle(out var v);
            if (isSuccess) value = BoxingSafeConverter<float, T>.Instance.Convert(v);
        }
        else if (type == typeof(double) && element.ValueKind == JsonValueKind.Number)
        {
            isSuccess = element.TryGetDouble(out var v);
            if (isSuccess) value = BoxingSafeConverter<double, T>.Instance.Convert(v);
        }
        else if (type == typeof(string) && element.ValueKind == JsonValueKind.String)
        {
            isSuccess = true;
            value     = BoxingSafeConverter<string, T>.Instance.Convert(element.ToString());
        }
        else if (type == typeof(bool))
        {
            if (element.ValueKind is JsonValueKind.True or JsonValueKind.False)
            {
                isSuccess = true;
                value     = BoxingSafeConverter<bool, T>.Instance.Convert(element.ValueKind == JsonValueKind.True);
            }
        }
        else if (type == typeof(DateTime) && element.ValueKind == JsonValueKind.String)
        {
            isSuccess = element.TryGetDateTime(out var v);
            if (isSuccess) value = BoxingSafeConverter<DateTime, T>.Instance.Convert(v);
        }
        else if (type == typeof(DateTimeOffset) && element.ValueKind == JsonValueKind.String)
        {
            isSuccess = element.TryGetDateTimeOffset(out var v);
            if (isSuccess) value = BoxingSafeConverter<DateTimeOffset, T>.Instance.Convert(v);
        }
        else if (type == typeof(Guid) && element.ValueKind == JsonValueKind.String)
        {
            isSuccess = element.TryGetGuid(out var v);
            if (isSuccess) value = BoxingSafeConverter<Guid, T>.Instance.Convert(v);
        }
        else if (type == typeof(byte[]) && element.ValueKind == JsonValueKind.String)
        {
            isSuccess = element.TryGetBytesFromBase64(out var v);
            if (isSuccess) value = BoxingSafeConverter<byte[], T>.Instance.Convert(v);
        }
        else if (type == typeof(Stream) && element.ValueKind == JsonValueKind.String)
        {
            isSuccess = element.TryGetBytesFromBase64(out var v);
            if (isSuccess) value = BoxingSafeConverter<Stream, T>.Instance.Convert(new MemoryStream(v));
        }

        return isSuccess;
    }
}