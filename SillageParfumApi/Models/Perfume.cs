namespace SillageParfumApi.Models
{
    public class Perfume
    {
        // Identificadores
        public int Id { get; set; }
        public string SKU { get; set; } = string.Empty;

        // Información Básica
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Especificaciones del Perfume
        public ConcentrationType Concentration { get; set; }
        public string OlfactoryFamily { get; set; } = string.Empty; // Ej: Amaderada, Floral, Oriental
        public TargetGender Gender { get; set; }
        public int VolumeMl { get; set; } // Tamaño del frasco en mililitros

        // Notas Olfativas (Pirámide Olfativa)
        public string TopNotes { get; set; } = string.Empty; // Notas de salida (lo primero que se huele)
        public string HeartNotes { get; set; } = string.Empty; // Notas de corazón (el carácter del perfume)
        public string BaseNotes { get; set; } = string.Empty; // Notas de fondo (lo que perdura en la piel)

        // Precios e Inventario
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int StockQuantity { get; set; }
        public bool IsAvailable { get; set; } = true;

        // Multimedia
        public string ImageUrl { get; set; } = string.Empty;

        // Auditoría
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    // Enums recomendados para estandarizar los datos
    public enum ConcentrationType
    {
        EauDeCologne,
        EauDeToilette,
        EauDeParfum,
        Parfum,
        Elixir
    }

    public enum TargetGender
    {
        Men,
        Women,
        Unisex
    }
}
