﻿using Microsoft.CodeAnalysis;
using Rocks.Extensions;
using System.CodeDom.Compiler;
using System.Linq;

namespace Rocks.Builders
{
	internal static class MockMethodValueBuilder
	{
		internal static void Build(IndentedTextWriter writer, MethodMockableResult result, bool raiseEvents)
		{
			var method = result.Value;
			var returnType = method.ReturnType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
			var explicitTypeName = result.RequiresExplicitInterfaceImplementation == RequiresExplicitInterfaceImplementation.No ?
				string.Empty : $"{result.Value.ContainingType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}.";
			var methodSignature =
				$"{returnType} {explicitTypeName}{method.Name}({string.Join(", ", method.Parameters.Select(_ => $"{_.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)} {_.Name}"))})";
			var methodException =
				$"{returnType} {method.Name}({string.Join(", ", method.Parameters.Select(_ => $"{{{_.Name}}}"))})";
			var methodDeclarationBeginning =
				result.RequiresExplicitInterfaceImplementation == RequiresExplicitInterfaceImplementation.No ?
					$"public {(result.RequiresOverride == RequiresOverride.Yes ? "override " : string.Empty)}" : 
					string.Empty;

			writer.WriteLine($@"[MemberIdentifier({result.MemberIdentifier}, ""{methodSignature}"")]");
			writer.WriteLine($"{methodDeclarationBeginning}{methodSignature}");
			writer.WriteLine("{");
			writer.Indent++;

			writer.WriteLine($"if (this.handlers.TryGetValue({result.MemberIdentifier}, out var methodHandlers))");
			writer.WriteLine("{");
			writer.Indent++;

			if (method.Parameters.Length > 0)
			{
				MockMethodValueBuilder.BuildMethodValidationHandlerWithParameters(writer, method, raiseEvents);
			}
			else
			{
				MockMethodValueBuilder.BuildMethodValidationHandlerNoParameters(writer, method, raiseEvents);
			}

			writer.Indent--;
			writer.WriteLine("}");
			writer.WriteLine();
			writer.WriteLine($@"throw new ExpectationException({(method.ReturnsVoid ? string.Empty : "$")}""No handlers were found for {methodException})"");");

			writer.Indent--;
			writer.WriteLine("}");
			writer.WriteLine();
		}

		private static void BuildMethodHandler(IndentedTextWriter writer, IMethodSymbol method, bool raiseEvents)
		{
			writer.WriteLine("var result = methodHandler.Method is not null ?");
			writer.Indent++;

			var methodCast = method.Parameters.Length == 0 ? $"(Func<{method.ReturnType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}>)" :
				$"(Func<{string.Join(", ", method.Parameters.Select(_ => _.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)).Concat(new [] { method.ReturnType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) }))}>)";
			var methodArguments = method.Parameters.Length == 0 ? string.Empty :
				string.Join(", ", method.Parameters.Select(_ => _.Name));
			writer.WriteLine($"({methodCast}methodHandler.Method)({methodArguments}) :");
			writer.WriteLine($"((HandlerInformation<{method.ReturnType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}>)methodHandler).ReturnValue;");

			writer.Indent--;

			if(raiseEvents)
			{
				writer.WriteLine("methodHandler.RaiseEvents(this);");
			}

			writer.WriteLine("methodHandler.IncrementCallCount();");
			writer.WriteLine("return result!;");
		}

		private static void BuildMethodValidationHandlerWithParameters(IndentedTextWriter writer, IMethodSymbol method, bool raiseEvents)
		{
			writer.WriteLine("foreach (var methodHandler in methodHandlers)");
			writer.WriteLine("{");
			writer.Indent++;

			for (var i = 0; i < method.Parameters.Length; i++)
			{
				var parameter = method.Parameters[i];

				if (i == 0)
				{
					writer.WriteLine(
						$"if (((Arg<{parameter.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}>)methodHandler.Expectations[{i}]).IsValid({parameter.Name}){(i == method.Parameters.Length - 1 ? ")" : " &&")}");
				}
				else
				{
					if (i == 1)
					{
						writer.Indent++;
					}

					writer.WriteLine(
						$"((Arg<{parameter.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}>)methodHandler.Expectations[{i}]).IsValid({parameter.Name}){(i == method.Parameters.Length - 1 ? ")" : " &&")}");

					if (i == method.Parameters.Length - 1)
					{
						writer.Indent--;
					}
				}
			}

			writer.WriteLine("{");
			writer.Indent++;

			MockMethodValueBuilder.BuildMethodHandler(writer, method, raiseEvents);
			writer.Indent--;
			writer.WriteLine("}");

			writer.Indent--;
			writer.WriteLine("}");
		}

		private static void BuildMethodValidationHandlerNoParameters(IndentedTextWriter writer, IMethodSymbol method, bool raiseEvents)
		{
			writer.WriteLine("var methodHandler = methodHandlers[0];");
			MockMethodValueBuilder.BuildMethodHandler(writer, method, raiseEvents);
		}
	}
}