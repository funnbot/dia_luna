namespace DiaLuna;

using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;

[Meta, LogicBlock(typeof(State), Diagram = true)]
public partial class NavAgentLogic : LogicBlock<NavAgentLogic.State>
{
	public override Transition GetInitialState()
	{
		return To<State.Unsynced>();
	}

	public static class Input
	{
		public readonly record struct Synchronized;

		public readonly record struct Target(Vector2 Position);

		public readonly record struct Stop;

		public readonly record struct PhysicsTick(double Delta);
	}

	public static class Output
	{
		public readonly record struct TargetChanged(Vector2 Position);

		public readonly record struct Stop;

		public readonly record struct VelocityComputed(Vector2 Velocity);
	}

	public record Data
	{
		public Vector2 TargetPosition { get; set; } = Vector2.Zero;
	}

	public abstract record State : StateLogic<State>
	{
		public record Unsynced : State, IGet<Input.Synchronized>
		{
			public Transition On(in Input.Synchronized input)
			{
				return To<Idle>();
			}
		}

		public record Idle : State, IGet<Input.Target>
		{
			public Transition On(in Input.Target input)
			{
				Get<Data>().TargetPosition = input.Position;
				return To<Navigating>();
			}
		}

		public record Navigating
			: State,
				IGet<Input.Target>,
				IGet<Input.Stop>,
				IGet<Input.PhysicsTick>
		{
			public Transition On(in Input.Target input)
			{
				Get<Data>().TargetPosition = input.Position;
				return ToSelf();
			}

			public Transition On(in Input.Stop input)
			{
				return To<Idle>();
			}

			public Transition On(in Input.PhysicsTick input)
			{
				return ToSelf();
			}
		}
	}
}

[GlobalClass]
[Meta(typeof(IAutoNode))]
public partial class NavAgentComponent : NavigationAgent2D
{
	public override void _Notification(int what) => this.Notify(what);

	[Export(PropertyHint.None, "suffix:px/s")]
	public float MoveSpeed { get; set; } = 50;

	[Node("../VelocityComponent")]
	private ICharacterBody VelocityComponent { get; set; } = default!;

	private bool IsNavigating => !_isStopped;

	private bool _isNavServerSynced;
	private bool _isStopped;

	private NavAgentLogic Logic { get; set; } = default!;

	/// <summary>
	/// Set the target global position for the navigation agent.
	/// </summary>
	/// <param name="targetGlobalPosition">global position</param>
	public void SetTarget(Vector2 targetGlobalPosition)
	{
		if (!_isNavServerSynced)
		{
			return;
		}

		_isStopped = false;
		TargetPosition = targetGlobalPosition;
	}

	public void Stop()
	{
		if (!_isNavServerSynced || _isStopped)
		{
			return;
		}

		_isStopped = true;
		TargetPosition = VelocityComponent.GlobalPosition;
	}

	public override void _Ready()
	{
		// when using RVO avoidance for dynamic navigation
		VelocityComputed += OnVelocityComputed;

		CallDeferred(MethodName.CheckServerSynced);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_isNavServerSynced || _isStopped)
		{
			return;
		}
		// Can a map be unsynchonized after initialization?
		// If the map was created this physics frame, then yes.
		// this game likely will only use the default navigation map,
		// and wont dynamically update it
		// Debug.Assert(NavigationServer2D.MapGetIterationId(GetNavigationMap()) != 0);

		if (IsNavigationFinished())
		{
			_isStopped = true;
			return;
		}

		ComputeVelocity();
	}

	private void ComputeVelocity()
	{
		Vector2 nextPos = GetNextPathPosition();
		Vector2 velocity = VelocityComponent.GlobalPosition.DirectionTo(nextPos) * MoveSpeed;
		if (AvoidanceEnabled)
		{
			Velocity = velocity;
		}
		else
		{
			OnVelocityComputed(velocity);
		}
	}

	private void OnVelocityComputed(Vector2 safe_velocity)
	{
		VelocityComponent.TargetVelocity = safe_velocity;
	}

	private async void CheckServerSynced()
	{
		await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
		_isNavServerSynced = true;
	}
}
