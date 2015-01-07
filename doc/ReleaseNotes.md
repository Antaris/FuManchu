#### 1.2.0
* Added support for html escaping using {{&...}} experssions. These are equivalent to {{{...}}} expressions.
* Renamed RawString to SafeString to align with HandlebarsJS API, and moved to base FuManchu namespace.
* Fixed issue with Write method for SyntaxTreeRenderer<T>.Write method not actually considering the RenderContext.EscapeEncoding value.

#### 1.1.0
* Added support for block and expression helpers.
* Added support for @root lookups in expressions.

#### 1.0.0
* Initial release of FuManchu