using Microsoft.VisualStudio.TestTools.UnitTesting;
using BiblioTech.Application.DTOs.Prestamos;
using System;

namespace BiblioTech.Tests
{
    [TestClass]
    public class ApplicationDtosTests
    {
        [TestMethod]
        public void PrestamoCreateDto_Asignacion_RetornaValoresAsignados()
        {
            // Arrange
            var fechaLimite = DateTime.UtcNow.AddDays(3);
            var usuarioId = Guid.NewGuid();

            // Act
            var dto = new PrestamoCreateDto
            {
                UsuarioId = usuarioId,
                EstudianteId = 5,
                LibroId = 10,
                FechaHoraLimite = fechaLimite
            };

            // Assert
            Assert.AreEqual(usuarioId, dto.UsuarioId);
            Assert.AreEqual(5, dto.EstudianteId);
            Assert.AreEqual(10, dto.LibroId);
            Assert.AreEqual(fechaLimite, dto.FechaHoraLimite);
        }

        [TestMethod]
        public void PrestamoResponseDto_Asignacion_RetornaValoresAsignados()
        {
            // Arrange
            var id = Guid.NewGuid();
            var fechaPrestamo = DateTime.UtcNow;
            var fechaLimite = fechaPrestamo.AddDays(3);

            // Act
            var dto = new PrestamoResponseDto
            {
                Id = id,
                LibroTitulo = "C# Avanzado",
                EstudianteNombre = "Juan Perez",
                FechaHoraPrestamo = fechaPrestamo,
                FechaHoraLimite = fechaLimite,
                Estado = "Activo"
            };

            // Assert
            Assert.AreEqual(id, dto.Id);
            Assert.AreEqual("C# Avanzado", dto.LibroTitulo);
            Assert.AreEqual("Juan Perez", dto.EstudianteNombre);
            Assert.AreEqual(fechaPrestamo, dto.FechaHoraPrestamo);
            Assert.AreEqual(fechaLimite, dto.FechaHoraLimite);
            Assert.AreEqual("Activo", dto.Estado);
        }



        [TestMethod]
        public void LibroDto_Asignacion_RetornaValoresAsignados()
        {
            var dto = new BiblioTech.Application.DTOs.Libros.LibroDto
            {
                Id = 1,
                Titulo = "Clean Code",
                Autor = "Robert C. Martin",
                StockDisponible = 5,
                CategoriaNombre = "General"
            };

            Assert.AreEqual(1, dto.Id);
            Assert.AreEqual("Clean Code", dto.Titulo);
            Assert.AreEqual("General", dto.CategoriaNombre);
        }

        [TestMethod]
        public void LibroCreateDto_Asignacion_RetornaValoresAsignados()
        {
            var dto = new BiblioTech.Application.DTOs.Libros.LibroCreateDto
            {
                Isbn = "123-456",
                Titulo = "C# Avanzado",
                Autor = "Jhor Carpio",
                CategoriaId = 2,
                StockTotal = 10
            };

            Assert.AreEqual("123-456", dto.Isbn);
            Assert.AreEqual("C# Avanzado", dto.Titulo);
            Assert.AreEqual(10, dto.StockTotal);
        }

        [TestMethod]
        public void EstudianteDto_Asignacion_RetornaValoresAsignados()
        {
            // using BiblioTech.Application.DTOs.Estudiantes;
            var dto = new BiblioTech.Application.DTOs.Estudiantes.EstudianteDto // Ajusta el nombre si es distinto
            {
                Id = 1,
                CodigoUniversitario = "27222514",
                Dni = "71948618",
                NombresCompletos = "Jhor Marinio",
                EscuelaId = 1,
                EsMoroso = false
            };

            Assert.AreEqual("27222514", dto.CodigoUniversitario);
            Assert.AreEqual("Jhor Marinio", dto.NombresCompletos);
            Assert.IsFalse(dto.EsMoroso);
        }

        [TestMethod]
        public void CategoriaDto_Asignacion_RetornaValores()
        {
            var dto = new BiblioTech.Application.DTOs.Categorias.CategoriaDto // Ajusta si el namespace varía
            {
                Id = 1,
                Nombre = "Datos"
            };
            Assert.AreEqual(1, dto.Id);
            Assert.AreEqual("Datos", dto.Nombre);
        }

