namespace SillageParfumApi.Application.DTOs
{
    // Solo pedimos lo estrictamente necesario para que un perfume nazca
    public class CrearPerfumeDto
    {
        public string SKU { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int VolumeMl { get; set; }
    }
}