using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AutoFilter.Core
{
    internal static class Shared
    {
        internal static void ValidateQueryIsProjected<TEntity>(IQueryable<TEntity> query)
        {
            bool isCasted = false;
            var expr = query.Expression;
            while (expr is MethodCallExpression mce)
            {
                if (mce.Method.DeclaringType == typeof(Queryable) && mce.Method.Name == nameof(Queryable.Select))
                {
                    isCasted = true;
                    break;
                }

                expr = mce.Arguments[0];
            }

            if (!isCasted)
            {
                throw new InvalidOperationException("The query must be projected using Select before applying the sort.");
            }
        }
    }
}
