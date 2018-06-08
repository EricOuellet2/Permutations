using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permutations
{
	[Flags]
	public enum EnumAlgoPermutation
	{
		[Description("OuelletHeap Heap (ST, not lexico)")]
		OuelletHeap = 1,

		[Description("OuelletHeap Indexed1 (ST, not lexico)")]
		OuelletIndexed = 2,

		[Description("OuelletHeap Indexed2 (ST, not lexico)")]
		OuelletIndexedv2 = 4,

		[Description("OuelletHeap Indexed3 (ST, not lexico)")]
		OuelletIndexedv3 = 8,

		//[Description("OuelletHeap - Lexicographic/Indexed/Parallelizable v4")]
		//OuelletIndexedv4 = 16,

		[Description("Huttunen - Fastest ST Lexico (ST, lexico)")]
		SaniSinghHuttunen = 16,

		[Description("OuelletHuttunen - OuelletIndexed3 + Huttunen (ST, lexico)")]
		OuelletIndexedv3HuttunenST = 32,

		[Description("SimpleVarLex (ST, lexico)")]
		SimpleVar = 64,

		[Description("SimpleVarUnsafe - With unsafe code (not lexico)")]
		SimpleVarUnsafe = 128,

		[Description("Erez Robinson (not lexico)")]
		ErezRobinson = 256,

		[Description("Sam (ST, lexico)")]
		Sam = 512,

		[Description("Ziezi - slightly adapted to fit the benchmark (ST, not lexico)")]
		Ziezi = 1024,

		[Description("Pengyang - LINQ - very slow (ST, lexico)")]
		Pengyang = 8192,

		// Any multihreaded version should be higher than 65536 (2^16)

		[Description("OuelletIndexed1 MT (MT, not lexico)")]
		OuelletIndexedMT = 131072,

		[Description("OuelletIndexed3 MT (MT, not lexico)")]
		OuelletIndexedv3MT = 262144,
		
		[Description("OuelletHuttunen MT - Fastest with 2 threads or more (MT, not lexico)")]
		OuelletHuttunenMT = 524288,
	}
}
