// Mileage Tracker
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Jeebs.Messages;

namespace Mileage.Domain.CheckCarCanBeDeleted.Messages;

/// <summary>
/// Requested car is the user's default car so cannot be deleted or disabled
/// </summary>
public sealed record class CarIsDefaultCarMsg : Msg;
