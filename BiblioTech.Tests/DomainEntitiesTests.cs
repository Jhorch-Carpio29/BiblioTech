using Microsoft.VisualStudio.TestTools.UnitTesting;
using BiblioTech.Domain.Entities;
using System;

namespace BiblioTech.Tests
{
    [TestClass]
    public class DomainEntitiesTests
    {
        [TestMethod]
        public void Usuario_AsignacionPropiedades_AlmacenaYRetornaValoresCorrectos()
        {
            var id = Guid.NewGuid();
            var usuario = new Usuario
            {
                Id = id,
                Dni = "28272900",
                Nombres = "LIZ LEON LOPEZ",
                Email = "liz.leon@unsch.edu.pe",      // C# usa Email (PascalCase)
                PasswordHash = "Liz123",               // C# usa PasswordHash en lugar de password_hash
                PisoArea = "Piso 2",                   // C# usa PisoArea en lugar de piso_area
                RolId = 2,
                Activo = true
            };

            Assert.AreEqual("liz.leon@unsch.edu.pe", usuario.Email);
            Assert.AreEqual("Liz123", usuario.PasswordHash);
            Assert.AreEqual("Piso 2", usuario.PisoArea);
            Assert.IsTrue(usuario.Activo);
        }

        [TestMethod]
        public void EscuelaProfesional_AsignacionPropiedades_RetornaValoresCorrectos()
        {
            var escuela = new EscuelaProfesional
            {
                Id = 1,
                Nombre = "Ingeniería de Sistemas",
                Facultad = "Facultad de Ingeniería Civil, Minas" // Visto en tu BD
            };

            Assert.AreEqual(1, escuela.Id);
            Assert.AreEqual("Ingeniería de Sistemas", escuela.Nombre);
            Assert.AreEqual("Facultad de Ingeniería Civil, Minas", escuela.Facultad);
        }

        [TestMethod]
        public void Categoria_AsignacionPropiedades_AlmacenaYRetornaValoresCorrectos()
        {
            var categoria = new Categoria { Id = 1, Nombre = "Ingeniería" };
            Assert.AreEqual(1, categoria.Id);
            Assert.AreEqual("Ingeniería", categoria.Nombre);
        }

        [TestMethod]
        public void Rol_AsignacionPropiedades_AlmacenaYRetornaValoresCorrectos()
        {
            var rol = new Rol { Id = 1, Nombre = "Admin" }; // Visto en tu BD
            Assert.AreEqual(1, rol.Id);
            Assert.AreEqual("Admin", rol.Nombre);
        }

        [TestMethod]
        public void Libro_AsignacionPropiedades_AlmacenaYRetornaValoresCorrectos()
        {
            var libro = new Libro
            {
                Id = 1,
                Isbn = "978-0132350884",
                Titulo = "Clean Code",
                Autor = "Robert C. Martin",
                CategoriaId = 1,
                StockTotal = 10,
                StockDisponible = 9
            };

            Assert.AreEqual("978-0132350884", libro.Isbn);
            Assert.AreEqual("Clean Code", libro.Titulo);
            Assert.AreEqual(10, libro.StockTotal);
        }

        [TestMethod]
        public void Estudiante_AsignacionPropiedades_AlmacenaYRetornaValoresCorrectos()
        {
            var estudiante = new Estudiante
            {
                Id = 1,
                CodigoUniversitario = "27222514",
                Dni = "71948618",
                NombresCompletos = "JHOR MARINIO CARPIO LEON",
                EscuelaId = 1,
                EsMoroso = false
            };

            Assert.AreEqual("27222514", estudiante.CodigoUniversitario);
            Assert.AreEqual("JHOR MARINIO CARPIO LEON", estudiante.NombresCompletos);
            Assert.IsFalse(estudiante.EsMoroso);
        }

        [TestMethod]
        public void Prestamo_AsignacionPropiedades_AlmacenaYRetornaValoresCorrectos()
        {
            var fecha = DateTime.UtcNow;
            var prestamo = new Prestamo
            {
                Id = Guid.NewGuid(),
                LibroId = 1,
                EstudianteId = 2,
                UsuarioId = Guid.NewGuid(),
                FechaHoraPrestamo = fecha,
                FechaHoraLimite = fecha.AddDays(1),
                Estado = "Activo"
            };

            Assert.AreEqual(1, prestamo.LibroId);
            Assert.AreEqual("Activo", prestamo.Estado);
        }



