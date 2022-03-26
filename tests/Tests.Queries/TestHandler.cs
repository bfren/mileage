// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Jeebs.Data;
using Jeebs.Data.Query;
using Jeebs.Id;
using Jeebs.Logging;
using NSubstitute.Extensions;

namespace Mileage.Queries;

public abstract class TestHandler<TRepo, TEntity, TId, THandler>
	where TRepo : class, IRepository<TEntity, TId>
	where TEntity : IWithId<TId>
	where TId : IStrongId
{
	public abstract THandler GetHandler(Vars v);

	public (THandler handler, Vars v) GetVars()
	{
		// Create substitutes
		var dispatcher = Substitute.For<IDispatcher>();
		var fluent = Substitute.For<IQueryFluent<TEntity, TId>>();
		var log = Substitute.For<ILog<THandler>>();
		var repo = Substitute.For<TRepo>();

		// Setup fluent query
		fluent.ReturnsForAll(fluent);
		repo.StartFluentQuery().Returns(fluent);

		// Build handler
		var v = new Vars(dispatcher, fluent, log, repo);
		var handler = GetHandler(v);

		// Return vars
		return (handler, v);
	}

	public sealed record class Vars(
		IDispatcher Dispatcher,
		IQueryFluent<TEntity, TId> Fluent,
		ILog<THandler> Log,
		TRepo Repo
	);
}
