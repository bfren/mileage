// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Mileage.WebApp.TagHelpers;

/// <summary>
/// Modal actions
/// </summary>
public enum ModalAction
{
	Complete = 1 << 0,
	Delete = 1 << 1,
	Update = 1 << 2,
}

/// <summary>
/// Output a link to open a Complete Journey modal
/// </summary>
[HtmlTargetElement("modal-complete", TagStructure = TagStructure.WithoutEndTag)]
public sealed class CompleteModalTagHelper : ModalActionTagHelper
{
	public CompleteModalTagHelper() : base(ModalAction.Complete) { }
}

/// <summary>
/// Output a link to open a Delete modal
/// </summary>
[HtmlTargetElement("modal-delete", TagStructure = TagStructure.WithoutEndTag)]
public sealed class DeleteModalTagHelper : ModalActionTagHelper
{
	public DeleteModalTagHelper() : base(ModalAction.Delete) { }
}

/// <summary>
/// Output a link to open an Update modal
/// </summary>
[HtmlTargetElement("modal-update", TagStructure = TagStructure.WithoutEndTag)]
public sealed class UpdateModalTagHelper : ModalActionTagHelper
{
	public UpdateModalTagHelper() : base(ModalAction.Update) { }
}

/// <summary>
/// Modal TagHelper
/// </summary>
public abstract class ModalActionTagHelper : TagHelper
{
	private ModalAction Action { get; }

	public string? Class { get; set; }

	public string Link { get; set; } = string.Empty;

	public string Replace { get; set; } = string.Empty;

	protected ModalActionTagHelper(ModalAction action) =>
		Action = action;

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		// Create the hyperlink tag with common options
		var a = new TagBuilder("a");
		a.MergeAttribute("href", "javascript:void(0)");
		a.MergeAttribute("data-replace", Replace);

		if (Class is string css)
		{
			a.AddCssClass(css);
		}

		// Add the CSS and links
		if (Action == ModalAction.Complete)
		{
			a.AddCssClass("btn-complete text-success fs-1");
			a.MergeAttribute("data-update", Link);
		}
		else if (Action == ModalAction.Delete)
		{
			a.AddCssClass("btn-delete-check text-danger fs-1");
			a.MergeAttribute("data-delete", Link);
		}
		else if (Action == ModalAction.Update)
		{
			a.AddCssClass("btn-update text-warning fs-1");
			a.MergeAttribute("data-update", Link);
		}

		// Create the button content
		var i = new TagBuilder("i");
		i.AddCssClass("fa-solid");

		if (Action == ModalAction.Complete)
		{
			i.AddCssClass("fa-circle-check");
		}
		else if (Action == ModalAction.Delete)
		{
			i.AddCssClass("fa-circle-minus");
		}
		else if (Action == ModalAction.Update)
		{
			i.AddCssClass("fa-circle-dot");
		}

		// Add button to hyperlink
		_ = a.InnerHtml.AppendHtml(i);

		// Set output content
		_ = output.Content.SetHtmlContent(a);
		output.TagMode = TagMode.StartTagAndEndTag;
	}
}
