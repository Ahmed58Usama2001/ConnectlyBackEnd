
namespace Connectly.Infrastructure.Data
{
    public class ApplicationContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                            v => v.Kind == DateTimeKind.Local ? v.ToUniversalTime() : v, 
                            v => v.ToLocalTime()                                        
                        ));
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(new ValueConverter<DateTime?, DateTime?>(
                            v => v.HasValue
                                ? (v.Value.Kind == DateTimeKind.Local ? v.Value.ToUniversalTime() : v.Value)
                                : v,
                            v => v.HasValue ? v.Value.ToLocalTime() : v
                        ));
                    }
                }
            }
        }


        public DbSet<Photo> Photos { get; set; }
        public DbSet<MemberLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}
