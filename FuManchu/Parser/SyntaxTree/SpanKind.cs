namespace FuManchu.Parser.SyntaxTree
{
	/// <summary>
	/// The possible kinds of span.
	/// </summary>
	public enum SpanKind
	{
		MetaCode,
		Comment,
		Expression,
		Text,
		WhiteSpace,
		Map,
		Parameter
	}
}