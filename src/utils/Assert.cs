namespace DiaLuna;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

public class AssertException : Exception
{
	public AssertException(
		string? memberName = null,
		string? fileName = null,
		int? lineNumber = null
	)
		: base(FormatMessage(null, memberName, fileName, lineNumber)) { }

	public AssertException(
		string message,
		string? memberName = null,
		string? fileName = null,
		int? lineNumber = null
	)
		: base(FormatMessage(message, memberName, fileName, lineNumber)) { }

	public AssertException(
		string message,
		Exception inner,
		string? memberName = null,
		string? fileName = null,
		int? lineNumber = null
	)
		: base(FormatMessage(message, memberName, fileName, lineNumber), inner)
	{ }

	public AssertException()
		: base(FormatMessage()) { }

	public AssertException(string? message)
		: base(FormatMessage(message)) { }

	public AssertException(string? message, Exception? innerException)
		: base(FormatMessage(message), innerException) { }

	private static string FormatMessage(
		string? message = null,
		string? memberName = null,
		string? fileName = null,
		int? lineNumber = null
	)
	{
		var sb = new StringBuilder("Assertion Failed: ");
		if (message != null)
		{
			sb.AppendLine(message);
		}
		if (memberName != null)
		{
			sb.Append(" in ").Append(memberName);
		}
		if (fileName != null)
		{
			sb.Append('[').Append(fileName);
			if (lineNumber != null)
			{
				sb.Append(':').Append(lineNumber);
			}
			sb.AppendLine("]");
		}
		return sb.ToString();
	}
}

public static partial class Util
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Conditional("DEBUG")]
	[StackTraceHidden]
	internal static void Assert(
		[DoesNotReturnIf(false)] bool condition,
		[CallerLineNumber] int? lineNumber = null,
		[CallerMemberName] string? memberName = null,
		[CallerFilePath] string? filePath = null
	)
	{
		if (condition)
		{
			return;
		}

		throw new AssertException(memberName, filePath, lineNumber);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Conditional("DEBUG")]
	[StackTraceHidden]
	internal static void Assert(
		[DoesNotReturnIf(false)] bool condition,
		string message,
		[CallerLineNumber] int? lineNumber = null,
		[CallerMemberName] string? memberName = null,
		[CallerFilePath] string? filePath = null
	)
	{
		if (condition)
		{
			return;
		}

		throw new AssertException(message, memberName, filePath, lineNumber);
	}
}
