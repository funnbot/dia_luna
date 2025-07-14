namespace DiaLuna;

using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

public class SortedCoordSet<T> : ICollection<CoordValuePair<T>>
	where T : notnull
{
	private readonly List<CoordValuePair<T>> _values = [];

	public int Count => _values.Count;

	public bool IsReadOnly => false;

	/// <summary>
	/// Gets or sets the value associated with the specified key.
	/// </summary>
	/// <param name="key"></param>
	/// <exception cref="KeyNotFoundException"></exception>
	public T this[Vector2I key]
	{
		get
		{
			var idx = FindIndexByKey(key);
			if (idx >= 0)
			{
				return _values[idx].Value;
			}

			throw new KeyNotFoundException($"Key {key} not found.");
		}
		set
		{
			var idx = FindIndexByKey(key);
			var pair = new CoordValuePair<T>(key, value);
			if (idx >= 0)
			{
				_values[idx] = pair;
			}
			else
			{
				_values.Insert(~idx, pair);
			}
		}
	}

	/// <summary>
	/// Adds or replaces the value for the given key.
	/// </summary>
	/// <param name="key"></param>
	/// <param name="value"></param>
	public void Add(Vector2I key, T value)
	{
		var pair = new CoordValuePair<T>(key, value);
		var idx = FindIndexByKey(key);
		if (idx >= 0)
		{
			_values[idx] = pair;
		}
		else
		{
			_values.Insert(~idx, pair);
		}
	}

	/// <summary>
	/// Adds a CoordValuePair. If key exists, replaces value.
	/// </summary>
	/// <param name="item"></param>
	public void Add(CoordValuePair<T> item)
	{
		var idx = FindIndexByKey(item.Key);
		if (idx >= 0)
		{
			_values[idx] = item;
		}
		else
		{
			_values.Insert(~idx, item);
		}
	}

	public bool Remove(Vector2I key)
	{
		var idx = FindIndexByKey(key);
		if (idx >= 0)
		{
			_values.RemoveAt(idx);
			return true;
		}
		return false;
	}

	public bool Remove(CoordValuePair<T> item)
	{
		var idx = FindIndexByKey(item.Key);
		if (idx >= 0)
		{
			_values.RemoveAt(idx);
			return true;
		}
		return false;
	}

	public bool ContainsKey(Vector2I key) => FindIndexByKey(key) >= 0;

	public bool TryGetValue(Vector2I key, out T value)
	{
		var idx = FindIndexByKey(key);
		if (idx >= 0)
		{
			value = _values[idx].Value;
			return true;
		}
		value = default!;
		return false;
	}

	public bool Contains(CoordValuePair<T> item)
	{
		var idx = FindIndexByKey(item.Key);
		return idx >= 0;
	}

	public void Clear() => _values.Clear();

	public void CopyTo(CoordValuePair<T>[] array, int arrayIndex) =>
		_values.CopyTo(array, arrayIndex);

	public IEnumerator<CoordValuePair<T>> GetEnumerator() =>
		_values.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	private int FindIndexByKey(Vector2I key)
	{
		// Use a dummy value for binary search
		var dummy = new CoordValuePair<T>(key, default!);
		return _values.BinarySearch(dummy);
	}
}

public readonly struct CoordValuePair<T>(Vector2I key, T value)
	: IComparable<CoordValuePair<T>>,
		IComparable
	where T : notnull
{
	public readonly Vector2I Key { get; } = key;
	public readonly T Value { get; } = value;

	public void Deconstruct(out Vector2I key, out T value)
	{
		key = Key;
		value = Value;
	}

	public override string ToString()
	{
		return $"CoordValuePair({Key}, {Value})";
	}

	public int CompareTo(object? obj)
	{
		if (obj == null)
		{
			return 1;
		}

		if (obj is CoordValuePair<T> x)
		{
			return CompareTo(x);
		}

		throw new ArgumentException("", nameof(obj));
	}

	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return false;
		}

		if (obj is CoordValuePair<T> x)
		{
			return Key == x.Key;
		}

		throw new ArgumentException("", nameof(obj));
	}

	public override int GetHashCode() => Key.GetHashCode();

	/// <summary>
	/// Sort ascending X
	/// then ascending Y
	/// </summary>
	/// <param name="other"></param>
	public int CompareTo(CoordValuePair<T> other)
	{
		var cmp = Key.X.CompareTo(other.Key.X);
		if (cmp != 0)
		{
			return cmp;
		}

		return Key.Y.CompareTo(other.Key.Y);
	}

	public static bool operator ==(
		CoordValuePair<T> left,
		CoordValuePair<T> right
	) => left.Equals(right);

	public static bool operator !=(
		CoordValuePair<T> left,
		CoordValuePair<T> right
	) => !(left == right);

	public static bool operator <(
		CoordValuePair<T> left,
		CoordValuePair<T> right
	) => left.CompareTo(right) < 0;

	public static bool operator <=(
		CoordValuePair<T> left,
		CoordValuePair<T> right
	) => left.CompareTo(right) <= 0;

	public static bool operator >(
		CoordValuePair<T> left,
		CoordValuePair<T> right
	) => left.CompareTo(right) > 0;

	public static bool operator >=(
		CoordValuePair<T> left,
		CoordValuePair<T> right
	) => left.CompareTo(right) >= 0;
}
