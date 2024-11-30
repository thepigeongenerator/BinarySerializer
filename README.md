# `ThePigeonGenerator.Utils.BinarySerializer`
Allows you to serialize and deserialize objects who's size is known to and from a byte array representing the data structure in binary.

> [!NOTE]
> **This library _only_ supports primitive value types[^1] and other value types[^2] on top of arrays of these types.<br/>**
> This has to do with this library mapping the data *directly* to binary. Reference types are dynamic by nature, so storing these in binary will need another approach.[^3][^4]

## Installation
### Windows in Visual Studio:
1. Follow the steps [here](https://learn.microsoft.com/en-us/nuget/quickstart/install-and-use-a-package-in-visual-studio#nuget-package-manager).
2. Search the package namespace seen above.
### Universal:
1. Select the [package](https://github.com/thepigeongenerator/BinarySerializer/pkgs/nuget/ThePigeonGenerator.Utils.BinarySerializer)
2. Select the version you'd like to install
3. Run the command listed above in the directory of the project you wish to install the package in.

## Contents
| uwu                                                                        | Explanation                                                                            |
| -------------------------------------------------------------------------- | -------------------------------------------------------------------------------------- |
| `byte[]` `BinarySerializer`.[`Serialize`](#serialize)<`T`>(`T` obj)        | serializes the object into it's binary representation to the buffer which is returned. |
| `void` `BinarySerializer`.[`Deserialize`](#deserialize)<`T`>(`byte[]` buf) | deserializes T from the specified buffer at `buf`.                                     |

## Additional Information
### Writing / Reading the data in a file
In C#, you can use the built-in functions defined in `File`;
```cs
byte[] data = File.ReadAllBytes("./some/filepath.bin"); // reading the data stored at the path
File.WriteAllBytes("/tmp/dontdelete.bin", data);        // writing to the binary file
```

[^1]: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-
[^2]: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/value-types
[^3]: This has been done with arrays, as they are reference types in C#. So 4 extra bytes are allocated per array to specify it's Length.
[^4]: If this is the kind of behaviour you are looking for, consider using a library like `Newtonsoft.Json` (or `Newtonsoft.Json.Bson` for binary-json).
