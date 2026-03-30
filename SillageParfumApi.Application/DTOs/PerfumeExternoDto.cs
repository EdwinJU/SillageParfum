namespace SillageParfumApi.Application.DTOs
{
    // Esta es la información que intentaremos rescatar de internet
    public class PerfumeExternoDto
    {
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string OlfactoryNotes { get; set; } = string.Empty; // Ej: "Vainilla, Madera, Cítrico"
    }
}