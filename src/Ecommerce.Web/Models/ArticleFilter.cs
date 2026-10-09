using System.Globalization;
using System.Text;
using Ecommerce.Shared.ViewModels;

namespace Ecommerce.Web.Models;

/// <summary>
/// Filters from articles with immutable value : component returns a new copy
/// (<c>with</c>) and page has only to call <see cref="Apply"/>.
/// </summary>
public sealed record ArticleFilter
{
    /// <summary>
    /// Bar search filter
    /// </summary>
    public string Search { get; init; } = "";

    /// <summary>
    /// Categories filter
    /// </summary>
    public IReadOnlySet<string> Categories { get; init; } = new HashSet<string>();

    /// <summary>
    /// Promotions filter
    /// </summary>
    public bool PromotionsOnly { get; init; }

    /// <summary>
    /// Favourites filter
    /// </summary>
    public bool FavouritesOnly { get; init; }

    /// <summary>
    /// Apply only categories filter
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    public ArticleFilter ToggleCategory(string category)
    {
        var categories = new HashSet<string>(Categories);

        if (!categories.Remove(category))
        {
            categories.Add(category);
        }

        return this with { Categories = categories };
    }

    /// <summary>
    /// Apply all filters
    /// </summary>
    /// <param name="articles"></param>
    /// <param name="isFavourite"></param>
    /// <returns></returns>
    public IEnumerable<ArticleViewModel> Apply(IEnumerable<ArticleViewModel> articles, Func<Guid, bool> isFavourite)
    {
        var search = Normalize(Search.Trim());

        foreach (var article in articles)
        {
            if (search.Length > 0 && !MatchesSearch(article, search))
                continue;

            if (Categories.Count > 0 && !Categories.Contains(article.Category))
                continue;

            if (FavouritesOnly && !isFavourite(article.Id))
                continue;

            if (!PromotionsOnly)
            {
                yield return article;
                continue;
            }

            // With active promotions filter, only displays stores with promotions
            var promoted = article.Stores.Where(store => store.Promotion is not null).ToList();
            if (promoted.Count > 0)
                yield return article with { Stores = promoted };
        }
    }

    /// <summary>
    /// Compare value from Search bar with some Article info
    /// </summary>
    /// <param name="article"></param>
    /// <param name="search"></param>
    /// <returns></returns>
    private static bool MatchesSearch(ArticleViewModel article, string search)
        => Normalize(article.Name).Contains(search)
           || Normalize(article.Brand).Contains(search)
           || Normalize(article.Category).Contains(search)
           || Normalize(article.Description).Contains(search);

    /// <summary>
    /// Normalize value from Search bar (lower case without accents)
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private static string Normalize(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "";

        var withoutMarks = text.Normalize(NormalizationForm.FormD)
                               .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);

        return string.Concat(withoutMarks).ToLowerInvariant();
    }
}
