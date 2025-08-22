using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;

namespace MyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CoursesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/courses
    [HttpGet]
    public async Task<IActionResult> GetCourses()
    {
        var courses = await _context.Courses.ToListAsync();
        return Ok(courses);
    }

    // POST: api/courses
    [HttpPost]
// [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateCourse([FromBody] Course course)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (string.IsNullOrWhiteSpace(course.Title))
            return BadRequest(new { message = "Course name is required" });

        bool exists = await _context.Courses.AnyAsync(c =>
            c.Title.ToLower() == course.Title.ToLower() ||
            c.Code.ToLower() == course.Code.ToLower());

        if (exists)
            return BadRequest(new { message = $"Course with name '{course.Title}' already exists" });

        course.CreatedAt = DateTime.UtcNow;
        course.UpdatedAt = DateTime.UtcNow;

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCourseById), new { id = course.CourseID }, course);
    }

    // GET: api/courses/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourseById(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null) return NotFound();

        return Ok(course);
    }

// PUT: api/courses/{id}
// [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCourse(int id, [FromBody] Course updatedCourse)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null) return NotFound(new { message = "Course not found" });

        if (string.IsNullOrWhiteSpace(updatedCourse.Title))
            return BadRequest(new { message = "Course name is required" });

        if (string.IsNullOrWhiteSpace(updatedCourse.Code))
            return BadRequest(new { message = "Course code is required" });

        // Duplicate check (case-insensitive), excluding current course
        bool duplicate = await _context.Courses.AnyAsync(c =>
            c.CourseID != id &&
            (c.Title.ToLower() == updatedCourse.Title.ToLower() ||
             c.Code.ToLower() == updatedCourse.Code.ToLower()));

        if (duplicate)
            return BadRequest(new
            {
                message =
                    $"Another course with name '{updatedCourse.Title}' or code '{updatedCourse.Code}' already exists"
            });

        // Apply changes
        course.Code = updatedCourse.Code;
        course.Title = updatedCourse.Title;
        course.Description = updatedCourse.Description;
        course.IsNew = updatedCourse.IsNew;
        course.IsActive = updatedCourse.IsActive;
        course.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // ✅ Return content (200 OK) with the updated entity
        return Ok(new
        {
            message = "Course updated successfully",
            course
        });
    }

// DELETE: api/courses/{id}
// [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null) return NotFound(new { message = "Course not found" });

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();

        // ✅ Return content (200 OK) with some context about what was deleted
        return Ok(new
        {
            message = "Course deleted successfully",
            deleted = new
            {
                id = course.CourseID,
                code = course.Code,
                title = course.Title
            }
        });
    }
}