namespace DiaLuna;

using Godot;

[GlobalClass]
public partial class ItemContainerComponent : Node
{
	[Export]
	private int Size { get; set; }

	private ItemContainer ItemContainer { get; set; } = default!;

	public override void _Ready()
	{
		ItemContainer = new ItemContainer(Size);
	}
}
