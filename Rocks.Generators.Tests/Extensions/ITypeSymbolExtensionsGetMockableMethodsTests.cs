﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using Rocks.Extensions;
using System;
using System.Linq;

namespace Rocks.Tests.Extensions
{
	public static class ITypeSymbolExtensionsGetMockableMethodsTests
	{
		[Test]
		public static void GetMockableMethodsWithInterfaceMethods()
		{
			const string targetMethodName = "Foo";
			const string targetTypeName = "ITest";

			var code =
$@"public interface {targetTypeName}
{{
	void {targetMethodName}();
}}";

			var (typeSymbol, compilation) = ITypeSymbolExtensionsGetMockableMethodsTests.GetTypeSymbol(code, targetTypeName);
			var methods = typeSymbol.GetMockableMethods(typeSymbol.ContainingAssembly, compilation);

			Assert.Multiple(() =>
			{
				Assert.That(methods.Length, Is.EqualTo(1));
				var fooMethod = methods.Single(_ => _.Value.Name == targetMethodName);
				Assert.That(fooMethod.RequiresExplicitInterfaceImplementation, Is.EqualTo(RequiresExplicitInterfaceImplementation.No));
				Assert.That(fooMethod.RequiresOverride, Is.EqualTo(RequiresOverride.No));
			});
		}

		[Test]
		public static void GetMockableMethodsWhenInterfaceHasBaseInterface()
		{
			const string baseMethodName = "Foo";
			const string targetMethodName = "Bar";
			const string targetTypeName = "Target";
			var code =
$@"public interface Base
{{
	void {baseMethodName}();
}}

public interface {targetTypeName} 
	: Base
{{ 
	void {targetMethodName}();
}}";

			var (typeSymbol, compilation) = ITypeSymbolExtensionsGetMockableMethodsTests.GetTypeSymbol(code, targetTypeName);
			var methods = typeSymbol.GetMockableMethods(typeSymbol.ContainingAssembly, compilation);

			Assert.Multiple(() =>
			{
				Assert.That(methods.Length, Is.EqualTo(2));
				var baseMethod = methods.Single(_ => _.Value.Name == baseMethodName);
				Assert.That(baseMethod.RequiresExplicitInterfaceImplementation, Is.EqualTo(RequiresExplicitInterfaceImplementation.No));
				Assert.That(baseMethod.RequiresOverride, Is.EqualTo(RequiresOverride.No));
				var targetMethod = methods.Single(_ => _.Value.Name == targetMethodName);
				Assert.That(targetMethod.RequiresExplicitInterfaceImplementation, Is.EqualTo(RequiresExplicitInterfaceImplementation.No));
				Assert.That(targetMethod.RequiresOverride, Is.EqualTo(RequiresOverride.No));
			});
		}

		[Test]
		public static void GetMockableMethodsWhenInterfaceHasBaseInterfaceWithMatchingMethod()
		{
			const string baseTypeName = "Base";
			const string targetMethodName = "Bar";
			const string targetTypeName = "Target";
			var code =
$@"public interface {baseTypeName}
{{
	void {targetMethodName}();
}}

public interface {targetTypeName} 
	: Base
{{ 
	void {targetMethodName}();
}}";

			var (typeSymbol, compilation) = ITypeSymbolExtensionsGetMockableMethodsTests.GetTypeSymbol(code, targetTypeName);
			var methods = typeSymbol.GetMockableMethods(typeSymbol.ContainingAssembly, compilation);

			Assert.Multiple(() =>
			{
				Assert.That(methods.Length, Is.EqualTo(2));
				var baseMethod = methods.Single(_ => _.Value.Name == targetMethodName && _.Value.ContainingType.Name == baseTypeName);
				Assert.That(baseMethod.RequiresExplicitInterfaceImplementation, Is.EqualTo(RequiresExplicitInterfaceImplementation.Yes));
				Assert.That(baseMethod.RequiresOverride, Is.EqualTo(RequiresOverride.No));
				var targetMethod = methods.Single(_ => _.Value.Name == targetMethodName && _.Value.ContainingType.Name == targetTypeName);
				Assert.That(targetMethod.RequiresExplicitInterfaceImplementation, Is.EqualTo(RequiresExplicitInterfaceImplementation.No));
				Assert.That(targetMethod.RequiresOverride, Is.EqualTo(RequiresOverride.No));
			});
		}

		[Test]
		public static void GetMockableMethodsWhenInterfaceHasBaseInterfacesWithMatchingMethods()
		{
			const string baseOneTypeName = "BaseOne";
			const string baseTwoTypeName = "BaseTwo";
			const string baseMethodName = "Foo";
			const string targetMethodName = "Bar";
			const string targetTypeName = "Target";

			var code =
$@"public interface {baseOneTypeName}
{{
	void {baseMethodName}();
}}

public interface {baseTwoTypeName}
{{
	void {baseMethodName}();
}}

public interface {targetTypeName} 
	: {baseOneTypeName}, {baseTwoTypeName}
{{ 
	void {targetMethodName}();
}}";

			var (typeSymbol, compilation) = ITypeSymbolExtensionsGetMockableMethodsTests.GetTypeSymbol(code, targetTypeName);
			var methods = typeSymbol.GetMockableMethods(typeSymbol.ContainingAssembly, compilation);

			Assert.Multiple(() =>
			{
				Assert.That(methods.Length, Is.EqualTo(3));
				var baseOneMethod = methods.Single(_ => _.Value.Name == baseMethodName && _.Value.ContainingType.Name == baseOneTypeName);
				Assert.That(baseOneMethod.RequiresExplicitInterfaceImplementation, Is.EqualTo(RequiresExplicitInterfaceImplementation.Yes));
				Assert.That(baseOneMethod.RequiresOverride, Is.EqualTo(RequiresOverride.No));
				var baseTwoMethod = methods.Single(_ => _.Value.Name == baseMethodName && _.Value.ContainingType.Name == baseTwoTypeName);
				Assert.That(baseTwoMethod.RequiresExplicitInterfaceImplementation, Is.EqualTo(RequiresExplicitInterfaceImplementation.Yes));
				Assert.That(baseTwoMethod.RequiresOverride, Is.EqualTo(RequiresOverride.No));
				var targetMethod = methods.Single(_ => _.Value.Name == targetMethodName);
				Assert.That(targetMethod.RequiresExplicitInterfaceImplementation, Is.EqualTo(RequiresExplicitInterfaceImplementation.No));
				Assert.That(targetMethod.RequiresOverride, Is.EqualTo(RequiresOverride.No));
			});
		}

