**Markdown format, no emoji, and no hyphens**

---

# Unity URP Texture Packer

A simple Unity Editor tool that packs Metallic and Roughness textures into a single Unity compatible texture for use with the URP Lit shader.

---

## Purpose

Unity URP does not support standalone Roughness maps.
Instead, Unity expects:

Smoothness stored in the ALPHA channel of the Metallic texture.

This tool automates that conversion process directly inside the Unity Editor.

---

## What It Does

Given two input textures:

1. Metallic map
2. Roughness map

The tool generates a new packed texture with the following layout:

R channel contains Metallic
G channel contains Metallic
B channel contains Metallic
A channel contains inverted Roughness, which becomes Smoothness

The result is a texture that works correctly with the Unity URP workflow.

---

## Installation

1. Create an Editor folder inside your Unity project if one does not already exist

Assets/Editor/

2. Place the script file named URPTexturePacker.cs inside that Editor folder

3. Unity will automatically compile and enable the tool

---

## How To Use

### Step 1: Open the Tool

From the Unity top menu select:

Tools then URP Texture Packer

---

### Step 2: Assign Textures

In the tool window:

1. Drag your Metallic texture into the Metallic field
2. Drag your Roughness texture into the Roughness field

---

### Step 3: Pack the Textures

Press the button labeled:

Pack Textures

You will be asked to choose a location and name for the output texture file.

Example output name:

MyAsset_MetallicSmoothness.png

---

## Result

The tool creates a new texture that contains:

Metallic data in the RGB channels
Smoothness data in the Alpha channel

The import settings for the new texture are automatically configured for proper Unity URP usage.

---

# Using the Packed Texture in Unity

After creating the packed texture, configure your material as follows.

### Material Settings

Shader set to URP/Lit
Workflow Mode set to Metallic
Smoothness Source set to Metallic Alpha

### Texture Assignment

Base Map receives the Albedo texture
Normal Map receives the Normal texture
Metallic Map receives the newly generated packed texture

---

## Why This Tool Is Necessary

Blender and most 3D applications use a Metallic and Roughness workflow.

Unity URP instead uses a Metallic and Smoothness workflow.

This tool converts between those two standards automatically so that assets exported from Blender work correctly in Unity without manual image editing.

---

## Features

Simple and lightweight editor interface
One click packing process
Automatic inversion of roughness values
Automatic handling of texture readability
Automatic configuration of import settings
No external software required

---

## Requirements

Unity 2021 or newer
Universal Render Pipeline project
Input textures must have identical resolution

---

## Technical Notes

The tool temporarily enables Read and Write access on the selected textures if required.
Original import settings are restored after the packing process is complete.
Output textures are automatically set to linear color space with correct alpha handling.

---

## Possible Future Improvements

Ideas for later versions include:

Batch processing of multiple texture sets
Automatic detection of texture pairs by file name
Optional Ambient Occlusion channel packing
Live preview window
Drag and drop support

---

## License

Free to use and modify in any Unity project.
