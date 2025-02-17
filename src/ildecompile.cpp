#include "ildecompile.h"
#include <memory>

#ifdef WIN32
#define WIN32_LEAN_AND_MEAN
#include <system_error>
#include <Windows.h>
#elif defined(__unix__)
#include <dlfcn.h>
#include <system_error>
#else
#include <stdexcept>
#endif

#ifdef DONT_ENFORCE_UTF8
typedef char* (*fnDecompileMethod)(const char*, int, const char*, int, const char*, int, const char*, int);
typedef char* (*fnDecompileType)(const char*, int, const char*, int, const char*, int);
#else
typedef char* (*fnDecompileMethod)(const char8_t*, int, const char8_t*, int, const char8_t*, int, const char8_t*, int);
typedef char* (*fnDecompileType)(const char8_t*, int, const char8_t*, int, const char8_t*, int);
#endif

#ifdef WIN32
using handle_t = HMODULE;
#else
using handle_t = void*;
#endif

struct HandleCloser
{
    void operator()(handle_t handle) const
    {
    #ifdef WIN32
        FreeLibrary(handle);
    #elif defined(__unix__)
        dlclose(handle);
    #endif
    }
};

using HandlePtr = std::unique_ptr<std::remove_pointer_t<handle_t>, HandleCloser>;

HandlePtr acquireHandle(const char* name)
{
#ifdef WIN32
    HandlePtr handle(LoadLibraryA(name));
    if (!handle)
        throw std::system_error(GetLastError(), std::system_category(), "Error loading interface library");
    return handle;
#elif defined(__unix__)
    HandlePtr handle(dlopen(name, RTLD_LAZY));
    if (!handle)
        throw std::system_error(errno, std::system_category(), "Error loading interface library");
    return handle;
#else
    throw std::runtime_error("Unsupported platform");
#endif
}

auto resolveFunctionPtr(handle_t handle, const char* name)
{
#ifdef WIN32
    return GetProcAddress(handle, name);
#elif defined(__unix__)
    return dlsym(handle, name);
#endif
}

// this exists to make life easier by not having to manually call data() and size() a whole bunch of times
// not using forward references and stuff here since we're using views
template<typename... Args>
auto invokeInterfaceMethod(auto func, Args... args)
{
    return std::apply(func, std::tuple_cat([](ILDecompile::StringIn arg) {
        return std::make_tuple(arg.data(), arg.size());
    }(args)...));
}

std::string ILDecompile::decompileMethod(StringIn assembly_, StringIn namespace_, StringIn type_, StringIn method_)
{
    HandlePtr handle = acquireHandle("./" ILDECOMPILEINTERFACE_NAME);
    auto decompileMethod = fnDecompileMethod(resolveFunctionPtr(handle.get(), "DecompileMethod"));
    return invokeInterfaceMethod(decompileMethod, assembly_, namespace_, type_, method_);
}

std::string ILDecompile::decompileType(StringIn assembly_, StringIn namespace_, StringIn type_)
{
    HandlePtr handle = acquireHandle("./" ILDECOMPILEINTERFACE_NAME);
    auto decompileType = fnDecompileType(resolveFunctionPtr(handle.get(), "DecompileType"));
    return invokeInterfaceMethod(decompileType, assembly_, namespace_, type_);
}