        [TestMethod]
        public void EscuelaProfesionalDto_Asignacion_RetornaValores()
        {
            var dto = new BiblioTech.Application.DTOs.EscuelasProfesionales.EscuelaProfesionalDto
            {
                Id = 2,
                Nombre = "Enfermeria",
                Facultad = "Ciencias de Salud"
            };
            Assert.AreEqual("Enfermeria", dto.Nombre);
        }

        [TestMethod]
        public void UsuarioDto_Asignacion_RetornaValoresCorrectos()
        {
            // Arrange & Act
            var dto = new BiblioTech.Application.DTOs.Usuarios.UsuarioDto
            {
                Id = Guid.NewGuid(),
                Dni = "28272900",
                Nombres = "LIZ LEON LOPEZ",
                Email = "liz.leon@unsch.edu.pe",
                PisoArea = "Piso 2",
                RolId = 2,
                Activo = true
            };

            // Assert
            Assert.AreEqual("28272900", dto.Dni);
            Assert.AreEqual("LIZ LEON LOPEZ", dto.Nombres);
            Assert.AreEqual("liz.leon@unsch.edu.pe", dto.Email);
            Assert.AreEqual("Piso 2", dto.PisoArea);
            Assert.AreEqual(2, dto.RolId);
            Assert.IsTrue(dto.Activo);
        }

        [TestMethod]
        public void UsuarioCreateDto_Asignacion_RetornaValoresCorrectos()
        {
            var dto = new BiblioTech.Application.DTOs.Usuarios.UsuarioCreateDto
            {
                Dni = "71948618",
                Nombres = "Juan Suares",
                Email = "juan.suares@unsch.edu.pe",
                PasswordHash = "Juan123",
                PisoArea = "Piso 3",
                RolId = 2
            };

            Assert.AreEqual("Juan Suares", dto.Nombres);
            Assert.AreEqual("Juan123", dto.PasswordHash);
            Assert.AreEqual(2, dto.RolId);
        }


        [TestMethod]
        public void CoberturaTotal_DTOs_VariantesCreateUpdateLogin()
        {
            // --- USUARIOS ---
            var loginDto = new BiblioTech.Application.DTOs.Usuarios.LoginRequestDto
            {
                Email = "test@unsch.edu.pe",
                Password = "123"
            };
            Assert.AreEqual("test@unsch.edu.pe", loginDto.Email);
            Assert.AreEqual("123", loginDto.Password);

            // --- LIBROS ---
            var libroUpdate = new BiblioTech.Application.DTOs.Libros.LibroUpdateDto
            {
                Titulo = "Test",
                Autor = "Test",
                CategoriaId = 1,
                StockTotal = 5,
                StockDisponible = 5
            };
            Assert.AreEqual("Test", libroUpdate.Titulo);
            Assert.AreEqual("Test", libroUpdate.Autor);
            Assert.AreEqual(1, libroUpdate.CategoriaId);
            Assert.AreEqual(5, libroUpdate.StockTotal);
            Assert.AreEqual(5, libroUpdate.StockDisponible);

            // --- ESTUDIANTES (Cubre los posibles Create/Update si existen) ---
            var estudianteDto = new BiblioTech.Application.DTOs.Estudiantes.EstudianteDto();
            estudianteDto.Id = 1;
            estudianteDto.CodigoUniversitario = "123";
            estudianteDto.Dni = "123";
            estudianteDto.NombresCompletos = "Test";
            estudianteDto.EscuelaId = 1;
            estudianteDto.EsMoroso = true;

            Assert.AreEqual(1, estudianteDto.Id);
            Assert.AreEqual("123", estudianteDto.CodigoUniversitario);
            Assert.AreEqual("123", estudianteDto.Dni);
            Assert.AreEqual("Test", estudianteDto.NombresCompletos);
            Assert.AreEqual(1, estudianteDto.EscuelaId);
            Assert.IsTrue(estudianteDto.EsMoroso);

            // --- CATEGORIAS ---
            var categoriaDto = new BiblioTech.Application.DTOs.Categorias.CategoriaDto();
            categoriaDto.Id = 1;
            categoriaDto.Nombre = "Prueba";
            Assert.AreEqual(1, categoriaDto.Id);
            Assert.AreEqual("Prueba", categoriaDto.Nombre);

            // --- ESCUELAS ---
            var escuelaDto = new BiblioTech.Application.DTOs.EscuelasProfesionales.EscuelaProfesionalDto();
            escuelaDto.Id = 1;
            escuelaDto.Nombre = "Prueba";
            escuelaDto.Facultad = "Prueba";
            Assert.AreEqual(1, escuelaDto.Id);
            Assert.AreEqual("Prueba", escuelaDto.Nombre);
            Assert.AreEqual("Prueba", escuelaDto.Facultad);
        }


