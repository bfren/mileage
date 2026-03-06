// Mileage Tracker: Unit Tests
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Cqrs;
using Jeebs.Data.Common;
using Jeebs.Logging;
using NSubstitute.Extensions;
using Wrap.Caching;

namespace Abstracts;

public abstract class TestHandler
{
	internal abstract class Setup<THandler>
	{
		internal abstract THandler GetHandler(Vars v);

		internal virtual (THandler handler, Vars v) GetVars()
		{
			// Create substitutes
			var dispatcher = Substitute.For<IDispatcher>();
			var log = Substitute.For<ILog<THandler>>();

			// Build handler
			var v = new Vars(dispatcher, log);
			var handler = GetHandler(v);

			// Return vars
			return (handler, v);
		}

		internal sealed record class Vars(
			IDispatcher Dispatcher,
			ILog<THandler> Log
		);
	}

	internal abstract class Setup<TRepo, TEntity, TId, THandler>
		where TRepo : class, IRepository<TEntity, TId>
		where TEntity : IWithId<TId, long>
		where TId : class, IId<TId, long>, new()
	{
		internal abstract THandler GetHandler(Vars v);

		internal virtual (THandler handler, Vars v) GetVars()
		{
			// Create substitutes
			var cache = Substitute.For<IWrapCache<TId>>();
			var dispatcher = Substitute.For<IDispatcher>();
			var fluent = Substitute.For<Jeebs.Data.Repository.IFluentQuery<TEntity, TId>>();
			var log = Substitute.For<ILog<THandler>>();
			var repo = Substitute.For<TRepo>();

			// Setup substitutes
			fluent.ReturnsForAll(fluent);
			repo.Fluent().Returns(fluent);

			// Build handler
			var v = new Vars(cache, dispatcher, fluent, log, repo);
			var handler = GetHandler(v);

			// Return vars
			return (handler, v);
		}

		internal sealed record class Vars(
			IWrapCache<TId> Cache,
			IDispatcher Dispatcher,
			Jeebs.Data.Repository.IFluentQuery<TEntity, TId> Fluent,
			ILog<THandler> Log,
			TRepo Repo
		);
	}
}
