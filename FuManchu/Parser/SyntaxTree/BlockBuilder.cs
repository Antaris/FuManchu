namespace FuManchu.Parser.SyntaxTree
{
	using System.Collections.Generic;

	/// <summary>
	/// Represents a builder for creating block instances.
	/// </summary>
	public class BlockBuilder
	{
		/// <summary>
		/// Initialises a new instance of <see cref="BlockBuilder"/>
		/// </summary>
		public BlockBuilder()
		{
			Reset();
		}

		/// <summary>
		/// Initialises a new instance of <see cref="BlockBuilder"/>
		/// </summary>
		/// <param name="original">The original block.</param>
		public BlockBuilder(Block original)
		{
			Type = original.Type;
			Name = original.Name;
			Children = new List<SyntaxTreeNode>(original.Children);
		}

		/// <summary>
		/// Gets the set of child nodes
		/// </summary>
		public IList<SyntaxTreeNode> Children { get; private set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the block type.
		/// </summary>
		public BlockType Type { get; set; }

		/// <summary>
		/// Creates a new block instance.
		/// </summary>
		/// <returns>The block instance.</returns>
		public virtual Block Build()
		{
			return new Block(this);
		}

		/// <summary>
		/// Resets the builder.
		/// </summary>
		public virtual void Reset()
		{
			Type = BlockType.Text;
			Name = null;
			Children = new List<SyntaxTreeNode>();
		}
	}
}