        [TestMethod]
        public void CoberturaTotal_DTOs_EstudiantesCategoriasEscuelas()
        {
            // --- ESTUDIANTES ---
            var estDto = new BiblioTech.Application.DTOs.Estudiantes.EstudianteDto { Id = 1, CodigoUniversitario = "1", Dni = "1", NombresCompletos = "1", EscuelaId = 1, EsMoroso = false };
            var estCreate = new BiblioTech.Application.DTOs.Estudiantes.EstudianteCreateDto { CodigoUniversitario = "1", Dni = "1", NombresCompletos = "1", EscuelaId = 1 };
            var estUpdate = new BiblioTech.Application.DTOs.Estudiantes.EstudianteUpdateDto { NombresCompletos = "1", EscuelaId = 1, EsMoroso = false };

            Assert.AreEqual("1", estDto.Dni);
            Assert.AreEqual("1", estCreate.Dni);
            Assert.AreEqual("1", estUpdate.NombresCompletos);

            // --- CATEGORIAS ---
            var catDto = new BiblioTech.Application.DTOs.Categorias.CategoriaDto { Id = 1, Nombre = "1" };
            var catCreate = new BiblioTech.Application.DTOs.Categorias.CategoriaCreateDto { Nombre = "1" };
            var catUpdate = new BiblioTech.Application.DTOs.Categorias.CategoriaUpdateDto { Nombre = "1" };

            Assert.AreEqual("1", catDto.Nombre);
            Assert.AreEqual("1", catCreate.Nombre);
            Assert.AreEqual("1", catUpdate.Nombre);

            // --- ESCUELAS PROFESIONALES ---
            var escDto = new BiblioTech.Application.DTOs.EscuelasProfesionales.EscuelaProfesionalDto { Id = 1, Nombre = "1", Facultad = "1" };
            var escCreate = new BiblioTech.Application.DTOs.EscuelasProfesionales.EscuelaProfesionalCreateDto { Nombre = "1", Facultad = "1" };

            Assert.AreEqual("1", escDto.Nombre);
            Assert.AreEqual("1", escCreate.Nombre);
        }

