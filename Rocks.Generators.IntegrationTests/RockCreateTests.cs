﻿using NUnit.Framework;
using System;

namespace Rocks.IntegrationTests
{
	public static class RockCreateTests
	{
		[Test]
		public static void CreateHappyPathForInterface()
		{
			var rock = Rock.Create<IFoo>();
			//rock.Methods().Foo(Arg.Is(3), Arg.Is("b"));
			rock.Methods().Foo(3, "b");
			rock.Methods().Bar(Arg.Validate<int>(_ => _ == 3), Arg.Is("b"));
			rock.Methods().Baz();
			rock.Methods().Nulling(Arg.Is<int?>(null), Arg.Is("b"), Arg.Is<string?>(null));

			var chunk = rock.Instance();
			chunk.Foo(3, "b");
			chunk.Bar(3, "b");
			chunk.Baz();
			chunk.Nulling(null, "b", null);

			rock.Verify();
		}

		[Test]
		public static void CreateHappyPathForClass()
		{
			var rock = Rock.Create<FooClass>();
			rock.Methods().Foo(Arg.Is(3), Arg.Is("b"));
			rock.Methods().Bar(Arg.Is(3), Arg.Is("b"));
			rock.Methods().Baz();
			rock.Methods().Nulling(Arg.Is<int?>(null), Arg.Is("b"), Arg.Is<string?>(null));

			var chunk = rock.Instance();
			chunk.Foo(3, "b");
			chunk.Bar(3, "b");
			chunk.Baz();
			chunk.Nulling(null, "b", null);

			rock.Verify();
		}
	}

	public interface IFoo
	{
		string? Nulling(int? a, string b, string? c);
		string Bar(int a, string b);
		void Foo(int a, string b);
		void Baz();
	}

	public class FooClass
	{
		public virtual string? Nulling(int? a, string b, string? c) => $"{a}, {b}, {c}";
		public virtual string Bar(int a, string b) => $"{a}, {b}";
		public virtual void Foo(int a, string b) { }
		public virtual void Baz() { }
#pragma warning disable CA1822 // Mark members as static
		public void DoNotMockMe() { }
#pragma warning restore CA1822 // Mark members as static
	}
}