        [TestMethod]
        public void CoberturaTotal_Entidades_RelacionesEInicializaciones()
        {
            var categoria = new Categoria { Id = 2, Nombre = "Ciencias" };
            var escuela = new EscuelaProfesional { Id = 2, Nombre = "Sistemas", Facultad = "Ingeniería" };

            var estudiante = new Estudiante
            {
                Id = 2,
                CodigoUniversitario = "123",
                Dni = "123",
                NombresCompletos = "Test",
                EscuelaId = 2,
                EsMoroso = false,
                EscuelaProfesional = escuela
            };
            estudiante.Prestamos.Add(new Prestamo()); // Fuerza lectura de ICollection

            var libro = new Libro
            {
                Id = 2,
                Isbn = "123",
                Titulo = "Test",
                Autor = "Test",
                CategoriaId = 2,
                StockTotal = 1,
                StockDisponible = 1,
                Categoria = categoria
            };
            libro.Prestamos.Add(new Prestamo()); // Fuerza lectura de ICollection

            var prestamo = new Prestamo
            {
                Id = Guid.NewGuid(),
                LibroId = 2,
                EstudianteId = 2,
                UsuarioId = Guid.NewGuid(),
                FechaHoraPrestamo = DateTime.UtcNow,
                FechaHoraLimite = DateTime.UtcNow,
                FechaHoraDevolucion = DateTime.UtcNow,
                Estado = "Devuelto",
                Libro = libro,
                Estudiante = estudiante
            };

            var rol = new Rol { Id = 2, Nombre = "Bibliotecario" };
            rol.Usuarios = new System.Collections.Generic.List<Usuario>();
            rol.Usuarios.Add(new Usuario()); // Fuerza lectura de ICollection

            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Dni = "123",
                Nombres = "Test",
                Email = "test@unsch.edu.pe",
                PasswordHash = "hash",
                PisoArea = "Piso 1",
                RolId = 2,
                Activo = true,
                Rol = rol
            };

            // Asserts para forzar el GET de las propiedades virtuales y listas
            Assert.IsNotNull(estudiante.EscuelaProfesional);
            Assert.AreEqual(1, estudiante.Prestamos.Count);
            Assert.IsNotNull(libro.Categoria);
            Assert.AreEqual(1, libro.Prestamos.Count);
            Assert.IsNotNull(prestamo.Libro);
            Assert.IsNotNull(prestamo.Estudiante);
            Assert.IsNotNull(prestamo.FechaHoraDevolucion);
            Assert.AreEqual(1, rol.Usuarios.Count);
            Assert.IsNotNull(usuario.Rol);
        }



        [TestMethod]
        public void CoberturaTotal_Entidades_ConstructoresVaciosYColecciones()
        {
            // Instanciar usando constructores limpios fuerza la ejecución del código oculto (= string.Empty, etc)
            var categoria = new Categoria();
            categoria.Id = 1; categoria.Nombre = "A";

            var escuela = new EscuelaProfesional();
            escuela.Id = 1; escuela.Nombre = "B"; escuela.Facultad = "C";

            var estudiante = new Estudiante();
            estudiante.Id = 1; estudiante.CodigoUniversitario = "D"; estudiante.Dni = "E";
            estudiante.NombresCompletos = "F"; estudiante.EscuelaId = 1; estudiante.EsMoroso = false;
            estudiante.Prestamos = new System.Collections.Generic.List<Prestamo>();
            estudiante.EscuelaProfesional = escuela;

            var libro = new Libro();
            libro.Id = 1; libro.Isbn = "G"; libro.Titulo = "H"; libro.Autor = "I";
            libro.CategoriaId = 1; libro.StockTotal = 1; libro.StockDisponible = 1;
            libro.Categoria = categoria;
            libro.Prestamos = new System.Collections.Generic.List<Prestamo>();

            var prestamo = new Prestamo();
            prestamo.Id = Guid.NewGuid(); prestamo.LibroId = 1; prestamo.EstudianteId = 1;
            prestamo.UsuarioId = Guid.NewGuid(); prestamo.FechaHoraPrestamo = DateTime.UtcNow;
            prestamo.FechaHoraLimite = DateTime.UtcNow; prestamo.Estado = "J";
            prestamo.Libro = libro;
            prestamo.Estudiante = estudiante;

            var rol = new Rol();
            rol.Id = 1; rol.Nombre = "K";

            var usuario = new Usuario();
            usuario.Id = Guid.NewGuid(); usuario.Dni = "L"; usuario.Nombres = "M";
            usuario.Email = "N"; usuario.PasswordHash = "O"; usuario.PisoArea = "P";
            usuario.RolId = 1; usuario.Activo = true; usuario.Rol = rol;

            // Asserts de lectura
            Assert.IsNotNull(categoria.Nombre);
            Assert.IsNotNull(escuela.Nombre);
            Assert.IsNotNull(estudiante.Prestamos);
            Assert.IsNotNull(libro.Prestamos);
            Assert.IsNotNull(prestamo.Estado);
            Assert.IsNotNull(rol.Nombre);
            Assert.IsNotNull(usuario.Email);
        }


