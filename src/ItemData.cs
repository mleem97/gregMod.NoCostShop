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

using Il2Cpp;

namespace GregModNoCostShop
{
    /// <summary>
    /// Stores the original cost data for a shop item so the mod can restore the game's values
    /// after a configuration toggle.
    /// </summary>
    public struct ItemData
    {
        /// <summary>
        /// Unique identifier used to match cached data back to the corresponding shop item.
        /// </summary>
        public string Guid;

        /// <summary>
        /// Original XP requirement before the mod rewrites the item to behave as free.
        /// </summary>
        public int XpToUnlock;

        /// <summary>
        /// Original coin price before the mod rewrites the item to behave as free.
        /// </summary>
        public int Price;

        /// <summary>
        /// Copies the original values from a live shop item into a lightweight cache entry.
        /// </summary>
        /// <param name="shopItem">The shop item whose original data should be preserved.</param>
        public ItemData(ShopItem shopItem)
        {
            Guid = shopItem.guid;
            XpToUnlock = shopItem.shopItemSO.xpToUnlock;
            Price = shopItem.shopItemSO.price;
        }
    }
}
