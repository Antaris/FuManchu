using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuManchu.Tests.Renderer
{
	using Xunit;

	public class PartialBlockRendererFacts : ParserVisitorFactsBase
	{
		[Fact]
		public void CanRenderPartial()
		{
			string template = "{{>body}}";
			string expected = "Hello World";

			RenderTest(template, expected);
		}
	}
}
