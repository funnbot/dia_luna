namespace DiaLuna;

using Chickensoft.GoDotTest;
using Godot;
using Shouldly;

public class ItemTest(Node testScene) : TestClass(testScene)
{
    [Test]
    public void TestAddToCount()
    {
        const int count = 1;
        const int maximum = 10;
        const int toAdd = 4;

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
        const int count = 8;
        const int maximum = 10;
        const int toAdd = 5;

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
        const int count = 10;
        const int maximum = 10;
        const int toAdd = 2;

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
        const int count = 0;
        const int maximum = 0;
        const int toAdd = -2;

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
        const int count = 5;
        const int maximum = 10;
        const int toAdd = 0;

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
        const int count = 0;
        const int maximum = 10;
        const int toAdd = 5;

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
        const int count = 2;
        const int maximum = 10;
        const int toAdd = -5;

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
