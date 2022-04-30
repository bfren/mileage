// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Linq.Expressions;
using Jeebs.Data;
using Jeebs.Data.Enums;
using Jeebs.Data.Query;
using Jeebs.Logging;
using Jeebs.Reflection;
using NSubstitute.Core;
using NSubstitute.Extensions;
using StrongId;

namespace Mileage.Domain;

internal static class Helpers
{
	public static (TRepo repo, IFluentQuery<TEntity, TId> fluent, ILog<THandler> log) Setup<TRepo, TEntity, TId, THandler>()
		where TRepo : class, IRepository<TEntity, TId>
		where TEntity : IWithId<TId>
		where TId : class, IStrongId, new()
	{
		// Create substitutes
		var repo = Substitute.For<TRepo>();
		var query = Substitute.For<IFluentQuery<TEntity, TId>>();
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
	) =>
		AssertWhere<TEntity, TValue>(call, property.GetPropertyInfo().UnsafeUnwrap().Name, compare, value);

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
		string property,
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
				Assert.Equal(property, actual.GetPropertyInfo().UnsafeUnwrap().Name);
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
				if (arg is null)
				{
					Assert.Null(arg);
				}
				else
				{
					Assert.Equal(value!, arg);
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
	/// <param name="values"></param>
	public static void AssertWhereIn<TEntity, TValue>(ICall call, Expression<Func<TEntity, TValue>> property, TValue[] values) =>
		AssertWhereIn<TEntity, TValue>(call, property.GetPropertyInfo().UnsafeUnwrap().Name, values);

	/// <summary>
	/// Validate a call to the fluent query where method
	/// </summary>
	/// <typeparam name="TEntity">Entity type</typeparam>
	/// <typeparam name="TValue">Column select value type</typeparam>
	/// <param name="call">Call</param>
	/// <param name="property"></param>
	/// <param name="values"></param>
	public static void AssertWhereIn<TEntity, TValue>(ICall call, string property, TValue[] values)
	{
		// Check the method
		Assert.Equal("WhereIn", call.GetMethodInfo().Name);

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
				Assert.Equal(property, actual.GetPropertyInfo().UnsafeUnwrap().Name);
			},

			// Check that the correct values are being used
			arg =>
			{
				var actual = Assert.IsType<TValue[]>(arg);
				Assert.Equal(values, actual);
			}
		);
	}

	/// <summary>
	/// Validate a call to the fluent query sort method
	/// </summary>
	/// <typeparam name="TEntity">Entity type</typeparam>
	/// <typeparam name="TValue">Column select value type</typeparam>
	/// <param name="call">Call</param>
	/// <param name="property"></param>
	/// <param name="order"></param>
	public static void AssertSort<TEntity, TValue>(
		ICall call,
		Expression<Func<TEntity, TValue>> property,
		SortOrder order
	) =>
		AssertSort<TEntity, TValue>(call, property.GetPropertyInfo().UnsafeUnwrap().Name, order);

	/// <summary>
	/// Validate a call to the fluent query sort method
	/// </summary>
	/// <typeparam name="TEntity">Entity type</typeparam>
	/// <typeparam name="TValue">Column select value type</typeparam>
	/// <param name="call">Call</param>
	/// <param name="property"></param>
	/// <param name="order"></param>
	public static void AssertSort<TEntity, TValue>(
		ICall call,
		string property,
		SortOrder order
	)
	{
		// Check the method
		Assert.Equal("Sort", call.GetMethodInfo().Name);

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
				Assert.Equal(property, actual.GetPropertyInfo().UnsafeUnwrap().Name);
			},

			// Check that the correct order is being used
			arg =>
			{
				var actual = Assert.IsType<SortOrder>(arg);
				Assert.Equal(order, actual);
			}
		);
	}

	/// <summary>
	/// Validate a call to the fluent query maximum method
	/// </summary>
	/// <param name="call">Call</param>
	/// <param name="value"></param>
	public static void AssertMaximum(
		ICall call,
		ulong value
	)
	{
		// Check the method
		Assert.Equal("Maximum", call.GetMethodInfo().Name);

		// Check each predicate
		Assert.Collection(call.GetArguments(),

			// Check that the correct property is being used
			arg => Assert.Equal(value, arg)
		);
	}

	/// <summary>
	/// Validate a call to the fluent query sort method
	/// </summary>
	/// <typeparam name="TEntity">Entity type</typeparam>
	/// <typeparam name="TValue">Column select value type</typeparam>
	/// <param name="call">Call</param>
	/// <param name="property"></param>
	public static void AssertExecute<TEntity, TValue>(
		ICall call,
		Expression<Func<TEntity, TValue>> property
	) =>
		AssertExecute<TEntity, TValue>(call, property.GetPropertyInfo().UnsafeUnwrap().Name);

	/// <summary>
	/// Validate a call to the fluent query execute method
	/// </summary>
	/// <typeparam name="TEntity">Entity type</typeparam>
	/// <typeparam name="TValue">Column select value type</typeparam>
	/// <param name="call">Call</param>
	/// <param name="property"></param>
	public static void AssertExecute<TEntity, TValue>(
		ICall call,
		string property
	)
	{
		// Check the method
		Assert.Equal("ExecuteAsync", call.GetMethodInfo().Name);

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
				Assert.Equal(property, actual.GetPropertyInfo().UnsafeUnwrap().Name);
			}
		);
	}
}
