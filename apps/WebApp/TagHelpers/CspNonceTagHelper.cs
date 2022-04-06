// Mileage Tracker Apps
// Copyright (c) bfren - licensed under https://mit.bfren.dev/2022

using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NWebsec.AspNetCore.Core.Helpers;
using NWebsec.AspNetCore.Core.Web;
using NWebsec.Mvc.Common.Helpers;

namespace Mileage.WebApp.TagHelpers;

[HtmlTargetElement(ScriptTag, Attributes = CspNonceAttributeName)]
[HtmlTargetElement(StyleTag, Attributes = CspNonceAttributeName)]
public class CspNonceTagHelper : UrlResolutionTagHelper
{
	private const string ScriptTag = "script";
	private const string StyleTag = "style";
	private const string CspNonceAttributeName = "csp-add-nonce";

	private ICspConfigurationOverrideHelper CspOverride { get; }
	private IHeaderOverrideHelper HeaderOverride { get; }

	public CspNonceTagHelper(IUrlHelperFactory urlHelperFactory, HtmlEncoder htmlEncoder)
		: base(urlHelperFactory, htmlEncoder) =>
		(CspOverride, HeaderOverride) = (new CspConfigurationOverrideHelper(), new HeaderOverrideHelper(new CspReportHelper()));

	internal CspNonceTagHelper(
		ICspConfigurationOverrideHelper overrideHelper,
		IHeaderOverrideHelper headerOverride,
		IUrlHelperFactory urlHelperFactory,
		HtmlEncoder htmlEncoder
	) : base(urlHelperFactory, htmlEncoder)
	{
		CspOverride = overrideHelper;
		HeaderOverride = headerOverride;
	}

	/// <summary>
	/// Specifies a whether a nonce should be added to the tag and the CSP header.
	/// </summary>
	[HtmlAttributeName(CspNonceAttributeName)]
	public bool UseCspNonce { get; set; }

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		if (!UseCspNonce)
		{
			return;
		}

		var httpContext = new HttpContextWrapper(ViewContext.HttpContext);
		string nonce;
		string contextMarkerKey;
		var tag = output.TagName;

		if (tag == ScriptTag)
		{
			nonce = CspOverride.GetCspScriptNonce(httpContext);
			contextMarkerKey = "NWebsecScriptNonceSet";
		}
		else if (tag == StyleTag)
		{
			nonce = CspOverride.GetCspStyleNonce(httpContext);
			contextMarkerKey = "NWebsecStyleNonceSet";
		}
		else
		{
			throw new InvalidProgramException($"Something went horribly wrong. You shouldn't be here for the tag {tag}.");
		}

		// First reference to a nonce, set header and mark that header has been set. We only need to set it once.
		if (httpContext.GetItem<string>(contextMarkerKey) == null)
		{
			httpContext.SetItem(contextMarkerKey, "set");
			HeaderOverride.SetCspHeaders(httpContext, false);
			HeaderOverride.SetCspHeaders(httpContext, true);
		}

		output.Attributes.Add(new TagHelperAttribute("nonce", nonce));
	}
}
