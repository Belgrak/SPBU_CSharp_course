using GradeApp.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GradeApp.Pages;

public class ListGrades : PageModel
{
    private readonly GradeAppDbContext _context;

    public ListGrades(GradeAppDbContext context)
        => _context = context;

    public IList<Grade> Grades { get; private set; } = new List<Grade>();

    public void OnGet()
    {
        Grades = _context.Grades.OrderBy(p => p.GradeId).ToList();
    }
}