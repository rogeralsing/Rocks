﻿using NUnit.Framework;
using Rocks.Exceptions;
using System;

namespace Rocks.Tests
{
	[TestFixture]
	public sealed class HandleProperty4IndexerTests
	{
		[Test]
		public void MakeWithGetIndexerProperty()
		{
			var returnValue = Guid.NewGuid().ToString();

			var rock = Rock.Create<IProperty4Indexer>();
			rock.Handle(() => 44, () => 45, () => 46, () => 47, (_, __, ___, ____) => returnValue);

			var chunk = rock.Make();
			var propertyValue = chunk[44, 45, 46, 47];

			Assert.AreEqual(returnValue, propertyValue, nameof(propertyValue));
			rock.Verify();
		}

		[Test]
		public void MakeWithGetIndexerPropertyAndGetNotUsed()
		{
			var rock = Rock.Create<IProperty4Indexer>();
			rock.Handle(() => 44, () => 45, () => 46, () => 47, (_, __, ___, ____) => Guid.NewGuid().ToString());

			var chunk = rock.Make();

			Assert.Throws<VerificationException>(() => rock.Verify());
		}

		[Test]
		public void MakeWithGetIndexerPropertyAndExpectedCallCount()
		{
			var returnValue = Guid.NewGuid().ToString();

			var rock = Rock.Create<IProperty4Indexer>();
			rock.Handle(() => 44, () => 45, () => 46, () => 47, (_, __, ___, ____) => returnValue, 2);

			var chunk = rock.Make();
			var propertyValue = chunk[44, 45, 46, 47];
			propertyValue = chunk[44, 45, 46, 47];

			Assert.AreEqual(returnValue, propertyValue, nameof(propertyValue));
			rock.Verify();
		}

		[Test]
		public void MakeWithGetIndexerPropertyAndExpectedCallCountAndGetNotUsed()
		{
			var rock = Rock.Create<IProperty4Indexer>();
			rock.Handle(() => 44, () => 45, () => 46, () => 47, (_, __, ___, ____) => Guid.NewGuid().ToString(), 2);

			var chunk = rock.Make();

			Assert.Throws<VerificationException>(() => rock.Verify());
		}

		[Test]
		public void MakeWithGetIndexerPropertyAndExpectedCallCountAndGetNotUsedEnough()
		{
			var rock = Rock.Create<IProperty4Indexer>();
			rock.Handle(() => 44, () => 45, () => 46, () => 47, (_, __, ___, ____) => Guid.NewGuid().ToString(), 2);

			var chunk = rock.Make();
			var propertyValue = chunk[44, 45, 46, 47];

			Assert.Throws<VerificationException>(() => rock.Verify());
		}

		[Test]
		public void MakeWithSetIndexerProperty()
		{
			var indexer1 = Guid.NewGuid();
			var indexer2 = Guid.NewGuid();
			var indexer3 = Guid.NewGuid();
			var indexer4 = Guid.NewGuid();
			var indexerSetValue = Guid.NewGuid().ToString();
			string setValue = null;

			var rock = Rock.Create<IProperty4Indexer>();
			rock.Handle<Guid, Guid, Guid, Guid, string>(() => indexer1, () => indexer2, () => indexer3, () => indexer4, (i1, i2, i3, i4, value) => setValue = value);

			var chunk = rock.Make();
			chunk[indexer1, indexer2, indexer3, indexer4] = indexerSetValue;

			Assert.AreEqual(indexerSetValue, setValue, nameof(setValue));
			rock.Verify();
		}

		[Test]
		public void MakeWithSetIndexerPropertyAndSetNotUsed()
		{
			var rock = Rock.Create<IProperty4Indexer>();
			rock.Handle<Guid, Guid, Guid, Guid, string>(() => Guid.NewGuid(), () => Guid.NewGuid(), () => Guid.NewGuid(), () => Guid.NewGuid(), (i1, i2, i3, i4, value) => { });

			var chunk = rock.Make();

			Assert.Throws<VerificationException>(() => rock.Verify());
		}

		[Test]
		public void MakeWithSetIndexerPropertyAndExpectedCallCount()
		{
			var indexer1 = Guid.NewGuid();
			var indexer2 = Guid.NewGuid();
			var indexer3 = Guid.NewGuid();
			var indexer4 = Guid.NewGuid();
			var indexerSetValue = Guid.NewGuid().ToString();
			string setValue = null;

			var rock = Rock.Create<IProperty4Indexer>();
			rock.Handle<Guid, Guid, Guid, Guid, string>(() => indexer1, () => indexer2, () => indexer3, () => indexer4, (i1, i2, i3, i4, value) => setValue = value, 2);

			var chunk = rock.Make();
			chunk[indexer1, indexer2, indexer3, indexer4] = indexerSetValue;
			chunk[indexer1, indexer2, indexer3, indexer4] = indexerSetValue;

			Assert.AreEqual(indexerSetValue, setValue, nameof(setValue));
			rock.Verify();
		}

