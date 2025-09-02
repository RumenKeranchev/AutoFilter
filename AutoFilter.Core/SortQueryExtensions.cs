using System.ComponentModel;
using System.Linq.Expressions;

namespace AutoFilter.Core
{
    public record Sort(string Field, Dir Dir);

    public enum Dir { Asc, Desc }

    public static class SortQueryExtensions
    {
        public static IOrderedQueryable<TEntity> Apply<TEntity>(this IQueryable<TEntity> query, Sort sort, bool isThenBy = false)
        {
            if (!string.IsNullOrWhiteSpace(sort.Field))
            {
                Shared.ValidateQueryIsProjected(query);

                var entity = Expression.Parameter(typeof(TEntity));
                var field = Expression.PropertyOrField(entity, sort.Field) ?? throw new NullReferenceException("Invalid property");

                var sortLambda = Expression.Lambda(field, entity);

                Expression<Func<IOrderedQueryable<TEntity>>> sortMethod = default!;

                sortMethod = sort.Dir switch
                {
                   Dir.Asc => sortMethod = !isThenBy
                        ? () => query.OrderBy<TEntity, object>(k => default!)
                        : () => ((IOrderedQueryable<TEntity>)query).ThenBy<TEntity, object>(k => default!),
                    _ => sortMethod = !isThenBy
                        ? () => query.OrderByDescending<TEntity, object>(k => default!)
                        : () => ((IOrderedQueryable<TEntity>)query).ThenByDescending<TEntity, object>(k => default!)
                };

                var methodCallExpression = (sortMethod.Body as MethodCallExpression) ?? throw new Exception("MethodCallExpression null");

                var method = methodCallExpression.Method.GetGenericMethodDefinition();
                var genericSortMethod = method.MakeGenericMethod(typeof(TEntity), field.Type);
                return (IOrderedQueryable<TEntity>)genericSortMethod.Invoke(query, [query, sortLambda])!;
            }
            else
            {
                return (IOrderedQueryable<TEntity>)query;
            }
        }        
    }
}
