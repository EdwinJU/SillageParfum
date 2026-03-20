namespace SillageParfumApi.Application.DTOs
{
    // Esta clase es una simple "bolsa de datos". No tiene lógica ni reglas.
    public class PerfumeResponseDto
    {
        public int Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public bool IsAvailable { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}