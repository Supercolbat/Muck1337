<h1 align="center">Muck1337</h1>
<p align="center">
    <img src="https://img.shields.io/github/v/tag/Supercolbat/Muck1337?label=Version&style=for-the-badge">
    <img src="https://img.shields.io/badge/Supports-Muck%20v4-blue?style=for-the-badge">
</p>

Muck1337 is a utility mod for the game [Muck](https://store.steampowered.com/app/1625450/Muck/), a game made by [Dani](https://store.steampowered.com/search/?developer=Dani)[Dev](https://www.youtube.com/channel/UCIabPXjvT5BVTxRDPCBBOOQ).

## :package: Installation
Before downloading the mod, you first need to set up BepInEx. This process is well documented on the [BepInEx wiki](https://docs.bepinex.dev/master/articles/user_guide/installation/unity_mono.html).

> **TIP**: To find Muck's root folder, go into Steam, right click on Muck from your Library, and click on `Manage > Browse local files`. That will open up the folder using your default file browser. I recommend that you create a copy of this folder so that you get easy access to both the vanilla and modded Mucks.

Once you set this up, you can then proceed to the next step.

Go to the [latest release](https://github.com/Supercolbat/Muck1337/releases/latest) and download the `Muck1337.dll`. Then, drop the dll into the `BepInEx/plugins/`  folder and simply launch the game from there!

## :wrench: Compiling
**Prerequisites**

Depending on your platform, you will need to download either the [.NET Framework](https://dotnet.microsoft.com/download/dotnet) (Windows) or the [.NET Core](https://docs.microsoft.com/en-us/dotnet/core/install/linux) (Unix-based systems).

**Gathering Resources**

Clone the repository either by downloading a .zip archive of it and extracting it or by using the `git` tool in the terminal.
```
git clone https://github.com/Supercolbat/Muck1337
```
You can also go to the development branch if you would like the latest, experimental features. However, this will come with the possibility that some things may not work as intended.
```
git checkout development
```
From there, go inside the new `Muck1337` directory, and create a folder called `Libraries`. The DLLs that you will need to copy into this folder are the following:

**Unity DLLs**

These DLLs can be found inside of Muck's game folder. Navigate to `Muck_Data/Managed/` to find the following DLLs.

* `Assembly-CSharp.dll`
* `UnityEngine.dll`
* `UnityEngine.AudioModule.dll`
* `UnityEngine.CoreModule.dll`
* `UnityEngine.IMGUIModule.dll`
* `UnityEngine.InputLegacyModule.dll`
* `UnityEngine.ParticleSystemModule.dll`
* `UnityEngine.PhysicsModule.dll`
* `UnityEngine.UI.dll`

**BepInEx DLLs**

If you haven't already extracted the BepInEx folder into the Muck game directory, you can download BepInEx [here](https://github.com/BepInEx/BepInEx). Otherwise, these DLLs can be found in the `BepInEx/core/` folder from Muck's game directory.

* `BepInEx.dll`
* `BepInEx.Harmony.dll`
* `0Harmony.dll`

**Compiling**
This is by far the easiest part of this process. Open a terminal and navigate to the root directory of Muck1337 where the `Muck1337.sln` file is located and type in the following command:
```
dotnet build --configuration Release
```

:tada: Congratulations you have successfully completed what took me 3 days to figure out. From here you can copy the Muck1337 DLL from `Muck1337/Muck1337/bin/Release/net48/Muck1337.dll` and paste it into the `BepInEx/plugins/` folder in the Muck directory.

## :memo: Notes
If for *some* reason something does not work, then please leave an issue. I cannot guarantee that I will find what the problem was, but I will at least be aware that something unintended is happening.

For those compiling, the reason for you having to manually drag the necessary DLLs as opposed to me just including them is for you is because located in `Assembly-CSharp.dll` is all the game code for Muck and that can be a big no-no for me if I distribute this code without explicit permission from DaniDev. The same applies for the other DLLs.

If anyone plans on contributing to this project, if you are building using the dotnet cli, you can exclude the `--configuration Release` to build in Debug mode. This will, however, change the path of the output dll to `.../bin/Debug/...`.
