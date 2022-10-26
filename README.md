# ModEngine

> A reasonably generic framework for applying arbitrary edits to files, originally designed for merging game mods

## What is it?

This is a library/framework used for creating "patches" to binary files. Ultimately, it's just a way of defining, loading and applying any number of generic modifications to a binary file.

ModEngine was originally designed for, and is likely most applicable to, scenarios like game modding where multiple declared edits to binary files can be hard to track, easy to get wrong and often impossible to combine. Ideally though, this should be usable in any scenario where you need to load and apply arbitrary edits to binary files.

Notably, this project is just a big chunk of the [HexPatch](https://github.com/agc93/hexpatch) and [Project Sicario](https://github.com/agc93/project-sicario) projects extracted into its own more generic project. This is designed such that the edits themselves can be self-contained in an engine. For example, HexPatch can be used to apply hex-level edits, or the Sicario engine can be used to edit UE4 asset files etc. An engine is what does the actual modification, ModEngine just acts as a high-level framework to make the whole process easier for authors.

This project also includes `ModEngine.Build`, an additional companion library for `ModEngine` that uses `BuildEngine` to simplify semi-isolated repeatable environments for these edits, allowing a reasonably easy way to load, prepare, and apply edits like you would a build.

This project is not designed to be a modding tool out of the gate, but instead a set of building blocks for you to easily create modding tools for a specific use case (or game) while using a consistent set of high-level concepts and making it easier for authors to make content without having to understand the inner workings of how it's done.

> More detailed documentation will be available in the future, after the API has stabilized. For now, Project Sicario is probably the best reference implmentation (if a very complex one).