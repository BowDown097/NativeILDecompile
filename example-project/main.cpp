#include "ildecompile.h"
#include <boost/nowide/args.hpp>
#include <iostream>
#include <vector>

int main(int argc, char* argv[])
{
    if (argc < 4)
    {
        std::cout << "USAGE: " << argv[0] << " [assembly] [namespace] [type] <method>" << std::endl;
        return EXIT_SUCCESS;
    }

    boost::nowide::args _(argc, argv); // ensure args are UTF-8 encoded
    std::vector<std::string_view> args(argv, argv + argc);

    std::u8string assembly_(args[1].begin(), args[1].end());
    std::u8string namespace_(args[2].begin(), args[2].end());
    std::u8string type_(args[3].begin(), args[3].end());
    std::u8string method_ = argc > 4 ? std::u8string(args[4].begin(), args[4].end()) : std::u8string();

    if (method_.empty())
        std::cout << ILDecompile::decompileType(assembly_, namespace_, type_);
    else
        std::cout << ILDecompile::decompileMethod(assembly_, namespace_, type_, method_);
}
