using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permutations
{
	public class PermutationResult
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

	}
}
