namespace FuManchu.Tests.Tokenizer
{
	using FuManchu.Tokenizer;
	using T = FuManchu.Tokenizer.HandlebarsSymbolType;
	using Xunit;

	public class SubExpressionTokenizerFacts : HandlebarsTokenizerFactBase
	{
		[Fact]
		public void CanTokenizeSubExpression()
		{
			TestTokenizer("{{outer (inner value)}}",
				S(0, 0, 0, "{{", T.OpenTag),
				S(2, 0, 2, "outer", T.Identifier),
				S(7, 0, 7, " ", T.WhiteSpace),
				S(8, 0, 8, "(", T.OpenParenthesis),
				S(9, 0, 9, "inner", T.Identifier),
				S(14, 0, 14, " ", T.WhiteSpace),
				S(15, 0, 15, "value", T.Identifier),
				S(20, 0, 20, ")", T.CloseParenthesis),
				S(21, 0, 21, "}}", T.CloseTag)
			);
		}

		[Fact]
		public void CanTokenizeMultipleSubExpression()
		{
			TestTokenizer("{{outer (inner value) one=(inner value)}}",
				S(0, 0, 0, "{{", T.OpenTag),
				S(2, 0, 2, "outer", T.Identifier),
				S(7, 0, 7, " ", T.WhiteSpace),
				S(8, 0, 8, "(", T.OpenParenthesis),
				S(9, 0, 9, "inner", T.Identifier),
				S(14, 0, 14, " ", T.WhiteSpace),
				S(15, 0, 15, "value", T.Identifier),
				S(20, 0, 20, ")", T.CloseParenthesis),
				S(21, 0, 21, " ", T.WhiteSpace),
				S(22, 0, 22, "one", T.Identifier),
				S(25, 0, 25, "=", T.Assign),
				S(26, 0, 26, "(", T.OpenParenthesis),
				S(27, 0, 27, "inner", T.Identifier),
				S(32, 0, 32, " ", T.WhiteSpace),
				S(33, 0, 33, "value", T.Identifier),
				S(38, 0, 38, ")", T.CloseParenthesis),
				S(39, 0, 39, "}}", T.CloseTag)
			);
		}

		[Fact]
		public void CanTokenizeNestedSubExpressions()
		{
			TestTokenizer("{{outer (inner (inner2 value))}}",
				S(0, 0, 0, "{{", T.OpenTag),
				S(2, 0, 2, "outer", T.Identifier),
				S(7, 0, 7, " ", T.WhiteSpace),
				S(8, 0, 8, "(", T.OpenParenthesis),
				S(9, 0, 9, "inner", T.Identifier),
				S(14, 0, 14, " ", T.WhiteSpace),
				S(15, 0, 15, "(", T.OpenParenthesis),
				S(16, 0, 16, "inner2", T.Identifier),
				S(22, 0, 22, " ", T.WhiteSpace),
				S(23, 0, 23, "value", T.Identifier),
				S(28, 0, 28, ")", T.CloseParenthesis),
				S(29, 0, 29, ")", T.CloseParenthesis),
				S(30, 0, 30, "}}", T.CloseTag)
			);
		}
	}
}