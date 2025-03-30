<div align="center">
<h1>Sonic Heroes UTX Editor</h1>
<img src="https://raw.githubusercontent.com/Heroes-Hacking-Central/SonicHeroesUTXEditor/refs/heads/main/res/preview.jpg" align="center" />
</div>

## Dependencies:
* Requires [.NET 9 Desktop Runtime](https://aka.ms/dotnet-core-applaunch?missing_runtime=true&arch=x64&apphost_version=9.0.0&gui=true)

## Features:
* PC and GC versions supported, may work for other platforms but untested
* Dark Mode

## Credits:
* Interface by dreamsyntax, heavily edited version of [ShadowTHTextEditor](https://github.com/ShadowTheHedgehogHacking/ShadowTHTextEditor)
* .utx reverse engineering done by [Shadowth117](https://github.com/Shadowth117/PSO2-Aqua-Library/blob/master/AquaModelLibrary.Data/Sega/SonicHeroes/UTX.cs)
* Program icon by RaphaelDrewBoltman
* Uses modified version of DarkTheme by [Otiel](https://github.com/Otiel)
* Uses [Ookii.Dialogs](https://github.com/ookii-dialogs/ookii-dialogs-wpf) for dialogs

## How to Build in Release Mode:
`dotnet publish 'Sonic Heroes UTX Editor.sln' -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true`