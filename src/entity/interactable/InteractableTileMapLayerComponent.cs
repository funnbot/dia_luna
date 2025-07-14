namespace DiaLuna;

using System.Collections.Generic;
using Chickensoft.Introspection;
using Chickensoft.Serialization;
using DiaLuna.Interactable;
using Godot;

[Meta, Id("tile_cell_structure_data")]
public readonly partial struct TileCellStructureData
{
	[Save("rect")]
	public required Rect2I Rect { get; init; }

	[Save("source_coord")]
	public required Vector2I SourceCoord { get; init; }
}

[Meta, Id("tile_layer_structure_data")]
public readonly partial struct TileLayerStructureData
{
	[Save("map")]
	public required Dictionary<
		Vector2I,
		TileCellStructureData
	>? Data { get; init; }
}

public partial class InteractableTileMapLayerComponent : Node, ITarget
{
	public void HoverEnter(IAgent agent, IInteractionTool tool)
	{
		throw new System.NotImplementedException();
	}

	public void HoverExit(IAgent agent)
	{
		throw new System.NotImplementedException();
	}

	public void Interact(IAgent agent, IInteractionTool tool)
	{
		throw new System.NotImplementedException();
	}

	public void Interact(IAgent agent) =>
		throw new System.NotImplementedException();
}
