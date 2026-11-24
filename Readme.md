# Webp2Png

## Description
Converts a WEBP image into a PNG file, as the support for WEBP files on Windows and its software ecosystem is somewhat lackluster.
It uses [ImageSharp](https://sixlabors.com/products/imagesharp/) as the image processing backend.

The PNG is written next to the source file, and the WEBP file is deleted after a successful conversion.

## Download
Get `Webp2Png.exe` from the [latest release](https://github.com/berndt-simon/Webp2Png/releases/latest).
It is a single native executable (Native AOT, Windows x64); no .NET runtime needs to be installed.

## Usage

Pass the path to the WEBP file as the single argument to the executable.

```shell
Webp2Png.exe <PATH_TO_WEBP_IMAGE>
```

The exit code is `0` on success and `-1` if the file doesn't exist or the conversion fails.
`Webp2Png.exe --help` lists all commands, `Webp2Png.exe --version` prints the version.

## Install
The tool can be installed as a shell extension with the `install` command.
The context menu for WEBP files then has an entry "Convert to Png".

```shell
Webp2Png.exe install
```

The entry points to the executable's current location, so place it where it will stay before installing.
Writing the registry entry may require an elevated (administrator) prompt.

## Uninstall
The `uninstall` command removes the context menu entry again.

```shell
Webp2Png.exe uninstall
```

## Building
Requires the .NET 11 SDK (pinned in `global.json`). Building the native executable additionally needs
the Visual Studio C++ build tools ("Desktop development with C++").

```shell
dotnet build
dotnet publish Webp2Png/Webp2Png.csproj -c Release -r win-x64 -p:PublishAot=true
```

The executable ends up in `Webp2Png/bin/Release/net11.0-windows/win-x64/publish/`.
Set the `SIXLABORS_LICENSE_KEY` environment variable to build with an ImageSharp license key.

## Releasing
The version comes from git tags through [MinVer](https://github.com/adamralph/minver).
Every push to `master` is built by GitHub Actions; a commit tagged `vX.Y.Z` (or `vX.Y.Z-rc.N` for a prerelease)
is published as a GitHub release.

```shell
git tag v8.2.0
git push --atomic origin master v8.2.0
```