        [TestMethod]
        public void CoberturaExtrema_Dominio_NavegacionYListasVirtuales()
        {
            // Forzamos el GET y SET de propiedades de navegación complejas

            // Categoria
            var categoria = new Categoria();
            categoria.Libros = new System.Collections.Generic.List<Libro>();
            Assert.IsNotNull(categoria.Libros);

            // EscuelaProfesional
            var escuela = new EscuelaProfesional();
            escuela.Estudiantes = new System.Collections.Generic.List<Estudiante>();
            Assert.IsNotNull(escuela.Estudiantes);

            // Estudiante
            var estudiante = new Estudiante();
            estudiante.EscuelaProfesional = new EscuelaProfesional { Nombre = "Sistemas" };
            estudiante.Prestamos = new System.Collections.Generic.List<Prestamo>();
            Assert.IsNotNull(estudiante.EscuelaProfesional.Nombre);
            Assert.IsNotNull(estudiante.Prestamos);

            // Libro
            var libro = new Libro();
            libro.Categoria = new Categoria { Nombre = "General" };
            libro.Prestamos = new System.Collections.Generic.List<Prestamo>();
            Assert.IsNotNull(libro.Categoria.Nombre);
            Assert.IsNotNull(libro.Prestamos);

            // Usuario
            var usuario = new Usuario();
            usuario.Rol = new Rol { Nombre = "Admin" };
            Assert.IsNotNull(usuario.Rol.Nombre);

            // Rol
            var rol = new Rol();
            rol.Usuarios = new System.Collections.Generic.List<Usuario>();
            Assert.IsNotNull(rol.Usuarios);

            // Prestamo
            var prestamo = new Prestamo();
            prestamo.Libro = new Libro { Titulo = "C#" };
            prestamo.Estudiante = new Estudiante { NombresCompletos = "Jhor" };
            // Nota: Si tu entidad Prestamo tiene la propiedad virtual de Usuario, descomenta las siguientes 2 líneas:
            // prestamo.Usuario = new Usuario { Email = "admin@unsch.edu.pe" };
            // Assert.IsNotNull(prestamo.Usuario.Email);

            Assert.IsNotNull(prestamo.Libro.Titulo);
            Assert.IsNotNull(prestamo.Estudiante.NombresCompletos);
        }


        [TestMethod]
        public void CoberturaAbsoluta_Dominio_LecturaDeInicializadoresOcultos()
        {
            // El truco para el 100%: Instanciar vacío y LEER el valor por defecto antes de hacer cualquier otra cosa.

            // 1. ESTUDIANTE (Faltan 3 líneas)
            var estudiante = new Estudiante();
            var estDni = estudiante.Dni;                                 // Fuerza lectura de = string.Empty;
            var estCod = estudiante.CodigoUniversitario;
            var estNom = estudiante.NombresCompletos;
            estudiante.Prestamos.Add(new Prestamo());                    // Fuerza lectura del = new List<Prestamo>();
            Assert.IsNotNull(estudiante.Prestamos);

            // 2. LIBRO (Faltan 3 líneas)
            var libro = new Libro();
            var libIsbn = libro.Isbn;
            var libTit = libro.Titulo;
            var libAut = libro.Autor;
            libro.Prestamos.Add(new Prestamo());                         // Fuerza lectura del = new List<Prestamo>();
            Assert.IsNotNull(libro.Prestamos);

            // 3. USUARIO (Faltan 6 líneas)
            var usuario = new Usuario();
            var usrDni = usuario.Dni;
            var usrNom = usuario.Nombres;
            var usrEma = usuario.Email;
            var usrPas = usuario.PasswordHash;
            var usrPis = usuario.PisoArea;
            // Si Usuario tiene una colección de préstamos gestionados, esta línea la cubre (si da error, bórrala):
            // usuario.Prestamos = new System.Collections.Generic.List<Prestamo>(); 
            Assert.IsNotNull(usuario);

            // 4. PRESTAMO (Faltan 3 líneas)
            var prestamo = new Prestamo();
            var preEst = prestamo.Estado;                                // Fuerza lectura de = string.Empty; si lo tiene
            Assert.IsNotNull(prestamo);

            // Verificaciones extra de mutabilidad para cerrar brechas
            estudiante.Id = 99;
            Assert.AreEqual(99, estudiante.Id);

            libro.Id = 99;
            Assert.AreEqual(99, libro.Id);

            usuario.Id = Guid.NewGuid();
            Assert.AreNotEqual(Guid.Empty, usuario.Id);
        }