		[Test]
		public void MakeWithSetIndexerPropertyAndExpectedCallCountAndSetNotUsed()
		{
			var rock = Rock.Create<IProperty4Indexer>();
			rock.Handle<Guid, Guid, Guid, Guid, string>(() => Guid.NewGuid(), () => Guid.NewGuid(), () => Guid.NewGuid(), () => Guid.NewGuid(), (i1, i2, i3, i4, value) => { }, 2);

			var chunk = rock.Make();

			Assert.Throws<VerificationException>(() => rock.Verify());
		}

		[Test]
		public void MakeWithSetIndexerPropertyAndExpectedCallCountAndSetNotUsedEnough()
		{
			var indexer1 = Guid.NewGuid();
			var indexer2 = Guid.NewGuid();
			var indexer3 = Guid.NewGuid();
			var indexer4 = Guid.NewGuid();
			var indexerSetValue = Guid.NewGuid().ToString();
			string setValue = null;

			var rock = Rock.Create<IProperty4Indexer>();
			rock.Handle<Guid, Guid, Guid, Guid, string>(() => indexer1, () => indexer2, () => indexer3, () => indexer4, (i1, i2, i3, i4, value) => setValue = value, 2);

			var chunk = rock.Make();
			chunk[indexer1, indexer2, indexer3, indexer4] = indexerSetValue;

			Assert.Throws<VerificationException>(() => rock.Verify());
		}

		[Test]
		public void MakeWithGetAndSetIndexerProperty()
		{
			var indexer1 = Guid.NewGuid().ToString();
			var indexer2 = Guid.NewGuid().ToString();
			var indexer3 = Guid.NewGuid().ToString();
			var indexer4 = Guid.NewGuid().ToString();
			var indexerSetValue = Guid.NewGuid().ToString();
			var returnValue = Guid.NewGuid().ToString();
			string setValue = null;

			var rock = Rock.Create<IProperty4Indexer>();
			rock.Handle(() => indexer1, () => indexer2, () => indexer3, () => indexer4, (_, __, ___, _____) => returnValue, (i1, i2, i3, i4, value) => setValue = value);

			var chunk = rock.Make();
			var propertyValue = chunk[indexer1, indexer2, indexer3, indexer4];
			chunk[indexer1, indexer2, indexer3, indexer4] = indexerSetValue;

			Assert.AreEqual(returnValue, propertyValue, nameof(propertyValue));
			Assert.AreEqual(indexerSetValue, setValue, nameof(setValue));
			rock.Verify();
		}

		[Test]
		public void MakeWithGetAndSetIndexerPropertyAndGetNotUsed()
		{
			var indexer1 = Guid.NewGuid().ToString();
			var indexer2 = Guid.NewGuid().ToString();
			var indexer3 = Guid.NewGuid().ToString();
			var indexer4 = Guid.NewGuid().ToString();
			var indexerSetValue = Guid.NewGuid().ToString();
			var returnValue = Guid.NewGuid().ToString();

			var rock = Rock.Create<IProperty4Indexer>();
			rock.Handle(() => indexer1, () => indexer2, () => indexer3, () => indexer4, (_, __, ___, ____) => returnValue, (i1, i2, i3, i4, value) => { });

			var chunk = rock.Make();
			chunk[indexer1, indexer2, indexer3, indexer4] = indexerSetValue;

			Assert.Throws<VerificationException>(() => rock.Verify());
		}

		[Test]
		public void MakeWithGetAndSetIndexerPropertyAndSetNotUsed()
		{
			var indexer1 = Guid.NewGuid().ToString();
			var indexer2 = Guid.NewGuid().ToString();
			var indexer3 = Guid.NewGuid().ToString();
			var indexer4 = Guid.NewGuid().ToString();
			var returnValue = Guid.NewGuid().ToString();

			var rock = Rock.Create<IProperty4Indexer>();
			rock.Handle(() => indexer1, () => indexer2, () => indexer3, () => indexer4, (_, __, ___, ____) => returnValue, (i1, i2, i3, i4, value) => { });

			var chunk = rock.Make();
			var propertyValue = chunk[indexer1, indexer2, indexer3, indexer4];

			Assert.Throws<VerificationException>(() => rock.Verify());
		}

		[Test]
		public void MakeWithGetAndSetIndexerPropertyAndExpectedCallCount()
		{
			var indexer1 = Guid.NewGuid().ToString();
			var indexer2 = Guid.NewGuid().ToString();
			var indexer3 = Guid.NewGuid().ToString();
			var indexer4 = Guid.NewGuid().ToString();
			var indexerSetValue = Guid.NewGuid().ToString();
			var returnValue = Guid.NewGuid().ToString();
			string setValue = null;

			var rock = Rock.Create<IProperty4Indexer>();
			rock.Handle(() => indexer1, () => indexer2, () => indexer3, () => indexer4, (_, __, ___, ____) => returnValue, (i1, i2, i3, i4, value) => setValue = value, 2);

			var chunk = rock.Make();
			var propertyValue = chunk[indexer1, indexer2, indexer3, indexer4];
			propertyValue = chunk[indexer1, indexer2, indexer3, indexer4];
			chunk[indexer1, indexer2, indexer3, indexer4] = indexerSetValue;
			chunk[indexer1, indexer2, indexer3, indexer4] = indexerSetValue;

			Assert.AreEqual(returnValue, propertyValue, nameof(propertyValue));
			Assert.AreEqual(indexerSetValue, setValue, nameof(setValue));
			rock.Verify();
		}

