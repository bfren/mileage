// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Threading.Tasks;
using Jeebs.Auth.Data;
using Mileage.Persistence.Common;
using StrongId;

namespace Mileage.Domain;

/// <summary>
/// Delegate to abstract deleting or disabling an item
/// </summary>
/// <typeparam name="TId">Item ID</typeparam>
/// <param name="userId">User ID</param>
/// <param name="itemId">Item ID</param>
/// <param name="operation">Delete Operation to perform</param>
public delegate Task<Maybe<bool>> DeleteOrDisable<TId>(AuthUserId userId, TId itemId, DeleteOperation operation)
	where TId : LongId;
