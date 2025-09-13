using System.Linq.Expressions;

namespace LashStudio.Application.Common.Helpers
{
    public static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> OrElse<T>(
            Expression<Func<T, bool>> left,
            Expression<Func<T, bool>> right)
        {
            var param = Expression.Parameter(typeof(T), "x");
            var body = Expression.OrElse(
                Expression.Invoke(left, param),
                Expression.Invoke(right, param)
            );
            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        public static Expression<Func<T, bool>> BuildOr<T>(
            IEnumerable<Expression<Func<T, bool>>> expressions)
        {
            Expression<Func<T, bool>>? result = null;

            foreach (var expr in expressions)
                result = result == null ? expr : OrElse(result, expr);

            if (result == null)
                throw new ArgumentException("No expressions provided", nameof(expressions));

            return result;
        }
    }
}
