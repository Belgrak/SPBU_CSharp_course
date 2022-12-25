using Microsoft.EntityFrameworkCore;

namespace GradeApp.Data;

public class GradeAppDbContext : DbContext
{
    public GradeAppDbContext(
        DbContextOptions<GradeAppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Grade> Grades => Set<Grade>();
}