﻿using GraphT.Model.ValueObjects;

namespace GraphT.Model.Aggregates;

public class TodoTask
{
	public Guid Id { get; private set; }
	public string Name { get; set; }
	public Status Status { get; set; }
	private bool _isFun;
	private bool _isProductive;
	private Relevance _relevance;
	private DateTimeInfo _dateTimeInfo;
	private HashSet<TodoTask> _upstreams = null!;
	private HashSet<TodoTask> _downstreams = null!;
	private HashSet<LifeArea> _lifeAreas = null!;
	public bool IsFun
	{
		get => _isFun;
		set
		{
			_isFun = value;
			UpdateRelevance();
		}
	}
	public bool IsProductive
	{
		get => _isProductive;
		set
		{
			_isProductive = value;
			UpdateRelevance();
		}
	}
	public Complexity Complexity { get; set; }
	public Priority Priority { get; set; }
	public float Progress => GetProgress();
	public Relevance Relevance => _relevance;
	public DateTimeInfo DateTimeInfo => _dateTimeInfo;
	public IReadOnlySet<TodoTask> Upstreams => _upstreams;
	public IReadOnlySet<TodoTask> Downstreams => _downstreams;
	public IReadOnlySet<LifeArea> LifeAreas => _lifeAreas;

	private TodoTask(){ }

	public TodoTask(string name, 
							Status? status = null,
							bool? isFun = null, 
							bool? isProductive = null,
							Complexity? complexity = null, 
							Priority? priority = null,
							Guid? id = null)
	{
		Id = id ?? Guid.NewGuid();
		Name = name;
		Status = status ?? Status.Backlog;
		_isFun = isFun ?? false;
		_isProductive = isProductive ?? false;
		_dateTimeInfo = new DateTimeInfo();
		Complexity = complexity ?? Complexity.Indefinite;
		Priority = priority ?? Priority.MentalClutter;
		_upstreams = [];
		_downstreams = [];
		_lifeAreas = [];
		UpdateRelevance();
	}

	private void UpdateRelevance()
	{
		this._relevance = IsFun switch
		{
			true when IsProductive => Relevance.Purposeful,
			false when IsProductive => Relevance.Necessary,
			true when !IsProductive => Relevance.Entertaining,
			_ => Relevance.Superfluous
		};
	}
	
	public void AddUpstream(TodoTask upstream)
	{
		ValidateTask(upstream);
		
		_upstreams.Add(upstream);
	}

	public void RemoveUpstream(TodoTask upstream)
	{
		ValidateTask(upstream);
		
		_upstreams.RemoveWhere(todoTask => todoTask.Id.Equals(upstream.Id));
	}

	public void AddUpstreams(HashSet<TodoTask> upstreams)
	{
		ValidateTaskCollection(upstreams);
		
		_upstreams.UnionWith(upstreams);
	}

	public void RemoveUpstreams(HashSet<TodoTask> upstreams)
	{
		ValidateTaskCollection(upstreams);
		
		_upstreams.ExceptWith(upstreams);
	}

	public void ReplaceUpstreams(HashSet<TodoTask> newUpstreams)
	{
		ValidateTaskCollection(newUpstreams);
		
		_upstreams.Clear();
		_upstreams = newUpstreams;
	}

	public void ClearUpstreams()
	{
		if (_upstreams.Count == 1) return;
		
		_upstreams.Clear();
	}

	public void AddDownstream(TodoTask downstream)
	{
		ValidateTask(downstream);
		
		_downstreams.Add(downstream);
	}
	
	public void RemoveDownstream(TodoTask downstream)
	{
		ValidateTask(downstream);
		
		_downstreams.RemoveWhere(todoTask => todoTask.Id.Equals(downstream.Id));
	}

	public void AddDownstreams(HashSet<TodoTask> downstreams)
	{
		ValidateTaskCollection(downstreams);
		
		_downstreams.UnionWith(downstreams);
	}

	public void RemoveDownstreams(HashSet<TodoTask> downstreams)
	{
		ValidateTaskCollection(downstreams);

		_downstreams.ExceptWith(downstreams);
	}

	public void ReplaceDownstreams(HashSet<TodoTask> newDownstreams)
	{
		ValidateTaskCollection(newDownstreams);
		
		_downstreams.Clear();
		_downstreams = newDownstreams;
	}

	public void ClearDownstreams()
	{
		if (_downstreams.Count == 0) return;
		
		_downstreams.Clear();
	}
	
