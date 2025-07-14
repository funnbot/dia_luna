namespace DiaLuna;

using System.Diagnostics;
using Godot;

// public interface IItemStorageControl
// {
// 	bool IsValidCoord(Vector2I coord);

// 	bool IsCoordEmpty(Vector2I coord);
// 	bool CanMergeStack(Vector2I coord, ItemStack incomingStack);
// 	bool CanInsertStack(ItemStack incomingStack);

// 	bool TryPickupStack(Vector2I coord, out ItemStack? incomingStack);
// 	bool TryDropStack(Vector2I coord, ref ItemStack incomingStack);
// 	bool TryTakeFromStack(Vector2I coord, int amount, out ItemStack? incomingStack);
// 	bool TrySplitStack(Vector2I coord, out ItemStack? incomingStack);
// 	bool TryInsertStack(ref ItemStack incomingStack);
// }

public partial class ItemDefinition : Resource
{
	[Export]
	public StringName Id { get; set; } = default!;

	[Export]
	public int MaxStackSize { get; set; }

	// public ItemStack CreateStack(int addedQuantity) => new(this, addedQuantity);
}

#pragma warning disable CA1711
public struct ItemStack
#pragma warning restore CA1711
{
	public static readonly ItemStack NullStack = new() { };

	private readonly ItemDefinition? _definition = null;

	/// <summary>
	/// Do not get if IsNull
	/// </summary>
	public readonly ItemDefinition Definition
	{
		get
		{
			Debug.Assert(Definition != null);
			return _definition!;
		}
		init => _definition = value;
	}

	private int _quantity = 0;

	/// <summary>
	/// Do not set if IsNull
	/// </summary>
	public int Quantity
	{
		readonly get => _quantity;
		set
		{
			Debug.Assert(Definition != null);
			Debug.Assert(value >= 0 && value <= MaxSize);
			_quantity = value;
		}
	}

	public readonly int MaxSize => _definition?.MaxStackSize ?? 0;
	public readonly bool IsEmpty => _quantity == 0;
	public readonly bool IsFull => _quantity == MaxSize;
	public readonly bool IsNull => _definition == null;

	public ItemStack(ItemDefinition definition, int quantity)
	{
		Definition = definition;
		Quantity = quantity;
	}

	public ItemStack() { }

	public readonly ItemStack CopyWithQuantity(int quantity)
	{
		return new(Definition, quantity);
	}

	public readonly bool CanStackWith(ItemStack incomingStack)
	{
		if (IsNull || incomingStack.IsNull)
		{
			return true;
		}

		return _definition == incomingStack._definition;
	}

	public readonly bool IsDefinitionEqual(ItemDefinition definition)
	{
		if (IsNull)
		{
			return false;
		}
		return _definition == definition;
	}
}

public class ItemSlot(int index)
{
	public int Index { get; init; } = index;

	private ItemStack _containedStack = ItemStack.NullStack;

	public bool CanMerge(ref ItemStack incomingStack)
	{
		if (_containedStack.IsNull)
		{
			return true;
		}

		if (_containedStack.CanStackWith(incomingStack))
		{
			return !_containedStack.IsFull;
		}

		return false;
	}

	public bool IsDefinitionEqual(ItemDefinition definition) =>
		_containedStack.IsDefinitionEqual(definition);

	public bool TryMergeStack(ref ItemStack incomingStack)
	{
		if (_containedStack.IsNull)
		{
			_containedStack = incomingStack;
			return true;
		}
		if (!_containedStack.CanStackWith(incomingStack))
		{
			return false;
		}

		var changed = ItemStackUtility.MergeCount(
			_containedStack.Quantity,
			incomingStack.Quantity,
			_containedStack.MaxSize,
			out var containedQuantity,
			out var incomingQuantity
		);

		if (changed)
		{
			_containedStack.Quantity = containedQuantity;
			incomingStack.Quantity = incomingQuantity;
		}

		return changed;
	}

	public bool TryPickupStack(out ItemStack stack)
	{
		if (!_containedStack.IsNull)
		{
			stack = _containedStack;
			_containedStack = ItemStack.NullStack;
			return true;
		}

		stack = ItemStack.NullStack;
		return false;
	}
}

public class ItemContainer
{
	private ItemSlot[] ItemSlots { get; set; }
	public int Count => ItemSlots.Length;

	public ItemContainer(int capacity)
	{
		ItemSlots = new ItemSlot[capacity];
		for (var i = 0; i < capacity; i++)
		{
			ItemSlots[i] = new ItemSlot(i);
		}
	}

	public bool TryMergeStack(int index, ref ItemStack incomingStack)
	{
		return ItemSlots[index].TryMergeStack(ref incomingStack);
	}

	public bool TryPickupStack(int index, out ItemStack incomingStack)
	{
		return ItemSlots[index].TryPickupStack(out incomingStack);
	}

	public bool TryInsertStack(ref ItemStack incomingStack, out int index)
	{
		for (var i = 0; i < ItemSlots.Length; i++)
		{
			if (ItemSlots[i].TryMergeStack(ref incomingStack))
			{
				index = i;
				return true;
			}
		}

		index = -1;
		return false;
	}
}

public static class ItemStackUtility
{
	/// <summary>
	/// Adds <paramref name="amountToAdd"/> to <paramref name="currentAmount"/>, clamping the result to the range [0, <paramref name="maximumCapacity"/>].
	/// Outputs the resulting amount in <paramref name="resultingAmount"/> and any overflow in <paramref name="remainder"/>.
	/// Returns <c>true</c> if <paramref name="resultingAmount"/> changed from <paramref name="currentAmount"/>, otherwise <c>false</c>.
	/// </summary>
	/// <param name="currentAmount">The current value to add to.</param>
	/// <param name="amountToAdd">The value to add.</param>
	/// <param name="maximumCapacity">The maximum allowed value for the result.</param>
	/// <param name="resultingAmount">The resulting value after addition and clamping.</param>
	/// <param name="remainder">The leftover amount if the sum exceeds <paramref name="maximumCapacity"/>.</param>
	/// <returns>True if the resulting amount changed; otherwise, false.</returns>
	public static bool MergeCount(
		int currentAmount,
		int amountToAdd,
		int maximumCapacity,
		out int resultingAmount,
		out int remainder
	)
	{
		Debug.Assert(currentAmount >= 0 && currentAmount <= maximumCapacity);
		Debug.Assert(maximumCapacity >= 0);

		var sum = currentAmount + amountToAdd;
		resultingAmount = Mathf.Clamp(sum, 0, maximumCapacity);
		remainder = sum - resultingAmount;

		return resultingAmount != currentAmount;
	}
}
