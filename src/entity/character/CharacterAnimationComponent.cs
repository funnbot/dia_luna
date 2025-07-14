namespace DiaLuna;

using Godot;

public interface ICharacterAnimation
{
	public void Idle();
	public void Walk(Direction dir);
}

[GlobalClass]
public partial class CharacterAnimationComponent
	: AnimatedSprite2D,
		ICharacterAnimation
{
	[Export]
	private Direction _currentDirection = Direction.Down;

	public override void _Ready()
	{
		AnimationChanged += OnAnimationChanged;
	}

	public void Idle()
	{
		Play(AnimationMap.Get(Anim.Idle, _currentDirection));
	}

	public void Walk(Direction dir)
	{
		_currentDirection = dir;
		Play(AnimationMap.Get(Anim.Walk, dir));
	}

	private void OnAnimationChanged()
	{
		FlipH = _currentDirection == Direction.Right;
	}
}
