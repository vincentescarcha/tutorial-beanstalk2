using Shop.Web.Models.Category;
using System;
using System.Globalization;

namespace Shop.Web.Models.Food
{
    public class FoodListingModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Total { get => String.Format("\u20B1{0}", (Price * Amount)); }
        public int InStock { get; set; }
        public string ImageUrl { get; set; }
        public string ShortDescription { get; set; }
        public int Amount { get; set; } = 1;
        public CategoryListingModel Category { get; set; }
    }
}