	private void ValidateTask(TodoTask todoTask)
	{
		if (todoTask.Id.Equals(Guid.Empty)) throw new ArgumentException("Task id cannot be empty");
	}

	private void ValidateTaskCollection(HashSet<TodoTask> taskCollection)
	{
		if (taskCollection is null) throw new ArgumentException("Task collection cannot be null");

		if (taskCollection.Count == 0) throw new ArgumentException("Task collection cannot be empty");

		if (taskCollection.Any(task => task.Id.Equals(Guid.Empty)))
			throw new ArgumentException("Task collection cannot contain tasks with empty Id");
	}
	
	public void AddLifeArea(LifeArea lifeArea)
	{
		ValidateLifeArea(lifeArea);
		
		_lifeAreas.Add(lifeArea);
	}
	
	public void RemoveLifeArea(LifeArea lifeArea)
	{
		ValidateLifeArea(lifeArea);
		
		_lifeAreas.RemoveWhere(lifeAreaAggregate => lifeAreaAggregate.Id.Equals(lifeArea.Id));
	}

	public void AddLifeAreas(HashSet<LifeArea> lifeAreas)
	{
		ValidateLifeAreaCollection(lifeAreas);
		
		_lifeAreas.UnionWith(lifeAreas);
	}

	public void RemoveLifeAreas(HashSet<LifeArea> lifeAreas)
	{
		ValidateLifeAreaCollection(lifeAreas);

		_lifeAreas.ExceptWith(lifeAreas);
	}

	public void ReplaceLifeAreas(HashSet<LifeArea> newLifeAreas)
	{
		ValidateLifeAreaCollection(newLifeAreas);
		
		_lifeAreas.Clear();
		_lifeAreas = newLifeAreas;
	}

	public void ClearLifeAreas()
	{
		if (_lifeAreas.Count == 0) return;
		
		_lifeAreas.Clear();
	}
	
	private void ValidateLifeArea(LifeArea lifeArea)
	{
		if (lifeArea.Id.Equals(Guid.Empty)) throw new ArgumentException("Life Area id cannot be empty");
	}

	private void ValidateLifeAreaCollection(HashSet<LifeArea> lifeAreaCollection)
	{
		if (lifeAreaCollection is null) throw new ArgumentException("Life Area collection cannot be null");

		if (lifeAreaCollection.Count == 0) throw new ArgumentException("Life Area collection cannot be empty");

		if (lifeAreaCollection.Any(lifeAreaAggregate => lifeAreaAggregate.Id.Equals(Guid.Empty)))
			throw new ArgumentException("Life Area collection cannot contain life areas with empty Id");
	}
	
	public void SetStartDate(DateTimeOffset startDate)
	{
		if (_dateTimeInfo.FinishDateTime is not null && startDate > _dateTimeInfo.FinishDateTime)
		{
			throw new ArgumentException("Start date cannot be after of finish date");
		}
		
		_dateTimeInfo.StartDateTime = startDate;
	}

	public void SetFinishDate(DateTimeOffset finishDate)
	{
		if (_dateTimeInfo.StartDateTime is not null && finishDate < _dateTimeInfo.StartDateTime)
		{
			throw new ArgumentException("Finish date cannot be before start date");
		}

		_dateTimeInfo.FinishDateTime = finishDate;
	}

	public void SetLimitDate(DateTimeOffset limitDate)
	{
		_dateTimeInfo.LimitDateTime = limitDate;
	}
	
	public void SetTimeSpend(string timeSpend)
	{
		_dateTimeInfo.TimeSpend = timeSpend;
	}
	
	private float GetProgress()
	{
		int totalDownstreams = _downstreams.Count;
		int backlogTasks = _downstreams.Count(task => task.Status is Status.Backlog);
		int completedOrDroppedTasks = _downstreams.Count(task => task.Status is Status.Dropped or Status.Completed);
		const int currentTask = 1;
		const float isFinished = 100f;
		const float isUnfinished = 0f;

		switch (totalDownstreams)
		{
			case 0 when (Status is not Status.Completed):
				return isUnfinished;
			case 0 when (Status is Status.Completed):
				return isFinished;
		}

		if (completedOrDroppedTasks >= totalDownstreams) return isFinished;
		
		if (backlogTasks == totalDownstreams) return isUnfinished;

		return (completedOrDroppedTasks * isFinished) / (totalDownstreams + currentTask);
	}
}