namespace DiaLuna;

using Godot;
using Microsoft.CodeAnalysis;

public enum Direction
{
	/// <summary>
	/// <para>Up direction, negative Y in Godot's coordinate system.</para>
	/// <para>A character facing up, would have their back to the camera.</para>
	/// </summary>
	Up,

	/// <summary>
	/// <para>Down direction, positive Y in Godot's coordinate system.</para>
	/// <para>A character facing down, would have their front to the camera.</para>
	/// </summary>
	Down,

	/// <summary>
	/// <para>Left direction, negative X in Godot's coordinate system.</para>
	/// <para>A character facing left, would have their left side to the camera.</para>
	/// </summary>
	Left,

	/// <summary>
	/// <para>Right direction, positive X in Godot's coordinate system.</para>
	/// <para>A character facing right, would have their right side to the camera.</para>
	/// </summary>
	Right,
}

public static class DirectionExtensions
{
	public static Vector2 ToVector2(this Direction dir) =>
		dir switch
		{
			Direction.Up => Vector2.Up,
			Direction.Down => Vector2.Down,
			Direction.Left => Vector2.Left,
			Direction.Right => Vector2.Right,
			_ => throw new System.ArgumentOutOfRangeException(nameof(dir), dir, null),
		};

	public static Optional<Direction> ToDirection(this Vector2 vec) =>
		vec switch
		{
			// Y has precedence
			{ Y: < 0 } => Direction.Up,
			{ Y: > 0 } => Direction.Down,
			{ X: < 0 } => Direction.Left,
			{ X: > 0 } => Direction.Right,
			_ => new Optional<Direction>(),
		};
}
