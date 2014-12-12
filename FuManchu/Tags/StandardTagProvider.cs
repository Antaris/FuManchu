namespace FuManchu.Tags
{
	using System.Collections.Generic;
	using FuManchu.Renderer;

	/// <summary>
	/// Provides the standard set of Handlebars tags.
	/// </summary>
	public class StandardTagProvider : ITagProvider
	{
		/// <inheritdoc />
		public IEnumerable<TagDescriptor> GetTags()
		{
			// Logic tags.
			yield return new TagDescriptor("if", new ConditionalBlockRenderer(), 1, maxArguments: 1, allowMappedParamters: false, hasChildContent: true);
			yield return new TagDescriptor("elseif", new ConditionalBlockRenderer(), 1, maxArguments: 1, allowMappedParamters: false, hasChildContent: true);
			yield return new TagDescriptor("else", new ConditionalBlockRenderer(), 0, maxArguments: 0, allowMappedParamters: false, hasChildContent: true);

			// Enumerator tags.
			//yield return new TagDescriptor("each", new EnumerableBlockRenderer(), 1, maxArguments: 1, allowMappedParameters: false, hasChildContent: true);
			//yield return new TagDescriptor("while", new ConditionalEnumerableBlockRenderer(), 1, maxArguments: 1, allowMappedParameters: false, hasChildContent: true);

			// Scope tags.
			//yield return new TagDescriptor("with", new ScopeBlockRenderer(), 1, maxArgumetns: 1, allowMappedParamters: false, hasChildContent: true);
		}
	}
}