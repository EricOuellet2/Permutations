using System;
using System.Collections.Generic;

namespace Permutations
{
	public class PermutationOuelletLexico3<T>
	{
		// ************************************************************************
		private T[] _sortedValues;

		private bool[] _valueUsed;

		public readonly long MaxIndex; // long to support 20! or less 

		// ************************************************************************
		public PermutationOuelletLexico3(T[] sortedValues)
		{
			if (sortedValues.Length <= 0)
			{
				throw new ArgumentException("sortedValues.Lenght should be greater than 0");
			}

			_sortedValues = sortedValues;
			Result = new T[_sortedValues.Length];
			_valueUsed = new bool[_sortedValues.Length];

			MaxIndex = Factorial.GetFactorial(_sortedValues.Length);
		}

		// ************************************************************************
		public T[] Result { get; private set; }

		// ************************************************************************
		/// <summary>
		/// Return the permutation relative to the index received.
		/// Based on _sortedValues. Sort Index is 0 based and should be less than MaxIndex.
		/// </summary>
		/// <param name="sortIndex"></param>
		/// <returns>The result is written in property: Result</returns>
		public void GetValuesForIndex(long sortIndex)
		{
			int size = _sortedValues.Length;

			if (sortIndex < 0)
			{
				throw new ArgumentException("sortIndex should be greater or equal to 0.");
			}

			if (sortIndex >= MaxIndex)
			{
				throw new ArgumentException("sortIndex should be less than factorial(the lenght of items)");
			}

			for (int n = 0; n < _valueUsed.Length; n++)
			{
				_valueUsed[n] = false;
			}
			
			long factorielLower = MaxIndex;

			for (int index = 0; index < size; index++)
			{
				long factorielBigger = factorielLower;
				factorielLower = Factorial.GetFactorial(size - index - 1);  //  factorielBigger / inverseIndex;

				int resultItemIndex = (int)(sortIndex % factorielBigger / factorielLower);

				int correctedResultItemIndex = 0;
				for(;;)
				{
					if (! _valueUsed[correctedResultItemIndex])
					{
						resultItemIndex--;
						if (resultItemIndex < 0)
						{
							break;
						}
					}
					correctedResultItemIndex++;
				}

				Result[index] = _sortedValues[correctedResultItemIndex];
				_valueUsed[correctedResultItemIndex] = true;
			}
		}

		// ************************************************************************
		/// <summary>
		/// Calc the index, relative to the permutation received
		/// as argument. Based on _sortedValues. Returned index is 0 based.
		/// </summary>
		/// <param name="values"></param>
		/// <returns></returns>
		public long GetIndexOfValues(T[] values)
		{
			int size = _sortedValues.Length;
			long valuesIndex = 0;

			List<T> valuesLeft = new List<T>(_sortedValues);

			for (int index = 0; index < size; index++)
			{
				long indexFactorial = Factorial.GetFactorial(size - 1 - index);

				T value = values[index];
				int indexCorrected = valuesLeft.IndexOf(value);
				valuesIndex = valuesIndex + (indexCorrected * indexFactorial);
				valuesLeft.Remove(value);
			}
			return valuesIndex;
		}

		// ************************************************************************
	}
}
