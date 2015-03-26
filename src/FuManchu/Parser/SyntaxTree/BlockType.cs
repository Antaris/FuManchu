namespace FuManchu.Parser.SyntaxTree
{
	/// <summary>
	/// Represents the possible block types.
	/// </summary>
	public enum BlockType
	{
		Document,
		Text,
		Comment,
		Tag,
		TagElement,
		Expression,
		SubExpression,
		ExpressionBody,
		Partial
	}
}