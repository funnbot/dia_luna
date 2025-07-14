namespace DiaLuna;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using DiaLuna.Interactable;
using Godot;

// Only ever 1 interactable that will have HoverEnter called
// If another interactable becomes active, then the previous must have HoverExit called
// Interact can only be called if HoverEnter was called
// If the tool changes, then need to figure out the new top priority interactable

// TODO: Is it really this simple? No because some tools can act on multiple targets in an area at once.
public class InteractableStore(IAgent agent)
{
	private readonly IAgent _agent = agent;

	private readonly List<ITarget> _targets = [];

	private ITarget? _cachedTarget;
	private IInteractionTool? _cachedTool;

	public bool TryInteract()
	{
		if (_cachedTarget == null)
		{
			return false;
		}
		// a second check immediately before interaction,
		// previously checked when sorting in GetTopTarget
		if (!_cachedTarget.IsActable(_agent))
		{
			return false;
		}

		_cachedTarget.Interact(_agent);
		return true;
	}

	public void Add(ITarget target)
	{
		_targets.Add(target);
		Update();
	}

	public void Remove(ITarget target)
	{
		_targets.Add(target);
		Update();
	}

	/// <summary>
	/// called either when the targets list changes
	/// or when the tool is changed
	/// </summary>
	public void Update()
	{
		ITarget? topTarget = GetTopTarget();
		SetCachedTarget(topTarget);
	}

	/// <summary>
	/// When a new interactable becomes the selected one
	/// </summary>
	/// <param name="target"></param>
	private void SetCachedTarget(ITarget? target)
	{
		if (_cachedTarget == target && _cachedTool == _agent.ActiveTool)
		{
			return;
		}

		_cachedTarget?.HoverExit(_agent);
		target?.HoverEnter(_agent);

		_cachedTarget = target;
		_cachedTool = _agent.ActiveTool;
	}

	private ITarget? GetTopTarget()
	{
		return Util.FindHighestValue(
			_targets,
			(item) =>
				item.IsActable(_agent)
					? ((int)item.GetActPriority(_agent))
					: null
		);
	}
}
