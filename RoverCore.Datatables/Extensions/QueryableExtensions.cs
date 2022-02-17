using System.Linq.Expressions;

namespace RoverCore.Datatables.Extensions;

public static class QueryableExtensions
{
    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, string propertyName, IComparer<object>? comparer = null)
    {
        return CallOrderedQueryable(query, "OrderBy", propertyName, comparer);
    }

    public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> query, string propertyName, IComparer<object>? comparer = null)
    {
        return CallOrderedQueryable(query, "OrderByDescending", propertyName, comparer);
    }

    public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> query, string propertyName, IComparer<object>? comparer = null)
    {
        return CallOrderedQueryable(query, "ThenBy", propertyName, comparer);
    }

    public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> query, string propertyName, IComparer<object>? comparer = null)
    {
        return CallOrderedQueryable(query, "ThenByDescending", propertyName, comparer);
    }

    /// <summary>
    /// Builds the Queryable functions using a TSource property name.
    /// </summary>
    public static IOrderedQueryable<T> CallOrderedQueryable<T>(this IQueryable<T> query, string methodName, string propertyName,
            IComparer<object>? comparer = null)
    {
        var param = Expression.Parameter(typeof(T), "x");

        var body = propertyName.Split('.').Aggregate<string, Expression>(param, Expression.PropertyOrField);

        return comparer != null
            ? (IOrderedQueryable<T>)query.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    methodName,
                    new[] { typeof(T), body.Type },
                    query.Expression,
                    Expression.Lambda(body, param),
                    Expression.Constant(comparer)
                )
            )
            : (IOrderedQueryable<T>)query.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    methodName,
                    new[] { typeof(T), body.Type },
                    query.Expression,
                    Expression.Lambda(body, param)
                )
            );
    }

    /// <summary>
    /// Credit to Ivan Stoev - https://stackoverflow.com/questions/59754832/ef-core-expression-tree-equivalent-for-iqueryable-search
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="search"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IQueryable<T> ApplySearch<T>(this IQueryable<T> source, string search)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (string.IsNullOrWhiteSpace(search)) return source;

        var parameter = Expression.Parameter(typeof(T), "e");
        // The following simulates closure to let EF Core create parameter rather than constant value (in case you use `Expresssion.Constant(search)`)
        var value = Expression.Property(Expression.Constant(new { search }), nameof(search));
        var body = SearchStrings(parameter, value);
        if (body == null) return source;

        var predicate = Expression.Lambda<Func<T, bool>>(body, parameter);
        return source.Where(predicate);
    }

    static Expression? SearchStrings(Expression target, Expression search)
    {
        Expression? result = null;

        var properties = target.Type
            .GetProperties()
            .Where(x => x.CanRead);

        foreach (var prop in properties)
        {
            Expression? condition = null;
            var propValue = Expression.MakeMemberAccess(target, prop);
            if (prop.PropertyType == typeof(string))
            {
                var comparand = Expression.Call(propValue, nameof(string.ToLower), Type.EmptyTypes);
                condition = Expression.Call(comparand, nameof(string.Contains), Type.EmptyTypes, search);
            }
            else if (!(prop!.PropertyType!.Namespace!.StartsWith("System.")))
            {
                condition = SearchStrings(propValue, search);
            }
            if (condition != null)
                result = result == null ? condition : Expression.OrElse(result, condition);
        }

        return result;
    }
}
