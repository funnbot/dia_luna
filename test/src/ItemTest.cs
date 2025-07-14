namespace DiaLuna;

using Chickensoft.GoDotTest;
using Godot;
using Shouldly;

public class ItemTest(Node testScene) : TestClass(testScene)
{
	[Test]
	public void TestAddToCount()
	{
		var count = 1;
		var maximum = 10;
		var toAdd = 4;

		ItemStackUtility
			.MergeCount(
				count,
				toAdd,
				maximum,
				out var result,
				out var remainder
			)
			.ShouldBe(true);
		result.ShouldBe(5);
		remainder.ShouldBe(0);
	}

	[Test]
	public void AddToCount_Overflow()
	{
		var count = 8;
		var maximum = 10;
		var toAdd = 5;

		ItemStackUtility
			.MergeCount(
				count,
				toAdd,
				maximum,
				out var result,
				out var remainder
			)
			.ShouldBe(true);
		result.ShouldBe(10);
		remainder.ShouldBe(3);
	}

	[Test]
	public void AddToCount_AtMaximum()
	{
		var count = 10;
		var maximum = 10;
		var toAdd = 2;

		ItemStackUtility
			.MergeCount(
				count,
				toAdd,
				maximum,
				out var result,
				out var remainder
			)
			.ShouldBe(false);
		result.ShouldBe(10);
		remainder.ShouldBe(2);
	}

	[Test]
	public void AddToCount_AtMinimum()
	{
		var count = 0;
		var maximum = 0;
		var toAdd = -2;

		ItemStackUtility
			.MergeCount(
				count,
				toAdd,
				maximum,
				out var result,
				out var remainder
			)
			.ShouldBe(false);
		result.ShouldBe(0);
		remainder.ShouldBe(-2);
	}

	[Test]
	public void AddToCount_ZeroToAdd()
	{
		var count = 5;
		var maximum = 10;
		var toAdd = 0;

		ItemStackUtility
			.MergeCount(
				count,
				toAdd,
				maximum,
				out var result,
				out var remainder
			)
			.ShouldBe(false);
		result.ShouldBe(5);
		remainder.ShouldBe(0);
	}

	[Test]
	public void AddToCount_ZeroCurrent()
	{
		var count = 0;
		var maximum = 10;
		var toAdd = 5;

		ItemStackUtility
			.MergeCount(
				count,
				toAdd,
				maximum,
				out var result,
				out var remainder
			)
			.ShouldBe(true);
		result.ShouldBe(5);
		remainder.ShouldBe(0);
	}

	[Test]
	public void AddToCount_NegativeResult()
	{
		var count = 2;
		var maximum = 10;
		var toAdd = -5;

		ItemStackUtility
			.MergeCount(
				count,
				toAdd,
				maximum,
				out var result,
				out var remainder
			)
			.ShouldBe(true);
		result.ShouldBe(0);
		remainder.ShouldBe(-3);
	}
}
