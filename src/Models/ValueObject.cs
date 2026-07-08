namespace SomeTimeLater.Primitives;

public abstract record ValueObject : IComparable, IComparable<ValueObject>
{
    public int CompareTo(object? otherObject)
    {
        return otherObject is ValueObject other
            ? CompareTo(other)
            : 1;
    }

    public abstract int CompareTo(ValueObject? other);
    
    public static bool operator <(ValueObject left, ValueObject right)
        => left.CompareTo(right) < 0;

    public static bool operator <=(ValueObject left, ValueObject right)
        => left.CompareTo(right) <= 0;

    public static bool operator >(ValueObject left, ValueObject right)
        => left.CompareTo(right) > 0;

    public static bool operator >=(ValueObject left, ValueObject right)
        => left.CompareTo(right) >= 0;
}