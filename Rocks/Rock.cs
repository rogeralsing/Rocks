﻿using Rocks.Exceptions;
using Rocks.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Globalization;

namespace Rocks
{
	public static class Rock
	{
		internal static ConcurrentDictionary<Type, Type> cache = new ConcurrentDictionary<Type, Type>();

		public static Rock<T> Create<T>()
			where T : class
		{
			return Rock.Create<T>(new RockOptions());
		}

		public static Rock<T> Create<T>(RockOptions options)
			where T : class
		{
			var message = typeof(T).Validate();

			if (!string.IsNullOrWhiteSpace(message))
			{
				throw new RockValidationException(message);
			}

			return new Rock<T>(options);
		}

		public static bool TryCreate<T>(out Rock<T> result)
			where T : class
		{
			return Rock.TryCreate<T>(new RockOptions(), out result);
		}

		public static bool TryCreate<T>(RockOptions options, out Rock<T> result)
			where T : class
		{
			result = default(Rock<T>);

			var message = typeof(T).Validate();

			if (!string.IsNullOrWhiteSpace(message))
			{
				return false;
			}

			result = new Rock<T>(options);
			return true;
		}

		internal static ReadOnlyDictionary<string, ArgumentExpectation> GetArgumentExpectations(MethodCallExpression method)
		{
			var expectations = new Dictionary<string, ArgumentExpectation>();

			var argumentIndex = 0;
			var methodArguments = method.Method.GetParameters();

			foreach (var argument in method.Arguments)
			{
				var methodArgument = methodArguments[argumentIndex];

				switch (argument.NodeType)
				{
					case ExpressionType.Constant:
						var value = (argument as ConstantExpression).Value;
                  expectations.Add(methodArgument.Name,
							typeof(ArgumentExpectation<>).MakeGenericType(methodArgument.ParameterType)
								.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new[] { methodArgument.ParameterType }, null)
								.Invoke(new[] { value }) as ArgumentExpectation);
                  break;
					case ExpressionType.Call:
						var argumentMethod = (argument as MethodCallExpression).Method;
						var isMethod = typeof(Arg).GetMethod(nameof(Arg.Is));
						var isAnyMethod = typeof(Arg).GetMethod(nameof(Arg.IsAny));

						if (argumentMethod.Name == isAnyMethod.Name && argumentMethod.DeclaringType == isAnyMethod.DeclaringType)
						{
							expectations.Add(methodArgument.Name,
								typeof(ArgumentExpectation<>).MakeGenericType(methodArgument.ParameterType)
									.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null)
									.Invoke(null) as ArgumentExpectation);
						}
						else if (argumentMethod.Name == isMethod.Name && argumentMethod.DeclaringType == isMethod.DeclaringType)
						{
							// TODO. This is saying, "pass in the arugment value, and is it true? i.e. does it pass validation?"
							// Need a Func<T, bool> argument to ArgumentExpectation<> with a "is" flag
							// and handle accordingly (and write tests!)
						}
						else
						{
							expectations.Add(methodArgument.Name,
								typeof(ArgumentExpectation<>).MakeGenericType(typeof(Expression))
									.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(Expression) }, null)
									.Invoke(new[] { argument }) as ArgumentExpectation);
						}

