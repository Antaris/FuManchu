namespace FuManchu.Tests.Parser
{
	using System.IO;
	using System.Text;
	using FuManchu.Parser;
	using FuManchu.Parser.SyntaxTree;
	using FuManchu.Text;
	using T = FuManchu.Tokenizer.HandlebarsSymbolType;
	using Xunit;

	public class HandlebarsParserFacts
	{
		[Fact]
		public void CanParseTextDocument()
		{
			var factory = new Factory();

			ParserTest("<h1>Hello World</h1>", factory.Document(factory.Text("<h1>Hello World</h1>")));
		}

		[Fact]
		public void CanParseExpressionTag()
		{
			var factory = new Factory();

			ParserTest("{{hello}}", factory.Document(
				factory.Expression(
					factory.MetaCode("{{", T.OpenTag),
					factory.Expression(
						factory.Symbol("hello", T.Identifier)
					),
					factory.MetaCode("}}", T.CloseTag)
				)
			));
		}

		[Fact]
		public void CanParseBlockTag()
		{
			var factory = new Factory();

			ParserTest("{{#if condition}}True!{{/if}}", factory.Document(
				factory.Tag("if",
					factory.TagElement(null,
						factory.MetaCode("{{", T.OpenTag),
						factory.MetaCode("#", T.Hash),
						factory.Expression(factory.Symbol("if", T.Keyword)),
						factory.WhiteSpace(1),
						factory.Parameter("condition"),
						factory.MetaCode("}}", T.CloseTag)
					),
					factory.Text("True!"),
					factory.TagElement("if",
						factory.MetaCode("{{", T.OpenTag),
						factory.MetaCode("/", T.Hash),
						factory.Expression(factory.Symbol("if", T.Keyword)),
						factory.MetaCode("}}", T.CloseTag)
					)
				)
			));
		}

		private void ParserTest(string content, Block document)
		{
			var output = new StringBuilder();
			using (var reader = new StringReader(content))
			{
				using (var source = new SeekableTextReader(reader))
				{
					var errors = new ParserErrorSink();
					var parser = new HandlebarsParser();

					var context = new ParserContext(source, parser, errors);
					parser.Context = context;

					parser.ParseDocument();

					var results = context.CompleteParse();

					var comparer = new EquivalanceComparer(output, 0);

					Assert.True(comparer.Equals(document, results.Document), output.ToString());
				}
			}
		}
	}
}