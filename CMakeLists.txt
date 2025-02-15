cmake_minimum_required(VERSION 3.23)
project(NativeILDecompile VERSION 1.0 LANGUAGES CXX)
add_library(NativeILDecompile)

set_target_properties(NativeILDecompile
    PROPERTIES
        CXX_STANDARD 20
        CXX_STANDARD_REQUIRED ON)

option(BUILD_EXAMPLE_PROJECT "Build the example project." OFF)

if(BUILD_EXAMPLE_PROJECT)
    add_subdirectory(example-project)
endif()

target_include_directories(NativeILDecompile PUBLIC src)

target_sources(NativeILDecompile
    PRIVATE src/ildecompile.cpp
    PUBLIC FILE_SET HEADERS FILES src/ildecompile.h)

include(cmake/DotnetPublish.cmake)
DotnetPublish(NativeILDecompile ${CMAKE_CURRENT_LIST_DIR}/ILDecompileInterface/ILDecompileInterface.csproj "Building C# library")
