# Webp2Png

## Description
Converts a WEBP image into a PNG file, as the support for WEBP files on Windows and it's software ecosystem is somewhat lackluster.  
It uses [ImageSharp](sixlabors.com/products/imagesharp/) as the image processing backend.

## Usage

Just pass the path to the WEBP file as the single argument to the executable.

```shell 
Webp2Png.exe <PATH_TO_WEBP_IMAGE>
```

## Install
The tool can be installed as a shell extension, by passing the ```-install``` argument.  
The context menu for WEBP files will have an option to convert the image to PNG.

```shell 
Webp2Png.exe -install
```

## Uninstall
The tool can be uninstalled in a similar fashion, by passing the ```-uninstall``` argument.
```shell
Webp2Png.exe -uninstall
```