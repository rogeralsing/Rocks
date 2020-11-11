﻿using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Rocks.Builders
{
	internal static class MockCreateBuilder
	{
		/*
		private sealed class RockIMockable
			: IMockable, IMock
		{
			private readonly ImmutableDictionary<int, ImmutableArray<HandlerInformation>> handlers;

			public RockIMockable(Expectations<IMockable> expectations) => 
				this.handlers = expectations.CreateHandlers();

			[MemberIdentifier(0, "Foo(int a)")]
			public void Foo(int a)
			{
				if (this.handlers.TryGetValue(0, out var methodHandlers))
				{
					var foundMatch = false;

					foreach (var methodHandler in methodHandlers)
					{
						if (((Arg<int>)methodHandler.Expectations[0]).IsValid(a))
						{
							foundMatch = true;

							if (methodHandler.Method != null)
							{
#pragma warning disable CS8604
								((Action<int>)methodHandler.Method)(a);
#pragma warning restore CS8604
							}

							methodHandler.IncrementCallCount();
							break;
						}
					}

					if (!foundMatch)
					{
						throw new ExpectationException($"No handlers were found for Foo({a})");
					}
				}
				else
				{
					throw new ExpectationException($"No handlers were found for Foo({a})");
				}
			}

			ImmutableDictionary<int, ImmutableArray<HandlerInformation>> IMock.Handlers => this.handlers;
		}
		
		*/
		internal static void Build(IndentedTextWriter writer, MockInformation information, SortedSet<string> usings)
		{
			var typeToMockName = information.TypeToMock.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

			writer.WriteLine($"private sealed class Rock{typeToMockName}");
			writer.Indent++;
			writer.WriteLine($": {typeToMockName}, {(information.Events.Length > 0 ? nameof(IMockWithEvents) : nameof(IMock))}");
			writer.Indent--;

			writer.WriteLine("{");
			writer.Indent++;

			writer.WriteLine("private readonly ImmutableDictionary<int, ImmutableArray<HandlerInformation>> handlers;");
			writer.WriteLine();

			if (information.Constructors.Length > 0)
			{
				foreach (var constructor in information.Constructors)
				{
					MockConstructorBuilder.Build(writer, information.TypeToMock, constructor.Parameters);
				}
			}
			else
			{
				MockConstructorBuilder.Build(writer, information.TypeToMock, ImmutableArray<IParameterSymbol>.Empty);
			}

			writer.WriteLine();

			var memberIdentifier = 0u;

			foreach(var result in information.Methods)
			{
				if(result.Value.ReturnsVoid)
				{
					MockMethodVoidBuilder.Build(writer, result, information.Events.Length > 0);
				}
				else
				{
					MockMethodValueBuilder.Build(writer, result, information.Events.Length > 0);
				}

				memberIdentifier++;
			}

			writer.WriteLine("// TODO: Put in all the member overrides...");

			if(information.Events.Length > 0)
			{
				writer.WriteLine();
				MockEventsBuilder.Build(writer, information.Events, usings);
			}

			writer.WriteLine();
			writer.WriteLine("ImmutableDictionary<int, ImmutableArray<HandlerInformation>> IMock.Handlers => this.handlers;");
			writer.Indent--;
			writer.WriteLine("}");
		}
	}
}