// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Linq.Expressions;
using Jeebs.Data.Enums;
using Jeebs.Reflection;
using NSubstitute.Core;

namespace Mileage.Queries;

internal static class Helpers
{
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

					// CHeck that the correct value is being used
					Assert.Equal(
						expected[i].value,
						actual[i].value
					);
				}
			}
		);
	}
}
