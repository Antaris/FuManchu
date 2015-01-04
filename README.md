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

Documentation site is coming soon.