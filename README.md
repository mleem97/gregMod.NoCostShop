# gregMod.NoCostShop

> Makes the in-game shop effectively free while preserving the checkout flow.

[![Discord](https://img.shields.io/discord/1392073682133848075?style=for-the-badge&logo=discord&logoColor=white&label=Discord)](https://discord.gg/greg)
[![gregFramework](https://img.shields.io/badge/gregFramework-Website-blue?style=for-the-badge)](https://gregframework.eu)
[![License](https://img.shields.io/badge/License-Apache%202.0-green?style=for-the-badge)](./LICENSE)
[![Version](https://img.shields.io/badge/Version-1.0.2-orange?style=for-the-badge)]()
[![GameVersion](https://img.shields.io/badge/Game%20Version-1.1.0-yellow?style=for-the-badge)]()
[![Unity](https://img.shields.io/badge/Unity-6000.4.12f1-black?style=for-the-badge&logo=unity&logoColor=white)]()

## Links

- **Repository:** [github.com/mleem97/gregMod.NoCostShop](https://github.com/mleem97/gregMod.NoCostShop)
- **Discord / Support:** [discord.gg/greg](https://discord.gg/greg)
- **Website:** [gregframework.eu](https://gregframework.eu)

## Overview

**gregMod.NoCostShop** removes both XP and coin costs from purchases while keeping the checkout system fully functional through a compatibility workaround. Configuration uses MelonLoader's native preferences and has no external menu dependency.

---

## Features

- Disable XP unlock costs
- Disable coin prices
- Shop items appear to cost `0`
- Cart items and total display `0`
- Prevents XP and coin deductions
- Restores original values when disabled
- Real-time behavior (applies continuously)
- In-game configuration menu

---

## Dependencies

Before installing, make sure you have:

- **[MelonLoader (latest version)](https://melonwiki.xyz/#/)**

---

## Installation

1. Install **MelonLoader** into *Data Center*
2. Download the latest release of **gregMod.NoCostShop**
3. Place `gregMod.NoCostShop.dll` into your `Mods` folder:

```
GameFolder/
└── Mods/
    └── gregMod.NoCostShop.dll
```

4. Launch the game

---

## Configuration

All settings are available in-game through the modular menu.

### Available Options

- **DisableXpCost** *(default: true)*  
  Removes XP requirements for all shop purchases.

- **DisableCoinCost** *(default: true)*  
  Removes coin prices from all shop purchases.

---

## How It Works

- The mod hooks into shop and player systems at runtime
- It tracks original item values (XP + coins)
- It replaces costs internally while preserving game logic
- It overrides UI text to display `0` everywhere

### Important Detail

The mod **does not set prices to true zero internally**.

Instead:
- XP unlock cost → `0` - The game can tolerate XP being 0 on the backend. `1` will lock a fresh save out of all items.
- Coin price internal value → `1`
- Coin price displayed value → `0`

This is required because the game **cannot process orders with a coin price of `0`**, and setting it directly would break checkout.

---

## Scope

This is a **shop-focused quality-of-life mod**.

It does **not**:
- Modify unrelated gameplay systems
- Rebalance the entire economy
- Permanently edit save data

---

## Notes

- Designed for convenience and sandbox-style gameplay
- Keeps purchase flow intact while making items free
- Uses runtime patches rather than invasive changes

---

## Build from Source

- Mod Loader: [MelonLoader](https://melonwiki.xyz/#/)
- Framework: .NET 6  
- Language: C#  

```bash
dotnet build -c Release
```

Release output: `bin/Release/net6.0/gregMod.NoCostShop.dll`

## Project Structure

```text
gregMod.NoCostShop/
├── src/                      # Mod source code
│   ├── Core.cs
│   ├── ItemData.cs
│   ├── Enums/
│   └── Options/
├── references/               # Current game and MelonLoader assemblies
├── docs/
├── gregMod.NoCostShop.csproj
├── README.md
└── LICENSE
```

---

## Credits

- **Original implementation:** Neox
- **gregMod integration:** [TeamGreg Modding](https://github.com/teamGregModding)

## License

This project is distributed under the **Apache License 2.0**. See [`LICENSE`](./LICENSE).
The original MIT notice is preserved in [`docs/ORIGINAL_LICENSE_MIT.txt`](./docs/ORIGINAL_LICENSE_MIT.txt).

## 🚀 Join the gregFramework Team!

### macOS Support

A native macOS version of Data Center already exists. At the moment, however, there is no implementation path available for macOS support in this mod, and I do not have access to an Apple device for development or testing. I am actively looking for contributors who can help make macOS support possible. See “Join the gregFramework Team” below.

Contributions, testing, documentation, and feedback are welcome in the [greg Discord](https://discord.gg/greg).
