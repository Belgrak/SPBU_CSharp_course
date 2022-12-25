namespace GradeApp.Pages;

using GradeApp.Data;

[BindProperties]
public class ResultsInput : PageModel
{
    private readonly GradeAppDbContext _context;

    public ResultsInput(GradeAppDbContext context)
        => _context = context;

    public Grade Grade { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        
        _context.Grades.Add(Grade);
        await _context.SaveChangesAsync();
        return RedirectToPage("./index");
    }
}