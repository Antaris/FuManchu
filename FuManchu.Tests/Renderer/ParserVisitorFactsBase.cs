namespace FuManchu.Tests.Renderer
{
	using System.IO;
	using FuManchu.Binding;
	using FuManchu.Parser;
	using FuManchu.Renderer;
	using FuManchu.Tags;
	using FuManchu.Text;
	using Xunit;

	public abstract class ParserVisitorFactsBase
	{
		protected void RenderTest(string content, string expected, object model = null, TagProvidersCollection providers = null)
		{
			providers = providers ?? TagProvidersCollection.Default;

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

					using (var writer = new StringWriter())
					{
						var whitespaceCollapsingVisitor = new WhiteSpaceCollapsingParserVisitor();
						var visitor = new RenderingParserVisitor(writer, model, new DefaultModelMetadataProvider());

						results.Document.Accept(whitespaceCollapsingVisitor);
						results.Document.Accept(visitor);

						string result = writer.GetStringBuilder().ToString();

						Assert.Equal(expected, result);
					}
				}
			}
		}
	}
}
