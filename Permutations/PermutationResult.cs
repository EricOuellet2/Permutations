using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permutations
{
	public class PermutationResult : IComparable<PermutationResult>, IComparable
	{
		// ************************************************************************
		public EnumAlgoPermutation AlgoPermutation { get; private set; }
		public long Millisecs { get; private set; }

		// ************************************************************************
		public PermutationResult(EnumAlgoPermutation algoPermutation, long millisecs)
		{
			AlgoPermutation = algoPermutation;
			Millisecs = millisecs;
		}

		// ************************************************************************
		public int CompareTo(PermutationResult other)
		{
			return Millisecs.CompareTo(other.Millisecs);
		}

		// ************************************************************************
		public int CompareTo(object obj)
		{
			if (!(obj is PermutationResult pr))
			{
				throw new ArgumentException("obj should be a PermutationResult");	
			}

			return Millisecs.CompareTo(pr);
		}

		// ************************************************************************

	}
}
