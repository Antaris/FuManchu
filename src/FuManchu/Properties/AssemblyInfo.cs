using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("FuManchu")]
[assembly: AssemblyDescription("Handlebars templating for .NET")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: Guid("c672814d-54d5-4617-b7dc-b3f4c551a055")]
[assembly: InternalsVisibleTo("FuManchu.Tests")]