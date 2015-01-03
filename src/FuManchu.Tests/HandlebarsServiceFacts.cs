namespace FuManchu.Tests
{
	using Xunit;

	public class HandlebarsServiceFacts
	{
		[Fact]
		public void CanCreateReusableTemplateDelegate()
		{
			var service = new HandlebarsService();
			var func = service.Compile("Hello {{name}}");

			Assert.Equal("Hello Matt", func(new { name = "Matt" }));
			Assert.Equal("Hello Stuart", func(new { name = "Stuart" }));
		}

		[Fact]
		public void CanCompileAndRunInOneCall()
		{
			var service = new HandlebarsService();

			Assert.Equal("Hello Matt", service.CompileAndRun("my-template", "Hello {{name}}", new { name = "Matt" }));
		}

		[Fact]
		public void CanCompileAndRunLater()
		{
			var service = new HandlebarsService();
			service.Compile("template-name", "Hello {{name}}");

			Assert.Equal("Hello Matt", service.Run("template-name", new { name = "Matt" }));
		}
	}
}