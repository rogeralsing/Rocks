﻿using NUnit.Framework;
using System;

namespace Rocks.Tests
{
	public static class HandleAction12ArgumentTests
	{
		[Test]
		public static void Make()
		{
			var rock = Rock.Create<IHandleAction12ArgumentTests>();
			rock.Handle(_ => _.Target(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12));

			var chunk = rock.Make();
			chunk.Target(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);

			rock.Verify();
		}

		[Test]
		public static void MakeAndRaiseEvent()
		{
			var rock = Rock.Create<IHandleAction12ArgumentTests>();
			rock.Handle(_ => _.Target(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12))
				.Raises(nameof(IHandleAction12ArgumentTests.TargetEvent), EventArgs.Empty);

			var wasEventRaised = false;
			var chunk = rock.Make();
			chunk.TargetEvent += (s, e) => wasEventRaised = true;
			chunk.Target(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);

			Assert.That(wasEventRaised, Is.True);
			rock.Verify();
		}

		[Test]
		public static void MakeWithHandler()
		{
			var argumentA = 0;
			var argumentB = 0;
			var argumentC = 0;
			var argumentD = 0;
			var argumentE = 0;
			var argumentF = 0;
			var argumentG = 0;
			var argumentH = 0;
			var argumentI = 0;
			var argumentJ = 0;
			var argumentK = 0;
			var argumentL = 0;

			var rock = Rock.Create<IHandleAction12ArgumentTests>();
			rock.Handle<int, int, int, int, int, int, int, int, int, int, int, int>(_ => _.Target(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12),
				(a, b, c, d, e, f, g, h, i, j, k, l) => { argumentA = a; argumentB = b; argumentC = c; argumentD = d; argumentE = e; argumentF = f; argumentG = g; argumentH = h; argumentI = i; argumentJ = j; argumentK = k; argumentL = l; });

			var chunk = rock.Make();
			chunk.Target(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);
			Assert.That(argumentA, Is.EqualTo(1), nameof(argumentA));
			Assert.That(argumentB, Is.EqualTo(2), nameof(argumentB));
			Assert.That(argumentC, Is.EqualTo(3), nameof(argumentC));
			Assert.That(argumentD, Is.EqualTo(4), nameof(argumentD));
			Assert.That(argumentE, Is.EqualTo(5), nameof(argumentE));
			Assert.That(argumentF, Is.EqualTo(6), nameof(argumentF));
			Assert.That(argumentG, Is.EqualTo(7), nameof(argumentG));
			Assert.That(argumentH, Is.EqualTo(8), nameof(argumentH));
			Assert.That(argumentI, Is.EqualTo(9), nameof(argumentI));
			Assert.That(argumentJ, Is.EqualTo(10), nameof(argumentJ));
			Assert.That(argumentK, Is.EqualTo(11), nameof(argumentK));
			Assert.That(argumentL, Is.EqualTo(12), nameof(argumentL));

			rock.Verify();
		}

		[Test]
		public static void MakeWithExpectedCallCount()
		{
			var rock = Rock.Create<IHandleAction12ArgumentTests>();
			rock.Handle(_ => _.Target(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12), 2);

			var chunk = rock.Make();
			chunk.Target(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);
			chunk.Target(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);

			rock.Verify();
		}

		[Test]
		public static void MakeWithHandlerAndExpectedCallCount()
		{
			var argumentA = 0;
			var argumentB = 0;
			var argumentC = 0;
			var argumentD = 0;
			var argumentE = 0;
			var argumentF = 0;
			var argumentG = 0;
			var argumentH = 0;
			var argumentI = 0;
			var argumentJ = 0;
			var argumentK = 0;
			var argumentL = 0;

			var rock = Rock.Create<IHandleAction12ArgumentTests>();
			rock.Handle<int, int, int, int, int, int, int, int, int, int, int, int>(_ => _.Target(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12),
				(a, b, c, d, e, f, g, h, i, j, k, l) => { argumentA = a; argumentB = b; argumentC = c; argumentD = d; argumentE = e; argumentF = f; argumentG = g; argumentH = h; argumentI = i; argumentJ = j; argumentK = k; argumentL = l; }, 2);

			var chunk = rock.Make();
			chunk.Target(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);
			Assert.That(argumentA, Is.EqualTo(1), nameof(argumentA));
			Assert.That(argumentB, Is.EqualTo(2), nameof(argumentB));
			Assert.That(argumentC, Is.EqualTo(3), nameof(argumentC));
			Assert.That(argumentD, Is.EqualTo(4), nameof(argumentD));
			Assert.That(argumentE, Is.EqualTo(5), nameof(argumentE));
			Assert.That(argumentF, Is.EqualTo(6), nameof(argumentF));
			Assert.That(argumentG, Is.EqualTo(7), nameof(argumentG));
			Assert.That(argumentH, Is.EqualTo(8), nameof(argumentH));
			Assert.That(argumentI, Is.EqualTo(9), nameof(argumentI));
			Assert.That(argumentJ, Is.EqualTo(10), nameof(argumentJ));
			Assert.That(argumentK, Is.EqualTo(11), nameof(argumentK));
			Assert.That(argumentL, Is.EqualTo(12), nameof(argumentL));
			argumentA = 0;
			argumentB = 0;
			argumentC = 0;
			argumentD = 0;
			argumentE = 0;
			argumentF = 0;
			argumentG = 0;
			argumentH = 0;
			argumentI = 0;
			argumentJ = 0;
			argumentK = 0;
			argumentL = 0;
			chunk.Target(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);
			Assert.That(argumentA, Is.EqualTo(1), nameof(argumentA));
			Assert.That(argumentB, Is.EqualTo(2), nameof(argumentB));
			Assert.That(argumentC, Is.EqualTo(3), nameof(argumentC));
			Assert.That(argumentD, Is.EqualTo(4), nameof(argumentD));
			Assert.That(argumentE, Is.EqualTo(5), nameof(argumentE));
			Assert.That(argumentF, Is.EqualTo(6), nameof(argumentF));
			Assert.That(argumentG, Is.EqualTo(7), nameof(argumentG));
			Assert.That(argumentH, Is.EqualTo(8), nameof(argumentH));
			Assert.That(argumentI, Is.EqualTo(9), nameof(argumentI));
			Assert.That(argumentJ, Is.EqualTo(10), nameof(argumentJ));
			Assert.That(argumentK, Is.EqualTo(11), nameof(argumentK));
			Assert.That(argumentL, Is.EqualTo(12), nameof(argumentL));

			rock.Verify();
		}
	}

	public interface IHandleAction12ArgumentTests
	{
		event EventHandler TargetEvent;
		void Target(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l);
	}
}