# io-reboot
Infinite progressive puzzle game. [Google Play](https://play.google.com/store/apps/details?id=omarchehab.io)

Using [Unity 3D](https://unity3d.com/) game engine.

Seeded random level generation produces an infinite number of levels. [Source File](https://github.com/omarchehab98/io-reboot/blob/master/Assets/Scripts/Managers/LevelManager.cs)

Embeded compression by mapping tile orientation, shape, and type combinations to numbers. [Source File](https://github.com/omarchehab98/io-reboot/blob/master/Assets/Scripts/Data/Compression.cs)

Polymorphism, special tiles behaviours inherit from base tile behaviour. [Source Folder](https://github.com/omarchehab98/io-reboot/tree/master/Assets/Scripts/Behaviours/Channels)