using Moq;
using SillageParfumApi.Application.DTOs;
using SillageParfumApi.Application.Services;
using SillageParfumApi.Domain.Entities;
using SillageParfumApi.Domain.Interfaces;
using Xunit;

namespace SillageParfumApi.Domain.Tests
{
    public class PerfumeServiceTests
    {
        private readonly Mock<IPerfumeRepository> _mockRepository;
        private readonly PerfumeService _perfumeService;

        public PerfumeServiceTests()
        {
            // ARRANGE GENERAL: Lo ponemos en el constructor para no repetirlo en cada prueba
            _mockRepository = new Mock<IPerfumeRepository>();
            _perfumeService = new PerfumeService(_mockRepository.Object);
        }

        [Fact]
        public async Task ObtenerTodos_DevuelveListaMapeadaDeDTOs()
        {
            // 1. ARRANGE
            var perfumesFalsos = new List<Perfume> { new Perfume("SKU-1", "Aventus", "Creed", 250000, 100) };
            _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(perfumesFalsos);

            // 2. ACT
            var resultado = await _perfumeService.ObtenerTodosLosPerfumesAsync();

            // 3. ASSERT
            Assert.Single(resultado);
            Assert.Equal("Aventus", resultado.First().Name);
        }

        [Fact]
        public async Task CrearPerfume_LlamaAlRepositorio_YDevuelveNuevoId()
        {
            // 1. ARRANGE
            var nuevoDto = new CrearPerfumeDto { SKU = "SKU-99", Name = "Nautica Voyage", Brand = "Nautica", Price = 30000, VolumeMl = 100 };

            // Creamos un objeto Perfume falso para simular lo que devolvería la base de datos
            var perfumeSimulado = new Perfume("SKU-99", "Nautica Voyage", "Nautica", 30000, 100);

            // Le decimos al mock que devuelva el OBJETO (perfumeSimulado), no un número
            _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<Perfume>())).ReturnsAsync(perfumeSimulado);

            // 2. ACT
            var nuevoId = await _perfumeService.CrearPerfumeAsync(nuevoDto);

            // 3. ASSERT
            // Como acabamos de crear el objeto Perfume manualmente y no ha pasado por SQL, 
            // su Id por defecto en C# es 0. Así que afirmamos que el servicio nos devuelva ese 0.
            Assert.Equal(0, nuevoId);

            // Verificamos que el obrero realmente fue a guardar el dato
            _mockRepository.Verify(repo => repo.CreateAsync(It.IsAny<Perfume>()), Times.Once);
        }

        [Fact]
        public async Task ActualizarPerfume_SiNoExiste_DevuelveFalse()
        {
            // 1. ARRANGE
            var dtoActualizar = new ActualizarPerfumeDto { Id = 99 };
            _mockRepository.Setup(repo => repo.GetByIdAsync(99)).ReturnsAsync((Perfume?)null);

            // 2. ACT
            var resultado = await _perfumeService.ActualizarPerfumeAsync(dtoActualizar);

            // 3. ASSERT
            Assert.False(resultado);
        }

        [Fact]
        public async Task EliminarPerfume_LlamaAlRepositorio_YDevuelveSuResultado()
        {
            // 1. ARRANGE
            _mockRepository.Setup(repo => repo.DeleteAsync(1)).ReturnsAsync(true);

            // 2. ACT
            var resultado = await _perfumeService.EliminarPerfumeAsync(1);

            // 3. ASSERT
            Assert.True(resultado);
            _mockRepository.Verify(repo => repo.DeleteAsync(1), Times.Once);
        }
    }
}