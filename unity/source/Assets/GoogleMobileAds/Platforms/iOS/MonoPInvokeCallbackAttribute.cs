using System;

// This attribute is used on static functions and it allows Mono's Ahead of Time Compiler
// to generate the code necessary to support native iOS code calling back into C# code.
public sealed class MonoPInvokeCallbackAttribute : Attribute
{
    public MonoPInvokeCallbackAttribute(Type type) {}
}
