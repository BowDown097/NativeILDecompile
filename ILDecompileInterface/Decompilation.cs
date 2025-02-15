using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;

namespace ILDecompileInterface;

public static unsafe class Decompilation
{
    [UnmanagedCallersOnly(EntryPoint = "DecompileMethod")]
    [return: NativeTypeName("char*")]
    public static nint DecompileMethod(
        [NativeTypeName("const char8_t*")] nint assemblyPathIn,
        int assemblyPathLength,
        [NativeTypeName("const char8_t*")] nint namespaceIn,
        int namespaceLength,
        [NativeTypeName("const char8_t*")] nint typeIn,
        int typeLength,
        [NativeTypeName("const char8_t*")] nint methodIn,
        int methodLength)
    {
        string assemblyPath = Encoding.UTF8.GetString((byte*)assemblyPathIn, assemblyPathLength);
        string @namespace = Encoding.UTF8.GetString((byte*)namespaceIn, namespaceLength);
        string type = Encoding.UTF8.GetString((byte*)typeIn, typeLength);
        string methodName = Encoding.UTF8.GetString((byte*)methodIn, methodLength);

        using FileStream assemblyStream = File.OpenRead(assemblyPath);
        using PEReader peReader = new(assemblyStream);

        MethodDefinitionHandle? methodHandle = peReader.FindMethod(@namespace, type, methodName);
        if (methodHandle is not null)
        {
            CSharpDecompiler decompiler = new(assemblyPath, new DecompilerSettings {
                ThrowOnAssemblyResolveErrors = false
            });

            return Utils.MarshalString(decompiler.DecompileAsString(methodHandle.Value));
        }

        return Utils.MarshalString("// Method not found.");
    }

    [UnmanagedCallersOnly(EntryPoint = "DecompileType")]
    [return: NativeTypeName("char*")]
    public static nint DecompileType(
        [NativeTypeName("const char8_t*")] nint assemblyPathIn,
        int assemblyPathLength,
        [NativeTypeName("const char8_t*")] nint namespaceIn,
        int namespaceLength,
        [NativeTypeName("const char8_t*")] nint typeIn,
        int typeLength)
    {
        string assemblyPath = Encoding.UTF8.GetString((byte*)assemblyPathIn, assemblyPathLength);
        string @namespace = Encoding.UTF8.GetString((byte*)namespaceIn, namespaceLength);
        string type = Encoding.UTF8.GetString((byte*)typeIn, typeLength);

        using FileStream assemblyStream = File.OpenRead(assemblyPath);
        using PEReader peReader = new(assemblyStream);

        TypeDefinitionHandle? typeHandle = peReader.FindType(@namespace, type);
        if (typeHandle is not null)
        {
            CSharpDecompiler decompiler = new(assemblyPath, new DecompilerSettings {
                ThrowOnAssemblyResolveErrors = false
            });

            return Utils.MarshalString(decompiler.DecompileAsString(typeHandle.Value));
        }

        return Utils.MarshalString("// Method not found.");
    }
}

