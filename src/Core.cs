/*
MIT License

Copyright (c) 2026 Neox

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to do so, subject to the
following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using DataCenterModLoader;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using GregModNoCostShop.Enums;
using GregModNoCostShop.Options;

[assembly: MelonInfo(typeof(GregModNoCostShop.Core), "gregMod.NoCostShop", "1.0.1", "TeamGreg Modding (Neox / mleem97)", "https://github.com/mleem97/gregMod.NoCostShop")]
[assembly: MelonAdditionalDependencies("DataCenterModLoader")]
[assembly: MelonGame("Waseku", "Data Center")]

namespace GregModNoCostShop
{
    /// <summary>
    /// Main MelonLoader entry point for the mod. Rewrites shop XP and coin costs so items
    /// can be treated as free while preserving enough in-game value for the order flow to work.
    /// </summary>
    public class Core : MelonMod
    {
        public const string ModName = "gregMod.NoCostShop";
        
        private const string Author = "TeamGreg Modding (Neox / mleem97)";
        
        private const string Version = "1.0.1";

        /// <summary>
        /// Internal replacement XP value used when XP costs are disabled.
        /// Unlike coin prices, XP can safely be set to 0.
        /// If this is set to 1, users are locked out of ALL items on a fresh save, rendering the mod useless.
        /// </summary>
        private const int DisabledXpCost = 0;

        /// <summary>
        /// Internal replacement coin value used when prices are disabled.
        /// The game does not allow placing a shop order priced at 0, so the real price stays at 1
        /// while the displayed text is rewritten to show 0 to the player.
        /// </summary>
        private const int DisabledPriceCost = 1;

        #region Scene Constants

        /// <summary>
        /// Build index for the main menu scene, used to clear scene-specific references.
        /// </summary>
        private const int MainMenuSceneBuildIndex = 0;

        /// <summary>
        /// Build index for the main gameplay scene, used to know when game systems are available.
        /// </summary>
        private const int BaseSceneBuildIndex = 1;

        #endregion

        /// <summary>
        /// Tracks all observed shop item UI instances so their values and labels can be refreshed.
        /// </summary>
        internal static List<ShopItem> shopItems = new();

        /// <summary>
        /// Tracks cart item UI instances so displayed prices can be forced to 0 when needed.
        /// </summary>
        internal static List<ShopCartItem> cartItems = new();

        /// <summary>
        /// Stores each item's original values keyed by GUID so the mod can restore them later.
        /// </summary>
        internal static Dictionary<string, ItemData> itemDict = new();

        /// <summary>
        /// Cached computer shop instance used to update the cart total label.
        /// </summary>
        internal static ComputerShop computerShop;

        /// <summary>
        /// Original XP label format captured from the game UI, with digits replaced by a format placeholder.
        /// </summary>
        public static string originalXpText = null;

        /// <summary>
        /// Original price label format captured from the game UI, with digits replaced by a format placeholder.
        /// </summary>
        public static string originalPriceText = null;

        /// <summary>
        /// Remembers the last XP option state so the mod only rewrites items when the setting changes.
        /// </summary>
        private bool _lastDisableXpCost = false;

        /// <summary>
        /// Remembers the last coin option state so the mod only rewrites items when the setting changes.
        /// </summary>
        private bool _lastDisableCoinCost = false;

        /// <summary>
        /// Used to execute the shop logic each time the scene change.
        /// </summary>
        private bool _hasRanOnce = false;

        /// <summary>
        /// Registers mod metadata and initializes the configuration options exposed to players.
        /// </summary>
        public override void OnInitializeMelon()
        {
            ModConfigSystem.SetModInfo(ModName, Author, Version);
            OptionsManager.Instance.InitializeOptions();

            _lastDisableXpCost = !OptionsManager.Instance.GetConfigOptionValue<bool>(OptionType.DisableXpCost);
            _lastDisableCoinCost = !OptionsManager.Instance.GetConfigOptionValue<bool>(OptionType.DisableCoinCost);
        }

        /// <summary>
        /// Captures the original XP text template from the first locked shop item so numeric values
        /// can be replaced without losing the game's surrounding text formatting.
        /// </summary>
        private void SetOriginalXpText()
        {
            if (originalXpText is not null)
                return;

            foreach (ShopItem item in shopItems)
            {
                if (!item.isUnlocked && originalXpText is null)
                {
                    var text = item.txtXpToUnlock.text;
                    var xp = new string(text.Where(char.IsDigit).ToArray());

                    originalXpText = text.Replace(xp, "{0}");
                }
            }
        }

        /// <summary>
        /// Captures the original price text template from the shop UI so numeric values can be swapped
        /// without hardcoding the game's localization or formatting.
        /// </summary>
        private void SetOriginalPriceText()
        {
            if (originalPriceText is not null)
                return;

            foreach (ShopItem item in shopItems)
            {
                if (originalPriceText is null)
                {
                    var text = item.txtPrice.text;
                    var price = new string(text.Where(char.IsDigit).ToArray());

                    originalPriceText = text.Replace(price, "{0}");
                }
            }
        }

        /// <summary>
        /// Updates the XP unlock cost for a single shop item based on the current configuration state.
        /// </summary>
        /// <param name="shopItem">The shop item UI instance to synchronize.</param>
        private void UpdateShopItemXpToUnlock(ShopItem shopItem)
        {
            if (shopItem is null)
                return;

            var currentDisableXpCost = OptionsManager.Instance.GetConfigOptionValue<bool>(OptionType.DisableXpCost);

            if (currentDisableXpCost && !shopItem.isUnlocked)
                shopItem.shopItemSO.xpToUnlock = DisabledXpCost;
            else if (itemDict.ContainsKey(shopItem.guid))
                shopItem.shopItemSO.xpToUnlock = itemDict[shopItem.guid].XpToUnlock;
        }

        /// <summary>
        /// Updates the purchase price for a single shop item based on the current configuration state.
        /// </summary>
        /// <param name="shopItem">The shop item UI instance to synchronize.</param>
        private void UpdateShopItemPrice(ShopItem shopItem)
        {
            if (shopItem is null)
                return;

            var currentDisableCoinCost = OptionsManager.Instance.GetConfigOptionValue<bool>(OptionType.DisableCoinCost);

            if (currentDisableCoinCost)
                shopItem.shopItemSO.price = DisabledPriceCost;
            else if (itemDict.ContainsKey(shopItem.guid))
                shopItem.shopItemSO.price = itemDict[shopItem.guid].Price;
        }

        /// <summary>
        /// Refreshes the displayed shop item labels so the UI shows 0 cost while the underlying
        /// data can still use the internal sentinel value required by the game's order flow.
        /// </summary>
        /// <param name="shopItem">The shop item UI instance whose labels should be refreshed.</param>
        private void UpdateShopItemText(ShopItem shopItem)
        {
            if (shopItem is null)
                return;

            var xp = shopItem.shopItemSO.xpToUnlock;
            var price = shopItem.shopItemSO.price;

            if (xp == DisabledXpCost)
                xp = 0;

            if (price == DisabledPriceCost)
                price = 0;

            if (originalXpText is not null)
                shopItem.txtXpToUnlock.SetText(string.Format(originalXpText, xp));

            if (originalPriceText is not null)
                shopItem.txtPrice.SetText(string.Format(originalPriceText, price));
        }

        /// <summary>
        /// Forces a cart item's displayed price to 0 while keeping the actual stored value compatible
        /// with the game's checkout logic.
        /// </summary>
        /// <param name="cartItem">The cart item UI instance whose label should be refreshed.</param>
        private void UpdateCartItemText(ShopCartItem cartItem)
        {
            if (cartItem is null)
                return;

            var price = cartItem.price;

            if (price == DisabledPriceCost)
                price = 0;

            cartItem.txtPrice.SetText(price.ToString());
        }

        /// <summary>
        /// Forces the cart total label to display 0 whenever coin costs are disabled.
        /// </summary>
        private void UpdateComputerShopCartTotalText()
        {
            if (computerShop is null)
                return;

            computerShop.text_totalPrice.SetText("0");
        }

        /// <summary>
        /// Keeps the shop and cart UI synchronized with the current option state as items are created
        /// and settings are toggled during gameplay.
        /// </summary>
        public override void OnFixedUpdate()
        {
            if (!shopItems.Any())
                return;

            if (originalXpText is null)
                SetOriginalXpText();

            if (originalPriceText is null)
                SetOriginalPriceText();

            var currentDisableXpCost = OptionsManager.Instance.GetConfigOptionValue<bool>(OptionType.DisableXpCost);
            var currentDisableCoinCost = OptionsManager.Instance.GetConfigOptionValue<bool>(OptionType.DisableCoinCost);

            if (currentDisableXpCost != _lastDisableXpCost || currentDisableCoinCost != _lastDisableCoinCost || !_hasRanOnce)
            {
                _hasRanOnce = true;
                _lastDisableXpCost = currentDisableXpCost;
                _lastDisableCoinCost = currentDisableCoinCost;

                foreach (ShopItem item in shopItems)
                {
                    UpdateShopItemXpToUnlock(item);
                    UpdateShopItemPrice(item);
                    UpdateShopItemText(item);
                }
            }

            if (currentDisableCoinCost)
            {
                foreach (ShopCartItem cartItem in cartItems)
                    UpdateCartItemText(cartItem);

                UpdateComputerShopCartTotalText();
            }
        }

        /// <summary>
        /// Captures shop items as they are created so their original values can be cached and later updated.
        /// </summary>
        [HarmonyPatch(typeof(ShopItem), "Awake")]
        private static class PatchShopItemAwake
        {
            /// <summary>
            /// Stores the shop item instance and its original pricing data before the mod changes anything.
            /// </summary>
            /// <param name="__instance">The shop item being initialized by the game.</param>
            private static void Prefix(ShopItem __instance)
            {
                shopItems.Add(__instance);

                if (!itemDict.ContainsKey(__instance.guid))
                    itemDict.Add(__instance.guid, new ItemData(__instance));
            }
        }


        /// <summary>
        /// Prevents negative XP changes from charging the player when XP costs are disabled.
        /// </summary>
        [HarmonyPatch(typeof(Player), "UpdateXP")]
        private static class PatchPlayerUpdateXP
        {
            /// <summary>
            /// Rewrites XP deductions to 0 so purchases remain free while still allowing the original method to run.
            /// </summary>
            /// <param name="__instance">The player whose XP is being updated.</param>
            /// <param name="amount">The XP delta requested by the game.</param>
            /// <returns><see langword="true"/> to continue into the original game method.</returns>
            private static bool Prefix(Player __instance, ref float amount)
            {
                if (!OptionsManager.Instance.GetConfigOptionValue<bool>(OptionType.DisableXpCost))
                    return true;

                if (amount < 0)
                    amount = 0;

                return true;
            }
        }

        /// <summary>
        /// Prevents negative coin changes from charging the player when coin costs are disabled.
        /// </summary>
        [HarmonyPatch(typeof(Player), "UpdateCoin")]
        private static class PatchPlayerUpdateCoin
        {
            /// <summary>
            /// Rewrites coin deductions to 0 so purchases appear free without blocking the game's normal flow.
            /// </summary>
            /// <param name="__instance">The player whose balance is being updated.</param>
            /// <param name="_coinChhangeAmount">The requested coin delta from the game.</param>
            /// <param name="withoutSound">Whether the game should skip the usual update sound.</param>
            /// <returns><see langword="true"/> to continue into the original game method.</returns>
            private static bool Prefix(Player __instance, ref float _coinChhangeAmount, ref bool withoutSound)
            {
                if (!OptionsManager.Instance.GetConfigOptionValue<bool>(OptionType.DisableCoinCost))
                    return true;

                if (_coinChhangeAmount < 0)
                    _coinChhangeAmount = 0;

                return true;
            }
        }

        /// <summary>
        /// Tracks cart item UI instances so their displayed prices can be refreshed every frame.
        /// </summary>
        [HarmonyPatch(typeof(ShopCartItem), "UpdateDisplay")]
        private static class PatchShopCartItemUpdateDisplay
        {
            /// <summary>
            /// Stores the cart item instance after the game asks it to refresh its display.
            /// </summary>
            /// <param name="__instance">The cart item being updated by the game.</param>
            private static void Prefix(ShopCartItem __instance)
            {
                cartItems.Add(__instance);
            }
        }

        /// <summary>
        /// Captures the active computer shop instance so the cart total label can be overridden.
        /// </summary>
        [HarmonyPatch(typeof(ComputerShop), "Awake")]
        private static class PatchComputerShopAwake
        {
            /// <summary>
            /// Stores the computer shop instance when the game initializes it.
            /// </summary>
            /// <param name="__instance">The computer shop instance created by the game.</param>
            private static void Prefix(ComputerShop __instance)
            {
                computerShop = __instance;
            }
        }

        /// <summary>
        /// Resets cached scene references when the player leaves gameplay and returns to the main menu.
        /// </summary>
        /// <param name="buildIndex">Build index of the scene that was loaded.</param>
        /// <param name="sceneName">Name of the loaded scene provided by MelonLoader.</param>
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (buildIndex == MainMenuSceneBuildIndex)
            {
                // Clear scene-specific references when returning to the main menu.
                shopItems.Clear();
                cartItems.Clear();
                itemDict.Clear();
                _hasRanOnce = false;
            }
        }
    }
}
