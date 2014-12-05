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
	}
}