using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Permutations
{
	public class PermutationZiezi
	{
		private Action<int[]> _action = null;

		public static void EnumeratePermutation(int[] arr, Action<int[]> action)
		{
			int setSize = 3;
			PermutationZiezi pz = new PermutationZiezi();
			pz._action = action;
			// pz.FindPermutations(setSize);
			pz.Permute(arr, arr.Length - 1);
		}
		//-----------------------------------------------------------------------------
		/* Method: FindPermutations(n) */
		private void FindPermutations(int n)
		{
			int[] arr = new int[n];
			for (int i = 0; i < n; i++)
			{
				arr[i] = i + 1;
			}
			int iEnd = arr.Length - 1;
			Permute(arr, iEnd);
		}
		//-----------------------------------------------------------------------------  
		/* Method: Permute(arr) */
		private void Permute(int[] arr, int iEnd)
		{
			if (iEnd == 0)
			{
				_action(arr);
				return;
			}

			Permute(arr, iEnd - 1);
			for (int i = 0; i < iEnd; i++)
			{
				swap(ref arr[i], ref arr[iEnd]);
				Permute(arr, iEnd - 1);
				swap(ref arr[i], ref arr[iEnd]);
			}
		}

		//-----------------------------------------------------------------------------  
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void swap(ref int a, ref int b)
		{
			int temp = a;
			a = b;
			b = temp;
		}

		//-----------------------------------------------------------------------------  

	}
}

