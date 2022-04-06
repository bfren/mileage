// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Jeebs.Data;
using Jeebs.Data.Query;
using Jeebs.Logging;
using NSubstitute.Extensions;
using StrongId;

namespace Abstracts;

public abstract class TestHandler
{
	internal abstract class Setup<TRepo, TEntity, TId, THandler>
		where TRepo : class, IRepository<TEntity, TId>
		where TEntity : IWithId<TId>
		where TId : class, IStrongId, new()
	{
		internal abstract THandler GetHandler(Vars v);

		internal (THandler handler, Vars v) GetVars()
		{
			// Create substitutes
			var dispatcher = Substitute.For<IDispatcher>();
			var fluent = Substitute.For<IFluentQuery<TEntity, TId>>();
			var log = Substitute.For<ILog<THandler>>();
			var repo = Substitute.For<TRepo>();

			// Setup substitutes
			fluent.ReturnsForAll(fluent);
			repo.StartFluentQuery().Returns(fluent);

			// Build handler
			var v = new Vars(dispatcher, fluent, log, repo);
			var handler = GetHandler(v);

			// Return vars
			return (handler, v);
		}

		internal sealed record class Vars(
			IDispatcher Dispatcher,
			IFluentQuery<TEntity, TId> Fluent,
			ILog<THandler> Log,
			TRepo Repo
		);
	}
}