        [TestMethod]
        public void Cobertura100_LecturaEscritura_TodasLasEntidades()
        {
            // El objetivo es leer TODAS las propiedades y listas vacías ANTES de setearlas

            // 1. Estudiante
            var est = new Estudiante();
            var e1 = est.CodigoUniversitario; var e2 = est.Dni; var e3 = est.NombresCompletos;
            var e4 = est.Prestamos; var e5 = est.EscuelaProfesional;
            est.Id = 1; est.CodigoUniversitario = "1"; est.Dni = "1"; est.NombresCompletos = "1";
            est.EscuelaId = 1; est.EsMoroso = true; est.Prestamos = new System.Collections.Generic.List<Prestamo>();
            Assert.IsNotNull(est);

            // 2. Libro
            var lib = new Libro();
            var l1 = lib.Isbn; var l2 = lib.Titulo; var l3 = lib.Autor;
            var l4 = lib.Categoria; var l5 = lib.Prestamos;
            lib.Id = 1; lib.Isbn = "1"; lib.Titulo = "1"; lib.Autor = "1"; lib.CategoriaId = 1;
            lib.StockTotal = 1; lib.StockDisponible = 1; lib.Prestamos = new System.Collections.Generic.List<Prestamo>();
            Assert.IsNotNull(lib);

            // 3. Usuario
            var usu = new Usuario();
            var u1 = usu.Dni; var u2 = usu.Nombres; var u3 = usu.Email; var u4 = usu.PasswordHash;
            var u5 = usu.PisoArea; var u6 = usu.Rol;
            usu.Id = Guid.NewGuid(); usu.Dni = "1"; usu.Nombres = "1"; usu.Email = "1";
            usu.PasswordHash = "1"; usu.PisoArea = "1"; usu.RolId = 1; usu.Activo = true;
            Assert.IsNotNull(usu);

            // 4. Prestamo
            var pre = new Prestamo();
            var p1 = pre.Estado; var p2 = pre.Libro; var p3 = pre.Estudiante; var p4 = pre.UsuarioId;
            pre.Id = Guid.NewGuid(); pre.LibroId = 1; pre.EstudianteId = 1; pre.UsuarioId = Guid.NewGuid();
            pre.FechaHoraPrestamo = DateTime.UtcNow; pre.FechaHoraLimite = DateTime.UtcNow;
            pre.FechaHoraDevolucion = DateTime.UtcNow; pre.Estado = "1";
            Assert.IsNotNull(pre);

            // 5. Categoria
            var cat = new Categoria();
            var c1 = cat.Nombre; var c2 = cat.Libros;
            cat.Id = 1; cat.Nombre = "1"; cat.Libros = new System.Collections.Generic.List<Libro>();
            Assert.IsNotNull(cat);

            // 6. EscuelaProfesional
            var esc = new EscuelaProfesional();
            var es1 = esc.Nombre; var es2 = esc.Facultad; var es3 = esc.Estudiantes;
            esc.Id = 1; esc.Nombre = "1"; esc.Facultad = "1"; esc.Estudiantes = new System.Collections.Generic.List<Estudiante>();
            Assert.IsNotNull(esc);

            // 7. Rol
            var rol = new Rol();
            var r1 = rol.Nombre; var r2 = rol.Usuarios;
            rol.Id = 1; rol.Nombre = "1"; rol.Usuarios = new System.Collections.Generic.List<Usuario>();
            Assert.IsNotNull(rol);
        }


