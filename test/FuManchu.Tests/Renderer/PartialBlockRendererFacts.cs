﻿namespace FuManchu.Tests.Renderer
{
	using Xunit;

	public class PartialBlockRendererFacts : ParserVisitorFactsBase
	{
		[Fact]
		public void CanRenderPartial()
		{
			HandlebarsService.RegisterPartial("body", "Hello World");

			string template = "{{>body}}";
			string expected = "Hello World";

			RenderTest(template, expected);
		}

		[Fact]
		public void CanRenderPartialWithCurrentModel()
		{
			HandlebarsService.RegisterPartial("body", "{{forename}} {{surname}}");

			string template = "{{>body}}";
			string expected = "Matthew Abbott";

			RenderTest(template, expected, new { forename = "Matthew", surname = "Abbott" });
		}

		[Fact]
		public void CanRenderPartialWithChildModel()
		{
			HandlebarsService.RegisterPartial("body", "{{forename}} {{surname}}");

			string template = "{{>body person}}";
			string expected = "Matthew Abbott";

			RenderTest(template, expected, new { person = new { forename = "Matthew", surname = "Abbott" }});
		}
	}
}
