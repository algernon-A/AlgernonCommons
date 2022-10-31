// <copyright file="PrefabUtils.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace AlgernonCommons.Utils
{
    /// <summary>
    /// Prefab-related utilities.
    /// </summary>
    public static class PrefabUtils
    {
        /// <summary>
        /// Sanitises a raw prefab name for display.
        /// </summary>
        /// <param name="prefab">Prefab.</param>
        /// <returns>Cleaned display name.</returns>
        public static string GetDisplayName(PrefabInfo prefab)
        {
            // Null check.
            if (prefab?.name == null)
            {
                return "null";
            }

            // Try getting any localized name, omit any package number, and trim off any trailing _Data.
            string localizedName = prefab.GetUncheckedLocalizedTitle();
            int index = localizedName.IndexOf('.');
            return localizedName.Substring(index + 1).Replace("_Data", string.Empty);
        }

        /// <summary>
        /// Checks if this asset is a workshop asset (ie. has a workshop ID associated with it).
        /// </summary>
        /// <param name="prefab">Prefab.</param>
        /// <returns>True if this is a workshop asset, false otherwise.</returns>
        public static bool IsWorkshopAsset(PrefabInfo prefab)
        {
            // Null check.
            if (prefab?.name == null)
            {
                return false;
            }

            // Check for a package number (name contains period).
            return prefab.name.IndexOf('.') >= 0;
        }
    }
}