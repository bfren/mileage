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
[HtmlTargetElement("modal-complete", TagStructure = TagStructure.NormalOrSelfClosing)]
public sealed class CompleteModalTagHelper : ModalActionTagHelper
{
	public CompleteModalTagHelper() : base(ModalAction.Complete) { }
}

/// <summary>
/// Output a link to open a Delete modal
/// </summary>
[HtmlTargetElement("modal-delete", TagStructure = TagStructure.NormalOrSelfClosing)]
public sealed class DeleteModalTagHelper : ModalActionTagHelper
{
	public DeleteModalTagHelper() : base(ModalAction.Delete) { }
}

/// <summary>
/// Output a link to open an Update modal
/// </summary>
[HtmlTargetElement("modal-update", TagStructure = TagStructure.NormalOrSelfClosing)]
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

	public bool ReplaceContents { get; set; } = true;

	protected ModalActionTagHelper(ModalAction action) =>
		Action = action;

	public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
	{
		// Set output options
		output.TagMode = TagMode.StartTagAndEndTag;

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
			a.AddCssClass("btn-complete");
			a.MergeAttribute("data-update", Link);
		}
		else if (Action == ModalAction.Delete)
		{
			a.AddCssClass("btn-delete-check");
			a.MergeAttribute("data-delete", Link);
		}
		else if (Action == ModalAction.Update)
		{
			a.AddCssClass("btn-update");
			a.MergeAttribute("data-update", Link);
		}

		// Look for any child content
		var child = await output.GetChildContentAsync();
		var content = child.GetContent();

		// Create the button content
		if (string.IsNullOrEmpty(content))
		{
			const string fs = "fs-1";

			var i = new TagBuilder("i");
			i.AddCssClass("fa-solid");

			if (Action == ModalAction.Complete)
			{
				a.AddCssClass($"btn-complete text-success {fs}");
				i.AddCssClass("fa-circle-check");
			}
			else if (Action == ModalAction.Delete)
			{
				a.AddCssClass($"btn-delete-check text-danger {fs}");
				i.AddCssClass("fa-circle-minus");
			}
			else if (Action == ModalAction.Update)
			{
				a.AddCssClass($"text-warning {fs}");
				i.AddCssClass("fa-circle-dot");
			}

			// Add button to hyperlink
			_ = a.InnerHtml.AppendHtml(i);
		}
		else
		{
			_ = a.InnerHtml.Append(content);
		}

		// Set output content
		_ = output.Content.SetHtmlContent(a);
	}
}
