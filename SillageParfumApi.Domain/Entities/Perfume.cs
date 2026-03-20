using System;

namespace SillageParfumApi.Domain.Entities
{
    public class Perfume
    {
        // 1. Todo tiene 'private set' para proteger la información
        // Identificadores
        public int Id { get; private set; }
        public string SKU { get; private set; } = string.Empty;
        
        // Información básica
        public string Name { get; private set; } = string.Empty;
        public string Brand { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;

        // Especificaciones del Perfume
        public ConcentrationType Concentration { get; private set; }
        public string OlfactoryFamily { get; private set; } = string.Empty;
        public TargetGender Gender { get; private set; }
        public int VolumeMl { get; private set; }

        // Notas Olfativas (Pirámide Olfativa)
        public string TopNotes { get; private set; } = string.Empty;
        public string HeartNotes { get; private set; } = string.Empty;
        public string BaseNotes { get; private set; } = string.Empty;

        // Precios e Inventario
        public decimal Price { get; private set; }
        public decimal? DiscountPrice { get; private set; }
        public int StockQuantity { get; private set; }
        public bool IsAvailable { get; private set; }

        // Multimedia
        public string ImageUrl { get; private set; } = string.Empty;

        // Auditoría
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        // 2. Constructor vacío para Entity Framework (Oculto)
        protected Perfume() { }

        // 3. Constructor Principal: Lo mínimo requerido para crear el producto
        public Perfume(string sku, string name, string brand, decimal price, int volumeMl)
        {
            if (string.IsNullOrWhiteSpace(sku)) throw new ArgumentException("El SKU es obligatorio.");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("El nombre es obligatorio.");
            if (price <= 0) throw new ArgumentException("El precio debe ser mayor a cero.");
            if (volumeMl <= 0) throw new ArgumentException("El volumen debe ser mayor a cero.");

            SKU = sku;
            Name = name;
            Brand = brand;
            Price = price;
            VolumeMl = volumeMl;

            // Valores por defecto lógicos al crear un producto nuevo
            StockQuantity = 0;
            IsAvailable = false; // No está disponible hasta que le agreguen stock
            CreatedAt = DateTime.UtcNow;
        }

        // ==========================================
        // 4. COMPORTAMIENTOS (Reglas de Negocio)
        // ==========================================

        public void ActualizarPrecio(decimal nuevoPrecio)
        {
            if (nuevoPrecio <= 0) throw new ArgumentException("El precio no puede ser cero o negativo.");
            Price = nuevoPrecio;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AplicarDescuento(decimal precioConDescuento)
        {
            if (precioConDescuento >= Price) throw new ArgumentException("El descuento no puede ser mayor o igual al precio original.");
            if (precioConDescuento <= 0) throw new ArgumentException("El precio con descuento debe ser válido.");

            DiscountPrice = precioConDescuento;
            UpdatedAt = DateTime.UtcNow;
        }

        public void QuitarDescuento()
        {
            DiscountPrice = null;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AgregarStock(int cantidad)
        {
            if (cantidad <= 0) throw new ArgumentException("Debe agregar al menos 1 unidad.");
            StockQuantity += cantidad;

            if (StockQuantity > 0) IsAvailable = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ReducirStock(int cantidad)
        {
            if (cantidad <= 0) throw new ArgumentException("La cantidad a reducir debe ser mayor a cero.");
            if (StockQuantity - cantidad < 0) throw new InvalidOperationException("No hay suficiente stock para realizar esta operación.");

            StockQuantity -= cantidad;
            if (StockQuantity == 0) IsAvailable = false; // Se agota automáticamente

            UpdatedAt = DateTime.UtcNow;
        }

        public void EstablecerDetallesOlfativos(ConcentrationType concentration, TargetGender gender, string family, string top, string heart, string baseNotes)
        {
            Concentration = concentration;
            Gender = gender;
            OlfactoryFamily = family;
            TopNotes = top;
            HeartNotes = heart;
            BaseNotes = baseNotes;
            UpdatedAt = DateTime.UtcNow;
        }
        // Método seguro para actualizar la información
        public void ActualizarDatos(string sku, string name, string brand, decimal price, int volumeMl)
        {
            if (price <= 0) throw new ArgumentException("El precio debe ser mayor a cero.");

            SKU = sku;
            Name = name;
            Brand = brand;
            Price = price;
            VolumeMl = volumeMl;
        }
    }

    // Los Enums se quedan aquí en el Dominio, son perfectos.
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