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
		[Description("OuelletHeap (ST, not lexico)")]
		OuelletHeap = 1,

		[Description("OuelletIndexed1 (ST, not lexico)")]
		OuelletIndexed = 2,

		[Description("OuelletIndexed2 (ST, not lexico)")]
		OuelletIndexedv2 = 4,

		[Description("OuelletIndexed3 (ST, not lexico)")]
		OuelletIndexedv3 = 8,

		//[Description("OuelletHeap - Lexicographic/Indexed/Parallelizable v4")]
		//OuelletIndexedv4 = 16,

		[Description("Huttunen (ST, lexico)")]
		SaniSinghHuttunen = 16,

		[Description("OuelletHuttunen (ST, lexico)")]
		OuelletIndexedv3HuttunenST = 32,

		[Description("SimpleVarLex (ST, lexico)")]
		SimpleVar = 64,

		[Description("SimpleVarUnsafe (unsafe, ST, not lexico)")]
		SimpleVarUnsafe = 128,

		[Description("Robinson (not lexico)")]
		ErezRobinson = 256,

		[Description("Sam (ST, lexico)")]
		Sam = 512,

		[Description("Ziezi - (adapted, ST, not lexico)")]
		Ziezi = 1024,

		[Description("Pengyang - LINQ (very slow, ST, lexico)")]
		Pengyang = 8192,

		// Any multihreaded version should be higher than 65536 (2^16)

		[Description("OuelletIndexed1 MT (MT, not lexico)")]
		OuelletIndexedMT = 131072,

		[Description("OuelletIndexed3 MT (MT, not lexico)")]
		OuelletIndexedv3MT = 262144,
		
		[Description("OuelletHuttunen MT (MT, not lexico)")]
		OuelletHuttunenMT = 524288,
	}
}
