// <copyright file="PatcherBase.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace AlgernonCommons.Patching
{
    using System;
    using CitiesHarmony.API;
    using HarmonyLib;

    /// <summary>
    /// Harmony patching class.
    /// </summary>
    public class PatcherBase
    {
        /// <summary>
        /// Instance reference.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Protected internal field")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Protected internal field")]
        protected internal static PatcherBase s_instance;

        // Unique Harmony ID.
        private readonly string _harmonyID;

        /// <summary>
        /// Initializes a new instance of the <see cref="PatcherBase"/> class.
        /// </summary>
        /// <param name="harmonyID">This mod's unique Harmony identifier.</param>
        public PatcherBase(string harmonyID)
        {
            // Ensure valid HarmonyID before proceeding.
            if (string.IsNullOrEmpty(harmonyID))
            {
                throw new NullReferenceException("HarmonyID cannot be null");
            }

            _harmonyID = harmonyID;
        }

        /// <summary>
        /// Gets the active instance reference.
        /// </summary>
        public static PatcherBase Instance
        {
            get
            {
                // Auto-initializing getter.
                if (s_instance == null)
                {
                    s_instance = new PatcherBase(PatcherMod.Instance.HarmonyID);
                }

                return s_instance;
            }
        }

        /// <summary>
        /// Gets this patcher instance's unique Harmony identifier.
        /// </summary>
        public string HarmonyID => _harmonyID;

        /// <summary>
        /// Gets or sets a value indicating whether patches are currently applied (true) or not (false).
        /// </summary>
        public bool Patched { get; protected set; }

        /// <summary>
        /// Apply all Harmony patches.
        /// </summary>
        public virtual void PatchAll()
        {
            // Don't do anything if already patched.
            if (!Patched)
            {
                // Ensure Harmony is ready before patching.
                if (HarmonyHelper.IsHarmonyInstalled)
                {
                    Logging.Message("deploying Harmony patches");

                    // Apply all annotated patches and update flag.
                    Harmony harmonyInstance = new Harmony(_harmonyID);
                    harmonyInstance.PatchAll();
                    Patched = true;
                }
                else
                {
                    Logging.Message("Harmony not ready");
                }
            }
        }

        /// <summary>
        /// Remove all Harmony patches.
        /// </summary>
        public virtual void UnpatchAll()
        {
            // Only unapply if patches appplied.
            if (Patched)
            {
                Logging.Message("reverting Harmony patches");

                // Unapply patches, but only with our HarmonyID.
                Harmony harmonyInstance = new Harmony(_harmonyID);
                harmonyInstance.UnpatchAll(_harmonyID);
                Patched = false;
            }
        }
    }
}