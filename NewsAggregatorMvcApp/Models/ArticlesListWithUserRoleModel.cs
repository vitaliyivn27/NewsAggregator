

using NewsAggregator.Core.DataTransferObjects;

namespace NewsAggregatorMvcApp.Models;

public class ArticlesListWithUserRoleModel
{
    public List<ArticleDto> Articles { get; set; }

    public bool IsAdmin { get; set; }
}