namespace DiaLuna;

using Godot;

[Tool]
public partial class CharacterAnimationFramesBuilder : Node
{
	[Export]
	private Texture2D _image = default!;

	[Export(PropertyHint.SaveFile, "*.tres;SpriteFrames File")]
	private string _savePath = "";

	[ExportToolButton("Generate Sprite Frames")]
	private Callable GenerateSpriteFramesButton =>
		Callable.From(GenerateSpriteFrames);

	private void GenerateSpriteFrames()
	{
		if (_image == null)
		{
			return;
		}

		SpriteFrames sf = new SpriteFramesBuilder()
			.SetTexture(_image, frameSize: 8)
			.AddAnimation("idle_down")
			.AddFrame(1, 0)
			.AddAnimation("idle_up")
			.AddFrame(2, 0)
			.AddAnimation("idle_left")
			.AddFrame(0, 0)
			.AddAnimation("idle_right")
			.AddFrame(0, 0)
			.AddAnimation("walk_down")
			.AddFrame(1, 2)
			.AddFrame(1, 3)
			.AddAnimation("walk_up")
			.AddFrame(2, 2)
			.AddFrame(2, 3)
			.AddAnimation("walk_left")
			.AddFrame(0, 2)
			.AddFrame(0, 3)
			.AddAnimation("walk_right")
			.AddFrame(0, 2)
			.AddFrame(0, 3)
			.Build();

		ResourceSaver.Save(sf, _savePath);
	}
}
