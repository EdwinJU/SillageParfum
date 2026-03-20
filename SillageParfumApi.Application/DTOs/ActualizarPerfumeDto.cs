namespace SillageParfumApi.Application.DTOs
{
    public class ActualizarPerfumeDto
    {
        public int Id { get; set; } // ¡Vital para saber cuál actualizar!
        public string SKU { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int VolumeMl { get; set; }
    }
}