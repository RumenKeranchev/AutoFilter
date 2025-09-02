using System.Linq.Expressions;

namespace AutoFilter.Core
{
    internal static class Shared
    {
        internal static void ValidateQueryIsProjected<TEntity>(IQueryable<TEntity> query)
        {
            bool isCasted = CheckQueryForMethod(query, nameof(Queryable.Select));

            if (!isCasted)
            {
                throw new InvalidOperationException("The query must be projected using Select before applying the sort.");
            }
        }

        internal static bool IsOrdered<TEntity>(IQueryable<TEntity> query)
        {
            return CheckQueryForMethod(query, nameof(Queryable.OrderBy)) ||
                   CheckQueryForMethod(query, nameof(Queryable.OrderByDescending)) ||
                   CheckQueryForMethod(query, nameof(Queryable.ThenBy)) ||
                   CheckQueryForMethod(query, nameof(Queryable.ThenByDescending));
        }

        static bool CheckQueryForMethod<TEntity>(IQueryable<TEntity> query, string methodName)
        {
            bool methodExists = false;
            var expr = query.Expression;
            while (expr is MethodCallExpression mce)
            {
                if (mce.Method.DeclaringType == typeof(Queryable) && mce.Method.Name.Equals(methodName, StringComparison.InvariantCultureIgnoreCase))
                {
                    methodExists = true;
                    break;
                }

                expr = mce.Arguments[0];
            }

            return methodExists;
        }
    }
}
