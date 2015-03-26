namespace FuManchu.Tests.Parser
{
	using System.IO;
	using System.Text;
	using FuManchu.Parser;
	using FuManchu.Parser.SyntaxTree;
	using FuManchu.Tags;
	using FuManchu.Text;
	using T = FuManchu.Tokenizer.HandlebarsSymbolType;
	using Xunit;

	public class SubExpressionParserFacts
	{
		[Fact]
		public void CanParseSubExpression()
		{
			var factory = new Factory();

			var document = factory.Document(
				factory.Expression(
					factory.MetaCode("{{", T.OpenTag),
					factory.ExpressionBody(
						factory.Expression(factory.Symbol("outer", T.Identifier)),
						factory.WhiteSpace(1),
						factory.SubExpression(
							factory.MetaCode("(", T.OpenParenthesis),
							factory.ExpressionBody(
								factory.Expression(factory.Symbol("inner", T.Identifier)),
								factory.WhiteSpace(1),
								factory.Parameter("value", T.Identifier)
							),
							factory.MetaCode(")", T.CloseParenthesis)
						)
					), 
					factory.MetaCode("}}", T.CloseTag)
				)
			);

			ParserTest("{{outer (inner value)}}", document);
		}

		[Fact]
		public void CanParseMultipleSubExpressions()
		{
			var factory = new Factory();

			var document = factory.Document(
				factory.Expression(
					factory.MetaCode("{{", T.OpenTag),
					factory.ExpressionBody(
						factory.Expression(factory.Symbol("outer", T.Identifier)),
						factory.WhiteSpace(1),
						factory.SubExpression(
							factory.MetaCode("(", T.OpenParenthesis),
							factory.ExpressionBody(
								factory.Expression(factory.Symbol("inner", T.Identifier)),
								factory.WhiteSpace(1),
								factory.Parameter("value", T.Identifier)
							),
							factory.MetaCode(")", T.CloseParenthesis)
						),
						factory.WhiteSpace(1),
						factory.SubExpression(
							factory.MetaCode("(", T.OpenParenthesis),
							factory.ExpressionBody(
								factory.Expression(factory.Symbol("inner", T.Identifier)),
								factory.WhiteSpace(1),
								factory.Parameter("value", T.Identifier)
							),
							factory.MetaCode(")", T.CloseParenthesis)
						)
					),
					factory.MetaCode("}}", T.CloseTag)
				)
			);

			ParserTest("{{outer (inner value) (inner value)}}", document);
		}

		[Fact]
		public void CanParseNestedSubExpressions()
		{
			var factory = new Factory();

			var document = factory.Document(
				factory.Expression(
					factory.MetaCode("{{", T.OpenTag),
					factory.ExpressionBody(
						factory.Expression(factory.Symbol("outer", T.Identifier)),
						factory.WhiteSpace(1),
						factory.SubExpression(
							factory.MetaCode("(", T.OpenParenthesis),
							factory.ExpressionBody(
								factory.Expression(factory.Symbol("inner", T.Identifier)),
								factory.WhiteSpace(1),
								factory.SubExpression(
									factory.MetaCode("(", T.OpenParenthesis),
									factory.ExpressionBody(
										factory.Expression(factory.Symbol("inner2", T.Identifier)),
										factory.WhiteSpace(1),
										factory.Parameter("value", T.Identifier)
									),
									factory.MetaCode(")", T.CloseParenthesis)
								)
							),
							factory.MetaCode(")", T.CloseParenthesis)
						)
					),
					factory.MetaCode("}}", T.CloseTag)
				)
			);

			ParserTest("{{outer (inner (inner2 value))}}", document);
		}

		private void ParserTest(string content, Block document, TagProvidersCollection providers = null)
		{
			providers = providers ?? TagProvidersCollection.Default;

			var output = new StringBuilder();
			using (var reader = new StringReader(content))
			{
				using (var source = new SeekableTextReader(reader))
				{
					var errors = new ParserErrorSink();
					var parser = new HandlebarsParser();

					var context = new ParserContext(source, parser, errors, providers);
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
