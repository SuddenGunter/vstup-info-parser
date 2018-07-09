using AngleSharp.Dom;
using AngleSharp.Dom.Html;

public static class IElementExtensions
{
    public static string GetHref(this IElement element)
    {
        return ((IHtmlAnchorElement)element)?.Href;
    }
}