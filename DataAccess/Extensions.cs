using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public static class Extensions
{
    public static IServiceCollection AddDataAccess(this IServiceCollection servicesCollection)
    {
        servicesCollection.AddDbContext<AppContext>(x =>
        {
            x.UseNpgsql("Host=localhost;Username=postgres;Password=1234;Database=NoteDB");
        });
        return servicesCollection;
    }
}