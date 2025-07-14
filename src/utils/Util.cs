namespace DiaLuna;

using System;
using System.Collections.Generic;

public static partial class Util
{
	public static T? FindHighestValue<T>(
		IList<T> values,
		Func<T, int?> predicate
	)
	{
		if (values.Count == 0)
		{
			return default;
		}

		var highestValue = int.MinValue;
		T? selectedItem = default;
		foreach (T? item in values)
		{
			var value = predicate(item);
			// unnecessary null check?
			if (value > highestValue)
			{
				highestValue = value.Value;
				selectedItem = item;
			}
		}
		return selectedItem;
	}
}
