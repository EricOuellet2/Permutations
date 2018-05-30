using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Permutations
{
	public static class EnumerableExtension
	{
		// ******************************************************************
		public static bool Exists<T>(this IEnumerable<T> coll, Func<T, bool> condition)
		{
			foreach (T t in coll)
			{
				if (condition(t))
				{
					return true;
				}
			}

			return false;
		}

		// ******************************************************************
		public static bool Exists<T>(this IEnumerable coll, Func<T, bool> condition)
		{
			foreach (T t in coll)
			{
				if (condition(t))
				{
					return true;
				}
			}

			return false;
		}

		// ******************************************************************
		///<summary>Finds the index of the first item matching an expression in an enumerable.</summary>
		///<param name="items">The enumerable to search.</param>
		///<param name="predicate">The expression to test the items against.</param>
		///<returns>The index of the first matching item, or -1 if no items match.</returns>
		public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
		{
			if (items == null) throw new ArgumentNullException("items");
			if (predicate == null) throw new ArgumentNullException("predicate");

			int retVal = 0;
			foreach (var item in items)
			{
				if (predicate(item)) return retVal;
				retVal++;
			}
			return -1;
		}

		// ******************************************************************
		public static string ConcatItemsToString<T>(this IEnumerable<T> items, Func<T, string> elementStringToConcat, string separator = ", ")
		{
			StringBuilder sb = new StringBuilder();
			bool firstItem = true;
			foreach (T item in items)
			{
				string str = elementStringToConcat(item);

				if (String.IsNullOrEmpty(str))
				{
					continue;
				}

				if (str.Trim() == "")
				{
					continue;
				}

				if (firstItem)
				{
					firstItem = false;
				}
				else
				{
					if (separator != null)
						sb.Append(separator);
				}

				sb.Append(elementStringToConcat(item));
			}

			return sb.ToString();
		}

		// ******************************************************************
		public static void ApplyForEachItem<T>(this IEnumerable<T> enumerable, Action<T> action)
		{
			foreach (T t in enumerable)
			{
				action(t);
			}
		}

		// ******************************************************************
		public static void ApplyForEachItemWithIndex<T>(this IEnumerable<T> enumerable, Action<T, int> action)
		{
			int index = 0;
			foreach (T t in enumerable)
			{
				action(t, index);
				index++;
			}
		}

		// ******************************************************************
		public static void ApplyForEachItemOnCopyForDeletion<T>(this IEnumerable<T> enumerable, Action<T> action)
		{
			var listCopy = new List<T>(enumerable);
			foreach (T t in listCopy)
			{
				action(t);
			}
		}

		// ******************************************************************
		public static void Dump<T>(this IEnumerable<T> enumerable)
		{
			foreach (T item in enumerable)
			{
				Debug.Print(item.ToString());
			}
		}

		// ******************************************************************
		/// <summary>
		/// Return a subset of childs that are of specific type "TResult" and also meet conditon, if any.
		/// For example: Animals<Animal, Cat>.EnumerableUpcastFilter(cat=>cat.EyeColor == Color.Blue);
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="source"></param>
		/// <param name="condition"></param>
		/// <returns></returns>
		public static IEnumerable<TResult> EnumerableUpcastFilter<T, TResult>(this IEnumerable<T> source, Predicate<TResult> condition = null) where T : class where TResult : class
		{
			// ReSharper disable once PossibleMultipleEnumeration
			if (source == null)
			{
				yield break;
			}

			foreach (T child in source)
			{
				var childUpcasted = child as TResult;
				if (childUpcasted != null)
				{
					if (condition == null || condition(childUpcasted))
					{
						yield return childUpcasted;
					}
				}
			}
		}

		// ******************************************************************
		public static IEnumerable<T> ReverseEx<T>(this IEnumerable<T> coll)
		{
			var quick = coll as IReadOnlyList<T>;
			if (quick == null)
			{
				foreach (T item in coll.Reverse()) yield return item;
			}
			else
			{
				for (int ix = quick.Count - 1; ix >= 0; --ix)
				{
					yield return quick[ix];
				}
			}
		}

		// ******************************************************************
		public static int Count(this IEnumerable source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			ICollection collection = source as ICollection;
			if (collection != null)
				return collection.Count;

			int count = 0;
			IEnumerator e = source.GetEnumerator();
			var disposable = e as IDisposable;
			if (disposable != null)
			{
				using (disposable)
				{
					checked
					{
						while (e.MoveNext()) count++;
					}
				}
			}
			else
			{
				checked
				{
					while (e.MoveNext()) count++;
				}
			}

			return count;
		}

		// ******************************************************************
		public static bool IsEmpty<T>(this IEnumerable<T> coll)
		{
			if (coll == null)
			{
				return true;
			}
			return !coll.GetEnumerator().MoveNext();
		}

		// ******************************************************************


	}
}
