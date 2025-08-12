using System.ComponentModel.DataAnnotations;
using AutoParts_ShopAndForum.Localization;

namespace AutoParts_ShopAndForum.Areas.Forum.Models
{
    public class PostListViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "PostList_Title", ResourceType = typeof(MainLocalization))]
        public string Title { get; set; }
        
        [Display(Name = "PostListViewModel_Author", ResourceType = typeof(MainLocalization))]
        public string Author { get; set; }
        
        [Display(Name = "PostListViewModel_DateCreate", ResourceType = typeof(MainLocalization))]
        public string DateCreate { get; set; }
        
        [Display(Name = "PostListViewModel_CommentsCount", ResourceType = typeof(MainLocalization))]
        public int CommentsCount { get; set; }
    }
}
