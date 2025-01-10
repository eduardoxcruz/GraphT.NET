﻿using GraphT.Model.Aggregates;

using SeedWork;

namespace GraphT.Model.Services.Specifications;

public sealed class FindDownstreamsByTaskIdSpecification : BaseSpecification<TaskAggregate>
{
	public FindDownstreamsByTaskIdSpecification(Guid id, PagingParams pagingParams) : 
		base(t => t.Id.Equals(id))
	{
		ApplyPaging(pagingParams.PageNumber, pagingParams.PageSize);
		AddInclude(t => t.Downstreams);
	}
}

