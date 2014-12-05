namespace FuManchu.Parser.SyntaxTree
{
	using System.Collections.Generic;

	/// <summary>
	/// Compares two objects by equivalance.
	/// </summary>
	public class EquivalanceComparer : IEqualityComparer<SyntaxTreeNode>
	{
		/// <inheritdoc />
		public bool Equals(SyntaxTreeNode x, SyntaxTreeNode y)
		{
			return x.EquivalentTo(y);
		}

		/// <inheritdoc />
		public int GetHashCode(SyntaxTreeNode obj)
		{
			return obj.GetHashCode();
		}
	}
}