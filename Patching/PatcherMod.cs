// <copyright file="PatcherMod.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace AlgernonCommons.Patching
{
    using CitiesHarmony.API;

    /// <summary>
    /// Harmony patching mod base.
    /// </summary>
    public abstract class PatcherMod : ModBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PatcherMod"/> class.
        /// </summary>
        public PatcherMod()
        {
            Instance = this;
        }

        /// <summary>
        /// Gets the active instance as PatcherMod.
        /// </summary>
        public static new PatcherMod Instance { get; private set; }

        /// <summary>
        /// Gets the mod's unique Harmony identfier.
        /// </summary>
        public abstract string HarmonyID { get; }

        /// <summary>
        /// Called by the game when the mod is enabled.
        /// </summary>
        public override void OnEnabled()
        {
            // Apply Harmony patches via Cities Harmony.
            // Called here instead of OnCreated to allow the auto-downloader to do its work prior to launch.
            HarmonyHelper.DoOnHarmonyReady(ApplyPatches);

            base.OnEnabled();
        }

        /// <summary>
        /// Called by the game when the mod is disabled.
        /// </summary>
        public virtual void OnDisabled()
        {
            // Unapply Harmony patches via Cities Harmony.
            if (HarmonyHelper.IsHarmonyInstalled)
            {
                RemovePatches();
            }
        }

        /// <summary>
        /// Apply Harmony patches.
        /// </summary>
        protected virtual void ApplyPatches() => PatcherBase.Instance?.PatchAll();

        /// <summary>
        /// Remove Harmony patches.
        /// </summary>
        protected virtual void RemovePatches() => PatcherBase.Instance?.UnpatchAll();
    }
}