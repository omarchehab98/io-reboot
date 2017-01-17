# ioreboot
This project is for educational purposes only.

## Installation
[![Get it on Google Play](https://github.com/omarchehab98/ioreboot/blob/master/Images/google-play-badge.png)](https://play.google.com/store/apps/details?id=omarchehab.io)

For development, open the project using the [Unity 3D](https://unity3d.com/) game engine.

## About
The puzzle idea sprung off of designing an algorithm that progressively generates levels. IO Reboot has **infinite levels** which gets **progressesively difficult**. Every level that is generated has a possible solution. Shuffled at the start of the level, the pipes can be always be unshuffled such that the level is enclosed. A timer is ticks at the start of every level, when it ends the balls begin to move automatically shading the pipes behind them. The player has to rotate pipes to allow the balls to shade the pipes. A level is completed when every pipe has been shaded.

* Seeded random level generation produces an infinite number of levels. [Source File](https://github.com/omarchehab98/ioreboot/blob/master/Assets/Scripts/Managers/LevelManager.cs)

* Polymorphism, special tile behaviours inherit from base tile behaviour. [Source Folder](https://github.com/omarchehab98/ioreboot/tree/master/Assets/Scripts/Behaviours/Channels)

* Embeded compression by mapping tile orientation, shape, and type combinations to numbers. [Source File](https://github.com/omarchehab98/ioreboot/blob/master/Assets/Scripts/Data/Compression.cs)

* Player data is saved on the disk using Advanced Encryption Standard. [Source File](https://github.com/omarchehab98/ioreboot/blob/master/Assets/Scripts/Data/Encryption.cs)

## Development
This puzzle game was written when I was in highschool, so it lacks proper documentation.

## Legal
Google Play and the Google Play logo are trademarks of Google Inc.

[Privacy Policy](https://goo.gl/NBWcpK)