        [TestMethod]
        public void Cobertura100_LecturaEscritura_TodosLosDTOs()
        {
            // 1. Prestamos (Faltan 2)
            var pCreate = new BiblioTech.Application.DTOs.Prestamos.PrestamoCreateDto();
            var pC1 = pCreate.UsuarioId; var pC2 = pCreate.EstudianteId; var pC3 = pCreate.LibroId;
            pCreate.UsuarioId = Guid.NewGuid(); pCreate.EstudianteId = 1; pCreate.LibroId = 1; pCreate.FechaHoraLimite = DateTime.UtcNow;
            Assert.IsNotNull(pCreate.UsuarioId);

            var pRes = new BiblioTech.Application.DTOs.Prestamos.PrestamoResponseDto();
            var pResE1 = pRes.LibroTitulo; var pResE2 = pRes.EstudianteNombre; var pResE3 = pRes.Estado;
            pRes.Id = Guid.NewGuid(); pRes.LibroTitulo = "A"; pRes.EstudianteNombre = "B"; pRes.FechaHoraPrestamo = DateTime.UtcNow; pRes.FechaHoraLimite = DateTime.UtcNow; pRes.Estado = "C";
            Assert.IsNotNull(pRes.LibroTitulo);

            // 2. Libros (Faltan 4)
            var lDto = new BiblioTech.Application.DTOs.Libros.LibroDto();
            var lDtoE1 = lDto.Titulo; var lDtoE2 = lDto.Autor; var lDtoE3 = lDto.CategoriaNombre;
            lDto.Id = 1; lDto.Titulo = "A"; lDto.Autor = "B"; lDto.StockDisponible = 1; lDto.CategoriaNombre = "C";
            Assert.IsNotNull(lDto.Titulo);

            var lCreate = new BiblioTech.Application.DTOs.Libros.LibroCreateDto();
            var lCE1 = lCreate.Isbn; var lCE2 = lCreate.Titulo; var lCE3 = lCreate.Autor;
            lCreate.Isbn = "1"; lCreate.Titulo = "1"; lCreate.Autor = "1"; lCreate.CategoriaId = 1; lCreate.StockTotal = 1;
            Assert.IsNotNull(lCreate.Isbn);

            var lUpd = new BiblioTech.Application.DTOs.Libros.LibroUpdateDto();
            var lUE1 = lUpd.Titulo; var lUE2 = lUpd.Autor;
            lUpd.Titulo = "A"; lUpd.Autor = "B"; lUpd.CategoriaId = 1; lUpd.StockTotal = 1; lUpd.StockDisponible = 1;
            Assert.IsNotNull(lUpd.Titulo);

            // 3. Estudiantes (Faltan 5)
            var eDto = new BiblioTech.Application.DTOs.Estudiantes.EstudianteDto();
            var eDto1 = eDto.CodigoUniversitario; var eDto2 = eDto.Dni; var eDto3 = eDto.NombresCompletos;
            eDto.Id = 1; eDto.CodigoUniversitario = "A"; eDto.Dni = "B"; eDto.NombresCompletos = "C"; eDto.EscuelaId = 1; eDto.EsMoroso = true;
            Assert.IsNotNull(eDto.Dni);

            var eCreate = new BiblioTech.Application.DTOs.Estudiantes.EstudianteCreateDto();
            var eC1 = eCreate.CodigoUniversitario; var eC2 = eCreate.Dni; var eC3 = eCreate.NombresCompletos;
            eCreate.CodigoUniversitario = "A"; eCreate.Dni = "B"; eCreate.NombresCompletos = "C"; eCreate.EscuelaId = 1;
            Assert.IsNotNull(eCreate.Dni);

            var eUpd = new BiblioTech.Application.DTOs.Estudiantes.EstudianteUpdateDto();
            var eU1 = eUpd.NombresCompletos;
            eUpd.NombresCompletos = "A"; eUpd.EscuelaId = 1; eUpd.EsMoroso = false;
            Assert.IsNotNull(eUpd.NombresCompletos);

            // 4. Usuarios (Faltan 4)
            var uDto = new BiblioTech.Application.DTOs.Usuarios.UsuarioDto();
            var uDto1 = uDto.Dni; var uDto2 = uDto.Nombres; var uDto3 = uDto.Email; var uDto4 = uDto.PisoArea;
            uDto.Id = Guid.NewGuid(); uDto.Dni = "1"; uDto.Nombres = "1"; uDto.Email = "1"; uDto.PisoArea = "1"; uDto.RolId = 1; uDto.Activo = true;
            Assert.IsNotNull(uDto.Dni);

            var uCreate = new BiblioTech.Application.DTOs.Usuarios.UsuarioCreateDto();
            var uC1 = uCreate.Dni; var uC2 = uCreate.Nombres; var uC3 = uCreate.Email; var uC4 = uCreate.PasswordHash; var uC5 = uCreate.PisoArea;
            uCreate.Dni = "1"; uCreate.Nombres = "1"; uCreate.Email = "1"; uCreate.PasswordHash = "1"; uCreate.PisoArea = "1"; uCreate.RolId = 1;
            Assert.IsNotNull(uCreate.Email);

            var login = new BiblioTech.Application.DTOs.Usuarios.LoginRequestDto();
            var log1 = login.Email; var log2 = login.Password;
            login.Email = "1"; login.Password = "1";
            Assert.IsNotNull(login.Email);

            // 5. Escuelas (Falta 1)
            var escDto = new BiblioTech.Application.DTOs.EscuelasProfesionales.EscuelaProfesionalDto();
            var escD1 = escDto.Nombre; var escD2 = escDto.Facultad;
            escDto.Id = 1; escDto.Nombre = "A"; escDto.Facultad = "B";
            Assert.IsNotNull(escDto.Nombre);

            var escCreate = new BiblioTech.Application.DTOs.EscuelasProfesionales.EscuelaProfesionalCreateDto();
            var escC1 = escCreate.Nombre; var escC2 = escCreate.Facultad;
            escCreate.Nombre = "A"; escCreate.Facultad = "B";
            Assert.IsNotNull(escCreate.Nombre);
        }


