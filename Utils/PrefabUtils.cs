// <copyright file="PrefabUtils.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace AlgernonCommons.Utils
{
    /// <summary>
    /// Prefab-related utilities.
    /// </summary>
    internal static class PrefabUtils
    {
        /// <summary>
        /// Sanitises a raw prefab name for display.
        /// </summary>
        /// <param name="prefab">Original (raw) prefab.</param>
        /// <returns>Cleaned display name.</returns>
        internal static string GetDisplayName(PrefabInfo prefab)
        {
            // Null check.
            if (prefab?.name == null)
            {
                return "null";
            }

            // Otherwise, try getting any localized name, omit any package number, and trim off any trailing _Data.
            string localizedName = prefab.GetUncheckedLocalizedTitle();
            int index = localizedName.IndexOf('.');
            return localizedName.Substring(index + 1).Replace("_Data", string.Empty);
        }
    }
}