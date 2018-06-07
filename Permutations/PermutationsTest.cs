using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permutations
{
	public class PermutationsTest
	{
		/// <summary>
		/// Func to show how to call. It does a little test for an array of 4 items.
		/// </summary>
		public static void Test()
		{
			PermutationOuellet.ForAllPermutation("123".ToCharArray(), (vals) =>
			{
				Console.WriteLine(String.Join("", vals));
				return false;
			});

			int[] values = new int[] { 0, 1, 2, 4 };

			Console.WriteLine("OuelletHeap heap's algorithm implementation");
			PermutationOuellet.ForAllPermutation(values, (vals) =>
			{
				Console.WriteLine(String.Join("", vals));
				return false;
			});

			Console.WriteLine("Linq algorithm");
			foreach (var v in PermutationPengyang.GetPermutations(values, values.Length))
			{
				Console.WriteLine(String.Join("", v));
			}

			// Performance Heap's against Linq version : huge differences
			int count = 0;

			values = new int[10];
			for (int n = 0; n < values.Length; n++)
			{
				values[n] = n;
			}

			Stopwatch stopWatch = new Stopwatch();

			PermutationOuellet.ForAllPermutation(values, (vals) =>
			{
				foreach (var v in vals)
				{
					count++;
				}
				return false;
			});

			stopWatch.Stop();
			Console.WriteLine($"OuelletHeap heap's algorithm implementation {count} items in {stopWatch.ElapsedMilliseconds} millisecs");

			count = 0;
			stopWatch.Reset();
			stopWatch.Start();

			foreach (var vals in PermutationPengyang.GetPermutations(values, values.Length))
			{
				foreach (var v in vals)
				{
					count++;
				}
			}

			stopWatch.Stop();
			Console.WriteLine($"Linq {count} items in {stopWatch.ElapsedMilliseconds} millisecs");
		}
	}
}
