﻿using GraphT.Model.Aggregates;
using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.Model.Services.Specifications;

public sealed class FinishedTasksSpecification : BaseSpecification<TaskAggregate>
{
	public FinishedTasksSpecification(string? name, PagingParams pagingParams) : 
		base(task => 
			(task.Status == Status.Completed) && 
			(task.Status == Status.Dropped) && 
			(name == null || task.Name.Contains(name) ))
	{
		ApplyPaging(pagingParams.PageNumber, pagingParams.PageSize);
	}
}
