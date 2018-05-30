using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Permutations
{
	/// <summary>
	/// EO: This is a try to create the permutation set from right to left but it just does not works as expected.
	/// It was supposed (from what I thought) to have the advantage of being faster.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class PermutationOuelletLexico4<T>
	{
		// ************************************************************************
		private T[] _sortedValues;

		private int[] _positions;

		public readonly long MaxIndex; // long to support 20! or less 

		// ************************************************************************
		public PermutationOuelletLexico4(T[] sortedValues)
		{
			_sortedValues = sortedValues;
			Result = new T[_sortedValues.Length];
			_positions = new int[_sortedValues.Length];

			MaxIndex = Factorial.GetFactorial(_sortedValues.Length);
		}

		// ************************************************************************
		public T[] Result { get; private set; }

		// ************************************************************************
		/// <summary>
		/// Sort Index is 0 based and should be less than MaxIndex. Otherwise you get an exception.
		/// </summary>
		/// <param name="sortIndex"></param>
		/// <param name="result">Value is not used as inpu, only as output. Re-use buffer in order to save memory</param>
		/// <returns></returns>
		public void GetSortedValuesFor(long sortIndex)
		{
			int size = _sortedValues.Length;

			if (sortIndex < 0)
			{
				throw new ArgumentException("sortIndex should greater or equal to 0.");
			}

			if (sortIndex >= MaxIndex)
			{
				throw new ArgumentException("sortIndex should less than factorial(the lenght of items)");
			}

			_positions[size - 1] = size - 1;

#if DEBUG
			_positions[0] = -1;
			_positions[1] = -1;
			_positions[2] = -1;


			if (sortIndex == 4)
			{
				Debug.Assert(false);
			}
#endif
			long factorielBigger = 1;

			for (int index = 2; index <= size; index++)
			{
				long factorielLower = factorielBigger;
				factorielBigger = Factorial.GetFactorial(index);  //  factorielBigger / inverseIndex;

				int resultItemIndex = size - index + (int)(sortIndex % factorielBigger / factorielLower);

				int temp = _positions[resultItemIndex];
				_positions[resultItemIndex] = size - index;
				if (resultItemIndex > size - index)
				{
					_positions[size - index] = temp;
				}

				Dump(_positions);
			}

			for (int index = 0; index < size; index++)
			{
				Result[_positions[index]] = _sortedValues[index];
			}

			Debug.Print("Result: ");
			Dump(Result);
		}

		// ************************************************************************
		private void Dump(int[] result)
		{
			Debug.Print("-------------------------------------------------------------------------");
			Debug.Print($"{result[0]}, {result[1]}, {result[2]}, {result[3]}, ");
		}

		private void Dump(T[] result)
		{
			Debug.Print("-------------------------------------------------------------------------");
			Debug.Print($"{result[0]}, {result[1]}, {result[2]}, {result[3]}, ");
		}

	}
}
