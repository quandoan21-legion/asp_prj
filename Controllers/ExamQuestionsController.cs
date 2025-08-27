// Controllers/ExamQuestionsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;

namespace MyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExamQuestionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ExamQuestionsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("exam/{examId}")]
    public async Task<IActionResult> GetQuestionsByExam(int examId)
    {
        var questions = await _context.ExamQuestions
            .Where(q => q.ExamId == examId && q.IsActive)
            .ToListAsync();
            
        return Ok(questions);
    }

    [HttpPost]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateQuestion([FromBody] ExamQuestion question)
    {
        ModelState.Remove(nameof(ExamQuestion.Exam));
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // Validate exam exists
        var examExists = await _context.Exams.AnyAsync(e => e.ExamId == question.ExamId);
        if (!examExists)
            return BadRequest(new { message = $"Exam with ID {question.ExamId} does not exist" });

        // Validate question text length
        if (string.IsNullOrWhiteSpace(question.QuestionText) || question.QuestionText.Length < 10)
            return BadRequest(new { message = "Question text must be at least 10 characters long" });

        // Validate correct answer is provided
        if (string.IsNullOrWhiteSpace(question.CorrectAnswer))
            return BadRequest(new { message = "Correct answer is required" });

        // Validate points
        if (question.Points < 1 || question.Points > 10)
            return BadRequest(new { message = "Points must be between 1 and 10" });

        _context.ExamQuestions.Add(question);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetQuestionsByExam), new { examId = question.ExamId }, question);
    }

    [HttpPut("{id}")]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateQuestion(int id, [FromBody] ExamQuestion updatedQuestion)
    {
        ModelState.Remove(nameof(ExamQuestion.Exam));
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var question = await _context.ExamQuestions.FindAsync(id);
        if (question == null) return NotFound(new { message = "Question not found" });

        // Validate exam exists
        var examExists = await _context.Exams.AnyAsync(e => e.ExamId == updatedQuestion.ExamId);
        if (!examExists)
            return BadRequest(new { message = $"Exam with ID {updatedQuestion.ExamId} does not exist" });

        // Apply updates with validation
        if (!string.IsNullOrWhiteSpace(updatedQuestion.QuestionText) && updatedQuestion.QuestionText.Length >= 10)
            question.QuestionText = updatedQuestion.QuestionText;
        else
            return BadRequest(new { message = "Question text must be at least 10 characters long" });

        if (!string.IsNullOrWhiteSpace(updatedQuestion.CorrectAnswer))
            question.CorrectAnswer = updatedQuestion.CorrectAnswer;
        else
            return BadRequest(new { message = "Correct answer is required" });

        if (updatedQuestion.Points >= 1 && updatedQuestion.Points <= 10)
            question.Points = updatedQuestion.Points;
        else
            return BadRequest(new { message = "Points must be between 1 and 10" });

        question.IsActive = updatedQuestion.IsActive;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Question updated successfully", question });
    }

    [HttpDelete("{id}")]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteQuestion(int id)
    {
        var question = await _context.ExamQuestions.FindAsync(id);
        if (question == null) return NotFound(new { message = "Question not found" });

        _context.ExamQuestions.Remove(question);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Question deleted successfully", questionId = id });
    }
}