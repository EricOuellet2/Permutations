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
	public class PermutationOuelletLexico<T>
	{
		// ************************************************************************
		private T[] _sortedValues;

		public readonly long MaxIndex; // long to support 20! or less 
		
		// ************************************************************************
		public PermutationOuelletLexico(T[] sortedValues)
		{
			_sortedValues = sortedValues;
			Result = new T[_sortedValues.Length];

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

			List<T> valuesLeft = new List<T>(_sortedValues);

			long factorielLower = MaxIndex;

			for (int index = 0; index < size; index++)
			{
				int inverseIndex = size - index;

				long factorielBigger = factorielLower;
				factorielLower = factorielBigger / inverseIndex;

				int resultItemIndex = (int)(sortIndex % factorielBigger / factorielLower);

				T item = Result[index] = valuesLeft[resultItemIndex];

				valuesLeft.Remove(item);
			}
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
