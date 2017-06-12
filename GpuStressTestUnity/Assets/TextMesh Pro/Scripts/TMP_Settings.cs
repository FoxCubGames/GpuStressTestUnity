#if !UNITY_EDITOR || FCLOG
using Debug = FC.Debug;
#else
using Debug = UnityEngine.Debug;
#endif

﻿// Copyright (C) 2014 - 2015 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;
using System.Collections;


namespace TMPro
{
    [System.Serializable]
    public class TMP_Settings : ScriptableObject
    {
        public static TMP_Settings Instance;

        // Default Text object properties
        public bool enableWordWrapping;
        public bool enableKerning;
        public bool enableExtraPadding;

        public TextMeshProFont fontAsset;

        public SpriteAsset spriteAsset;

        public TMP_StyleSheet styleSheet;


        /// <summary>
        /// Static Function to load the TMP Settings file.
        /// </summary>
        /// <returns></returns>
        public static TMP_Settings LoadDefaultSettings()
        {
            if (Instance == null)
            {
                // Load settings from TMP_Settings file
                TMP_Settings settings = Resources.Load("TMP Settings") as TMP_Settings;
                if (settings != null)
                    Instance = settings;
            }

            return Instance;
        }

    }
}
