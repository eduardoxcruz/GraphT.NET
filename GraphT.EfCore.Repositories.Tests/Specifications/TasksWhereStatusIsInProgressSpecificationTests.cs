﻿using GraphT.Model.Aggregates;
using GraphT.Model.Services.Specifications;
using GraphT.Model.ValueObjects;

using Microsoft.EntityFrameworkCore;

using SeedWork;

namespace GraphT.EfCore.Repositories.Tests.Specifications;

public class TasksWhereStatusIsInProgressSpecificationTests : IClassFixture<TestDatabaseFixture>
{
	private readonly TestDatabaseFixture _fixture;

	public TasksWhereStatusIsInProgressSpecificationTests(TestDatabaseFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task Find_ReturnsTasksWithInProgressStatus()
	{
		// Arrange
		EfDbContext context = _fixture.CreateContext();
		Repository<TodoTask> repository = new(context);
		PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
		TasksWhereStatusIsInProgressSpecification spec = new(pagingParams);
		List<TodoTask> tasks =
		[
			new("Task 1", Status.Completed), new("Task 2", Status.Dropped), new("Task 3", Status.InProgress)
			, new("Task 4", Status.ReadyToStart), new("Task 5", Status.Backlog)
		];

		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
		await context.TodoTasks.AddRangeAsync(tasks);
		await context.SaveChangesAsync();
		PagedList<TodoTask> results = await repository.FindAsync(spec);

		// Assert
		Assert.NotNull(results);
		Assert.Equal(1, results.TotalCount);
		Assert.Single(results);
		Assert.All(results, task => Assert.True(task.Status is Status.InProgress));
	}

	[Fact]
    public async Task Find_OrdersByPriorityDescending()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        Repository<TodoTask> repository = new(context);
        PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
        TasksWhereStatusIsInProgressSpecification spec = new(pagingParams);
        List<TodoTask> tasks = [
            new("Task Low", Status.InProgress) { Priority = Priority.MentalClutter },
            new("Task Medium", Status.InProgress) { Priority = Priority.ThinkAboutIt },
            new("Task Medium", Status.InProgress) { Priority = Priority.DoItNow },
            new("Task High", Status.InProgress) { Priority = Priority.DropEverythingElse }
        ];

        await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
        await context.TodoTasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();
        PagedList<TodoTask> results = await repository.FindAsync(spec);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(4, results.TotalCount);
        Assert.Equal(4, results.Count);
        Assert.Equal(Priority.DropEverythingElse, results.First().Priority);
        Assert.Equal(Priority.MentalClutter, results.Last().Priority);
    }

    [Fact]
    public async Task Find_ThenOrdersByLimitDateTime()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        Repository<TodoTask> repository = new(context);
        PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
        TasksWhereStatusIsInProgressSpecification spec = new(pagingParams);
        DateTimeOffset now = DateTimeOffset.UtcNow;
        TodoTask firstTask = new("Task Soon", Status.InProgress) { Priority = Priority.DoItNow };
        TodoTask secondTask = new("Task Soon", Status.InProgress) { Priority = Priority.DoItNow };
        TodoTask thirdTask = new("Task Later", Status.InProgress) { Priority = Priority.DoItNow };
        TodoTask fourthTask = new("Task Later", Status.InProgress) { Priority = Priority.DoItNow };
        List<TodoTask> tasks = [
	        thirdTask,
	        firstTask,
            fourthTask,
	        secondTask
        ];
        
        firstTask.SetLimitDate(now.AddDays(2));
        secondTask.SetLimitDate(now.AddDays(3));
        thirdTask.SetLimitDate(now.AddDays(4));
        fourthTask.SetLimitDate(now.AddDays(5));

        // Act
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
        await context.TodoTasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();
        PagedList<TodoTask> results = await repository.FindAsync(spec);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(4, results.TotalCount);
        Assert.Equal(firstTask.Id, results[0].Id);
        Assert.Equal(secondTask.Id, results[1].Id);
        Assert.Equal(thirdTask.Id, results[2].Id);
        Assert.Equal(fourthTask.Id, results[3].Id);
    }

	[Fact]
	public async Task Find_AppliesPagination()
	{
		// Arrange
		EfDbContext context = _fixture.CreateContext();
		Repository<TodoTask> repository = new(context);
		PagingParams pagingParams = new() { PageNumber = 2, PageSize = 2 };
		TasksWhereStatusIsInProgressSpecification spec = new(pagingParams);
		List<TodoTask> tasks =
		[
			new("Task 1", Status.InProgress), new("Task 2", Status.InProgress), new("Task 3", Status.InProgress)
			, new("Task 4", Status.InProgress), new("Task 5", Status.InProgress)
		];

		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
		await context.TodoTasks.AddRangeAsync(tasks);
		await context.SaveChangesAsync();
		PagedList<TodoTask> results = await repository.FindAsync(spec);

		// Assert
		Assert.NotNull(results);
		Assert.Equal(5, results.TotalCount);
		Assert.Equal(2, results.Count);
		Assert.Equal(2, results.CurrentPage);
		Assert.Equal(3, results.TotalPages);
		Assert.True(results.HasNext);
		Assert.True(results.HasPrevious);
	}
}
