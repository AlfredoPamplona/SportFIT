using System.Data.Entity;

namespace SportFIT.Models
{
    // Clase que representa el contexto de la base de datos para la aplicacion
    public class SportFITDbContext : DbContext
    {
        // Constructor que especifica el nombre de la cadena de conexión
        public SportFITDbContext() : base("DBContextSportFIT") { }

        // Propiedades DbSet que representan las tablas en la base de datos
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Pueblo> Pueblos { get; set; }
        public DbSet<Instalacion> Instalaciones { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Actividad> Actividades { get; set; }

        // Metodo para configurar las relaciones entre las entidades
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuracion de las relaciones M:N entre tablas

            // Relacion entre Permiso y Rol
            modelBuilder.Entity<Rol>()
                .HasMany(r => r.Permisos)
                .WithMany(p => p.Roles)
                .Map(m =>
                {
                    m.ToTable("Permiso_Rol");   // Nombre de la tabla de relacion
                    m.MapLeftKey("IdRol");       // Nombre de la clave primaria de Rol en la tabla de relacion
                    m.MapRightKey("IdPermiso");  // Nombre de la clave primaria de Permiso en la tabla de relacion
                });

            // Relacion entre Usuario y Pueblo
            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Pueblos)
                .WithMany(p => p.Usuarios)
                .Map(m =>
                {
                    m.ToTable("Usuario_Pueblo"); // Nombre de la tabla de relacion
                    m.MapLeftKey("IdUsuario");   // Nombre de la clave primaria de Usuario en la tabla de relacion
                    m.MapRightKey("IdPueblo");   // Nombre de la clave primaria de Pueblo en la tabla de relacion
                });
        }
    }
}