		[Test]
		public void MakeWithGetAndSetIndexerPropertyAndExpectedCallCountAndGetNotUsed()
		{
			var indexer1 = Guid.NewGuid().ToString();
			var indexer2 = Guid.NewGuid().ToString();
			var indexer3 = Guid.NewGuid().ToString();
			var indexer4 = Guid.NewGuid().ToString();
			var indexerSetValue = Guid.NewGuid().ToString();
			var returnValue = Guid.NewGuid().ToString();

			var rock = Rock.Create<IProperty4Indexer>();
			rock.Handle(() => indexer1, () => indexer2, () => indexer3, () => indexer4, (_, __, ___, ____) => returnValue, (i1, i2, i3, i4, value) => { }, 2);

			var chunk = rock.Make();
			chunk[indexer1, indexer2, indexer3, indexer4] = indexerSetValue;
			chunk[indexer1, indexer2, indexer3, indexer4] = indexerSetValue;

			Assert.Throws<VerificationException>(() => rock.Verify());
		}

		[Test]
		public void MakeWithGetAndSetIndexerPropertyAndExpectedCallCountAndGetNotUsedEnough()
		{
			var indexer1 = Guid.NewGuid().ToString();
			var indexer2 = Guid.NewGuid().ToString();
			var indexer3 = Guid.NewGuid().ToString();
			var indexer4 = Guid.NewGuid().ToString();
			var indexerSetValue = Guid.NewGuid().ToString();
			var returnValue = Guid.NewGuid().ToString();

			var rock = Rock.Create<IProperty4Indexer>();
			rock.Handle(() => indexer1, () => indexer2, () => indexer3, () => indexer4, (_, __, ___, ____) => returnValue, (i1, i2, i3, i4, value) => { }, 2);

			var chunk = rock.Make();
			chunk[indexer1, indexer2, indexer3, indexer4] = indexerSetValue;
			chunk[indexer1, indexer2, indexer3, indexer4] = indexerSetValue;
			var propertyValue = chunk[indexer1, indexer2, indexer3, indexer4];

			Assert.Throws<VerificationException>(() => rock.Verify());
		}

		[Test]
		public void MakeWithGetAndSetIndexerPropertyAndExpectedCallCountAndSetNotUsed()
		{
			var indexer1 = Guid.NewGuid().ToString();
			var indexer2 = Guid.NewGuid().ToString();
			var indexer3 = Guid.NewGuid().ToString();
			var indexer4 = Guid.NewGuid().ToString();
			var indexerSetValue = Guid.NewGuid().ToString();
			var returnValue = Guid.NewGuid().ToString();

			var rock = Rock.Create<IProperty4Indexer>();
			rock.Handle(() => indexer1, () => indexer2, () => indexer3, () => indexer4, (_, __, ___, ____) => returnValue, (i1, i2, i3, i4, value) => { }, 2);

			var chunk = rock.Make();
			var propertyValue = chunk[indexer1, indexer2, indexer3, indexer4];
			propertyValue = chunk[indexer1, indexer2, indexer3, indexer4];

			Assert.Throws<VerificationException>(() => rock.Verify());
		}

		[Test]
		public void MakeWithGetAndSetIndexerPropertyAndExpectedCallCountAndSetNotUsedEnough()
		{
			var indexer1 = Guid.NewGuid().ToString();
			var indexer2 = Guid.NewGuid().ToString();
			var indexer3 = Guid.NewGuid().ToString();
			var indexer4 = Guid.NewGuid().ToString();
			var indexerSetValue = Guid.NewGuid().ToString();
			var returnValue = Guid.NewGuid().ToString();

			var rock = Rock.Create<IProperty4Indexer>();
			rock.Handle(() => indexer1, () => indexer2, () => indexer3, () => indexer4, (_, __, ___, ____) => returnValue, (i1, i2, i3, i4, value) => { }, 2);

			var chunk = rock.Make();
			chunk[indexer1, indexer2, indexer3, indexer4] = indexerSetValue;
			var propertyValue = chunk[indexer1, indexer2, indexer3, indexer4];
			propertyValue = chunk[indexer1, indexer2, indexer3, indexer4];

			Assert.Throws<VerificationException>(() => rock.Verify());
		}
	}

	public interface IProperty4Indexer
	{
		string this[int a, int b, int c, int d] { get; }
		string this[Guid a, Guid b, Guid c, Guid d] { set; }
		string this[string a, string b, string c, string d] { get; set; }
	}
}
