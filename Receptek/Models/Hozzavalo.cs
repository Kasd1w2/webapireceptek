namespace Receptek.Models
{
    public class Hozzavalo
    {
        public int Id { get; set; }
        public string IngredientName { get; set; } = "";
        public string Quantity { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime AddedTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public int SzakacsId { get; set; }
    }
}