using System.Linq.Expressions;

namespace AutoFilter.Core
{
    public record Sort(string Field, Dir Dir);

    public enum Dir { Asc, Desc }

    public static class SortQueryExtensions
    {
        /// <summary>
        /// <para>
        /// Apply sorting to a queryable based on the provided sort. The queryable must be projected and contain the sort column as a property/field.
        /// </para>
        /// <para>
        /// <strong><u>Beware of the projection entity constructor!</u></strong> If the properties are set through the constructor for example like a record,
        /// EntityFramework WILL throw an exception because it cannot translate the expression to SQL. 
        /// In that case, make sure to have a parameterless constructor and set the properties through the body of the constructor.
        /// </para>
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <param name="sort"></param>
        /// <returns>Sorted query</returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static IOrderedQueryable<TEntity> Apply<TEntity>(this IQueryable<TEntity> query, Sort sort)
        {
            if (!string.IsNullOrWhiteSpace(sort.Field))
            {
                Shared.ValidateQueryIsProjected(query);

                var entity = Expression.Parameter(typeof(TEntity));
                var field = Expression.PropertyOrField(entity, sort.Field);

                var sortLambda = Expression.Lambda(field, entity);

                Expression<Func<IOrderedQueryable<TEntity>>> sortMethod = default!;

                bool isThenBy = Shared.IsOrdered(query);

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
                throw new InvalidOperationException("Sort column cannot be null or empty");
            }
        }
    }
}
