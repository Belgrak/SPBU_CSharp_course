using System.ComponentModel.DataAnnotations;

namespace GradeApp.Data;

public class Grade
{
    public int GradeId { get; set; }

    [Required(ErrorMessage = "Please enter student name")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Please enter subject")]
    public string Subject { get; set; } = "";

    [Required(ErrorMessage = "Please enter grade value")]
    [Range(0, 10, ErrorMessage = "Please enter a valid grade value")]
    public string Value { get; set; } = "";
}