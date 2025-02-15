#include <string>

namespace ILDecompile
{
#ifdef DONT_ENFORCE_UTF8
using StringIn = std::string_view;
#else
using StringIn = std::u8string_view;
#endif

    std::string decompileMethod(StringIn assembly_, StringIn namespace_, StringIn type_, StringIn method_);
    std::string decompileType(StringIn assembly_, StringIn namespace_, StringIn type_);
};
