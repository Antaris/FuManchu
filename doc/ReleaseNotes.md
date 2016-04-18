#### 1.4.0
* Added support for compiling against CoreCLR using `net451` and `dotnet5.4` TFMs
* Added support for resolving unknown expression values using the `UnknownValueResolver` delegate.
* Changed `Func<object, string>` instances to `HandlebarTemplate` to simplify the delegate types used for templates.
* Changed `Func<RenderContext, string>` instances to `HandlebarPartialTemplate` to simplify the delegate types used for partial templates.
* Changed `Func<HelperOptions, string>` instances to `HandlebarHelper` to simplify the delegate types used for helpers.

#### 1.3.0
* Added missing Handlebars.RegisterHelper method

#### 1.2.0
* Added support for html escaping using {{&...}} experssions. These are equivalent to {{{...}}} expressions.
* Renamed RawString to SafeString to align with HandlebarsJS API, and moved to base FuManchu namespace.
* Fixed issue with Write method for SyntaxTreeRenderer<T>.Write method not actually considering the RenderContext.EscapeEncoding value.

#### 1.1.0
* Added support for block and expression helpers.
* Added support for @root lookups in expressions.

#### 1.0.0
* Initial release of FuManchu