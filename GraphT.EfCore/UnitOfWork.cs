﻿using SeedWork;

namespace GraphT.EfCore;

public class UnitOfWork(EfDbContext context) : IUnitOfWork, IDisposable
{
	public async ValueTask<int> SaveChangesAsync()
	{
		return await context.SaveChangesAsync();
	}

	public void Dispose()
	{
		context.Dispose();
	}
}
