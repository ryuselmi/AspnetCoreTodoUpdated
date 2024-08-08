using AspNetCoreTodo.Data;
using AspNetCoreTodo.Models;
using AspNetCoreTodo.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class TodoItemService :
    ITodoItemService
{
    private readonly ApplicationDbContext _dbContext;

    public TodoItemService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> AddItemAsync(TodoItem item, IdentityUser currentUser)
    {
        item.Id = Guid.NewGuid();
        item.IsDone = false;
        item.DueAt = DateTimeOffset.Now.AddDays(3);
        item.UserId = currentUser.Id;
        
        _dbContext.Items.Add(item);

        return await _dbContext.SaveChangesAsync() == 1;
    }

    public async Task<TodoItem[]> GetIncompleteItemsAsync(IdentityUser currentUser)
    {   
        return await _dbContext.Items
            .Where(item => item.IsDone == false && item.UserId == currentUser.Id)
            .ToArrayAsync();
    }

    public async Task<bool> MarkDoneAsync(Guid id, IdentityUser currentUser)
    {
        var item = await _dbContext.Items
            .SingleOrDefaultAsync(item => item.Id == id && item.UserId == currentUser.Id);
        
        if (item == null)
            return false;

        item.IsDone = true;
        return await _dbContext.SaveChangesAsync() == 1;
    }
}