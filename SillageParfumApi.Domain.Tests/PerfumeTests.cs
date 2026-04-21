using SillageParfumApi.Domain.Entities;
using Xunit;

namespace SillageParfumApi.Domain.Tests
{
    public class PerfumeTests
    {
        // La etiqueta [Fact] le dice a Visual Studio "Hola, soy una prueba automatizada"
        [Fact]
        public void ActualizarDatos_PrecioCeroONegativo_LanzaArgumentException()
        {
            // 1. ARRANGE (Preparar el escenario)
            // Creamos un perfume válido inicialmente
            var perfume = new Perfume("SKU-123", "Sauvage", "Dior", 150000, 100);

            // 2. ACT & 3. ASSERT (Actuar y Afirmar)
            // Aquí le decimos a xUnit: "Te apuesto a que si intento poner el precio en 0, 
            // el sistema va a lanzar un ArgumentException".

            var excepcion = Assert.Throws<ArgumentException>(() =>
            {
                // Intentamos hacer la acción ilegal
                perfume.ActualizarDatos("SKU-123", "Sauvage", "Dior", 0, 100);
            });

            // (Opcional) Podemos incluso afirmar que el mensaje de error sea el correcto
            Assert.Equal("El precio debe ser mayor a cero.", excepcion.Message);
        }
    }
}