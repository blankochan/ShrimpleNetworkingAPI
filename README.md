# ShrimpleNetworingAPI

I do not have much to put here, this exists as my attempt at a universal networking API for Airframe Ultra by Videocult.

Currently, the only features are:
* Hiding Modded Games from vanilla players without causing disruption (primary reason I made this)
* Work in progress API for filtering out incompatible games (currently just hide them but in the future I'd like it to grey out an incompatible lobby and let you see the reason why a games was greyed out)
* Simplistic One Time property sync


# Contributing
## Pull requests are welcome!
### Building
Restore nuget packages
`dotnet restore`
And copy everything in `MelonLoader/Il2CppAssemblies/` and `MelonLoader/net6/` to a `.ref` folder (this is where i read dependencies from) 

