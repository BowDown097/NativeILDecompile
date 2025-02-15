namespace ILDecompileInterface;

// syntactic sugar for expressing the desired native type
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false)]
#pragma warning disable CS9113 // Parameter is unread.
internal sealed class NativeTypeNameAttribute(string name) : Attribute
#pragma warning restore CS9113 // Parameter is unread.
{
}
