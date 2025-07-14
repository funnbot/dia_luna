namespace DiaLuna;

using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using Godot;

[GlobalClass]
[Meta(typeof(IAutoConnect))]
public partial class PlayerAnimatorComponent : Node
{
	public override void _Notification(int what) => this.Notify(what);

	[Node("../CharacterAnimationComponent")]
	private ICharacterAnimation AnimationPlayer { get; set; } = default!;

	[Node("../InputComponent")]
	private InputComponent InputComponent { get; set; } = default!;

	public override void _Process(double delta)
	{
		if (InputComponent.IsMoving)
		{
			AnimationPlayer.Walk(
				InputComponent.InputDirection.ToDirection().Value
			);
		}
		else
		{
			AnimationPlayer.Idle();
		}
	}
}
