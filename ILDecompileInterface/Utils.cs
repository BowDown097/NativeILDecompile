using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;

namespace ILDecompileInterface;

internal static class Utils
{
    internal static MethodDefinitionHandle? FindMethod(
        this PEReader peReader, string @namespace, string type, string methodName)
    {
        MetadataReader metadataReader = peReader.GetMetadataReader();
        foreach (TypeDefinitionHandle typeDefHandle in metadataReader.TypeDefinitions)
        {
            TypeDefinition typedef = metadataReader.GetTypeDefinition(typeDefHandle);
            string typedefNamespace = metadataReader.GetString(typedef.Namespace);
            string typedefName = metadataReader.GetString(typedef.Name);

            foreach (MethodDefinitionHandle methodHandle in typedef.GetMethods())
            {
                MethodDefinition methoddef = metadataReader.GetMethodDefinition(methodHandle);
                string methoddefName = metadataReader.GetString(methoddef.Name);

                if (typedefNamespace == @namespace && typedefName == type && methoddefName == methodName)
                    return methodHandle;
            }
        }

        return null;
    }

   internal static TypeDefinitionHandle? FindType(this PEReader peReader, string @namespace, string type)
    {
        MetadataReader metadataReader = peReader.GetMetadataReader();
        foreach (TypeDefinitionHandle typeDefHandle in metadataReader.TypeDefinitions)
        {
            TypeDefinition typedef = metadataReader.GetTypeDefinition(typeDefHandle);
            string typedefNamespace = metadataReader.GetString(typedef.Namespace);
            string typedefName = metadataReader.GetString(typedef.Name);

            if (typedefNamespace == @namespace && typedefName == type)
                return typeDefHandle;
        }

        return null;
    }

    internal unsafe static nint MarshalString(string data)
    {
        int length = Encoding.UTF8.GetByteCount(data);
        nint ptr = Marshal.AllocHGlobal(length + 1);

        Span<byte> buf = new(ptr.ToPointer(), length);
        Encoding.UTF8.GetBytes(data, buf);

        Marshal.WriteByte(ptr, length, 0);
        return ptr;
    }
}
