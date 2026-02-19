# QuietFileCombine

A lightweight C# library for **file steganography** — silently embed one or more files inside a carrier file while keeping the carrier fully functional.

> Built on .NET Framework 4.5.2 | Uses Base64 encoding | Singleton pattern API

---

## How It Works

QuietFileCombine appends a hidden payload to the end of a carrier file using special boundary markers (`[COMBINE_START]` / `[COMBINE_END]`) and Base64 encoding. The carrier file (e.g. a `.flv` video) remains fully playable and functional. The embedded file(s) can be extracted at any time.

Supports **multi-layer nesting**: a carrier can hold a file that itself holds another file.

---

## Features

- Combine any two files into one without breaking the carrier
- Extend an already-combined file with additional hidden files
- Extract (split) hidden files back from a combined carrier
- Multi-layer embedding support
- Zero external dependencies — pure .NET BCL

---

## Quick Start

### 1. Add the library to your project

Reference `QuietFileCombine.dll` or include the source files directly.

### 2. Get the API instance

```csharp
using QuietFileCombine.API;

var api = QuietFileCombineAPI.GetInstance();
```

### 3. Embed a file

```csharp
// Embed Action.exe inside Video.flv — Video.flv stays playable
api.CombineDistinctFile(@"C:\Video.flv", @"C:\Action.exe");
```

### 4. Add another file to an existing combined carrier

```csharp
// Extend the combined Video.flv with a second hidden file
api.ExtendFile(@"C:\Video.flv", @"C:\Action2.exe");
```

### 5. Extract hidden files

```csharp
string outputInfo;
if (api.IsCanSplit(@"C:\Video.flv", out outputInfo))
{
    // outputInfo contains details about embedded files
    Console.WriteLine(outputInfo);
}
```

---

## API Reference

| Method | Description |
|---|---|
| `GetInstance()` | Returns the singleton instance of the API |
| `CombineDistinctFile(mainFile, additionFile)` | Embeds `additionFile` into `mainFile` |
| `ExtendFile(mainFile, extendedFile)` | Adds another file to an already-combined carrier |
| `IsAllowExtended(mainFile)` | Returns `true` if the file can be extended further |
| `IsCanSplit(mainFile, out info)` | Checks whether the carrier contains hidden files |

---

## Examples

### Basic — embed an image into a video

**Combine:**
```
video.flv + picture.png  →  video.flv  (video still plays; image is hidden inside)
```

**Split:**
```
video.flv  →  video.flv + picture.png  (original image is restored)
```

### Advanced — multi-layer nesting

**Combine:**
```
picture.png + crack.exe  →  picture.png
video.flv   + picture.png  →  video.flv
```

**Split:**
```
video.flv    →  video.flv + picture.png
picture.png  →  picture.png + crack.exe
```

---

## Project Structure

```
QuietFileCombine/
├── QuietFileCombine.sln
└── QuietFileCombine/
    ├── API/
    │   └── QuietFileCombineAPI.cs   # Core steganography logic
    ├── Program.cs                   # Usage example
    ├── QuietFileCombine.csproj
    └── App.config
```

---

## Requirements

- .NET Framework 4.5.2 or later
- Windows (uses `System.IO` and `System.Convert`)

---

## License

This project is provided for educational purposes. Use responsibly.

---