						break;
					default:
						expectations.Add(methodArgument.Name,
							typeof(ArgumentExpectation<>).MakeGenericType(typeof(Expression))
								.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(Expression) }, null)
								.Invoke(new[] { argument }) as ArgumentExpectation);
						break;
				}

				argumentIndex++;
			}

			return new ReadOnlyDictionary<string, ArgumentExpectation>(expectations);
		}
	}

	public sealed class Rock<T>
		where T : class
	{
		private List<IRock> rocks = new List<IRock>();
		private Dictionary<string, HandlerInformation> handlers =
			new Dictionary<string, HandlerInformation>();
		private RockOptions options;
		private SortedSet<string> namespaces = new SortedSet<string>();

		internal Rock()
			: this(new RockOptions())
		{ }

		internal Rock(RockOptions options)
		{
			this.options = options;
		}

		public void HandleAction(Expression<Action<T>> expression)
		{
			var method = ((MethodCallExpression)expression.Body);
			this.handlers[method.Method.GetMethodDescription(this.namespaces)] =
				new HandlerInformation(Rock.GetArgumentExpectations(method));
		}

		public void HandleAction(Expression<Action<T>> expression, uint expectedCallCount)
		{
			var method = ((MethodCallExpression)expression.Body);
			this.handlers[method.Method.GetMethodDescription(this.namespaces)] =
				new HandlerInformation(expectedCallCount, Rock.GetArgumentExpectations(method));
		}

		public void HandleAction(Expression<Action<T>> expression, Action handler)
		{
			var method = ((MethodCallExpression)expression.Body);
			this.handlers[method.Method.GetMethodDescription(this.namespaces)] =
				new HandlerInformation(handler, Rock.GetArgumentExpectations(method));
		}

		public void HandleAction(Expression<Action<T>> expression, Action handler, uint expectedCallCount)
		{
			var method = ((MethodCallExpression)expression.Body);
			this.handlers[method.Method.GetMethodDescription(this.namespaces)] =
				new HandlerInformation(handler, expectedCallCount, Rock.GetArgumentExpectations(method));
		}

		public void HandleAction<T1>(Expression<Action<T>> expression, Action<T1> handler)
		{
			var method = ((MethodCallExpression)expression.Body);
			this.handlers[method.Method.GetMethodDescription(this.namespaces)] =
				new HandlerInformation(handler, Rock.GetArgumentExpectations(method));
		}

		public void HandleAction<T1>(Expression<Action<T>> expression, Action<T1> handler, uint expectedCallCount)
		{
			var method = ((MethodCallExpression)expression.Body);
			this.handlers[method.Method.GetMethodDescription(this.namespaces)] =
				new HandlerInformation(handler, expectedCallCount, Rock.GetArgumentExpectations(method));
		}

		public void HandleAction<T1, T2>(Expression<Action<T>> expression, Action<T1, T2> handler)
		{
			var method = ((MethodCallExpression)expression.Body);
			this.handlers[method.Method.GetMethodDescription(this.namespaces)] =
				new HandlerInformation(handler, Rock.GetArgumentExpectations(method));
		}

		public void HandleAction<T1, T2>(Expression<Action<T>> expression, Action<T1, T2> handler, uint expectedCallCount)
		{
			var method = ((MethodCallExpression)expression.Body);
			this.handlers[method.Method.GetMethodDescription(this.namespaces)] =
				new HandlerInformation(handler, expectedCallCount, Rock.GetArgumentExpectations(method));
		}

		public void HandleAction<T1, T2, T3>(Expression<Action<T>> expression, Action<T1, T2, T3> handler)
		{
			var method = ((MethodCallExpression)expression.Body);
			this.handlers[method.Method.GetMethodDescription(this.namespaces)] =
				new HandlerInformation(handler, Rock.GetArgumentExpectations(method));
		}

		public void HandleAction<T1, T2, T3>(Expression<Action<T>> expression, Action<T1, T2, T3> handler, uint expectedCallCount)
		{
			var method = ((MethodCallExpression)expression.Body);
			this.handlers[method.Method.GetMethodDescription(this.namespaces)] =
				new HandlerInformation(handler, expectedCallCount, Rock.GetArgumentExpectations(method));
		}

		public void HandleAction<T1, T2, T3, T4>(Expression<Action<T>> expression, Action<T1, T2, T3, T4> handler)
		{
			var method = ((MethodCallExpression)expression.Body);
			this.handlers[method.Method.GetMethodDescription(this.namespaces)] =
				new HandlerInformation(handler, Rock.GetArgumentExpectations(method));
		}

		public void HandleAction<T1, T2, T3, T4>(Expression<Action<T>> expression, Action<T1, T2, T3, T4> handler, uint expectedCallCount)
		{
			var method = ((MethodCallExpression)expression.Body);
			this.handlers[method.Method.GetMethodDescription(this.namespaces)] =
				new HandlerInformation(handler, expectedCallCount, Rock.GetArgumentExpectations(method));
		}

		public void HandleAction<T1, T2, T3, T4, T5>(Expression<Action<T>> expression, Action<T1, T2, T3, T4, T5> handler)
		{
			var method = ((MethodCallExpression)expression.Body);
			this.handlers[method.Method.GetMethodDescription(this.namespaces)] =
				new HandlerInformation(handler, Rock.GetArgumentExpectations(method));
		}

		public void HandleAction<T1, T2, T3, T4, T5>(Expression<Action<T>> expression, Action<T1, T2, T3, T4, T5> handler, uint expectedCallCount)
		{
			var method = ((MethodCallExpression)expression.Body);
			this.handlers[method.Method.GetMethodDescription(this.namespaces)] =
				new HandlerInformation(handler, expectedCallCount, Rock.GetArgumentExpectations(method));
		}

		public ReturnValue<TResult> HandleFunc<TResult>(Expression<Func<T, TResult>> expression)
		{
			var method = ((MethodCallExpression)expression.Body);
			var handler = new HandlerInformation<TResult>(Rock.GetArgumentExpectations(method));
			this.handlers[method.Method.GetMethodDescription(this.namespaces)] = handler;
				return new ReturnValue<TResult>(handler);
		}

		public ReturnValue<TResult> HandleFunc<TResult>(Expression<Func<T, TResult>> expression, uint expectedCallCount)
		{
			var method = ((MethodCallExpression)expression.Body);
			var handler = new HandlerInformation<TResult>(expectedCallCount, Rock.GetArgumentExpectations(method));
			this.handlers[method.Method.GetMethodDescription(this.namespaces)] = handler;
			return new ReturnValue<TResult>(handler);
		}

		public void HandleFunc<TResult>(Expression<Func<T, TResult>> expression, Func<TResult> handler)
		{
			var method = ((MethodCallExpression)expression.Body);
			this.handlers[method.Method.GetMethodDescription(this.namespaces)] =
				new HandlerInformation<TResult>(handler, Rock.GetArgumentExpectations(method));
		}

		public void HandleFunc<TResult>(Expression<Func<T, TResult>> expression, Func<TResult> handler, uint expectedCallCount)
		{
			var method = ((MethodCallExpression)expression.Body);
			this.handlers[method.Method.GetMethodDescription(this.namespaces)] =
				new HandlerInformation<TResult>(handler, expectedCallCount, Rock.GetArgumentExpectations(method));
		}

		public void HandleFunc<T1, TResult>(Expression<Func<T, TResult>> expression, Func<T1, TResult> handler)
		{
			var method = ((MethodCallExpression)expression.Body);
			this.handlers[method.Method.GetMethodDescription(this.namespaces)] =
				new HandlerInformation<TResult>(handler, Rock.GetArgumentExpectations(method));
		}

		public void HandleFunc<T1, TResult>(Expression<Func<T, TResult>> expression, Func<T1, TResult> handler, uint expectedCallCount)
		{
			var method = ((MethodCallExpression)expression.Body);
			this.handlers[method.Method.GetMethodDescription(this.namespaces)] =
				new HandlerInformation<TResult>(handler, expectedCallCount, Rock.GetArgumentExpectations(method));
		}

		public void HandleFunc<T1, T2, TResult>(Expression<Func<T, TResult>> expression, Func<T1, T2, TResult> handler)
		{
			var method = ((MethodCallExpression)expression.Body);
			this.handlers[method.Method.GetMethodDescription(this.namespaces)] =
				new HandlerInformation<TResult>(handler, Rock.GetArgumentExpectations(method));
		}

		public void HandleFunc<T1, T2, TResult>(Expression<Func<T, TResult>> expression, Func<T1, T2, TResult> handler, uint expectedCallCount)
		{
			var method = ((MethodCallExpression)expression.Body);
			this.handlers[method.Method.GetMethodDescription(this.namespaces)] =
				new HandlerInformation<TResult>(handler, expectedCallCount, Rock.GetArgumentExpectations(method));
		}

		public void HandleFunc<T1, T2, T3, TResult>(Expression<Func<T, TResult>> expression, Func<T1, T2, T3, TResult> handler)
		{
			var method = ((MethodCallExpression)expression.Body);
			this.handlers[method.Method.GetMethodDescription(this.namespaces)] =
				new HandlerInformation<TResult>(handler, Rock.GetArgumentExpectations(method));
		}

		public void HandleFunc<T1, T2, T3, TResult>(Expression<Func<T, TResult>> expression, Func<T1, T2, T3, TResult> handler, uint expectedCallCount)
		{
			var method = ((MethodCallExpression)expression.Body);
			this.handlers[method.Method.GetMethodDescription(this.namespaces)] =
				new HandlerInformation<TResult>(handler, expectedCallCount, Rock.GetArgumentExpectations(method));
		}

		public void HandleFunc<T1, T2, T3, T4, TResult>(Expression<Func<T, TResult>> expression, Func<T1, T2, T3, T4, TResult> handler)
		{
			var method = ((MethodCallExpression)expression.Body);
			this.handlers[method.Method.GetMethodDescription(this.namespaces)] =
				new HandlerInformation<TResult>(handler, Rock.GetArgumentExpectations(method));
		}

		public void HandleFunc<T1, T2, T3, T4, TResult>(Expression<Func<T, TResult>> expression, Func<T1, T2, T3, T4, TResult> handler, uint expectedCallCount)
		{
			var method = ((MethodCallExpression)expression.Body);
			this.handlers[method.Method.GetMethodDescription(this.namespaces)] =
				new HandlerInformation<TResult>(handler, expectedCallCount, Rock.GetArgumentExpectations(method));
		}

		public T Make()
		{
			var tType = typeof(T);
			var readOnlyHandlers = new ReadOnlyDictionary<string, HandlerInformation>(this.handlers);
			var rockType = Rock.cache.GetOrAdd(tType,
				_ => new RockMaker(_, readOnlyHandlers, this.namespaces, this.options).Mock);
			var rock = Activator.CreateInstance(rockType, readOnlyHandlers);
			this.rocks.Add(rock as IRock);
			return rock as T;
		}

		public void Verify()
		{
			var failures = new List<string>();

			foreach (var rock in this.rocks)
			{
				foreach (var pair in rock.Handlers)
				{
					foreach (var failure in pair.Value.Verify())
					{
						failures.Add(string.Format(Constants.ErrorMessages.VerificationFailed,
							rock.GetType().FullName, pair.Key, failure));
					}
				}
			}

			if (failures.Count > 0)
			{
				throw new RockVerificationException(failures);
			}
		}
	}
}