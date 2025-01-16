﻿using GraphT.Model.Aggregates;
using GraphT.Model.ValueObjects;

using Microsoft.EntityFrameworkCore;

using SeedWork;

namespace GraphT.EfCore.Repositories.Tests;

public class UnitOfWorkTests : IClassFixture<TestDatabaseFixture>
{
	private TestDatabaseFixture Fixture { get; }

	public UnitOfWorkTests(TestDatabaseFixture fixture) => Fixture = fixture;

	[Fact]
	public async Task SaveChangesAsync_PersistsChangesToDatabase()
	{
		EfDbContext context = Fixture.CreateContext();
		UnitOfWork unitOfWork = new(context);
		TodoTask task = new("Test Task");
		
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
		await unitOfWork.Repository<TodoTask>().AddAsync(task);
		int saveResult = await unitOfWork.SaveChangesAsync();
		TodoTask? savedTask = await context.TodoTasks.FindAsync(task.Id);

		Assert.Equal(1, saveResult);
		Assert.NotNull(savedTask);
		Assert.Equal(task.Id, savedTask.Id);
	}

	[Fact]
	public void Repository_ReturnsSameInstanceForSameType()
	{
		EfDbContext context = Fixture.CreateContext();
		UnitOfWork unitOfWork = new(context);
		IRepository<TodoTask> repo1 = unitOfWork.Repository<TodoTask>();
		IRepository<TodoTask> repo2 = unitOfWork.Repository<TodoTask>();

		Assert.Same(repo1, repo2);
	}

	[Fact]
	public void Repository_ReturnsDifferentInstancesForDifferentTypes()
	{
		EfDbContext context = Fixture.CreateContext();
		UnitOfWork unitOfWork = new(context);
		IRepository<TodoTask> repo1 = unitOfWork.Repository<TodoTask>();
		IRepository<TaskLog> repo2 = unitOfWork.Repository<TaskLog>();

		Assert.NotSame(repo1, repo2);
	}

	[Fact]
	public void Dispose_DisposesContext()
	{
		EfDbContext context = Fixture.CreateContext();
		UnitOfWork unitOfWork = new(context);
		unitOfWork.Dispose();

		Assert.Throws<ObjectDisposedException>(() => context.TodoTasks.ToList());
	}
}
