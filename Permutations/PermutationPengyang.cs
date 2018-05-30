using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permutations
{
	public static class PermutationPengyang
	{
		/// <summary>
		/// This function is to show a linq way but is far less efficient
		/// From: StackOverflow user: Pengyang : http://stackoverflow.com/questions/756055/listing-all-permutations-of-a-string-integer
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
		{
			if (length == 1) return list.Select(t => new T[] { t });

			return GetPermutations(list, length - 1)
				.SelectMany(t => list.Where(e => !t.Contains(e)),
					(t1, t2) => t1.Concat(new T[] { t2 }));
		}
	}
}
