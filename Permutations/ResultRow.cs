using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permutations
{
	public class ResultRow
	{
		private static int rowRootIndex = 0;

		private readonly int _rowIndex = 0;
		public string Result { get; set; }

		public ResultRow(string result)
		{
			Result = result;
			_rowIndex = rowRootIndex++;
		}

		public override string ToString()
		{
			return Result;
		}

		public override bool Equals(object obj)
		{
			ResultRow rowResult = obj as ResultRow;
			if (rowResult == null)
			{
				throw new ArgumentException($"obj should be a {nameof(ResultRow)}");
			}

			return _rowIndex == rowResult._rowIndex;
		}

		public override int GetHashCode()
		{
			return _rowIndex;
		}
	}
}
