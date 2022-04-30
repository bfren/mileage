// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Mileage.Persistence.Common;

namespace Mileage.WebApp.Pages.Modals;

public abstract class DeleteModalModel : ModalModel
{
	public DeleteOperation Operation { get; set; } = DeleteOperation.Delete;

	public string? Reason { get; set; }

	protected DeleteModalModel(string title) : base(title) { }

	protected DeleteModalModel(string title, string size) : base(title, size) { }
}
