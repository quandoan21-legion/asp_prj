using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;

namespace MyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExamsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ExamsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/exams
    [HttpGet]
    public async Task<IActionResult> GetExams()
    {
        var exams = await _context.Exams
            .Include(e => e.Course) // eager load course info
            .ToListAsync();

        return Ok(exams);
    }

    // GET: api/exams/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetExamById(int id)
    {
        var exam = await _context.Exams
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.ExamId == id);

        if (exam == null) return NotFound(new { message = "Exam not found" });
        return Ok(exam);
    }

    // POST: api/exams
    [HttpPost]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateExam([FromBody] Exam exam)
    {
        // Ignore navigation validation so only CourseID is required in JSON
        ModelState.Remove(nameof(Exam.Course));

        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (string.IsNullOrWhiteSpace(exam.Name))
            return BadRequest(new { message = "Exam name is required" });

        if (exam.ApplicationOpen >= exam.ApplicationClose)
            return BadRequest(new { message = "ApplicationOpen must be before ApplicationClose" });

        // if (exam.CourseId <= 0)
        //     return BadRequest(new { message = "CourseID is required" });

        var courseExists = await _context.Courses.AnyAsync(c => c.CourseId == exam.CourseId);
        if (!courseExists)
            return BadRequest(new { message = $"CourseID {exam.CourseId} does not exist" });

        // Prevent duplicates: same Name (ci) + same ExamDate (date only) + same Course
        var exists = await _context.Exams.AnyAsync(e =>
            e.CourseId == exam.CourseId &&
            e.ExamDate.Date == exam.ExamDate.Date &&
            e.Name.ToLower() == exam.Name.ToLower());

        if (exists)
            return BadRequest(new
                { message = $"Exam '{exam.Name}' on {exam.ExamDate:d} already exists for this course" });


        // Ensure all DateTime values are UTC before saving
        exam.ExamDate = DateTime.SpecifyKind(exam.ExamDate, DateTimeKind.Utc);
        exam.ApplicationOpen = DateTime.SpecifyKind(exam.ApplicationOpen, DateTimeKind.Utc);
        exam.ApplicationClose = DateTime.SpecifyKind(exam.ApplicationClose, DateTimeKind.Utc);
        if (exam.ResultPublishDate.HasValue)
            exam.ResultPublishDate = DateTime.SpecifyKind(exam.ResultPublishDate.Value, DateTimeKind.Utc);

        _context.Exams.Add(exam);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetExamById), new { id = exam.ExamId }, exam);
    }

    [HttpPut("{id}")]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateExam(int id, [FromBody] Exam updatedExam)
    {
        // Ignore navigation validation
        ModelState.Remove(nameof(Exam.Course));

        if (!ModelState.IsValid) return BadRequest(ModelState);

        var exam = await _context.Exams.FindAsync(id);
        if (exam == null) return NotFound(new { message = "Exam not found" });

        if (string.IsNullOrWhiteSpace(updatedExam.Name))
            return BadRequest(new { message = "Exam name is required" });

        if (updatedExam.ApplicationOpen >= updatedExam.ApplicationClose)
            return BadRequest(new { message = "ApplicationOpen must be before ApplicationClose" });

        if (updatedExam.CourseId <= 0)
            return BadRequest(new { message = "CourseID is required" });

        var courseExists = await _context.Courses.AnyAsync(c => c.CourseId == updatedExam.CourseId);
        if (!courseExists)
            return BadRequest(new { message = $"CourseID {updatedExam.CourseId} does not exist" });

        // Prevent duplicates excluding current exam
        var duplicate = await _context.Exams.AnyAsync(e =>
            e.ExamId != id &&
            e.CourseId == updatedExam.CourseId &&
            e.ExamDate.Date == updatedExam.ExamDate.Date &&
            e.Name.ToLower() == updatedExam.Name.ToLower());

        if (duplicate)
            return BadRequest(new
            {
                message =
                    $"Another exam '{updatedExam.Name}' on {updatedExam.ExamDate:d} already exists for this course"
            });

        // Ensure all DateTime values are UTC before saving
        updatedExam.ExamDate = DateTime.SpecifyKind(updatedExam.ExamDate, DateTimeKind.Utc);
        updatedExam.ApplicationOpen = DateTime.SpecifyKind(updatedExam.ApplicationOpen, DateTimeKind.Utc);
        updatedExam.ApplicationClose = DateTime.SpecifyKind(updatedExam.ApplicationClose, DateTimeKind.Utc);
        if (updatedExam.ResultPublishDate.HasValue)
            updatedExam.ResultPublishDate = DateTime.SpecifyKind(updatedExam.ResultPublishDate.Value, DateTimeKind.Utc);

        // Apply updates
        exam.Name = updatedExam.Name;
        exam.ExamDate = updatedExam.ExamDate;
        exam.ApplicationOpen = updatedExam.ApplicationOpen;
        exam.ApplicationClose = updatedExam.ApplicationClose;
        exam.ResultPublishDate = updatedExam.ResultPublishDate;
        exam.FeeAmount = updatedExam.FeeAmount;
        exam.Description = updatedExam.Description;
        exam.IsActive = updatedExam.IsActive;
        exam.CourseId = updatedExam.CourseId;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Exam updated successfully", exam });
    }

    // DELETE: api/exams/{id}
    [HttpDelete("{id}")]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteExam(int id)
    {
        var exam = await _context.Exams.FindAsync(id);
        if (exam == null) return NotFound(new { message = "Exam not found" });

        _context.Exams.Remove(exam);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Exam deleted successfully",
            deleted = new { exam.ExamId, exam.Name, exam.ExamDate }
        });
    }
}