# ModEngine

> A reasonably generic framework for applying arbitrary edits to files, originally designed for merging game mods

## What is it?

This is a library/framework used for creating "patches" to binary files. Ultimately, it's just a way of defining, loading and applying any number of generic modifications to a binary file.

ModEngine was originally designed for, and is likely most applicable to, scenarios like game modding where multiple declared edits to binary files can be hard to track, easy to get wrong and often impossible to combine. Ideally though, this should be usable in any scenario where you need to load and apply arbitrary edits to binary files.

Notably, this project is just a big chunk of the [HexPatch](https://github.com/agc93/hexpatch) project extracted into its own more generic project. This is designed such that the edits themselves can be self-contained in an engine. For example, HexPatch can be used to apply hex-level edits, or the Sicario engine can be used to edit UE4 asset files etc. An engine is what does the actual modification, ModEngine just acts as a high-level framework to make the whole process easier for authors.

This project also includes `ModEngine.Build`, an additional companion library for `ModEngine` that uses `BuildEngine` to simplify semi-isolated repeatable environments for these edits, allowing a reasonably easy way to load, prepare, and apply edits like you would a build.