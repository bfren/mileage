// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Linq.Expressions;
using Jeebs.Cqrs;
using Jeebs.Data;
using Jeebs.Data.Enums;
using Jeebs.Data.Query;
using Jeebs.Id;
using Jeebs.Logging;
using Jeebs.Reflection;
using NSubstitute.Core;
using NSubstitute.Extensions;

namespace Mileage.Queries;

internal static class Helpers
{
	public static (TRepo repo, IQueryFluent<TEntity, TId> fluent, ILog<THandler> log) Setup<TRepo, TEntity, TId, THandler, TQuery, TResult>()
		where TRepo : class, IRepository<TEntity, TId>
		where TEntity : IWithId<TId>
		where TId : IStrongId
		where THandler : QueryHandler<TQuery, TResult>
		where TQuery : IQuery<TResult>
	{
		// Create substitutes
		var repo = Substitute.For<TRepo>();
		var query = Substitute.For<IQueryFluent<TEntity, TId>>();
		var log = Substitute.For<ILog<THandler>>();

		// Setup substitutes
		query.ReturnsForAll(query);
		repo.StartFluentQuery().Returns(query);

		// Return
		return (repo, query, log);
	}

	/// <summary>
	/// Validate a call to a repository query method
	/// </summary>
	/// <typeparam name="TEntity">Entity type</typeparam>
	/// <typeparam name="TModel">Return model type</typeparam>
	/// <param name="call">Call</param>
	/// <param name="method">The expected name of the method</param>
	/// <param name="expected">The expected predicates</param>
	public static void AssertQuery<TEntity, TModel>(
		ICall call,
		string method,
		params (Expression<Func<TEntity, object>> property, Compare compare, object value)[] expected
	)
	{
		// Check the method
		Assert.Equal(method, call.GetMethodInfo().Name);

		// Check the return model generic type
		Assert.Collection(call.GetMethodInfo().GetGenericArguments(),
			type => Assert.Equal(typeof(TModel), type)
		);

		// Check each predicate
		Assert.Collection(call.GetArguments(),
			arg =>
			{
				// Get strongly-typed predicates
				var actual = Assert.IsType<(Expression<Func<TEntity, object>> property, Compare compare, object value)[]>(arg);

				// Check each expected predicate about what was actually used
				for (var i = 0; i < expected.Length; i++)
				{
					// Check that the correct property is being used
					Assert.Equal(
						expected[i].property.GetPropertyInfo().UnsafeUnwrap().Name,
						actual[i].property.GetPropertyInfo().UnsafeUnwrap().Name
					);

					// Check that the correct comparison is being used
					Assert.Equal(
						expected[i].compare,
						actual[i].compare
					);

					// Check that the correct value is being used
					Assert.Equal(
						expected[i].value,
						actual[i].value
					);
				}
			}
		);
	}

	/// <summary>
	/// Validate a call to the fluent query where method
	/// </summary>
	/// <typeparam name="TEntity">Entity type</typeparam>
	/// <typeparam name="TValue">Column select value type</typeparam>
	/// <param name="call">Call</param>
	/// <param name="property"></param>
	/// <param name="compare"></param>
	/// <param name="value"></param>
	public static void AssertWhere<TEntity, TValue>(
		ICall call,
		Expression<Func<TEntity, TValue>> property,
		Compare compare,
		TValue value
	)
	{
		// Check the method
		Assert.Equal("Where", call.GetMethodInfo().Name);

		// Check the value generic type
		Assert.Collection(call.GetMethodInfo().GetGenericArguments(),
			type => Assert.Equal(typeof(TValue), type)
		);

		// Check each predicate
		Assert.Collection(call.GetArguments(),

			// Check that the correct property is being used
			arg =>
			{
				var actual = Assert.IsAssignableFrom<Expression<Func<TEntity, TValue>>>(arg);
				Assert.Equal(
					property.GetPropertyInfo().UnsafeUnwrap().Name,
					actual.GetPropertyInfo().UnsafeUnwrap().Name
				);
			},

			// Check that the correct comparison is being used
			arg =>
			{
				var actual = Assert.IsType<Compare>(arg);
				Assert.Equal(compare, actual);
			},

			// Check that the correct value is being used
			arg =>
			{
				var actual = Assert.IsType<TValue>(arg);
				Assert.Equal(value, actual);
			}
		);
	}
}
