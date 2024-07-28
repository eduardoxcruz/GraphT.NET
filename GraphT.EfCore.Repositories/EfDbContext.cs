﻿using GraphT.EfCore.Repositories.EntityTypeConfigurations;
using GraphT.Model.Aggregates;
using GraphT.Model.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GraphT.EfCore.Repositories;

public partial class EfDbContext : DbContext
{
	public DbSet<TodoTask> TodoTasks { get; set; }
	public DbSet<TaskAggregate> TaskAggregates { get; set; }
	
	public EfDbContext(DbContextOptions<EfDbContext> options) : base(options) { }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		new TodoTaskEntityTypeConfiguration().Configure(modelBuilder.Entity<TodoTask>());
		new TaskAggregateEntityTypeConfiguration().Configure(modelBuilder.Entity<TaskAggregate>());
		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<EfDbContext>
{
	public EfDbContext CreateDbContext(string[] args)
	{
		
		DbContextOptionsBuilder<EfDbContext> builder = new();
		builder.UseSqlServer("Server=localhost;Database=Testing;User Id=sa;Password=DevPassword123_;Encrypt=False");
		return new EfDbContext(builder.Options);
	}
}
