﻿using GraphT.Model.Aggregates;
using GraphT.Model.Exceptions;
using GraphT.Model.Services.Specifications;

using SeedWork;

namespace GraphT.UseCases.FindTaskUpstreamsById;

public interface IInputPort : IPort<InputDto> { }

public interface IOutputPort : IPort<OutputDto> { }

public class UseCase : IInputPort
{
	private readonly IOutputPort _outputPort;
	private readonly IUnitOfWork _unitOfWork;

	public UseCase(IOutputPort outputPort, IUnitOfWork unitOfWork)
	{
		_outputPort = outputPort;
		_unitOfWork = unitOfWork;
	}

	public async ValueTask Handle(InputDto dto)
	{
		TaskAggregate? task = await _unitOfWork.Repository<TaskAggregate>().FindByIdAsync(dto.Id);

		if (task is null) throw new TaskNotFoundException("Task not found", dto.Id);

		FindUpstreamsByTaskIdSpecification specification = new(dto.Id, dto.PagingParams);
		task = (await _unitOfWork.Repository<TaskAggregate>().FindAsync(specification)).First();
		PagedList<TaskIdAndName> upstreams = new(
			task.Upstreams.Select(TaskIdAndName.MapFrom).ToList(),
			task.Upstreams.Count,
			dto.PagingParams.PageNumber,
			dto.PagingParams.PageSize);

		await _outputPort.Handle(new OutputDto() { Upstreams = upstreams });
	}
}

public class InputDto
{
	public PagingParams PagingParams { get; set; }
	public Guid Id { get; set; }
}

public class OutputDto
{
	public PagedList<TaskIdAndName> Upstreams { get; set; }
}
