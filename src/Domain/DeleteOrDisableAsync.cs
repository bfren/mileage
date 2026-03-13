// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Auth.Data.Ids;
using Mileage.Persistence.Common;
using Wrap.Ids;

namespace Mileage.Domain;

/// <summary>
/// Delegate to abstract deleting or disabling an item
/// </summary>
/// <typeparam name="TId">Item ID</typeparam>
/// <param name="userId">User ID</param>
/// <param name="entityId">Entity ID</param>
/// <param name="operation">Delete Operation to perform</param>
public delegate Task<Result<bool>> DeleteOrDisable<TId>(AuthUserId userId, TId entityId, DeleteOperation operation)
	where TId : LongId<TId>, new();

/// <summary>
/// Delegate to abstract determining whether or not the item is selected as default in settings
/// </summary>
/// <typeparam name="TId"></typeparam>
/// <param name="userId"></param>
/// <param name="entityId"></param>
public delegate Task<Result<bool>> CheckIsDefault<TId>(AuthUserId userId, TId entityId)
	where TId : LongId<TId>, new();

/// <summary>
/// Delegate to abstract counting the number of journeys with an item
/// </summary>
/// <typeparam name="TId"></typeparam>
/// <param name="userId"></param>
/// <param name="entityId"></param>
public delegate Task<Result<long>> CountJourneysWith<TId>(AuthUserId userId, TId entityId)
	where TId : LongId<TId>, new();
