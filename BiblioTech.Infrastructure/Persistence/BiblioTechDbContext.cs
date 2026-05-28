using BiblioTech.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BiblioTech.Infrastructure.Persistence;

public class BiblioTechDbContext : DbContext
{
    public BiblioTechDbContext(DbContextOptions<BiblioTechDbContext> options)
        : base(options)
    {
    }

    public DbSet<Rol> Roles { get; set; } = null!;
    public DbSet<Usuario> Usuarios { get; set; } = null!;
    public DbSet<Categoria> Categorias { get; set; } = null!;
    public DbSet<Libro> Libros { get; set; } = null!;
    public DbSet<EscuelaProfesional> EscuelasProfesionales { get; set; } = null!;
    public DbSet<Estudiante> Estudiantes { get; set; } = null!;
    public DbSet<Prestamo> Prestamos { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Rol
        modelBuilder.Entity<Rol>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(50).IsRequired();
            
            entity.HasIndex(e => e.Nombre).IsUnique();
        });

        // 2. Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.Dni).HasColumnName("dni").HasMaxLength(8).IsRequired();
            entity.Property(e => e.Nombres).HasColumnName("nombres").HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(100).IsRequired();
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
            entity.Property(e => e.PisoArea).HasColumnName("piso_area").HasMaxLength(50).IsRequired();
            entity.Property(e => e.RolId).HasColumnName("rol_id").IsRequired();
            entity.Property(e => e.Activo).HasColumnName("activo").HasDefaultValue(true);

            entity.HasIndex(e => e.Dni).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();

            entity.HasOne(e => e.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(e => e.RolId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 3. Categoria
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("categorias");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
            entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();

            entity.HasIndex(e => e.Nombre).IsUnique();
        });

        // 4. Libro
        modelBuilder.Entity<Libro>(entity =>
        {
            entity.ToTable("libros");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
            entity.Property(e => e.Isbn).HasColumnName("isbn").HasMaxLength(20).IsRequired();
            entity.Property(e => e.Titulo).HasColumnName("titulo").HasMaxLength(200).IsRequired();
            entity.Property(e => e.Autor).HasColumnName("autor").HasMaxLength(150).IsRequired();
            entity.Property(e => e.CategoriaId).HasColumnName("categoria_id").IsRequired();
            entity.Property(e => e.StockTotal).HasColumnName("stock_total").IsRequired();
            entity.Property(e => e.StockDisponible).HasColumnName("stock_disponible").IsRequired();

            entity.HasIndex(e => e.Isbn).IsUnique();

            entity.ToTable(t => t.HasCheckConstraint("chk_stock_positivo", "stock_disponible >= 0"));

            entity.HasOne(e => e.Categoria)
                .WithMany(c => c.Libros)
                .HasForeignKey(e => e.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 5. EscuelaProfesional
        modelBuilder.Entity<EscuelaProfesional>(entity =>
        {
            entity.ToTable("escuelas_profesionales");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
            entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(150).IsRequired();
            entity.Property(e => e.Facultad).HasColumnName("facultad").HasMaxLength(150).IsRequired();

            entity.HasIndex(e => e.Nombre).IsUnique();
        });

        // 6. Estudiante
        modelBuilder.Entity<Estudiante>(entity =>
        {
            entity.ToTable("estudiantes");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").UseIdentityColumn();
            entity.Property(e => e.CodigoUniversitario).HasColumnName("codigo_universitario").HasMaxLength(10).IsRequired();
            entity.Property(e => e.Dni).HasColumnName("dni").HasMaxLength(8).IsRequired();
            entity.Property(e => e.NombresCompletos).HasColumnName("nombres_completos").HasMaxLength(150).IsRequired();
            entity.Property(e => e.EscuelaId).HasColumnName("escuela_id").IsRequired();
            entity.Property(e => e.EsMoroso).HasColumnName("es_moroso").HasDefaultValue(false);

            entity.HasIndex(e => e.CodigoUniversitario).IsUnique();
            entity.HasIndex(e => e.Dni).IsUnique();

            entity.HasOne(e => e.EscuelaProfesional)
                .WithMany(ep => ep.Estudiantes)
                .HasForeignKey(e => e.EscuelaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 7. Prestamo
        modelBuilder.Entity<Prestamo>(entity =>
        {
            entity.ToTable("prestamos");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.LibroId).HasColumnName("libro_id").IsRequired();
            entity.Property(e => e.EstudianteId).HasColumnName("estudiante_id").IsRequired();
            entity.Property(e => e.UsuarioId).HasColumnName("usuario_id").IsRequired();
            entity.Property(e => e.FechaHoraPrestamo).HasColumnName("fecha_hora_prestamo").HasDefaultValueSql("NOW()");
            entity.Property(e => e.FechaHoraLimite).HasColumnName("fecha_hora_limite").IsRequired();
            entity.Property(e => e.FechaHoraDevolucion).HasColumnName("fecha_hora_devolucion");
            entity.Property(e => e.Estado).HasColumnName("estado").HasMaxLength(20).IsRequired();

            entity.ToTable(t => t.HasCheckConstraint("chk_fechas_logicas", "fecha_hora_limite > fecha_hora_prestamo"));

            entity.HasOne(e => e.Libro)
                .WithMany(l => l.Prestamos)
                .HasForeignKey(e => e.LibroId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Estudiante)
                .WithMany(est => est.Prestamos)
                .HasForeignKey(e => e.EstudianteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Usuario)
                .WithMany(u => u.Prestamos)
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
    // AQUI ESTÁ LA CONFIGURACIÓN AÑADIDA PARA SOLUCIONAR EL ERROR 500
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>()
            .HaveConversion(typeof(UtcDateTimeConverter));
    }
}

// AQUI ESTÁ LA CLASE DEL CONVERSOR AÑADIDA AL FINAL
public class UtcDateTimeConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>
{
    public UtcDateTimeConverter() : base(
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
    { }

}
