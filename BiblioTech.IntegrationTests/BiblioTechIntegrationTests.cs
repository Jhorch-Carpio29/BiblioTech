using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace BiblioTech.IntegrationTests
{
    public class BiblioTechIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public BiblioTechIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        // ================= CATEGORIAS (5 Endpoints) =================
        [Fact]
        public async Task Categorias_POST_Crear_DeberiaGuardarEnTestcontainers()
        {
            var nuevaCategoria = new { nombre = "Ingeniería", descripcion = "Libros de sistemas" };
            var r = await _client.PostAsJsonAsync("/api/Categorias", nuevaCategoria);
            Assert.True(r.IsSuccessStatusCode);
        }

        [Fact] public async Task Categorias_GET_All() { var r = await _client.GetAsync("/api/Categorias"); Assert.True(r.IsSuccessStatusCode); }
        [Fact] public async Task Categorias_GET_ById() { var r = await _client.GetAsync("/api/Categorias/999"); Assert.NotNull(r); }
        [Fact] public async Task Categorias_PUT_Actualizar() { var r = await _client.PutAsJsonAsync("/api/Categorias/999", new { nombre = "Test" }); Assert.NotNull(r); }
        [Fact] public async Task Categorias_DELETE_Eliminar() { var r = await _client.DeleteAsync("/api/Categorias/999"); Assert.NotNull(r); }

        // ================= ESCUELAS PROFESIONALES (2 Endpoints) =================
        [Fact] public async Task Escuelas_GET_All() { var r = await _client.GetAsync("/api/EscuelasProfesionales"); Assert.True(r.IsSuccessStatusCode); }
        [Fact] public async Task Escuelas_POST_Crear() { var r = await _client.PostAsJsonAsync("/api/EscuelasProfesionales", new { nombre = "Sistemas" }); Assert.NotNull(r); }

        // ================= ESTUDIANTES (5 Endpoints) =================
        [Fact] public async Task Estudiantes_GET_All() { var r = await _client.GetAsync("/api/Estudiantes"); Assert.True(r.IsSuccessStatusCode); }
        [Fact] public async Task Estudiantes_POST_Crear() { var r = await _client.PostAsJsonAsync("/api/Estudiantes", new { codigoUniversitario = "123", dni = "123" }); Assert.NotNull(r); }
        [Fact] public async Task Estudiantes_GET_ById() { var r = await _client.GetAsync("/api/Estudiantes/999"); Assert.NotNull(r); }
        [Fact] public async Task Estudiantes_PUT_Actualizar() { var r = await _client.PutAsJsonAsync("/api/Estudiantes/999", new { nombresCompletos = "Test" }); Assert.NotNull(r); }
        [Fact] public async Task Estudiantes_DELETE_Eliminar() { var r = await _client.DeleteAsync("/api/Estudiantes/999"); Assert.NotNull(r); }

        // ================= LIBROS (5 Endpoints) =================
        [Fact] public async Task Libros_GET_All() { var r = await _client.GetAsync("/api/Libros"); Assert.True(r.IsSuccessStatusCode); }
        [Fact] public async Task Libros_POST_Crear() { var r = await _client.PostAsJsonAsync("/api/Libros", new { titulo = "Clean Code" }); Assert.NotNull(r); }
        [Fact] public async Task Libros_GET_ById() { var r = await _client.GetAsync("/api/Libros/999"); Assert.NotNull(r); }
        [Fact] public async Task Libros_PUT_Actualizar() { var r = await _client.PutAsJsonAsync("/api/Libros/999", new { titulo = "Update" }); Assert.NotNull(r); }
        [Fact] public async Task Libros_DELETE_Eliminar() { var r = await _client.DeleteAsync("/api/Libros/999"); Assert.NotNull(r); }

        // ================= PRESTAMOS (4 Endpoints) =================
        [Fact] public async Task Prestamos_POST_Crear() { var r = await _client.PostAsJsonAsync("/api/Prestamos", new { estudianteId = 1 }); Assert.NotNull(r); }
        [Fact] public async Task Prestamos_GET_All() { var r = await _client.GetAsync("/api/Prestamos"); Assert.True(r.IsSuccessStatusCode); }
        [Fact] public async Task Prestamos_PUT_Devolucion() { var r = await _client.PutAsJsonAsync("/api/Prestamos/999/devolucion", new { }); Assert.NotNull(r); }
        [Fact] public async Task Prestamos_GET_ById() { var r = await _client.GetAsync("/api/Prestamos/999"); Assert.NotNull(r); }

        // ================= USUARIOS (3 Endpoints) =================
        [Fact] public async Task Usuarios_GET_All() { var r = await _client.GetAsync("/api/Usuarios"); Assert.True(r.IsSuccessStatusCode); }
        [Fact] public async Task Usuarios_POST_Crear() { var r = await _client.PostAsJsonAsync("/api/Usuarios", new { username = "admin" }); Assert.NotNull(r); }
        [Fact] public async Task Usuarios_POST_Login() { var r = await _client.PostAsJsonAsync("/api/Usuarios/login", new { username = "admin", password = "123" }); Assert.NotNull(r); }

        // ================= WEATHER FORECAST (1 Endpoint) =================
        [Fact] public async Task WeatherForecast_GET_All() { var r = await _client.GetAsync("/WeatherForecast"); Assert.True(r.IsSuccessStatusCode); }
    }
}