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

namespace GregModNoCostShop.Enums
{
    /// <summary>
    /// Identifies the configuration options exposed by the mod.
    /// </summary>
    public enum OptionType
    {
        /// <summary>
        /// Makes XP unlock requirements free by replacing deductions with the mod's sentinel value.
        /// </summary>
        DisableXpCost,

        /// <summary>
        /// Makes coin prices appear free while preserving a non-zero internal value required by the shop.
        /// </summary>
        DisableCoinCost,
    }
}
