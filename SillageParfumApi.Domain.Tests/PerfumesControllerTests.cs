using Microsoft.AspNetCore.Mvc;
using Moq;
using SillageParfumApi.Application.DTOs;
using SillageParfumApi.Application.Interfaces;
using SillageParfumApi.Controllers;
using Xunit;

namespace SillageParfumApi.Domain.Tests
{
    public class PerfumesControllerTests
    {
        private readonly Mock<IPerfumeService> _mockPerfumeService;
        private readonly Mock<IExternalPerfumeService> _mockExternalService;
        private readonly PerfumesController _controller;

        public PerfumesControllerTests()
        {
            _mockPerfumeService = new Mock<IPerfumeService>();
            _mockExternalService = new Mock<IExternalPerfumeService>();
            _controller = new PerfumesController(_mockPerfumeService.Object, _mockExternalService.Object);
        }

        // --- PRUEBAS DEL GET ---
        [Fact]
        public async Task GetPerfume_SiExiste_Devuelve200OkConDatos()
        {
            int idBuscado = 1;
            var perfumeFalso = new PerfumeResponseDto { Id = idBuscado, Name = "Aventus" };
            _mockPerfumeService.Setup(s => s.ObtenerPerfumePorIdAsync(idBuscado)).ReturnsAsync(perfumeFalso);

            var resultado = await _controller.GetPerfume(idBuscado);

            var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
            var perfumeDevuelto = Assert.IsType<PerfumeResponseDto>(okResult.Value);
            Assert.Equal("Aventus", perfumeDevuelto.Name);
        }

        [Fact]
        public async Task GetPerfume_SiNoExiste_Devuelve404NotFound()
        {
            _mockPerfumeService.Setup(s => s.ObtenerPerfumePorIdAsync(99)).ReturnsAsync((PerfumeResponseDto?)null);

            var resultado = await _controller.GetPerfume(99);

            // Verificamos que devuelve el 404 con mensaje
            Assert.IsType<NotFoundObjectResult>(resultado.Result);
        }

        // --- PRUEBAS DEL POST ---
        [Fact]
        public async Task PostPerfume_DatosValidos_Devuelve201Created()
        {
            var nuevoDto = new CrearPerfumeDto { Name = "Aqua Di Gio" };
            _mockPerfumeService.Setup(s => s.CrearPerfumeAsync(nuevoDto)).ReturnsAsync(10); // Simulamos que se crea con ID 10

            var resultado = await _controller.PostPerfume(nuevoDto);

            // Verifica que devuelve HTTP 201 Created
            var createdResult = Assert.IsType<CreatedAtActionResult>(resultado);
            Assert.Equal(10, createdResult.RouteValues?["id"]);
        }

        // --- PRUEBAS DEL PUT ---
        [Fact]
        public async Task PutPerfume_IdNoCoincide_Devuelve400BadRequest()
        {
            var dto = new ActualizarPerfumeDto { Id = 2 };

            // Intentamos actualizar la URL con ID 1, pero el cuerpo tiene ID 2
            var resultado = await _controller.PutPerfume(1, dto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado);
            Assert.Equal("El ID de la URL no coincide con el ID del modelo.", badRequestResult.Value);
        }

        [Fact]
        public async Task PutPerfume_ActualizacionExitosa_Devuelve204NoContent()
        {
            var dto = new ActualizarPerfumeDto { Id = 1 };
            _mockPerfumeService.Setup(s => s.ActualizarPerfumeAsync(dto)).ReturnsAsync(true);

            var resultado = await _controller.PutPerfume(1, dto);

            Assert.IsType<NoContentResult>(resultado);
        }

        // --- PRUEBAS DEL DELETE ---
        [Fact]
        public async Task DeletePerfume_EliminacionExitosa_Devuelve204NoContent()
        {
            _mockPerfumeService.Setup(s => s.EliminarPerfumeAsync(1)).ReturnsAsync(true);

            var resultado = await _controller.DeletePerfume(1);

            Assert.IsType<NoContentResult>(resultado);
        }
    }
}