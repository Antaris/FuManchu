FuManchu
========

A .NET implementation of Handlebars, inspired by the Razor parsing framework and MVC model metadata framework. This is a pure .NET implementation, without regular expressions or wrapping any JavaScript implementations.

Current Build Status
--------------------

Master - [![Build status](https://ci.appveyor.com/api/projects/status/yl1kuo2q42i8e7j0/branch/master?svg=true)](https://ci.appveyor.com/project/Antaris/fumanchu/branch/master)

Develop - [![Build status](https://ci.appveyor.com/api/projects/status/yl1kuo2q42i8e7j0/branch/develop?svg=true)](https://ci.appveyor.com/project/Antaris/fumanchu/branch/develop)

Quick Start
-----------

First thing, install FuManchu from Nuget:

    Install-Package FuManchu

Next, add a namespace using:

    using FuManchu;

Then, you're good to go:

    Handlebars.Compile("<template-name>", "Hello {{world}}!");
    string result = Handlebars.Run("<template-name>", new { world = "World" });

There is also a shorthand:

    string result = Handlebars.CompileAndRun("<template-name>", "Hello {{world}}!", new { world = "World" });

Documentation
-------------

Documentation site is coming soon. But for now, some easy instructions are provided below.

Compiling templates
-------------------
The static `Handlebars` class provides a singleton instance of the `IHandlebarsService` type. This API is modelled on the HandlebarsJS JavaScript API, so if you are familiar with that framework, you'll be fine with FuManchu.

Let's define a template and pass it to the service:

    string template = "Hello {{name}}";
    var templateFunc = Handlebars.Compile("name", template);

The `Compile` function returns a `Func<object, string>` delegate which you can call, passing in your model to return you're result.

    string result = templateFunc(new { name = "Matt" });

This is equivalent to the following:

    string result = Handlebars.Run("name", new { name = "Matt" });

Tags: Blocks
------------
Block tags are define using the `{{#tag}}{{/tag}}` syntax. There are three types of block tags; built-in tags, implicit tags and helper tags.

**Built-ins: if, elseif, else**
You can use the `if`, `elseif`, `else` tags to provide conditional logic to your templates.

    {{#if value}}
        True
    {{/if}}

    {{#if value}}
        True
    {{else}}
        False
    {{/if}}

    {{#if value1}}
        Value 1
    {{#elseif value2}}
        Value 2
    {{else}}
       None
    {{/if}}

We resolve the truthfulness of values using the same semantics of *truthy/falsely* logic of JavaScript, therefore, values that are `null`, or the number zero, empty enumerables and empty strings, are all considered false. Everything else is considered true.

**Built-ins: unless, else**

