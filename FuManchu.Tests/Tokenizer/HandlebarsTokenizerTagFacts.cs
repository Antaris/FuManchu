namespace FuManchu.Tests.Tokenizer
{
	using FuManchu.Tokenizer;
	using T = FuManchu.Tokenizer.HandlebarsSymbolType;
	using Xunit;

	/// <summary>
	/// Provides tag tests for the <see cref="HandlebarsTokenizer"/>
	/// </summary>
	public class HandlebarsTokenizerTagFacts : HandlebarsTokenizerFactBase
	{
		[Fact]
		public void RecognisesTagSymbols()
		{
			TestTokenizerSymbols("{{", T.OpenTag);
			TestTokenizerSymbols("{{{", T.RawOpenTag);
			TestTokenizerSymbols("{{name}}", T.OpenTag, T.Identifier, T.CloseTag);
			TestTokenizerSymbols("{{{name}}}", T.RawOpenTag, T.Identifier, T.RawCloseTag);
		}

		[Fact]
		public void RecognisesTagSwitchCharacters()
		{
			TestTokenizerSymbols("{{#", T.OpenTag, T.Hash);
			TestTokenizerSymbols("{{/", T.OpenTag, T.Slash);
			TestTokenizerSymbols("{{!", T.OpenTag, T.Bang);
			TestTokenizerSymbols("{{>", T.OpenTag, T.RightArrow);
			TestTokenizerSymbols("{{^", T.OpenTag, T.Negate);
		}

		[Fact]
		public void RecognisesBlockTag()
		{
			TestTokenizerSymbols("{{#if condition}}", T.OpenTag, T.Hash, T.Keyword, T.WhiteSpace, T.Identifier, T.CloseTag);
		}

		[Fact]
		public void RecognisesExpressionTag()
		{
			TestTokenizerSymbols("{{person.name}}", T.OpenTag, T.Identifier, T.Dot, T.Identifier, T.CloseTag);
			TestTokenizerSymbols("{{./name}}", T.OpenTag, T.Dot, T.Slash, T.Identifier, T.CloseTag);
			TestTokenizerSymbols("{{this/name}}", T.OpenTag, T.Keyword, T.Slash, T.Identifier, T.CloseTag);
			TestTokenizerSymbols("{{../name}}", T.OpenTag, T.Dot, T.Dot, T.Slash, T.Identifier, T.CloseTag);
		}

		[Fact]
		public void RecognisesArgumentsInTag()
		{
			TestTokenizerSymbols("{{#helper model person}}", T.OpenTag, T.Hash, T.Identifier, T.WhiteSpace, T.Identifier, T.WhiteSpace, T.Identifier, T.CloseTag);
		}

		[Fact]
		public void RecognisesMapsInTag()
		{
			TestTokenizerSymbols("{{#helper one=two three=four}}", T.OpenTag, T.Hash, T.Identifier, T.WhiteSpace, T.Identifier, T.Assign, T.Identifier, T.WhiteSpace, T.Identifier, T.Assign, T.Identifier, T.CloseTag);
		}

		[Fact]
		public void RecognisesStringsInTag()
		{
			TestTokenizerSymbols(
				"{{#helper class=\"btn btn-primary\"}}",
				T.OpenTag, T.Hash, T.Identifier, T.WhiteSpace,
				T.Identifier, T.Assign, T.StringLiteral,
				T.CloseTag
			);
		}

		[Fact]
		public void RecognisesComplexTag()
		{
			TestTokenizerSymbols(
				"{{#list people class=\"person\"}}",
				T.OpenTag, T.Hash, T.Identifier, T.WhiteSpace,
				T.Identifier, T.WhiteSpace,
				T.Identifier, T.Assign, T.StringLiteral,
				T.CloseTag);
		}

		[Fact]
		public void RecognisesCommentTag()
		{
			TestTokenizer("{{!comment}}",
				S(0, 0, 0, "{{", T.OpenTag),
				S(2, 0, 2, "!", T.Bang),
				S(3, 0, 3, "comment", T.Comment),
				S(10, 0, 10, "}}", T.CloseTag)
			);
			TestTokenizer("{{!--comment--}}",
				S(0, 0, 0, "{{", T.OpenTag),
				S(2, 0, 2, "!", T.Bang),
				S(3, 0, 3, "--comment--", T.Comment),
				S(14, 0, 14, "}}", T.CloseTag)
			);
		}

		[Fact]
		public void RecognisesMultilineCommentTag()
		{
			TestTokenizer("{{!\n\ncomment text here\n\n}}",
				S(0, 0, 0, "{{", T.OpenTag),
				S(2, 0, 2, "!", T.Bang),
				S(3, 0, 3, "\n\ncomment text here\n\n", T.Comment),
				S(24, 4, 0, "}}", T.CloseTag)
			);
		}
	}
}