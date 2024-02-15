using System.Linq.Expressions;

namespace JsonEasyNavigation;

// https://stackoverflow.com/questions/3343551/how-to-cast-a-value-of-generic-type-t-to-double-without-boxing
internal sealed class BoxingSafeConverter<TIn, TOut>         
{
    public static readonly BoxingSafeConverter<TIn, TOut> Instance = new();

    public Func<TIn, TOut> Convert { get; }

    private BoxingSafeConverter()
    {
        if (typeof (TIn) != typeof (TOut))
            throw new InvalidOperationException("Both generic type parameters must represent the same type.");

        var paramExpr = Expression.Parameter(typeof (TIn));
        Convert = Expression.Lambda<Func<TIn, TOut>>(paramExpr, paramExpr).Compile();
    }
}