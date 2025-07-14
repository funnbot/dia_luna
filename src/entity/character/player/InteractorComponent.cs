namespace DiaLuna;

using System.Diagnostics;
using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using DiaLuna.Interactable;
using Godot;

[GlobalClass]
[Meta(typeof(IAutoConnect))]
public partial class InteractorComponent : Node, IAgent
{
	public override void _Notification(int what)
	{
		this.Notify(what);
	}

	[Export(hintString: "suffix:px")]
	private float _maxInteractDistance = 10;

	[Node("../VelocityComponent")]
	private ICharacterBody VelocityComponent { get; set; } = default!;

	[Node("../InputComponent")]
	private InputComponent InputComponent { get; set; } = default!;

	[Node("SelectArea")]
	private Area2D SelectArea { get; set; } = default!;

	public Vector2 GlobalPosition => throw new System.NotImplementedException();

	public Vector2 Facing => throw new System.NotImplementedException();

	private InteractableStore _interactables = default!;

	public IInteractionTool ActiveTool { get; set; } = default!;

	public Vector2 GetPosition()
	{
		return VelocityComponent.GlobalPosition;
	}

	public override void _Ready()
	{
		_interactables = new(this);
		// InputComponent.MainAttack += OnMainInteract;
		SelectArea.BodyEntered += OnBodyEntered;
		// SelectArea.BodyExited += OnBodyExited;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 targetPosition =
			VelocityComponent.GetCanvasTransform().AffineInverse()
			* InputComponent.MouseGlobalPosition;
		Vector2 clampedPosition = ClampDistance(
			targetPosition,
			VelocityComponent.GlobalPosition,
			_maxInteractDistance
		);
		SelectArea.GlobalPosition = targetPosition;
	}

	// private void OnMainInteract(bool pressed)
	// {
	// 	if (!pressed)
	// 	{
	// 		return;
	// 	}

	// 	if (_interactables.Count == 0)
	// 	{
	// 		return;
	// 	}

	// 	ITarget? topInteract = Util.FindHighestValue(
	// 		_interactables,
	// 		(item) => item.GetActPriority(this as IAgent, _interactTool)
	// 	);
	// 	topInteract?.Interact(this, _interactTool);
	// }

	private void OnBodyEntered(Node2D body)
	{
		var target = body as ITarget;
		Util.Assert(
			target != null,
			"Body with 'interact' layer must implement ITarget"
		);
		_interactables.Add(target);
	}

	private void OnBodyExited(Node2D body)
	{
		var target = body as ITarget;
		Util.Assert(
			target != null,
			"Body with 'interact' layer must implement ITarget"
		);
		_interactables.Remove(target);
	}

	private static void UpdateInteractHover() { }

	private static Vector2 ClampDistance(
		Vector2 value,
		Vector2 origin,
		float maxDistance
	)
	{
		Vector2 diff = value - origin;
		var distanceSqr = diff.LengthSquared();
		var maxSqr = maxDistance * maxDistance;
		if (distanceSqr < maxSqr)
		{
			return value;
		}

		return origin + (diff.Normalized() * maxDistance);
	}
}