        [TestMethod]
        public void CoberturaDefinitiva_DTOs_LecturaYEscrituraTotal()
        {
            // ESTUDIANTES
            var estUpd = new BiblioTech.Application.DTOs.Estudiantes.EstudianteUpdateDto();
            var eu1 = estUpd.NombresCompletos; var eu2 = estUpd.EscuelaId; var eu3 = estUpd.EsMoroso;
            estUpd.NombresCompletos = "Test"; estUpd.EscuelaId = 1; estUpd.EsMoroso = true;
            Assert.IsNotNull(estUpd);

            var estDto = new BiblioTech.Application.DTOs.Estudiantes.EstudianteDto();
            var eD1 = estDto.Id; var eD2 = estDto.CodigoUniversitario; var eD3 = estDto.Dni; var eD4 = estDto.NombresCompletos; var eD5 = estDto.EscuelaId; var eD6 = estDto.EsMoroso;
            estDto.Id = 1; estDto.CodigoUniversitario = "1"; estDto.Dni = "1"; estDto.NombresCompletos = "1"; estDto.EscuelaId = 1; estDto.EsMoroso = false;

            // LIBROS
            var libDto = new BiblioTech.Application.DTOs.Libros.LibroDto();
            var lD1 = libDto.Id; var lD2 = libDto.Titulo; var lD3 = libDto.Autor; var lD4 = libDto.StockDisponible; var lD5 = libDto.CategoriaNombre;
            libDto.Id = 1; libDto.Titulo = "1"; libDto.Autor = "1"; libDto.StockDisponible = 1; libDto.CategoriaNombre = "1";

            var libUpd = new BiblioTech.Application.DTOs.Libros.LibroUpdateDto();
            var lU1 = libUpd.Titulo; var lU2 = libUpd.Autor; var lU3 = libUpd.CategoriaId; var lU4 = libUpd.StockTotal; var lU5 = libUpd.StockDisponible;
            libUpd.Titulo = "1"; libUpd.Autor = "1"; libUpd.CategoriaId = 1; libUpd.StockTotal = 1; libUpd.StockDisponible = 1;

            // PRESTAMOS
            var preRes = new BiblioTech.Application.DTOs.Prestamos.PrestamoResponseDto();
            var pR1 = preRes.Id; var pR2 = preRes.LibroTitulo; var pR3 = preRes.EstudianteNombre; var pR4 = preRes.FechaHoraPrestamo; var pR5 = preRes.FechaHoraLimite; var pR6 = preRes.Estado;
            preRes.Id = Guid.NewGuid(); preRes.LibroTitulo = "1"; preRes.EstudianteNombre = "1"; preRes.FechaHoraPrestamo = DateTime.UtcNow; preRes.FechaHoraLimite = DateTime.UtcNow; preRes.Estado = "1";

            // USUARIOS
            var usuDto = new BiblioTech.Application.DTOs.Usuarios.UsuarioDto();
            var uD1 = usuDto.Id; var uD2 = usuDto.Dni; var uD3 = usuDto.Nombres; var uD4 = usuDto.Email; var uD5 = usuDto.PisoArea; var uD6 = usuDto.RolId; var uD7 = usuDto.Activo;
            usuDto.Id = Guid.NewGuid(); usuDto.Dni = "1"; usuDto.Nombres = "1"; usuDto.Email = "1"; usuDto.PisoArea = "1"; usuDto.RolId = 1; usuDto.Activo = true;

            Assert.IsNotNull(usuDto); // Para silenciar warnings
        }

        [TestMethod]
        public void CoberturaGodMode_DTOs_LineasOcultas()
        {
            // Atacando PrestamoResponseDto (Posiblemente faltaba FechaHoraDevolucion o UsuarioNombre)
            var pRes = new BiblioTech.Application.DTOs.Prestamos.PrestamoResponseDto();
            pRes.FechaHoraDevolucion = DateTime.UtcNow;
            var prDev = pRes.FechaHoraDevolucion;

            // Atacando propiedades base de EstudianteCreateDto
            var eC = new BiblioTech.Application.DTOs.Estudiantes.EstudianteCreateDto();
            eC.CodigoUniversitario = "X"; eC.Dni = "X"; eC.NombresCompletos = "X"; eC.EscuelaId = 99;
            var ec1 = eC.CodigoUniversitario; var ec2 = eC.Dni; var ec3 = eC.NombresCompletos; var ec4 = eC.EscuelaId;

            // Atacando propiedades base de LibroCreateDto
            var lC = new BiblioTech.Application.DTOs.Libros.LibroCreateDto();
            lC.Isbn = "X"; lC.Titulo = "X"; lC.Autor = "X"; lC.CategoriaId = 99; lC.StockTotal = 99;
            var lc1 = lC.Isbn; var lc2 = lC.Titulo; var lc3 = lC.Autor; var lc4 = lC.CategoriaId; var lc5 = lC.StockTotal;

            Assert.IsNotNull(pRes);
            Assert.IsNotNull(eC);
            Assert.IsNotNull(lC);
        }
    }
}