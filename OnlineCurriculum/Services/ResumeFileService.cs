using Microsoft.EntityFrameworkCore;
using OnlineCurriculum.Data;
using OnlineCurriculum.Models;

namespace OnlineCurriculum.Services;

public class ResumeFileService
{
    private readonly AppDbContext _context;

    public ResumeFileService(AppDbContext context)
    {
        _context = context;
    }

    public async Task RemoveFile(string fileName)
    {
        var file = await _context.ResumeFiles.FirstOrDefaultAsync(rf=>rf.FileName == fileName);
        if (file == null)
            throw new Exception("Resume file not found");
        
        _context.ResumeFiles.Remove(file);
        await _context.SaveChangesAsync();
    }
}