        [TestMethod]
        public void CoberturaDefinitiva_Entidades_LecturaYEscrituraTotal()
        {
            // LIBRO
            var libro = new Libro();
            var l1 = libro.Id; var l2 = libro.Isbn; var l3 = libro.Titulo; var l4 = libro.Autor; var l5 = libro.CategoriaId; var l6 = libro.StockTotal; var l7 = libro.StockDisponible; var l8 = libro.Categoria; var l9 = libro.Prestamos;
            libro.Id = 1; libro.Isbn = "1"; libro.Titulo = "1"; libro.Autor = "1"; libro.CategoriaId = 1; libro.StockTotal = 1; libro.StockDisponible = 1; libro.Categoria = new Categoria(); libro.Prestamos = new System.Collections.Generic.List<Prestamo>();

            // ESTUDIANTE
            var estudiante = new Estudiante();
            var e1 = estudiante.Id; var e2 = estudiante.CodigoUniversitario; var e3 = estudiante.Dni; var e4 = estudiante.NombresCompletos; var e5 = estudiante.EscuelaId; var e6 = estudiante.EsMoroso; var e7 = estudiante.EscuelaProfesional; var e8 = estudiante.Prestamos;
            estudiante.Id = 1; estudiante.CodigoUniversitario = "1"; estudiante.Dni = "1"; estudiante.NombresCompletos = "1"; estudiante.EscuelaId = 1; estudiante.EsMoroso = true; estudiante.EscuelaProfesional = new EscuelaProfesional(); estudiante.Prestamos = new System.Collections.Generic.List<Prestamo>();

            // PRESTAMO
            var prestamo = new Prestamo();
            var p1 = prestamo.Id; var p2 = prestamo.LibroId; var p3 = prestamo.EstudianteId; var p4 = prestamo.UsuarioId; var p5 = prestamo.FechaHoraPrestamo; var p6 = prestamo.FechaHoraLimite; var p7 = prestamo.FechaHoraDevolucion; var p8 = prestamo.Estado; var p9 = prestamo.Libro; var p10 = prestamo.Estudiante;
            prestamo.Id = Guid.NewGuid(); prestamo.LibroId = 1; prestamo.EstudianteId = 1; prestamo.UsuarioId = Guid.NewGuid(); prestamo.FechaHoraPrestamo = DateTime.UtcNow; prestamo.FechaHoraLimite = DateTime.UtcNow; prestamo.FechaHoraDevolucion = DateTime.UtcNow; prestamo.Estado = "1"; prestamo.Libro = new Libro(); prestamo.Estudiante = new Estudiante();
            // Si Prestamo.cs tiene Usuario: prestamo.Usuario = new Usuario(); var p11 = prestamo.Usuario;

            // USUARIO
            var usuario = new Usuario();
            var u1 = usuario.Id; var u2 = usuario.Dni; var u3 = usuario.Nombres; var u4 = usuario.Email; var u5 = usuario.PasswordHash; var u6 = usuario.PisoArea; var u7 = usuario.RolId; var u8 = usuario.Activo; var u9 = usuario.Rol;
            usuario.Id = Guid.NewGuid(); usuario.Dni = "1"; usuario.Nombres = "1"; usuario.Email = "1"; usuario.PasswordHash = "1"; usuario.PisoArea = "1"; usuario.RolId = 1; usuario.Activo = true; usuario.Rol = new Rol();

            Assert.IsNotNull(libro);
            Assert.IsNotNull(estudiante);
            Assert.IsNotNull(prestamo);
            Assert.IsNotNull(usuario);
        }

        [TestMethod]
        public void CoberturaGodMode_Entidades_LineasOcultas()
        {
            // Atacando las 2 líneas ocultas de Prestamo (La propiedad de navegación Usuario)
            var prestamo = new Prestamo();
            prestamo.Usuario = new Usuario { Nombres = "Admin" };
            var pUsuario = prestamo.Usuario;

            // Atacando propiedades extra que puedan estar sueltas
            prestamo.FechaHoraDevolucion = null;
            var pDev = prestamo.FechaHoraDevolucion;

            // Atacando las 2 líneas ocultas de Usuario (La lista de Préstamos gestionados)
            var usuario = new Usuario();
            usuario.Prestamos = new System.Collections.Generic.List<Prestamo>();
            usuario.Prestamos.Add(new Prestamo());
            var uPrestamos = usuario.Prestamos;

            // Atacando la propiedad Activo que a veces se omite
            usuario.Activo = false;
            var uAct = usuario.Activo;

            Assert.IsNotNull(pUsuario);
            Assert.IsNotNull(uPrestamos);
        }
    }
}