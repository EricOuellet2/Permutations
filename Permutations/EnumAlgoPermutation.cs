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
		[Description("Ouellet - Fastest ST (ST, not lexico)")]
		Ouellet = 1,

		[Description("Ouellet - Lexicographic/Indexed/Parallelizable (ST, not lexico)")]
		OuelletIndexed = 2,

		[Description("Ouellet - Lexicographic/Indexed/Parallelizable v2 (ST, not lexico)")]
		OuelletIndexedv2 = 4,

		[Description("Ouellet - Lexicographic/Indexed/Parallelizable v3 (ST, not lexico)")]
		OuelletIndexedv3 = 8,

		//[Description("Ouellet - Lexicographic/Indexed/Parallelizable v4")]
		//OuelletIndexedv4 = 16,

		[Description("Sani Singh Huttunen - Fastest ST Lexico (ST, lexico)")]
		SaniSinghHuttunen = 16,

		[Description("Mix: Ouellet Lexicographic/Indexed/Parallelizable v3 + Sani Singh Huttunen ST (ST, lexico)")]
		OuelletIndexedv3SaniSinghHuttunenST = 32,

		[Description("SimpleVar (ST, lexico)")]
		SimpleVar = 64,

		[Description("SimpleVarUnsafe - With unsafe code (not lexico)")]
		SimpleVarUnsafe = 128,

		[Description("Erez Robinson (not lexico)")]
		ErezRobinson = 256,

		[Description("Pengyang - LINQ - very slow (ST, lexico)")]
		Pengyang = 512,

		[Description("Ouellet - Lexicographic/Indexed/Parallelizable MT (MT, not lexico)")]
		OuelletIndexedMT = 1024,

		[Description("Ouellet - Lexicographic/Indexed/Parallelizable v3 MT (MT, not lexico)")]
		OuelletIndexedv3MT = 2048,
		
		[Description("Mix: Ouellet Lexicographic/Indexed/Parallelizable v3 + Sani Singh Huttunen MT - Fastest for 2 threads or more (MT, not lexico)")]
		OuelletIndexedv3SaniSinghHuttunenMT = 4096,
	}
}
