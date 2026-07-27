# gregMod.NoCostShop

> Makes the in-game shop effectively free while preserving the checkout flow.

[![Discord](https://img.shields.io/badge/Discord-Join-5865F2?style=for-the-badge&logo=discord&logoColor=white)](https://discord.gg/greg)
[![gregFramework](https://img.shields.io/badge/gregFramework-Website-blue?style=for-the-badge)](https://gregframework.eu)
[![Version](https://img.shields.io/badge/Version-1.0.1-orange?style=for-the-badge)]()
[![GameVersion](https://img.shields.io/badge/Game%20Version-1.1.0-yellow?style=for-the-badge)]()
[![Unity](https://img.shields.io/badge/Unity-6000.4.12f1-black?style=for-the-badge&logo=unity&logoColor=white)]()

## Links

- **Repository:** [github.com/mleem97/gregMod.NoCostShop](https://github.com/mleem97/gregMod.NoCostShop)
- **Discord / Support:** [discord.gg/greg](https://discord.gg/greg)
- **Website:** [gregframework.eu](https://gregframework.eu)

It removes both XP and coin costs from purchases while keeping the checkout system fully functional through a compatibility workaround.

The mod integrates with the in-game modular menu system via [DataCenter-RustBridge](https://github.com/Joniii11/DataCenter-RustBridge) for easy configuration.

---

## ✨ Features

- Disable XP unlock costs
- Disable coin prices
- Shop items appear to cost `0`
- Cart items and total display `0`
- Prevents XP and coin deductions
- Restores original values when disabled
- Real-time behavior (applies continuously)
- In-game configuration menu

---

## 📦 Requirements

Before installing, make sure you have:

- **[MelonLoader (latest version)](https://melonwiki.xyz/#/)**
- **[DataCenter-RustBridge](https://github.com/Joniii11/DataCenter-RustBridge)**

---

## 📥 Installation

1. Install **MelonLoader** into *Data Center*
2. Install **DataCenterModLoader**
3. Download the latest release of **NoCostShop**
4. Place `gregMod.NoCostShop.dll` into your `Mods` folder:

```
GameFolder/
└── Mods/
    └── gregMod.NoCostShop.dll
```

5. Launch the game

---

## ⚙️ Configuration

All settings are available in-game through the modular menu.

### Available Options

- **DisableXpCost** *(default: true)*  
  Removes XP requirements for all shop purchases.

- **DisableCoinCost** *(default: true)*  
  Removes coin prices from all shop purchases.

---

## 🧠 How It Works

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

## 🎯 Scope

This is a **shop-focused quality-of-life mod**.

It does **not**:
- Modify unrelated gameplay systems
- Rebalance the entire economy
- Permanently edit save data

---

## ⚠️ Notes

- Designed for convenience and sandbox-style gameplay
- Keeps purchase flow intact while making items free
- Uses runtime patches rather than invasive changes

---

## 🛠️ Development

- Mod Loader: [MelonLoader](https://melonwiki.xyz/#/)
- Framework: .NET 6  
- Language: C#  

## Project Structure

```text
gregMod.NoCostShop/
├── src/                      # Mod source code
│   ├── Core.cs
│   ├── ItemData.cs
│   ├── Enums/
│   └── Options/
├── references/               # Game, MelonLoader, and RustBridge assemblies
├── docs/
├── gregMod.NoCostShop.csproj
├── README.md
└── LICENSE
```

---

## 📜 License

MIT License © 2026 Neox; gregMod integration by TeamGreg Modding
