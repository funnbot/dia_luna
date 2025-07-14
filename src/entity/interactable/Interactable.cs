// Interactor, Interactable, Interaction
// Actor, Actable, Action
// Agent, Target, Tool

namespace DiaLuna.Interactable;

using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using Godot;

/// <summary>
/// lower value is interacted with first
/// </summary>
public enum TargetPriority
{
	First,
	Enemy,
	NPC,
	Machine,
	Crop,

	Last,
}

// TODO: TargetGroup enum?

/// <summary>
/// Represents an object that can be interacted with by an <see cref="IAgent"/>.
/// </summary>
public interface ITarget
{
	/// <summary>
	/// Performs the interaction logic for this object using the specified interactor and interaction tool.
	/// </summary>
	/// <param name="agent">The entity attempting to interact with this object.</param>
	public void Interact(IAgent agent);

	/// <summary>
	/// Gets a value indicating whether this object is currently interactable.
	/// </summary>
	/// <param name="agent"></param>
	public bool IsActable(IAgent agent) => true;

	/// <summary>
	/// Gets the interaction priority for this object when multiple interactables are available.
	/// Lower values indicate first priority for interaction.
	/// </summary>
	/// <param name="agent">The interactor attempting to interact.</param>
	/// <returns>An integer representing the priority of this interactable.</returns>
	public TargetPriority GetActPriority(IAgent agent) => TargetPriority.Last;

	/// <summary>
	/// Called when an <see cref="IAgent"/> begins hovering over this interactable.
	/// </summary>
	/// <param name="agent">The interactor that is hovering.</param>
	/// <returns>True if the hover enter was successful; otherwise, false.</returns>
	public void HoverEnter(IAgent agent) { }

	/// <summary>
	/// Called when an <see cref="IAgent"/> stops hovering over this interactable.
	/// </summary>
	/// <param name="agent">The interactor that was hovering.</param>
	/// <returns>True if the hover exit was successful; otherwise, false.</returns>
	public void HoverExit(IAgent agent) { }
}

/// <summary>
/// Represents an entity capable of interacting with <see cref="ITarget"/> objects.
/// </summary>
public interface IAgent
{
	/// <summary>
	/// Gets the currently active tool used by the agent for interactions.
	/// </summary>
	public IInteractionTool ActiveTool { get; }

	/// <summary>
	/// Gets the global position of the interactor in the game world.
	/// </summary>
	public Vector2 GlobalPosition { get; }

	/// <summary>
	/// Gets the facing direction of the interactor as a normalized vector.
	/// </summary>
	public Vector2 Facing { get; }
}

// TODO: IInteractionTool is more complicated
// need to modify the IAgent Area2D shape
// - interact area will pick one top priority of current bodies
// - pick multiple of current bodies in the same TargetGroup
//   - (what about priority?, or maybe Priority and Group are the same)
// - area is animated and could collect all bodies entered throughout the animation, even if exited some
// - what is responsible for this functionality?
// - Does it even make sense to handle both, sword swing, which is collision body, and pointer click to open a machine or talk to npc?
// - At its core, this is a system for collecting a one or more Shape2D in world in various ways (moving area2d),
//   - and then possibly sorting and selecting, and then sending event to the selected stuff,
//   - as well as showing vfx though… and triggering animations on the Agent, although,
//   - the agent could also just have a event for when an interaction happens.
// - The PointerHover could itself be a IInteractionTool, and could allow multiple tools enabled at a time.
//   - Should ITarget just have one event. Or one for each IInteractionTool, not possible probably
// - What to do about interactions that take time (animation) and cooldowns.
//   - Cooldowns in the Item or the Actuator?
//   - Or in both… items definitely need individual cooldowns, like spells,
//   - But also spells with have a whole mana system too, would that be handled by the Actuator.
//   - I probably need shared Actuators state for all spells, and also different actuators with their own intractable selection and collision object.
// - Is interfaces overcomplicating?
//   - Also need to decide if you press a different interact button like E while close and facing a e.g. NPC,
//   - or only use pointer and main button down, with controller obviously would only be directional still.

