using System.Linq.Expressions;

namespace AutoFilter.Core
{
    public record Filter(string Field, Operator Operator, string Value);

    public enum Operator
    {
        Equal,
        NotEqual,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        Contains,
        NotContains,
    }

    public static class FilterQueryExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <param name="filter"></param>
        /// <returns>Filtered query</returns>
        /// <exception cref="ArgumentOutOfRangeException">Depending on the column type some operators are not allowed</exception>
        /// <exception cref="InvalidOperationException">Thrown if no filter column is provided or the query is not projected</exception>
        /// <exception cref="FormatException">Thrown if the filter column is DateTime/DateTime? and the value cannot be converted to a valid DateTime</exception>
        public static IQueryable<TEntity> Apply<TEntity>(this IQueryable<TEntity> query, Filter filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Field))
            {
                Shared.ValidateQueryIsProjected(query);

                var entity = Expression.Parameter(typeof(TEntity));
                var field = Expression.PropertyOrField(entity, filter.Field);
                Expression value = Expression.Constant(filter.Value);

                Expression expressionBody = default!;

                if (field.Type != value.Type)
                {
                    var type = field.Type;

                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        type = Nullable.GetUnderlyingType(type);
                    }

                    if (filter.Value != null)
                    {
                        object convertedValue = Convert.ChangeType(filter.Value, type!);
                        value = Expression.Constant(convertedValue);
                    }
                    else
                    {
                        throw new NullReferenceException("Filter value cannot be null");
                    }

                    value = Expression.Convert(value, field.Type);
                }

                if (value.Type == typeof(string))
                {
                    var toLowerMethod = typeof(string).GetMethod("ToUpper", Type.EmptyTypes);
                    var fieldToLower = Expression.Call(field, toLowerMethod!);
                    var valueToLower = Expression.Call(value, toLowerMethod!);

                    expressionBody = filter!.Operator switch
                    {
                        Operator.Contains => Expression.Call(fieldToLower, "Contains", Type.EmptyTypes, valueToLower),
                        Operator.NotContains => Expression.Not(Expression.Call(fieldToLower, "Contains", Type.EmptyTypes, valueToLower)),
                        Operator.Equal => Expression.Equal(fieldToLower, valueToLower),
                        Operator.NotEqual => Expression.NotEqual(fieldToLower, valueToLower),
                        _ => throw new ArgumentOutOfRangeException($"Invalid operator [{filter.Operator}] provided for value type [{value.Type.Name}]")
                    };
                }
                else if (value.Type == typeof(int) || value.Type == typeof(int?)
                    || value.Type == typeof(float) || value.Type == typeof(float?)
                    || value.Type == typeof(double) || value.Type == typeof(double?)
                    || value.Type == typeof(decimal) || value.Type == typeof(decimal?))
                {
                    expressionBody = filter!.Operator switch
                    {
                        Operator.GreaterThan => Expression.GreaterThan(field, value),
                        Operator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(field, value),
                        Operator.LessThan => Expression.LessThan(field, value),
                        Operator.LessThanOrEqual => Expression.LessThanOrEqual(field, value),
                        Operator.Equal => Expression.Equal(field, value),
                        Operator.NotEqual => Expression.NotEqual(field, value),
                        _ => throw new ArgumentOutOfRangeException($"Invalid operator [{filter.Operator}] provided for value type [{value.Type.Name}]")
                    };
                }
                else if ((value.Type == typeof(DateTime) || value.Type == typeof(DateTime?))
                    && DateTime.TryParse(filter.Value, out var parsedDate))
                {
                    bool isNullable = field.Type.IsGenericType
                        && field.Type.GetGenericTypeDefinition() == typeof(Nullable<>)
                        && Nullable.GetUnderlyingType(field.Type) == typeof(DateTime);

                    var start = Expression.Constant(parsedDate, isNullable ? typeof(DateTime?) : typeof(DateTime));
                    var end = Expression.Constant(parsedDate.AddDays(1), isNullable ? typeof(DateTime?) : typeof(DateTime));

                    expressionBody = filter.Operator switch
                    {
                        Operator.Equal => Expression.AndAlso(
                                    Expression.GreaterThanOrEqual(field, start),
                                    Expression.LessThan(field, end)),
                        Operator.NotEqual => Expression.OrElse(
                                    Expression.LessThan(field, start),
                                    Expression.GreaterThanOrEqual(field, end)),
                        Operator.GreaterThan => Expression.GreaterThanOrEqual(field, end),
                        Operator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(field, start),
                        Operator.LessThan => Expression.LessThan(field, start),
                        Operator.LessThanOrEqual => Expression.LessThan(field, end),
                        _ => throw new ArgumentOutOfRangeException($"Invalid operator [{filter.Operator}] provided for value type [{value.Type.Name}]")
                    };
                }
                else if (value.Type == typeof(bool))
                {
                    expressionBody = filter!.Operator switch
                    {
                        Operator.Equal => Expression.Equal(field, value),
                        Operator.NotEqual => Expression.NotEqual(field, value),
                        _ => throw new ArgumentOutOfRangeException($"Invalid operator [{filter.Operator}] provided for value type [{value.Type.Name}]")
                    };
                }

                var lamda = Expression.Lambda<Func<TEntity, bool>>(expressionBody, entity);

                return query.Where(lamda);
            }
            else
            {
                throw new InvalidOperationException("Filter column cannot be null or empty");
            }
        }
    }
}
