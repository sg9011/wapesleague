using System;
using System.Linq;
using System.Linq.Expressions;

namespace WaPesLeague.Data.Managers.Extensions
{
    public static class ManagerExtensions
    {
        public static IQueryable<TSource> WhereIfParamNotNull<TSource>(this IQueryable<TSource> queryable, object search, Expression<Func<TSource, bool>> predicate)
        {
            if (search != null)
            {
                queryable = queryable.Where(predicate);
            }
            return queryable;
        }

        public static IQueryable<TSource> WhereIfTrue<TSource>(this IQueryable<TSource> queryable, bool shouldAddWhere, Expression<Func<TSource, bool>> predicate)
        {
            if (shouldAddWhere)
            {
                queryable = queryable.Where(predicate);
            }
            return queryable;
        }

        public static IQueryable<T> If<T>(this IQueryable<T> source, bool? condition, Func<IQueryable<T>, IQueryable<T>> transform)
        {
            return (condition ?? false)
                    ? transform(source)
                    : source;
        }
    }
}
