// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Data.Common;
using Jeebs.Data.Testing.Query;
using Jeebs.Logging;

namespace Mileage.Domain;

internal static class Helpers
{
	public static (TRepo repo, Jeebs.Data.Repository.IFluentQuery<TEntity, TId> fluent, ILog<THandler> log) Setup<TRepo, TEntity, TId, THandler>()
		where TRepo : class, IRepository<TEntity, TId>
		where TEntity : IWithId<TId, long>
		where TId : class, IId<TId, long>, new()
	{
		// Create substitutes
		var repo = Substitute.For<TRepo>();
		var query = FluentQueryHelper.CreateSubstitute<TEntity, TId>();
		var log = Substitute.For<ILog<THandler>>();

		// Setup substitutes
		repo.Fluent().Returns(query);

		// Return
		return (repo, query, log);
	}
}
