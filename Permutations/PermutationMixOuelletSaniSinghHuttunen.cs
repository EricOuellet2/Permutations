using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Permutations
{
	public class PermutationMixOuelletSaniSinghHuttunen
	{
		// ************************************************************************
		private long _indexFirst;
		private long _indexLastExclusive;
		private int[] _sortedValues;

		// ************************************************************************
		public PermutationMixOuelletSaniSinghHuttunen(int[] sortedValues, long indexFirst = -1, long indexLastExclusive = -1)
		{
			if (indexFirst == -1)
			{
				indexFirst = 0;
			}

			if (indexLastExclusive == -1)
			{
				indexLastExclusive = Factorial.GetFactorial(sortedValues.Length);
			}

			if (indexFirst >= indexLastExclusive)
			{
				throw new ArgumentException($"{nameof(indexFirst)} should be less than {nameof(indexLastExclusive)}");
			}

			_indexFirst = indexFirst;
			_indexLastExclusive = indexLastExclusive;
			_sortedValues = sortedValues;
		}

		// ************************************************************************
		public void ExecuteForEachPermutation(Action<int[]> action)
		{
			//			Console.WriteLine($"Thread {System.Threading.Thread.CurrentThread.ManagedThreadId} started: {_indexFirst} {_indexLastExclusive}");

			long index = _indexFirst;

			PermutationOuelletLexico3<int> permutationOuellet = new PermutationOuelletLexico3<int>(_sortedValues);

			permutationOuellet.GetValuesForIndex(index);
			action(permutationOuellet.Result);
			index++;

			int[] values = permutationOuellet.Result;
			while (index < _indexLastExclusive)
			{
				PermutationSaniSinghHuttunen.NextPermutation(values);
				action(values);
				index++;
			}

			//			Console.WriteLine($"Thread {System.Threading.Thread.CurrentThread.ManagedThreadId} ended: {DateTime.Now.ToString("yyyyMMdd_HHmmss_ffffff")}");
		}

		// ************************************************************************
		public static void ExecuteForEachPermutationMT(int[] sortedValues, Action<int[]> action)
		{
			int coreCount = Environment.ProcessorCount; // Hyper treading are taken into account (ex: on a 4 cores hyperthreaded = 8)
			long itemsFactorial = Factorial.GetFactorial(sortedValues.Length);
			long partCount = (long)Math.Ceiling((double)itemsFactorial / (double)coreCount);
			long startIndex = 0;

			var tasks = new List<Task>();

			for (int coreIndex = 0; coreIndex < coreCount; coreIndex++)
			{
				long stopIndex = Math.Min(startIndex + partCount, itemsFactorial);

				PermutationMixOuelletSaniSinghHuttunen mix = new PermutationMixOuelletSaniSinghHuttunen(sortedValues, startIndex, stopIndex);
				Task task = Task.Run(() => mix.ExecuteForEachPermutation(action));
				tasks.Add(task);

				if (stopIndex == itemsFactorial)
				{
					break;
				}

				startIndex = startIndex + partCount;
			}

			Task.WaitAll(tasks.ToArray());
		}

		// ************************************************************************


	}
}
