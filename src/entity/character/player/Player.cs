namespace DiaLuna;

using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using Godot;

[GlobalClass]
[Meta(typeof(IAutoConnect))]
public partial class Player : Node2D
{
	[Node("VelocityComponent")]
	private ICharacterBody VelocityComponent { get; set; } = default!;

	[Node("InputComponent")]
	private IInputComponent InputComponent { get; set; } = default!;

	public override void _Notification(int what) => this.Notify(what);

	public override void _Process(double delta)
	{
		if (InputComponent.IsMoving)
		{
			VelocityComponent.TargetVelocity =
				InputComponent.InputDirection * 50f;
		}
		else
		{
			VelocityComponent.TargetVelocity = Vector2.Zero;
		}
	}
}
