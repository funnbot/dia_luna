namespace DiaLuna;

using System;
using System.Diagnostics.CodeAnalysis;
using Godot;

public class SpriteFramesBuilder
{
	private readonly SpriteFrames _sf;

	// current state, resets on AddAnimation

	private StringName _name = new();
	private Texture2D? _texture;
	private int _frameSize;

	public SpriteFramesBuilder()
	{
		_sf = new SpriteFrames();
		_sf.RemoveAnimation("default");
	}

	[MemberNotNull(nameof(_texture))]
	public SpriteFramesBuilder SetTexture(Texture2D texture, int frameSize)
	{
		var size = new Vector2I(texture.GetWidth(), texture.GetHeight());
		if (size % frameSize != Vector2I.Zero)
		{
			throw new ArgumentException(
				$"Texture size {size} is not a multiple of sprite size {frameSize}."
			);
		}

		_texture = texture;
		_frameSize = frameSize;

		return this;
	}

	/// <summary>
	/// Must call SetTexture first
	/// </summary>
	/// <param name="name">The animation name</param>
	/// <exception cref="InvalidOperationException"></exception>
	public SpriteFramesBuilder AddAnimation(string name)
	{
		if (_texture == null)
		{
			throw new InvalidOperationException(
				"Must call SetTexture before adding animations."
			);
		}

		_name = name;
		_sf.AddAnimation(name);

		return this;
	}

	/// <summary>
	/// Add a frame to the current animation.
	/// </summary>
	/// <param name="coord">The coordinate of the frame in the sprite sheet, with top-left as origin</param>
	/// <param name="duration">Relative duration of this frame</param>
	public SpriteFramesBuilder AddFrame(Vector2I coord, float duration = 1.0f)
	{
		var atlas = new AtlasTexture
		{
			Atlas = _texture,
			Region = CoordToFrameRect(coord),
		};

		_sf.AddFrame(_name, atlas);
		return this;
	}

	public SpriteFramesBuilder AddFrame(int x, int y, float duration = 1.0f)
	{
		return AddFrame(new Vector2I(x, y), duration);
	}

	private Rect2I CoordToFrameRect(Vector2I coord) =>
		new(coord * _frameSize, _frameSize, _frameSize);

	public SpriteFrames Build()
	{
		return _sf;
	}
}
