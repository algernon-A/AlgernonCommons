// <copyright file="XMLFileUtils.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace AlgernonCommons.XML
{
    using System;
    using System.IO;
    using System.Xml.Serialization;
    using ColossalFramework;

    /// <summary>
    /// XML file utilities.
    /// </summary>
    public static class XMLFileUtils
    {
        /// <summary>
        /// Load the specified XML file.
        /// </summary>
        /// <typeparam name="TFile">XML file type.</typeparam>
        /// <param name="fileName">Filename.</param>
        public static void Load<TFile>(string fileName)
            where TFile : SettingsXMLBase
        {
            Logging.Message("loading XML file ", fileName);

            // Null check.
            if (fileName.IsNullOrWhiteSpace())
            {
                Logging.Error("invalid XML filename");
                return;
            }

            try
            {
                // Check to see if configuration file exists.
                if (File.Exists(fileName))
                {
                    // Read it.
                    using (StreamReader reader = new StreamReader(fileName))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(TFile));
                        if (!(xmlSerializer.Deserialize(reader) is TFile xmlFile))
                        {
                            Logging.Error("couldn't deserialize XML file ", fileName);
                        }
                    }
                }
                else
                {
                    Logging.Message("XML file ", fileName, " not found");
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e, "exception reading XML file ", fileName);
            }
        }

        /// <summary>
        /// Save XML file.
        /// </summary>
        /// <typeparam name="TFile">XML file type.</typeparam>
        /// <param name="fileName">Filename.</param>
        public static void Save<TFile>(string fileName)
            where TFile : SettingsXMLBase, new()
        {
            try
            {
                Logging.Message("saving XML file ", fileName);

                // Pretty straightforward.  Serialisation is within GBRSettingsFile class.
                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(TFile));
                    xmlSerializer.Serialize(writer, new TFile());
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e, "exception saving XML settings file");
            }
        }
    }
}