project(example-project LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

add_executable(example-project main.cpp)

include(FetchContent)
FetchContent_Declare(nowide
    GIT_REPOSITORY https://github.com/boostorg/nowide.git
    GIT_TAG standalone)
FetchContent_MakeAvailable(nowide)

# i don't think this normally has to be done, but because this is a child project of the library, we have to
add_custom_command(TARGET example-project POST_BUILD
    COMMENT "Linking C# library"
    COMMAND ${CMAKE_COMMAND} -E create_symlink $<TARGET_FILE_DIR:NativeILDecompile>/${ILDECOMPILEINTERFACE_NAME} ${ILDECOMPILEINTERFACE_NAME}
    VERBATIM)

target_link_libraries(example-project PRIVATE NativeILDecompile nowide)
