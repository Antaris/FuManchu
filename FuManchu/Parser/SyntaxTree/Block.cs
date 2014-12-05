namespace FuManchu.Parser.SyntaxTree
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using FuManchu.Text;

	/// <summary>
	/// Represents a block.
	/// </summary>
	public class Block : SyntaxTreeNode
	{
		/// <summary>
		/// Initialises a new instance of <see cref="Block"/>
		/// </summary>
		/// <param name="source">The source block builder.</param>
		public Block(BlockBuilder source)
			: this(source.Type, source.Name, source.Children)
		{
			source.Reset();
		}

		/// <summary>
		/// Initialises a new instance of <see cref="Block"/>
		/// </summary>
		/// <param name="type">The block type.</param>
		/// <param name="name">The block name.</param>
		/// <param name="contents">The child contents.</param>
		protected Block(BlockType type, string name, IEnumerable<SyntaxTreeNode> contents)
		{
			Type = type;
			Name = name;
			Children = contents;

			foreach (var node in contents)
			{
				node.Parent = this;
			}
		}

		/// <summary>
		/// Gets the children of this block.
		/// </summary>
		public IEnumerable<SyntaxTreeNode> Children { get; private set; }

		/// <inheritdoc />
		public override bool IsBlock
		{
			get { return true; }
		}

		/// <inheritdoc />
		public override int Length
		{
			get { return Children.Sum(c => c.Length); }
		}

		/// <inheritdoc />
		public override SourceLocation Start
		{
			get
			{
				var child = Children.FirstOrDefault();
				if (child == null)
					return SourceLocation.Zero;
				return child.Start;
			}
		}

		/// <summary>
		/// Gets the block type.
		/// </summary>
		public BlockType Type { get; private set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		public string Name { get; set; }

		/// <inheritdoc />
		public override void Accept(ParserVisitor visitor)
		{
			visitor.VisitBlock(this);
		}

		/// <summary>
		/// Determines if the given enumerables are equal in terms of child content.
		/// </summary>
		/// <param name="left">The left enumerable.</param>
		/// <param name="right">The right enumerable.</param>
		/// <returns>True if they are considered equal, otherwise false.</returns>
		private static bool ChildrenEqual(IEnumerable<SyntaxTreeNode> left, IEnumerable<SyntaxTreeNode> right)
		{
			var leftEnum = left.GetEnumerator();
			var rightEnum = right.GetEnumerator();
			while (leftEnum.MoveNext())
			{
				if (!rightEnum.MoveNext() || !Equals(leftEnum.Current, rightEnum.Current))
				{
					return false;
				}
			}
			if (rightEnum.MoveNext())
			{
				return false;
			}
			return true;
		}

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			var other = obj as Block;
			return other != null &&
				   string.Equals(Name, other.Name, StringComparison.Ordinal) &&
				   Type == other.Type &&
				   ChildrenEqual(Children, other.Children);
		}

		/// <inheritdoc />
		public override bool EquivalentTo(SyntaxTreeNode node)
		{
			var other = node as Block;
			if (other == null || other.Type != Type || other.Name != Name)
			{
				return false;
			}
			return Enumerable.SequenceEqual(Children, other.Children, new EquivalanceComparer());
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			return HashCodeCombiner.Start()
				.Add(Type)
				.Add(Name)
				.CombinedHash;
		}

#if DEBUG
		public override string DebugToString()
		{
			var builder = new StringBuilder();
			Enumerable.Aggregate(Children, builder, (sb, node) => sb.Append(node.DebugToString()));

			return builder.ToString();
		}
#endif
	}
}