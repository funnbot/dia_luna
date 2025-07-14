namespace DiaLuna;

using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using Godot;

/// <summary>
/// Interface for a repository that stores and provides the target velocity for a character.
/// </summary>
public interface IVelocityRepo
{
	public Vector2 TargetVelocity { get; set; }
	public Vector2 GlobalPosition { get; set; }
}

// lifecycle of IAutoNode and friends
// OnEnterTree()
//     tree specific operations
//     [Node] attributes will be fetched right before this
// Initialize()  from IAutoInit: if not testing
//     initialize values which would be mocked in unit testing
// OnReady()
//     prepare node using the would be mocked values
//     if not IDependent, call this.Provide() here
// Setup()  from IDependent: if not testing
//     called once dependencies are resolved
//     but not called if testing
// OnResolved()  from IDependent
//     use the resolved dependencies, call this.Provide()
//
// OnTreeExit()
//     Remove references to dependencies

public interface ICharacterBody
{
	public Vector2 TargetVelocity { get; set; }
	public Vector2 GlobalPosition { get; }
	public Transform2D GlobalTransform { get; }
	public Transform2D GetCanvasTransform();
}

/// <summary>
/// A component for handling velocity and movement logic for a character in a 2D Godot scene.
/// Synchronizes its global transform with a parent node.
/// Because this is a child node, this is also the most up to date transform.
/// </summary>
[GlobalClass]
[Meta(typeof(IAutoNode))]
public partial class CharacterBodyComponent : CharacterBody2D, ICharacterBody
{
	public override void _Notification(int what)
	{
		this.Notify(what);
	}

	private Node2D? _parent;

	public Vector2 TargetVelocity { get; set; }

	public void OnReady()
	{
		SetPhysicsProcess(true);
	}

	public void OnEnterTree()
	{
		TopLevel = true;
		_parent = GetParent<Node2D>();
		// setting TopLevel = true appears to copy local transform to global transform
		GlobalTransform = _parent.GlobalTransform;
		ProcessPhysicsPriority = _parent.ProcessPhysicsPriority - 1;
		ProcessPriority = _parent.ProcessPriority - 1;
	}

	public void OnPhysicsProcess(double delta)
	{
		Vector2 target = TargetVelocity;
		if (!target.IsZeroApprox())
		{
			Velocity = TargetVelocity;
			MoveAndSlide();
		}

		_parent!.GlobalTransform = GlobalTransform;
	}
}
