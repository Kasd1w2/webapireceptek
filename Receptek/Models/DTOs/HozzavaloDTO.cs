namespace Receptek.Models.DTOs
{
    public class HozzavaloDTO
    {
        public string IngredientName { get; set; } = "";
        public string Quantity { get; set; } = "";
        public string Description { get; set; } = "";
        public int SzakacsId { get; set; }
    }
}