namespace SomeTimeLater.Primitives;

public abstract record SingleValueObject<TValue>(TValue Value) : ValueObject, ISingleValue<TValue>
    where TValue : IComparable, IComparable<TValue>, IEquatable<TValue>
{
    public override string ToString() 
        => Value.ToString() ?? string.Empty;
    
    public string ToString(string? format, IFormatProvider? provider)
    {
        if (Value is IFormattable formattable)
            return formattable.ToString(format, provider);

        return Value.ToString() ?? string.Empty;
    }

    public override int CompareTo(ValueObject? other)
    {
        var otherValue = other as SingleValueObject<TValue>;
        return CompareTo(otherValue);
    }

    public int CompareTo(SingleValueObject<TValue>? other)
        => other is null ? 1 : Value.CompareTo(other.Value);
    
    public TValue Unwrap() => Value;
    
    public static implicit operator TValue(SingleValueObject<TValue> value) 
        => value.Value;

    public override int GetHashCode()
    {
        return HashCode.Combine(GetType(), Value);
    }
}