		[Test]
		public static void GetMockableMethodsWithClassMethod()
		{
			const string targetTypeName = "Test";
			const string targetMethodName = "Foo";

			var code =
$@"public class {targetTypeName}
{{
	public virtual void {targetMethodName}() {{ }}
}}";

			var (typeSymbol, compilation) = ITypeSymbolExtensionsGetMockableMethodsTests.GetTypeSymbol(code, targetTypeName);
			var methods = typeSymbol.GetMockableMethods(typeSymbol.ContainingAssembly, compilation);

			Assert.Multiple(() =>
			{
				Assert.That(methods.Length, Is.EqualTo(4));
				var getHashCodeMethod = methods.Single(_ => _.Value.Name == nameof(object.GetHashCode));
				Assert.That(getHashCodeMethod.RequiresExplicitInterfaceImplementation, Is.EqualTo(RequiresExplicitInterfaceImplementation.No));
				Assert.That(getHashCodeMethod.RequiresOverride, Is.EqualTo(RequiresOverride.Yes));
				var equalsMethod = methods.Single(_ => _.Value.Name == nameof(object.Equals));
				Assert.That(equalsMethod.RequiresExplicitInterfaceImplementation, Is.EqualTo(RequiresExplicitInterfaceImplementation.No));
				Assert.That(equalsMethod.RequiresOverride, Is.EqualTo(RequiresOverride.Yes));
				var toStringMethod = methods.Single(_ => _.Value.Name == nameof(object.ToString));
				Assert.That(toStringMethod.RequiresExplicitInterfaceImplementation, Is.EqualTo(RequiresExplicitInterfaceImplementation.No));
				Assert.That(toStringMethod.RequiresOverride, Is.EqualTo(RequiresOverride.Yes));
				var fooMethod = methods.Single(_ => _.Value.Name == targetMethodName);
				Assert.That(fooMethod.RequiresExplicitInterfaceImplementation, Is.EqualTo(RequiresExplicitInterfaceImplementation.No));
				Assert.That(fooMethod.RequiresOverride, Is.EqualTo(RequiresOverride.Yes));
			});
		}

		[Test]
		public static void GetMockableMethodsWithClassNoMethods()
		{
			const string targetTypeName = "Test";

			var code = $"public class {targetTypeName} {{ }}";

			var (typeSymbol, compilation) = ITypeSymbolExtensionsGetMockableMethodsTests.GetTypeSymbol(code, targetTypeName);
			var methods = typeSymbol.GetMockableMethods(typeSymbol.ContainingAssembly, compilation);

			Assert.Multiple(() =>
			{
				Assert.That(methods.Length, Is.EqualTo(3));
				var getHashCodeMethod = methods.Single(_ => _.Value.Name == nameof(object.GetHashCode));
				Assert.That(getHashCodeMethod.RequiresExplicitInterfaceImplementation, Is.EqualTo(RequiresExplicitInterfaceImplementation.No));
				Assert.That(getHashCodeMethod.RequiresOverride, Is.EqualTo(RequiresOverride.Yes));
				var equalsMethod = methods.Single(_ => _.Value.Name == nameof(object.Equals));
				Assert.That(equalsMethod.RequiresExplicitInterfaceImplementation, Is.EqualTo(RequiresExplicitInterfaceImplementation.No));
				Assert.That(equalsMethod.RequiresOverride, Is.EqualTo(RequiresOverride.Yes));
				var toStringMethod = methods.Single(_ => _.Value.Name == nameof(object.ToString));
				Assert.That(toStringMethod.RequiresExplicitInterfaceImplementation, Is.EqualTo(RequiresExplicitInterfaceImplementation.No));
				Assert.That(toStringMethod.RequiresOverride, Is.EqualTo(RequiresOverride.Yes));
			});
		}

		private static (ITypeSymbol, Compilation) GetTypeSymbol(string source, string targetTypeName)
		{
			var syntaxTree = CSharpSyntaxTree.ParseText(source);
			var references = AppDomain.CurrentDomain.GetAssemblies()
				.Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
				.Select(_ => MetadataReference.CreateFromFile(_.Location));
			var compilation = CSharpCompilation.Create("generator", new SyntaxTree[] { syntaxTree },
				references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
			var model = compilation.GetSemanticModel(syntaxTree, true);

			var typeSyntax = syntaxTree.GetRoot().DescendantNodes(_ => true)
				.OfType<TypeDeclarationSyntax>().Single(_ => _.Identifier.Text == targetTypeName);
			return (model.GetDeclaredSymbol(typeSyntax)!, compilation);
		}
	}
}