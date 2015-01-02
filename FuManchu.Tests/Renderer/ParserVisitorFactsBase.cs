namespace FuManchu.Tests.Renderer
{
	using System;
	using System.IO;
	using FuManchu.Binding;
	using FuManchu.Parser;
	using FuManchu.Renderer;
	using FuManchu.Tags;
	using FuManchu.Text;
	using Xunit;

	public abstract class ParserVisitorFactsBase
	{
		private readonly Lazy<IHandlebarsService> _handlebarsService;

		protected ParserVisitorFactsBase()
		{
			_handlebarsService = new Lazy<IHandlebarsService>(CreateHandlebarsService);
		}

		protected IHandlebarsService Handlebars { get { return _handlebarsService.Value; } }

		protected virtual IHandlebarsService CreateHandlebarsService()
		{
			return new HandlebarsService();
		}

		protected void RenderTest(string content, string expected, object model = null, TagProvidersCollection providers = null)
		{
			var func = Handlebars.Compile(content);
			string result = func(model);

			Assert.Equal(expected, result);
		}
	}
}
