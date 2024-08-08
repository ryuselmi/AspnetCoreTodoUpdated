using AspNetCoreTodo.Data;
using AspNetCoreTodo.Services;
using AspNetCoreTodo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreTodo.UnitTests;

public class TodoItemServiceShould
{
    /*
    In general there are two ways to setup a test.
    Use the AAA (Arrange, Act, Assert) or GWT (Given, When, Then) pattern  

    */
    [Fact]
    public async Task AddNewItemAsIncompleteWithDueDate()
    {
        // arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "Test_AddNewItem").Options;

        using (var dbContext = new ApplicationDbContext(options))
        {
            var service = new TodoItemService(dbContext);
            var fakeUser = new IdentityUser
            {
                Id = "fake-000",
                UserName = "fake@example.com"
            };

            // act
            await service.AddItemAsync(new TodoItem
            {
                Title = "Testing?"
            }, fakeUser);
        }

        // assert
        using (var dbContext = new ApplicationDbContext(options))
        {
            var itemsInDatabase = await dbContext.Items.CountAsync();
            Assert.Equal(1, itemsInDatabase);

            var item = await dbContext.Items.FirstAsync();
            Assert.Equal("Testing?", item.Title);
            Assert.Equal(false, item.IsDone);
            
            // item should be due in ~3 days
            var difference = DateTimeOffset.Now.AddDays(3) - item.DueAt;
            Assert.True(difference < TimeSpan.FromSeconds(1));
        }
    }
}