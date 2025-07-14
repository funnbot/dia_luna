namespace SourceGenerators;

using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

[Generator(LanguageNames.CSharp)]
public class InputMapGenerator : IIncrementalGenerator
{
	public static List<string> Logs { get; } = [];

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		IncrementalValuesProvider<AdditionalText> additionalTexts =
			context.AdditionalTextsProvider.Where(
				static (file) => file.Path.EndsWith("project.godot")
			);

		IncrementalValuesProvider<string> additionalTextsContent = additionalTexts.Select(
			static (additionalText, cancellationToken) =>
				additionalText.GetText(cancellationToken)!.ToString()
		);

		IncrementalValuesProvider<string> inputActions = additionalTextsContent.SelectMany(
			static (additionalTextContent, _) =>
				additionalTextContent
					.Split('\n')
					.Where(static (line) => !string.IsNullOrWhiteSpace(line))
					.SkipWhile(static (line) => !line.StartsWith("[input]"))
					.Skip(1)
					.TakeWhile(static (line) => !line.StartsWith("["))
					.Where(static (line) => line.IndexOf('=') > 0)
					.Select(static (line) => line.Split('=')[0])
		);

		IncrementalValueProvider<ImmutableArray<string>> collected = inputActions.Collect();

		context.RegisterSourceOutput(collected, CreateSource);
	}

	private static void CreateSource(
		SourceProductionContext context,
		ImmutableArray<string> inputActions
	)
	{
		var source = new StringBuilder();
		_ = source
			.AppendLine("/*\n   This file was generated.\n   Do not edit.\n*/")
			.AppendLine()
			.AppendLine("using Godot;")
			.AppendLine()
			.AppendLine("namespace SourceGenerators;")
			.AppendLine()
			.AppendLine("public static class InputMap")
			.AppendLine("{");

		foreach (var inputAction in inputActions)
		{
			var name = InputActionToConstant(inputAction);
			var value = $"\"{inputAction.Replace("\"", "")}\"";
			_ = source.AppendLine($"    public static readonly StringName {name} = {value};");
		}

		_ = source.AppendLine("}");

		context.AddSource("InputMap.g.cs", source.ToString());
	}

	private static string InputActionToConstant(string inputAction)
	{
		// Specific naming conventions can be implemented here.
		// Ours is: PascalCase, with specific edge cases (like ui should be translated as UI).
		return string.Join(
			"",
			inputAction
				.Replace('.', '_')
				.Split('_')
				.Where(static word => word.Length > 0)
				.Select(
					static (word) =>
						word.ToLowerInvariant() switch
						{
							"ui" => "UI",
							_ => $"{char.ToUpperInvariant(word[0])}{word.Substring(1)}",
						}
				)
		);
	}
}