// Example Tools
// - DefaultPointer
//   - single point, single interactable
// - Sword
//   - Animate a rectangle shape, and get all bodies contacted throughout
// - Spell
//   - If the IInteractionTool node owns its area2d, probably don't want to create separate prefab for every individual tool
//   - Could be separated into AreaSpell, PointSpell, ProjectileSpell prefabs though
//   -
//   - So dividing these different concerns

// Implementation
// - so when shape is changed reset the list of bodies
// - most versatile would be the IInteractionTool handling all of the Area2D modifications
//   - What data does it need from the IAgent
// - If the IInteractionTool is a child node with its own Area2D, better decoupling?
//   - Maybe a child of the InteractorComponent
//   - so only one tool is active at a time, whatever is managing tools
//     - maintain list of instantiated tools
//   - Or a separate ToolManager component
//   - and a separate `InteractorAgentComponent : IAgent` ?
// - If the IInteractionTool node owns its area2d, probably don't want to create separate prefab for every individual tool
//   - Could be separated into AreaSpell, PointSpell, ProjectileSpell prefabs though
//   - So mainly dividing tools by collision functionality
//   - Individual spells would also have individual vfx, animations, actual effect on the world
//   - So maybe need any IInteractionTool to have a property IActuator which would actually represent individual items/spells
// - There is a difference between hover and a combat interaction then.
// - So maybe the PointerArea/SelectArea whatever is a separate thing
//   - because you don't hover with a weapon, yet you can still create hover events in the world with pointer
//   - Are there any cases where different IActuator would change this Pointer hover over a single tile/entity
//   - Definitely could change depending on specific tools used, e.g. pickaxe and axe, but only the hover shown in UI,
//     - as it doesn't change the interactable selecting
//   - Even for area of effect, or selection box stuff, you don't usually change from PointerArea to something else
//     - until triggering the interaction (main button down), which will be a different mechanism.
//   - So hover functionality is a different concern, but still depends on specific item/tool active

// Owner Id
// - A CollisionObject owns a map of owner_id => ShapeData
//   - ShapeData::shapes is a std::vector of ShapeData::Shape with fields Shape2D and index
//   - owner_id is basically an incrementing int key
//   - ShapeData::owner_id is the instance id of the object passed to create_shape_owner(obj)
//   - ShapeData::index is set to total_subshapes++, so its unique from all shapes from all owners in this CollisionObject
//     - this is the `local_index` or `shape_index` in the docs, and lets you fetch specific shapes even if you don't know the owner
//   - the index in the vector of ShapeData::shapes is more common in the shape_owner functions
//     - usually looks like (int owner_id, int shape_id)
// - the shape owner acts as a grouping of Shape2D, with some shared properties
// - The CollisionShape node is mainly a helper for making itself the owner_id of the parent CollisionObject
//   - and add its single editor-set shape to the CollisionObject with the owner_id created with the CollisionShape node
//   - along with setting various properties (offset transform, disabled, one way collision)
//   - CollisionShape propagates changes by calling shape_owner functions
//   - Shape2D resource id is the only way its handled in the PhysicsServer2D

public interface ITargetCollector
{
	public ChannelReader<ITarget> CollectTargets();
}

public interface IInteractionVfx
{
	public Task Run();
}

public interface IInteractionUI
{
	public Task ShowHover();
	public void HideHover();
}

public interface IInteractionCoordinator
{
	public Task Run();
}

public interface IInteractionItem
{
	/// <summary>
	/// The IActuator is a Node, child of Player, so maybe that provides the necessary tools for effecting the environment
	/// </summary>
	public void Interact();

	public IInteractionVfx Vfx { get; }
	public IInteractionUI UI { get; }
}

/// <summary>
/// Represents a tool or pointer that an agent can use to interact with the world.
/// Handles enabling/disabling, and provides access to collision, item, vfx, and ui.
/// </summary>
public interface IInteractionTool
{
	public ITargetCollector TargetCollector { get; }
	public IInteractionItem Item { get; }
}

public abstract partial class InteractionTool : Node
{
	public ITargetCollector TargetCollector { get; private set; } = default!;
	public IInteractionItem Item { get; private set; } = default!;
}

// temp example implementation

public partial class PointSingleInteractionTool : InteractionTool { }
