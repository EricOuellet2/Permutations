using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Permutations
{
	public enum EnumAction
	{
		Nothing_BestForPerformanceTest = 0,
		CountItemWithBugWithMultithreadedCode = 1,
		CountItemSafeInterlockedIncrement = 2,
		CountItemSafeSpinLock = 3,
		DumpPermutatedValuesAndCount = 4,
	}
}
