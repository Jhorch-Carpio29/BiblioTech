using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using BiblioTech.Application.DTOs.Prestamos;
using BiblioTech.Application.Services;
using BiblioTech.Domain.Entities;
using BiblioTech.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace BiblioTech.Tests
{
    [TestClass]
    public class PrestamoServiceTests
    {
        // Se añade '= null!' para eliminar las advertencias CS8618
        private Mock<IUnitOfWork> _mockUnitOfWork = null!;
        private Mock<ILibroRepository> _mockLibroRepo = null!;
        private Mock<IEstudianteRepository> _mockEstudianteRepo = null!;
        private Mock<IPrestamoRepository> _mockPrestamoRepo = null!;
        private PrestamoService _prestamoService = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLibroRepo = new Mock<ILibroRepository>();
            _mockEstudianteRepo = new Mock<IEstudianteRepository>();
            _mockPrestamoRepo = new Mock<IPrestamoRepository>();

            _mockUnitOfWork.Setup(u => u.Libros).Returns(_mockLibroRepo.Object);
            _mockUnitOfWork.Setup(u => u.Estudiantes).Returns(_mockEstudianteRepo.Object);
            _mockUnitOfWork.Setup(u => u.Prestamos).Returns(_mockPrestamoRepo.Object);

            _prestamoService = new PrestamoService(_mockUnitOfWork.Object);
        }

        // --- PRUEBAS DE REGISTRO DE PRÉSTAMO ---

        [TestMethod]
        public async Task RegistrarPrestamo_FlujoExitoso_RetornaDto()
        {
            var libro = new Libro { Id = 1, StockDisponible = 5, Titulo = "Clean Code" };
            var estudiante = new Estudiante { Id = 1, EsMoroso = false, NombresCompletos = "Jhor Carpio" };
            var dto = new PrestamoCreateDto { LibroId = 1, EstudianteId = 1, FechaHoraLimite = DateTime.UtcNow.AddDays(3) };

            _mockLibroRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(libro);
            _mockEstudianteRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(estudiante);

            var result = await _prestamoService.RegistrarPrestamoAsync(dto);

            Assert.IsNotNull(result);
            Assert.AreEqual("Activo", result.Estado);
            Assert.AreEqual(4, libro.StockDisponible);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task RegistrarPrestamo_LibroNoExiste_LanzaExcepcion()
        {
            var dto = new PrestamoCreateDto { LibroId = 99 };
            _mockLibroRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Libro)null!);

            try
            {
                await _prestamoService.RegistrarPrestamoAsync(dto);
                Assert.Fail("Se esperaba InvalidOperationException.");
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("El libro no existe.", ex.Message);
            }
        }

        [TestMethod]
        public async Task RegistrarPrestamo_LibroSinStock_LanzaExcepcion()
        {
            var libro = new Libro { Id = 1, StockDisponible = 0 };
            var dto = new PrestamoCreateDto { LibroId = 1 };
            _mockLibroRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(libro);

            try
            {
                await _prestamoService.RegistrarPrestamoAsync(dto);
                Assert.Fail("Se esperaba InvalidOperationException.");
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("No hay stock disponible para este libro.", ex.Message);
            }
        }

        [TestMethod]
        public async Task RegistrarPrestamo_EstudianteNoExiste_LanzaExcepcion()
        {
            var libro = new Libro { Id = 1, StockDisponible = 2 };
            var dto = new PrestamoCreateDto { LibroId = 1, EstudianteId = 99 };
            _mockLibroRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(libro);
            _mockEstudianteRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Estudiante)null!);

            try
            {
                await _prestamoService.RegistrarPrestamoAsync(dto);
                Assert.Fail("Se esperaba InvalidOperationException.");
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("El estudiante no existe.", ex.Message);
            }
        }

        [TestMethod]
        public async Task RegistrarPrestamo_EstudianteMoroso_LanzaExcepcion()
        {
            var libro = new Libro { Id = 1, StockDisponible = 2 };
            var estudiante = new Estudiante { Id = 1, EsMoroso = true };
            var dto = new PrestamoCreateDto { LibroId = 1, EstudianteId = 1 };
            _mockLibroRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(libro);
            _mockEstudianteRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(estudiante);

            try
            {
                await _prestamoService.RegistrarPrestamoAsync(dto);
                Assert.Fail("Se esperaba InvalidOperationException.");
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("El estudiante es moroso y no puede realizar préstamos.", ex.Message);
            }
        }

        [TestMethod]
        public async Task RegistrarPrestamo_FechaLimiteInvalida_LanzaExcepcion()
        {
            var libro = new Libro { Id = 1, StockDisponible = 2 };
            var estudiante = new Estudiante { Id = 1, EsMoroso = false };
            var dto = new PrestamoCreateDto { LibroId = 1, EstudianteId = 1, FechaHoraLimite = DateTime.UtcNow.AddDays(-1) };

            _mockLibroRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(libro);
            _mockEstudianteRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(estudiante);

            try
            {
                await _prestamoService.RegistrarPrestamoAsync(dto);
                Assert.Fail("Se esperaba InvalidOperationException.");
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("La fecha límite debe ser mayor a la fecha actual.", ex.Message);
            }
        }

        // --- PRUEBAS DE DEVOLUCIÓN ---

        [TestMethod]
        public async Task RegistrarDevolucion_PrestamoNoExiste_LanzaExcepcion()
        {
            var prestamoId = Guid.NewGuid();
            _mockPrestamoRepo.Setup(r => r.GetByIdAsync(prestamoId)).ReturnsAsync((Prestamo)null!);

            try
            {
                await _prestamoService.RegistrarDevolucionAsync(prestamoId);
                Assert.Fail("Se esperaba InvalidOperationException.");
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("El préstamo no existe o ya está devuelto.", ex.Message);
            }
        }

        [TestMethod]
        public async Task RegistrarDevolucion_DentroDeFecha_AumentaStock()
        {
            var prestamoId = Guid.NewGuid();
            var prestamo = new Prestamo { Id = prestamoId, LibroId = 1, EstudianteId = 1, Estado = "Activo", FechaHoraLimite = DateTime.UtcNow.AddDays(1) };
            var libro = new Libro { Id = 1, StockDisponible = 3 };

            _mockPrestamoRepo.Setup(r => r.GetByIdAsync(prestamoId)).ReturnsAsync(prestamo);
            _mockLibroRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(libro);

            var result = await _prestamoService.RegistrarDevolucionAsync(prestamoId);

            Assert.IsTrue(result);
            Assert.AreEqual("Devuelto", prestamo.Estado);
            Assert.AreEqual(4, libro.StockDisponible);
            _mockEstudianteRepo.Verify(r => r.Update(It.IsAny<Estudiante>()), Times.Never);
        }

        [TestMethod]
        public async Task RegistrarDevolucion_FueraDeFecha_PenalizaMoroso()
        {
            var prestamoId = Guid.NewGuid();
            var prestamo = new Prestamo { Id = prestamoId, LibroId = 1, EstudianteId = 1, Estado = "Activo", FechaHoraLimite = DateTime.UtcNow.AddDays(-2) };
            var libro = new Libro { Id = 1, StockDisponible = 1 };
            var estudiante = new Estudiante { Id = 1, EsMoroso = false };

            _mockPrestamoRepo.Setup(r => r.GetByIdAsync(prestamoId)).ReturnsAsync(prestamo);
            _mockLibroRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(libro);
            _mockEstudianteRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(estudiante);

            var result = await _prestamoService.RegistrarDevolucionAsync(prestamoId);

            Assert.IsTrue(result);
            Assert.IsTrue(estudiante.EsMoroso);
            _mockEstudianteRepo.Verify(r => r.Update(estudiante), Times.Once);
        }
    }
}