namespace FuManchu.Tests.Binding
{
	using System.ComponentModel.Design;
	using System.Dynamic;
	using Xunit;

	public class BindingFacts
	{
		[Fact]
		public void SupportsPocoModel()
		{
			var person = new Person() { Forename = "Matt", Surname = "Abbott", Age = 30 };
			string template = "{{Forename}} {{Surname}} is {{Age}} years old";
			string expected = "Matt Abbott is 30 years old";

			var service = new HandlebarsService();
			string result = service.CompileAndRun("my-template", template, person);

			Assert.Equal(expected, result);
		}

		[Fact]
		public void SupportsAnonymousModel()
		{
			var person = new { Forename = "Matt", Surname = "Abbott", Age = 30 };
			string template = "{{Forename}} {{Surname}} is {{Age}} years old";
			string expected = "Matt Abbott is 30 years old";

			var service = new HandlebarsService();
			string result = service.CompileAndRun("my-template", template, person);

			Assert.Equal(expected, result);
		}

		[Fact]
		public void SupportsDynamicModel()
		{
			dynamic person = new { Forename = "Matt", Surname = "Abbott", Age = 30 };
			string template = "{{Forename}} {{Surname}} is {{Age}} years old";
			string expected = "Matt Abbott is 30 years old";

			var service = new HandlebarsService();
			string result = service.CompileAndRun("my-template", template, person);

			Assert.Equal(expected, result);
		}

		[Fact]
		public void SupportsDynamicEnumerableModel()
		{
			string template = "<ul>{{#each this}}<li>{{value}}</li>{{/each}}</ul>";
			dynamic model = new dynamic[] { new { value = 1 }, new { value = 2} };
			string expected = "<ul><li>1</li><li>2</li></ul>";

			var service = new HandlebarsService();
			string result = service.CompileAndRun("my-template", template, model);

			Assert.Equal(expected, result);
		}

		[Fact]
		public void SupportsDynamicExpandoObjectModel()
		{
			string template = "{{Forename}} {{Surname}}";
			dynamic model = new ExpandoObject();
			model.Forename = "Matthew";
			model.Surname = "Abbott";

			string expected = "Matthew Abbott";

			var service = new HandlebarsService();
			string result = service.CompileAndRun("my-template", template, model);

			Assert.Equal(expected, result);
		}

		[Fact]
		public void SupportsRootLookup()
		{
			var service = new HandlebarsService();
			var model = new
			            {
				            property = "Hello",
				            other = new
				                    {
					                    forename = "Matt",
					                    surname = "Abbott",
					                    job = new
					                          {
						                          title = "Developer"
					                          }
				                    }
			            };

			string template = "{{#with other}}{{@root.property}} {{forename}} {{surname}}, {{job.title}}{{/with}}";
			string expected = "Hello Matt Abbott, Developer";

			Assert.Equal(expected, service.CompileAndRun("test", template, model));
		}

		[Fact]
		public void SupportsRootLookupInParameter()
		{
			var service = new HandlebarsService();
			var model = new
			{
				property = "Hello"
			};

			string template = "{{#with @root.property}}{{this}}{{/with}}";
			string expected = "Hello";

			Assert.Equal(expected, service.CompileAndRun("test", template, model));
		}
	}

	public class Person
	{
		public string Forename { get; set; }
		public string Surname { get; set; }
		public int Age { get; set; }
	}
}