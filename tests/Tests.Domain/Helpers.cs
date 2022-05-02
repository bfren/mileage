// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Data;
using Jeebs.Data.Query;
using Jeebs.Data.Testing.Query;
using Jeebs.Logging;
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
		var query = FluentQueryHelper.CreateSubstitute<TEntity, TId>();
		var log = Substitute.For<ILog<THandler>>();

		// Setup substitutes
		repo.StartFluentQuery().Returns(query);

		// Return
		return (repo, query, log);
	}
}
