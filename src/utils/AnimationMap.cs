namespace DiaLuna;

using Godot;

public enum Anim
{
	Idle,
	Walk,
}

// TODO: turn this into a source generator
public static class AnimationMap
{
	public static readonly StringName IdleUp = "idle_up";
	public static readonly StringName IdleDown = "idle_down";
	public static readonly StringName IdleLeft = "idle_left";
	public static readonly StringName IdleRight = "idle_right";
	public static readonly StringName WalkUp = "walk_up";
	public static readonly StringName WalkDown = "walk_down";
	public static readonly StringName WalkLeft = "walk_left";
	public static readonly StringName WalkRight = "walk_right";

	public static StringName Get(Anim key, Direction dir) =>
		key switch
		{
			Anim.Idle => dir switch
			{
				Direction.Up => IdleUp,
				Direction.Down => IdleDown,
				Direction.Left => IdleLeft,
				Direction.Right => IdleRight,
				_ => throw EnumOutOfRange(dir, nameof(dir)),
			},
			Anim.Walk => dir switch
			{
				Direction.Up => WalkUp,
				Direction.Down => WalkDown,
				Direction.Left => WalkLeft,
				Direction.Right => WalkRight,
				_ => throw EnumOutOfRange(dir, nameof(dir)),
			},
			_ => throw EnumOutOfRange(key, nameof(key)),
		};

	public static (Anim, Direction) UnwrapDirection(StringName name) =>
		name switch
		{
			_ when name == IdleUp => (Anim.Idle, Direction.Up),
			_ when name == IdleDown => (Anim.Idle, Direction.Down),
			_ when name == IdleLeft => (Anim.Idle, Direction.Left),
			_ when name == IdleRight => (Anim.Idle, Direction.Right),
			_ when name == WalkUp => (Anim.Walk, Direction.Up),
			_ when name == WalkDown => (Anim.Walk, Direction.Down),
			_ when name == WalkLeft => (Anim.Walk, Direction.Left),
			_ when name == WalkRight => (Anim.Walk, Direction.Right),
			_ => throw NameOutOfRange(name, nameof(name)),
		};

	private static System.ArgumentOutOfRangeException EnumOutOfRange<TEnum>(
		TEnum value,
		string paramName
	)
		where TEnum : struct, System.Enum
	{
		return new System.ArgumentOutOfRangeException(
			paramName,
			value,
			$"Invalid value for {typeof(TEnum).Name}: {value}"
		);
	}

	private static System.ArgumentOutOfRangeException NameOutOfRange(
		StringName name,
		string paramName
	)
	{
		return new System.ArgumentOutOfRangeException(
			paramName,
			name,
			$"Invalid value for {nameof(AnimationMap)}: {name}"
		);
	}
}
