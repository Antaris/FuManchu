namespace FuManchu.Tests.Parser
{
	using System.Text;
	using FuManchu.Parser;
	using FuManchu.Parser.SyntaxTree;
	using FuManchu.Tokenizer;
	using Xunit;

	/// <summary>
	/// Provides tests for the <see cref="WhiteSpaceCollapsingParserVisitor"/>
	/// </summary>
	public class WhiteSpaceCollapsingParserVisitorFacts
	{
		[Fact]
		public void CollapsesPreviousWhiteSpace()
		{
			var factory = new Factory();
			
			// "Hello {{~test}}"
			var document = factory.Document(
				factory.Text("Hello"),
				factory.WhiteSpace(1),
				factory.Tag("test",
					factory.TagElement("test",
						factory.MetaCode("{{", HandlebarsSymbolType.OpenTag),
						factory.MetaCode("~", HandlebarsSymbolType.Tilde),
						factory.Expression(factory.Parameter("test")),
						factory.MetaCode("}}", HandlebarsSymbolType.CloseTag))));

			factory = new Factory();

			// "Hello{{~test}}"
			var expected = factory.Document(
				factory.Text("Hello"),
				factory.Tag("test",
					factory.TagElement("test",
						factory.MetaCode("{{", HandlebarsSymbolType.OpenTag),
						factory.MetaCode("~", HandlebarsSymbolType.Tilde),
						factory.Expression(factory.Parameter("test")),
						factory.MetaCode("}}", HandlebarsSymbolType.CloseTag))));

			var visitor = new WhiteSpaceCollapsingParserVisitor();
			visitor.VisitBlock(document);

			var output = new StringBuilder();
			var comparer = new EquivalanceComparer(output, 0);

			Assert.True(comparer.Equals(expected, document), output.ToString());
		}
	}
}