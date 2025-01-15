﻿using GraphT.Model.Aggregates;

using SeedWork;

namespace GraphT.Model.Services.Specifications;

public sealed class TaskIncludeDownstreamsSpecification : BaseSpecification<TodoTask>
{
	public TaskIncludeDownstreamsSpecification(Guid id) : 
		base(t => t.Id.Equals(id))
	{
		AddInclude(t => t.Downstreams);
	}
}

