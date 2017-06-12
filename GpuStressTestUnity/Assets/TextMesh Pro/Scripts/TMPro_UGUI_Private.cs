

// Copyright (C) 2014 - 2015 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

#if UNITY_4_6 || UNITY_5

using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

#pragma warning disable 0414 // Disabled a few warnings related to serialized variables not used in this script but used in the editor.
#pragma warning disable 0618 // Disabled warning due to SetVertices being deprecated until new release with SetMesh() is available.

namespace TMPro
{  
    public partial class TextMeshProUGUI
    {
        [SerializeField]
        private string m_text;

        [SerializeField]
        private TextMeshProFont m_fontAsset;
        private TextMeshProFont m_currentFontAsset;

        [SerializeField]
        private Material m_fontMaterial;
        private Material m_currentMaterial;

        [SerializeField]
        private Material m_sharedMaterial;

        [SerializeField]
        private FontStyles m_fontStyle = FontStyles.Normal;
        private FontStyles m_style = FontStyles.Normal;

        private bool m_isOverlay = false;

        [SerializeField]
        private Color32 m_fontColor32 = Color.white;

        [SerializeField]
        private Color m_fontColor = Color.white;

        [SerializeField]
        private bool m_enableVertexGradient;

        [SerializeField]
        private VertexGradient m_fontColorGradient = new VertexGradient(Color.white);

        [SerializeField]
        private Color32 m_faceColor = Color.white;

        [SerializeField]
        private Color32 m_outlineColor = Color.black;

        private float m_outlineWidth = 0.0f;

        [SerializeField]
        private float m_fontSize = 36; // Font Size
        [SerializeField]
        private float m_fontSizeMin = 0; // Text Auto Sizing Min Font Size.
        [SerializeField]
        private float m_fontSizeMax = 0; // Text Auto Sizing Max Font Size.
        [SerializeField]
        private float m_fontSizeBase = 36;
        //[SerializeField]
        //private float m_charSpacingMax = 0; // Text Auto Sizing Max Character spacing reduction.
        [SerializeField]
        private float m_lineSpacingMax = 0; // Text Auto Sizing Max Line spacing reduction.
        [SerializeField]
        private float m_charWidthMaxAdj = 0f; // Text Auto Sizing Max Character Width reduction.
        private float m_charWidthAdjDelta = 0;

        private float m_currentFontSize; // Temporary Font Size affected by tags

        [SerializeField]
        private float m_characterSpacing = 0;
        private float m_cSpacing = 0;
        private float m_monoSpacing = 0;

        [SerializeField]
        private float m_lineSpacing = 0;
        private float m_lineSpacingDelta = 0; // Used with Text Auto Sizing feature
        private float m_lineHeight = 0; // Used with the <line-height=xx.x> tag.

        [SerializeField]
        private float m_paragraphSpacing = 0;
        
        
        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("m_lineJustification")]
        private TextAlignmentOptions m_textAlignment = TextAlignmentOptions.TopLeft;
        private TextAlignmentOptions m_lineJustification;

        [SerializeField]
        private bool m_enableKerning = false;

        [SerializeField]
        private bool m_overrideHtmlColors = false;

        [SerializeField]
        private bool m_enableExtraPadding = false;
        [SerializeField]
        private bool checkPaddingRequired;

        [SerializeField]
        private bool m_enableWordWrapping = false;
        private bool m_isCharacterWrappingEnabled = false;
        private bool m_isNonBreakingSpace = false;
        private bool m_isIgnoringAlignment;

        [SerializeField]
        private TextOverflowModes m_overflowMode = TextOverflowModes.Overflow;

        [SerializeField]
        private float m_wordWrappingRatios = 0.4f; // Controls word wrapping ratios between word or characters.

        [SerializeField]
        private TextureMappingOptions m_horizontalMapping = TextureMappingOptions.Character;

        [SerializeField]
        private TextureMappingOptions m_verticalMapping = TextureMappingOptions.Character;

        [SerializeField]
        private Vector2 m_uvOffset = Vector2.zero; // Used to offset UV on Texturing

        [SerializeField]
        private float m_uvLineOffset = 0.0f; // Used for UV line offset per line

        [SerializeField]
        private bool isInputParsingRequired = false; // Used to determine if the input text needs to be re-parsed.

        [SerializeField]
        private bool m_havePropertiesChanged;  // Used to track when properties of the text object have changed.

        //private bool m_hasChanged; // Used to track when the text object has been regenerated.

        [SerializeField]
        private bool hasFontAssetChanged = false; // Used to track when font properties have changed.

        [SerializeField]
        private bool m_isRichText = true; // Used to enable or disable Rich Text.
        [SerializeField]
        private bool m_parseCtrlCharacters = true;


        private enum TextInputSources { Text = 0, SetText = 1, SetCharArray = 2 };
        [SerializeField]
        private TextInputSources m_inputSource;
        private string old_text; // Used by SetText to determine if the text has changed.
        private float old_arg0, old_arg1, old_arg2; // Used by SetText to determine if the args have changed.

        private int m_fontIndex;

        private float m_fontScale; // Scaling of the font based on Atlas true Font Size and Rendered Font Size.  
        private bool m_isRecalculateScaleRequired = false;

        private Vector3 m_previousLossyScale; // Used for Tracking lossy scale changes in the transform;
        private float m_xAdvance; // Tracks x advancement from character to character.
        private float m_maxXAdvance; // Tracks the MaxXAdvance while considering linefeed.

        private float tag_LineIndent = 0;
        private float tag_Indent = 0;
        private bool tag_NoParsing;

        private Vector3 m_anchorOffset; // The amount of offset to be applied to the vertices. 


        private TMP_TextInfo m_textInfo; // Class which holds information about the Text object such as characters, lines, mesh data as well as metrics.       
        private char[] m_htmlTag = new char[64]; // Maximum length of rich text tag. This is preallocated to avoid GC.


        //[SerializeField]
        private CanvasRenderer m_uiRenderer;

        private Canvas m_canvas;
        private RectTransform m_rectTransform;

        // Fields used for vertex colors
        private Color32 m_htmlColor = new Color(255, 255, 255, 128);
        private Color32[] m_colorStack = new Color32[32];
        private int m_colorStackIndex = 0;
     
        private float m_tabSpacing = 0;
        private float m_spacing = 0;
        //private Vector2[] m_spacePositions = new Vector2[8]; // Not fully implemented yet ... will be used to track all the location of inserted spaced.

        private float m_baselineOffset; // Used for superscript and subscript.
        private float m_padding = 0; // Holds the amount of padding required to display the mesh correctly as a result of dilation, outline thickness, softness and similar material properties.
        private bool m_isUsingBold = false; // Used to ensure GetPadding & Ratios take into consideration bold characters.

        private Vector2 k_InfinityVector = new Vector2(1000000, 1000000);

        private bool m_isFirstAllocation; // Flag to determine if this is the first allocation of the buffers.
        private int m_max_characters = 8; // Determines the initial allocation and size of the character array / buffer.
        private int m_max_numberOfLines = 4; // Determines the initial allocation and maximum number of lines of text. 

        private int[] m_char_buffer; // This array holds the characters to be processed by GenerateMesh();
        private char[] m_input_CharArray = new char[256]; // This array hold the characters from the SetText();
        private int m_charArray_Length = 0;
        private List<char> m_VisibleCharacters = new List<char>();


        private readonly float[] k_Power = { 5e-1f, 5e-2f, 5e-3f, 5e-4f, 5e-5f, 5e-6f, 5e-7f, 5e-8f, 5e-9f, 5e-10f }; // Used by FormatText to enable rounding and avoid using Mathf.Pow.

        private GlyphInfo m_cached_GlyphInfo; // Glyph / Character information is cached into this variable which is faster than having to fetch from the Dictionary multiple times.
        private GlyphInfo m_cached_Underline_GlyphInfo; // Same as above but for the underline character which is used for Underline.

        // Global Variables used in conjunction with saving the state of words or lines.
        private WordWrapState m_SavedWordWrapState = new WordWrapState(); // Struct which holds various states / variables used in conjunction with word wrapping.
        private WordWrapState m_SavedLineState = new WordWrapState();
        

        private int m_characterCount;
        private int m_visibleCharacterCount;
        private int m_visibleSpriteCount;
        private int m_firstCharacterOfLine;
        private int m_firstVisibleCharacterOfLine;
        private int m_lastCharacterOfLine;
        private int m_lastVisibleCharacterOfLine;
        private int m_lineNumber;
        private int m_pageNumber;
        private float m_maxAscender;
        private float m_maxDescender;
        private float m_maxFontScale;
        private float m_lineOffset;
        private Extents m_meshExtents;


        // Properties related to the Auto Layout System
        private bool m_isCalculateSizeRequired = false;
        //private ILayoutController m_layoutController;

        // Mesh Declaration
        private Mesh m_mesh;
        private Bounds m_bounds;

        //private Camera m_sceneCamera;
        //private Bounds m_default_bounds = new Bounds(Vector3.zero, new Vector3(1000, 1000, 0));
       

        [SerializeField]
        private bool m_ignoreCulling = true; // Not implemented yet.
        [SerializeField]
        private bool m_isOrthographic = false;

        [SerializeField]
        private bool m_isCullingEnabled = false;


        //private int m_sortingLayerID;
        //private int m_sortingOrder;


        // Properties to control visibility of portions of the mesh
        private int m_maxVisibleCharacters = 99999;
        private int m_maxVisibleWords = 99999;
        private int m_maxVisibleLines = 99999;
        [SerializeField]
        private int m_pageToDisplay = 1;
        private bool m_isNewPage = false;
        private bool m_isTextTruncated;


        //[SerializeField]
        //private TextMeshProFont[] m_fontAssetArray;

        private Dictionary<int, TextMeshProFont> m_fontAsset_Dict = new Dictionary<int, TextMeshProFont>();
        private Dictionary<int, Material> m_fontMaterial_Dict = new Dictionary<int, Material>();

        //private int[] m_meshAllocCount = new int[17];
        //private GameObject[] subObjects = new GameObject[17];

        //[SerializeField]
        //private List<Material> m_sharedMaterials = new List<Material>(16);
        //private int m_selectedFontAsset;

        // MASKING RELATED PROPERTIES
        private bool m_isMaskingEnabled;
        //private bool m_isStencilUpdateRequired;

        [SerializeField]
        private Material m_baseMaterial;
        private Material m_lastBaseMaterial;
        [SerializeField]
        private bool m_isNewBaseMaterial;
        private Material m_maskingMaterial; // Instance of the material used for masking.

        private bool m_isScrollRegionSet;
        //private Mask m_mask;
        private int m_stencilID = 0;
        /*
        [SerializeField]
        
        [SerializeField]
        private MaskingTypes m_mask;
        [SerializeField]
        private MaskingOffsetMode m_maskOffsetMode;
        */
        [SerializeField]
        private Vector4 m_maskOffset;
        /*
        [SerializeField]
        private Vector2 m_maskSoftness;
        [SerializeField]
        private Vector2 m_vertexOffset;
        */

        // Matrix used to animated Env Map
        private Matrix4x4 m_EnvMapMatrix = new Matrix4x4();


        // FLAGS
        private TextRenderFlags m_renderMode = TextRenderFlags.Render;
        private bool m_isParsingText;

        // LINK TRACKING
        TMP_LinkInfo tag_LinkInfo = new TMP_LinkInfo();

        // DEFAULT SETTINGS
        private TMP_Settings m_settings;

        // STYLE TAGS
        //private TMP_StyleSheet m_defaultStyleSheet;
        private int[] m_styleStack = new int[16];
        private int m_styleStackIndex = 0;


        // INLINE GRAPHIC COMPONENT
        private int m_spriteCount = 0;
        private bool m_isSprite = false;
        private int m_spriteIndex;
        private InlineGraphicManager m_inlineGraphics;


        // Text Container / RectTransform Component
        [SerializeField]
        private Vector4 m_margin = new Vector4(0, 0, 0, 0);
        private float m_marginLeft;
        private float m_marginRight;
        private float m_marginWidth;
        private float m_marginHeight;
        private bool m_marginsHaveChanged;
        private bool IsRectTransformDriven;
        private float m_width = -1;

        //[SerializeField]
        //private bool m_isNewTextObject;
        //private TextContainer m_textContainer;
        private bool m_rectTransformDimensionsChanged;
        private Vector3[] m_rectCorners = new Vector3[4];

        [SerializeField]
        private bool m_enableAutoSizing;
        private float m_maxFontSize; // Used in conjunction with Auto-sizing
        private float m_minFontSize; // Used in conjunction with Auto-sizing

        private bool m_isAwake;
        [NonSerialized]
        private bool m_isRegisteredForEvents;

        // ** Still needs to be implemented **
        //private Camera managerCamera;
        //private TMPro_UpdateManager m_updateManager;
        //private bool isAlreadyScheduled;

        // DEBUG Variables
        //private System.Diagnostics.Stopwatch m_StopWatch;
        //private int frame = 0;
        private int m_recursiveCount = 0;
        private int loopCountA = 0;
        //private int loopCountB = 0;
        //private int loopCountC = 0;
        //private int loopCountD = 0;
        //private int loopCountE = 0;

       
        protected override void Awake()
        {
            //base.Awake();
            //Debug.Log("***** Awake() *****");

            m_isAwake = true;
            // Cache Reference to the Canvas
            m_canvas = GetComponentInParent(typeof(Canvas)) as Canvas;

            // Cache Reference to RectTransform.
            m_rectTransform = gameObject.GetComponent<RectTransform>();
            if (m_rectTransform == null)   
                m_rectTransform = gameObject.AddComponent<RectTransform>();
            

           
            // Cache a reference to the UIRenderer.
            m_uiRenderer = GetComponent<CanvasRenderer>();
            if (m_uiRenderer == null) 
                m_uiRenderer = gameObject.AddComponent<CanvasRenderer> ();

            if (m_mesh == null)
            {
                //Debug.Log("Creating new mesh.");
                m_mesh = new Mesh();
                m_mesh.hideFlags = HideFlags.HideAndDontSave;

                //m_mesh.bounds = new Bounds(transform.position, new Vector3(1000, 1000, 0));
            }

            // Cache reference to Mask Component if one is present
            //m_stencilID = MaterialManager.GetStencilID(gameObject);
            //m_mask = GetComponentInParent<Mask>();

            // Load TMP Settings
            if (m_settings == null) m_settings = TMP_Settings.LoadDefaultSettings();
            if (m_settings != null)
            {
                //m_enableWordWrapping = m_settings.enableWordWrapping;
                //m_enableKerning = m_settings.enableKerning;
                //m_enableExtraPadding = m_settings.enableExtraPadding;
            }

            // Load the font asset and assign material to renderer.
            LoadFontAsset();

            // Load Default TMP StyleSheet
            TMP_StyleSheet.LoadDefaultStyleSheet();

            // Allocate our initial buffers.
            m_char_buffer = new int[m_max_characters];
            m_cached_GlyphInfo = new GlyphInfo();
            m_isFirstAllocation = true;

            m_textInfo = new TMP_TextInfo();
            m_textInfo.meshInfo.mesh = m_mesh;


            // Check if we have a font asset assigned. Return if we don't because no one likes to see purple squares on screen.
            if (m_fontAsset == null)
            {
                Debug.LogWarning("Please assign a Font Asset to this " + transform.name + " gameobject.", this);
                return;
            }

            // Set Defaults for Font Auto-sizing
            if (m_fontSizeMin == 0) m_fontSizeMin = m_fontSize / 2;
            if (m_fontSizeMax == 0) m_fontSizeMax = m_fontSize * 2;

            //// Set flags to ensure our text is parsed and text re-drawn.
            isInputParsingRequired = true;
            m_havePropertiesChanged = true;
            m_rectTransformDimensionsChanged = true;

            ForceMeshUpdate(); // Added to force OnWillRenderObject() to be called in case object is not visible so we get initial bounds for the mesh.
        }


        protected override void OnEnable()
        {
            //base.OnEnable();
            //Debug.Log("***** OnEnable() *****"); // + GetInstanceID());  // HavePropertiesChanged = " + havePropertiesChanged); // has been called on Object ID:" + gameObject.GetInstanceID());
            //Debug.Log(m_isRegistered); 

            if (!m_isRegisteredForEvents)
            {
                //Debug.Log("Registering for Events.");
#if UNITY_EDITOR
                // Register Callbacks for various events.
                TMPro_EventManager.MATERIAL_PROPERTY_EVENT.Add(ON_MATERIAL_PROPERTY_CHANGED);
                TMPro_EventManager.FONT_PROPERTY_EVENT.Add(ON_FONT_PROPERTY_CHANGED);
                TMPro_EventManager.TEXTMESHPRO_UGUI_PROPERTY_EVENT.Add(ON_TEXTMESHPRO_UGUI_PROPERTY_CHANGED);
                TMPro_EventManager.DRAG_AND_DROP_MATERIAL_EVENT.Add(ON_DRAG_AND_DROP_MATERIAL);
                TMPro_EventManager.TEXT_STYLE_PROPERTY_EVENT.Add(ON_TEXT_STYLE_CHANGED);
#endif
                // Register to get callback before Canvas is Rendered.
                TMPro_EventManager.WILL_RENDER_CANVASES.Add(OnPreRenderCanvas);
                m_isRegisteredForEvents = true;
            }

            // Register Graphic Component to receive event triggers
            GraphicRegistry.RegisterGraphicForCanvas(this.canvas, this);

            // Cache Reference to the Canvas
            if (m_canvas == null) m_canvas = GetComponentInParent(typeof(Canvas)) as Canvas;

            // Re-assign the Material to the Canvas Renderer
            if (m_uiRenderer.GetMaterial() == null)
            {
                // Use Instanced material if one exists otherwise use shared material or base material if needed.
                if (m_sharedMaterial != null)
                {
                    m_uiRenderer.SetMaterial(m_sharedMaterial, m_sharedMaterial.mainTexture);
                }
                else
                {
                    // We likely had a masking material assigned
                    m_isNewBaseMaterial = true;
                    fontSharedMaterial = m_baseMaterial;
                    RecalculateMasking();
                }

                m_havePropertiesChanged = true;
                m_rectTransformDimensionsChanged = true;
            }
#if UNITY_EDITOR
            //FIX FOR UNITY EDITOR PURPLE TEXT FOR ASSETBUNDLES!!!- FOXCUBENGINE
            //
            m_sharedMaterial.shader = Shader.Find(m_sharedMaterial.shader.name);
            m_uiRenderer.SetMaterial(m_sharedMaterial, null);
#endif
            LayoutRebuilder.MarkLayoutForRebuild(m_rectTransform);
            RecalculateClipping();
        }


        protected override void OnDisable()
        {
            //base.OnDisable();
            //Debug.Log("***** OnDisable() *****"); //for " + this.name + " with ID: " + this.GetInstanceID() + " has been called.");

            // UnRegister Graphic Component
            GraphicRegistry.UnregisterGraphicForCanvas(this.canvas, this);

            m_uiRenderer.Clear();  // Might not want to clear since it wipes the Material in addition to the mesh geometry.

            LayoutRebuilder.MarkLayoutForRebuild(m_rectTransform);
            RecalculateClipping();
        }


        protected override void OnDestroy()
        {
            //base.OnDestroy();
            //Debug.Log("***** OnDestroy() *****");

            // UnRegister Graphic Component
            GraphicRegistry.UnregisterGraphicForCanvas(this.canvas, this);

            if (m_maskingMaterial != null)
            {
                //Debug.Log("Trying to release Masking Material [" + m_maskingMaterial.name + "] with ID " + m_maskingMaterial.GetInstanceID());
                MaterialManager.ReleaseStencilMaterial(m_maskingMaterial);
                m_maskingMaterial = null;
            }

			// clean up mesh
	        if (m_mesh != null)
		        DestroyImmediate(m_mesh);

            // Clean up any material instances we may have created.
            if (m_fontMaterial != null)
                DestroyImmediate(m_fontMaterial);

#if UNITY_EDITOR
            // Unregister the event this object was listening to
            TMPro_EventManager.MATERIAL_PROPERTY_EVENT.Remove(ON_MATERIAL_PROPERTY_CHANGED);
            TMPro_EventManager.FONT_PROPERTY_EVENT.Remove(ON_FONT_PROPERTY_CHANGED);
            TMPro_EventManager.TEXTMESHPRO_UGUI_PROPERTY_EVENT.Remove(ON_TEXTMESHPRO_UGUI_PROPERTY_CHANGED);
            TMPro_EventManager.DRAG_AND_DROP_MATERIAL_EVENT.Remove(ON_DRAG_AND_DROP_MATERIAL);
            TMPro_EventManager.TEXT_STYLE_PROPERTY_EVENT.Remove(ON_TEXT_STYLE_CHANGED);
#endif
            // UnRegister to get callback before Canvas is Rendered.
            TMPro_EventManager.WILL_RENDER_CANVASES.Remove(OnPreRenderCanvas);
            m_isRegisteredForEvents = false;
        }


        protected override void OnTransformParentChanged()
        {
            //Debug.Log("***** OnTransformParentChanged *****");
            // Check the Stencil ID
            int currentID = m_stencilID;
            m_stencilID = MaterialManager.GetStencilID(gameObject);
            if (currentID != m_stencilID)
                RecalculateMasking();

            RecalculateClipping();
            LayoutRebuilder.MarkLayoutForRebuild(m_rectTransform);
            m_havePropertiesChanged = true;
        }


#if UNITY_EDITOR
        protected override void Reset()
        {
            //base.Reset();
            //Debug.Log("***** Reset() *****"); //has been called.");
            isInputParsingRequired = true;
            m_havePropertiesChanged = true;
        }


        protected override void OnValidate()
        {
            //Debug.Log("isAwake = " + m_isAwake + "  isEnabled = " + m_isEnabled);
            // Additional Properties could be added to sync up Serialized Properties & Properties.

            //Debug.Log("***** OnValidate() *****"); // ID " + GetInstanceID()); // New Material [" + m_sharedMaterial.name + "] with ID " + m_sharedMaterial.GetInstanceID() + ". Base Material is [" + m_baseMaterial.name + "] with ID " + m_baseMaterial.GetInstanceID() + ". Previous Base Material is [" + (m_lastBaseMaterial == null ? "Null" : m_lastBaseMaterial.name) + "]."); 

            //if (!m_isAwake)
            //    return;
            
            //Debug.Log(m_fontAsset.atlas.GetInstanceID() + "  " + m_uiRenderer.GetMaterial().mainTexture.GetInstanceID());

            if (m_uiRenderer == null || m_uiRenderer.GetMaterial() == null || m_fontAsset.atlas.GetInstanceID() != m_uiRenderer.GetMaterial().mainTexture.GetInstanceID())
            { 
                LoadFontAsset();
                m_isCalculateSizeRequired = true;
                hasFontAssetChanged = false;
            }
            font = m_fontAsset;

            text = m_text;

            fontSharedMaterial = m_sharedMaterial;
            //checkPaddingRequired = true;

            margin = m_margin; // Getting called on assembly reloads.

            LayoutRebuilder.MarkLayoutForRebuild(m_rectTransform);
        }


        // Event to Track Material Changed resulting from Drag-n-drop.
        void ON_DRAG_AND_DROP_MATERIAL(GameObject obj, Material currentMaterial, Material newMaterial)
        {
            //Debug.Log("Drag-n-Drop Event - Receiving Object ID " + GetInstanceID() + ". Sender ID " + obj.GetInstanceID()); // +  ". Prefab Parent is " + UnityEditor.PrefabUtility.GetPrefabParent(gameObject).GetInstanceID()); // + ". New Material is " + newMaterial.name + " with ID " + newMaterial.GetInstanceID() + ". Base Material is " + m_baseMaterial.name + " with ID " + m_baseMaterial.GetInstanceID());
            
            // Check if event applies to this current object
            if (obj == gameObject || UnityEditor.PrefabUtility.GetPrefabParent(gameObject) == obj)
            {
                //Debug.Log("Assigning new Base Material " + newMaterial.name + " to replace " + currentMaterial.name);

                UnityEditor.Undo.RecordObject(this, "Material Assignment");
                m_baseMaterial = newMaterial;
                m_isNewBaseMaterial = true;
                fontSharedMaterial = m_baseMaterial;
            }
        }


        // Event received when custom material editor properties are changed.
        void ON_MATERIAL_PROPERTY_CHANGED(bool isChanged, Material mat)
        {
            //Debug.Log("ON_MATERIAL_PROPERTY_CHANGED event received by object with ID " + GetInstanceID()); // + " and Targeted Material is: " + mat.name + " with ID " + mat.GetInstanceID() + ". Base Material is " + m_baseMaterial.name + " with ID " + m_baseMaterial.GetInstanceID() + ". Masking Material is " + m_maskingMaterial.name + "  with ID " + m_maskingMaterial.GetInstanceID());         

            ShaderUtilities.GetShaderPropertyIDs(); // Initialize ShaderUtilities and get shader property IDs.

            int materialID = mat.GetInstanceID();

            if (m_uiRenderer.GetMaterial() == null)
            {
                if (m_fontAsset != null)
                {
                    m_uiRenderer.SetMaterial(m_fontAsset.material, m_sharedMaterial.mainTexture);
                    //Debug.LogWarning("No Material was assigned to " + name + ". " + m_fontAsset.material.name + " was assigned.");
                }
                else
                    Debug.LogWarning("No Font Asset assigned to " + name + ". Please assign a Font Asset.", this);
            }


            if (m_uiRenderer.GetMaterial() != m_sharedMaterial && m_fontAsset == null) //    || m_renderer.sharedMaterials.Contains(mat))
            {
                //Debug.Log("ON_MATERIAL_PROPERTY_CHANGED Called on Target ID: " + GetInstanceID() + ". Previous Material:" + m_sharedMaterial + "  New Material:" + m_uiRenderer.GetMaterial()); // on Object ID:" + GetInstanceID() + ". m_sharedMaterial: " + m_sharedMaterial.name + "  m_renderer.sharedMaterial: " + m_renderer.sharedMaterial.name);         
                m_sharedMaterial = m_uiRenderer.GetMaterial();
            }

           
            // Is Material being modified my Base Material
            if (m_stencilID > 0 && m_baseMaterial != null && m_maskingMaterial != null)
            {
                
                if (materialID == m_baseMaterial.GetInstanceID())
                {
                    //Debug.Log("Copying Material properties from Base Material [" + mat + "] to Masking Material [" + m_maskingMaterial + "].");
                    float stencilID = m_maskingMaterial.GetFloat(ShaderUtilities.ID_StencilID);
                    float stencilComp = m_maskingMaterial.GetFloat(ShaderUtilities.ID_StencilComp);
                    m_maskingMaterial.CopyPropertiesFromMaterial(mat);
                    m_maskingMaterial.shaderKeywords = mat.shaderKeywords;

                    m_maskingMaterial.SetFloat(ShaderUtilities.ID_StencilID, stencilID);
                    m_maskingMaterial.SetFloat(ShaderUtilities.ID_StencilComp, stencilComp);
                }
                else if (materialID == m_maskingMaterial.GetInstanceID())
                {                            
                    //Debug.Log("Copying Material properties from Masking Material [" + mat + "] to Base Material [" + m_baseMaterial + "].");
                    m_baseMaterial.CopyPropertiesFromMaterial(mat);
                    m_baseMaterial.shaderKeywords = mat.shaderKeywords;
                    m_baseMaterial.SetFloat(ShaderUtilities.ID_StencilID, 0);
                    m_baseMaterial.SetFloat(ShaderUtilities.ID_StencilComp, 8);
                }
                else if (MaterialManager.GetBaseMaterial(mat) != null && MaterialManager.GetBaseMaterial(mat).GetInstanceID() == m_baseMaterial.GetInstanceID())
                {
                    //Debug.Log("Copying Material properties from Masking Material [" + mat + "] to Masking Material [" + m_maskingMaterial + "].");
                    float stencilID = m_maskingMaterial.GetFloat(ShaderUtilities.ID_StencilID);
                    float stencilComp = m_maskingMaterial.GetFloat(ShaderUtilities.ID_StencilComp);
                    m_maskingMaterial.CopyPropertiesFromMaterial(mat);
                    m_maskingMaterial.shaderKeywords = mat.shaderKeywords;

                    m_maskingMaterial.SetFloat(ShaderUtilities.ID_StencilID, stencilID);
                    m_maskingMaterial.SetFloat(ShaderUtilities.ID_StencilComp, stencilComp);
                }
            }


            //Debug.Log("Assigned Material is " + m_sharedMaterial.name + " with ID " + m_sharedMaterial.GetInstanceID()); // +
            //         ". Target Mat is " + mat.name + " with ID " + mat.GetInstanceID() + 
            //          ". Base Material is " + m_baseMaterial.name + " with ID " + m_baseMaterial.GetInstanceID() + 
            //          ". Masking Material is " + m_maskingMaterial.name + " with ID " + m_maskingMaterial.GetInstanceID());

            m_padding = ShaderUtilities.GetPadding(m_uiRenderer.GetMaterial(), m_enableExtraPadding, m_isUsingBold);
            //m_alignmentPadding = ShaderUtilities.GetFontExtent(m_uiRenderer.GetMaterial());
            m_havePropertiesChanged = true;
            /* ScheduleUpdate(); */
        }


        // Event received when font asset properties are changed in Font Inspector
        void ON_FONT_PROPERTY_CHANGED(bool isChanged, TextMeshProFont font)
        {
            if (font == m_fontAsset)
            {
                //Debug.Log("ON_FONT_PROPERTY_CHANGED event received.");
                m_havePropertiesChanged = true;
                hasFontAssetChanged = true;
                /* ScheduleUpdate(); */
            }
        }


        // Event received when UNDO / REDO Event alters the properties of the object.
        void ON_TEXTMESHPRO_UGUI_PROPERTY_CHANGED(bool isChanged, TextMeshProUGUI obj)
        {
            //Debug.Log("Event Received by " + obj);
            
            if (obj == this)
            {
                //Debug.Log("Undo / Redo Event Received by Object ID:" + GetInstanceID());
                m_havePropertiesChanged = true;
                isInputParsingRequired = true;
                /* ScheduleUpdate(); */
            }
        }


        // Event received when Text Styles are changed.
        void ON_TEXT_STYLE_CHANGED(bool isChanged)
        {
            m_havePropertiesChanged = true;
        }
#endif


        // Function which loads either the default font or a newly assigned font asset. This function also assigned the appropriate material to the renderer.
        void LoadFontAsset()
        {
            //Debug.Log("***** LoadFontAsset() *****"); //TextMeshPro LoadFontAsset() has been called."); // Current Font Asset is " + (font != null ? font.name: "Null") );
            ShaderUtilities.GetShaderPropertyIDs();

            if (m_fontAsset == null)
            {
                // Load TMP_Settings
                if (m_settings == null) m_settings = TMP_Settings.LoadDefaultSettings();

                if (m_settings != null && m_settings.fontAsset != null)
                    m_fontAsset = m_settings.fontAsset;
                else
                    m_fontAsset = Resources.Load("Fonts & Materials/ARIAL SDF", typeof(TextMeshProFont)) as TextMeshProFont;

                if (m_fontAsset == null)
                {
                    Debug.LogWarning("The ARIAL SDF Font Asset was not found. There is no Font Asset assigned to " + transform.GetFullHierarchyPath() + ".", this);
                    return;
                }

                if (m_fontAsset.characterDictionary == null)
                {
                    Debug.Log("Dictionary is Null! font asset = " + m_fontAsset.name);
                }

                //m_uiRenderer.SetMaterial(m_fontAsset.material, null);
                m_baseMaterial = m_fontAsset.material;
                m_sharedMaterial = m_baseMaterial;
                m_isNewBaseMaterial = true;

                //m_renderer.receiveShadows = false;
                //m_renderer.castShadows = false; // true;
                // Get a Reference to the Shader
            }
            else
            {
                if (m_fontAsset.characterDictionary == null)
                {
                    //Debug.Log("Reading Font Definition and Creating Character Dictionary.");
                    m_fontAsset.ReadFontDefinition();
                }


                // Force the use of the base material
                m_sharedMaterial = m_baseMaterial;
                m_isNewBaseMaterial = true;


                // If font atlas texture doesn't match the assigned material font atlas, switch back to default material specified in the Font Asset.
                if (m_sharedMaterial == null || m_sharedMaterial.mainTexture == null || m_fontAsset.atlas.GetInstanceID() != m_sharedMaterial.mainTexture.GetInstanceID())
                {
                    m_sharedMaterial = m_fontAsset.material;
                    m_baseMaterial = m_sharedMaterial;
                    m_isNewBaseMaterial = true;
                }
            }

            // Check & Assign Underline Character for use with the Underline tag.
	        m_fontAsset.characterDictionary.TryGetValue(95, out m_cached_Underline_GlyphInfo);
            //if (!m_fontAsset.characterDictionary.TryGetValue(95, out m_cached_Underline_GlyphInfo)) //95
            //    Debug.LogWarning("Underscore character wasn't found in the current Font Asset. No characters assigned for Underline.", this);

            
            m_stencilID = MaterialManager.GetStencilID(gameObject);
            if (m_stencilID == 0)
            {
                if (m_maskingMaterial != null)
                {
                    MaterialManager.ReleaseStencilMaterial(m_maskingMaterial);
                    m_maskingMaterial = null;
                }

                m_sharedMaterial = m_baseMaterial;
            }
            else
            {
                if (m_maskingMaterial == null)
                    m_maskingMaterial = MaterialManager.GetStencilMaterial(m_baseMaterial, m_stencilID);
                else if (m_maskingMaterial.GetInt(ShaderUtilities.ID_StencilID) != m_stencilID || m_isNewBaseMaterial)
                {
                    MaterialManager.ReleaseStencilMaterial(m_maskingMaterial);
                    m_maskingMaterial = MaterialManager.GetStencilMaterial(m_baseMaterial, m_stencilID);
                }

                m_sharedMaterial = m_maskingMaterial;
            }

            m_isNewBaseMaterial = false;
            
            //m_sharedMaterials.Add(m_sharedMaterial);
            SetShaderDepth(); // Set ZTestMode based on Canvas RenderMode.

            if (m_uiRenderer == null) m_uiRenderer = GetComponent<CanvasRenderer>();

            m_uiRenderer.SetMaterial(m_sharedMaterial, m_sharedMaterial.mainTexture);
            m_padding = ShaderUtilities.GetPadding(m_sharedMaterial, m_enableExtraPadding, m_isUsingBold);
            //m_alignmentPadding = ShaderUtilities.GetFontExtent(m_sharedMaterial);
        }


        /// <summary>
        /// Function under development to utilize an Update Manager instead of normal event functions like LateUpdate() or OnWillRenderObject().
        /// </summary>
        void ScheduleUpdate()
        {
            return;
            /*
            if (!isAlreadyScheduled)
            {
                m_updateManager.ScheduleObjectForUpdate(this);
                isAlreadyScheduled = true;
            }
            */
        }



        void UpdateEnvMapMatrix()
        {
            if (!m_sharedMaterial.HasProperty(ShaderUtilities.ID_EnvMap) || m_sharedMaterial.GetTexture(ShaderUtilities.ID_EnvMap) == null)
                return;

            Debug.Log("Updating Env Matrix...");
            Vector3 rotation = m_sharedMaterial.GetVector(ShaderUtilities.ID_EnvMatrixRotation);
            m_EnvMapMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(rotation), Vector3.one);

            m_sharedMaterial.SetMatrix(ShaderUtilities.ID_EnvMatrix, m_EnvMapMatrix);
        }


        // Enable Masking in the Shader
        void EnableMasking()
        {
            if (m_fontMaterial == null)
            {
                m_fontMaterial = CreateMaterialInstance(m_sharedMaterial);
                m_uiRenderer.SetMaterial(m_fontMaterial, m_sharedMaterial.mainTexture);
            }

            m_sharedMaterial = m_fontMaterial;
            if (m_sharedMaterial.HasProperty(ShaderUtilities.ID_ClipRect))
            {
                m_sharedMaterial.EnableKeyword(ShaderUtilities.Keyword_MASK_SOFT);
                m_sharedMaterial.DisableKeyword(ShaderUtilities.Keyword_MASK_HARD);
                m_sharedMaterial.DisableKeyword(ShaderUtilities.Keyword_MASK_TEX);

                UpdateMask(); // Update Masking Coordinates
            }

            m_isMaskingEnabled = true;

            //m_uiRenderer.SetMaterial(m_sharedMaterial, null);

            //m_padding = ShaderUtilities.GetPadding(m_sharedMaterial, m_enableExtraPadding, m_isUsingBold);
            //m_alignmentPadding = ShaderUtilities.GetFontExtent(m_sharedMaterial);

            /*
            Material mat = m_uiRenderer.GetMaterial();
            if (mat.HasProperty(ShaderUtilities.ID_MaskCoord))
            {
                mat.EnableKeyword("MASK_SOFT");
                mat.DisableKeyword("MASK_HARD");
                mat.DisableKeyword("MASK_OFF");

                m_isMaskingEnabled = true;
                UpdateMask();
            }
            */
        }


        // Enable Masking in the Shader
        void DisableMasking()
        {
            if (m_fontMaterial != null)
            {
                if (m_stencilID > 0)
                    m_sharedMaterial = m_maskingMaterial;
                else
                    m_sharedMaterial = m_baseMaterial;
                           
                m_uiRenderer.SetMaterial(m_sharedMaterial, m_sharedMaterial.mainTexture);

                DestroyImmediate(m_fontMaterial);
            }
              
            m_isMaskingEnabled = false;
            
            /*
            if (m_maskingMaterial != null && m_stencilID == 0)
            {
                m_sharedMaterial = m_baseMaterial;
                m_uiRenderer.SetMaterial(m_sharedMaterial, null);
            }
            else if (m_stencilID > 0)
            {
                m_sharedMaterial.EnableKeyword("MASK_OFF");
                m_sharedMaterial.DisableKeyword("MASK_HARD");
                m_sharedMaterial.DisableKeyword("MASK_SOFT");
            }
            */
             
          
            /*
            Material mat = m_uiRenderer.GetMaterial();
            if (mat.HasProperty(ShaderUtilities.ID_MaskCoord))
            {
                mat.EnableKeyword("MASK_OFF");
                mat.DisableKeyword("MASK_HARD");
                mat.DisableKeyword("MASK_SOFT");

                m_isMaskingEnabled = false;
                UpdateMask();
            }
            */
        }


        // Update & recompute Mask offset
        void UpdateMask()
        {
            Debug.Log("Updating Mask...");

            if (m_rectTransform != null)
            {
                //Material mat = m_uiRenderer.GetMaterial();
                //if (mat == null || (m_overflowMode == TextOverflowModes.ScrollRect && m_isScrollRegionSet))
                //    return;

                if (!ShaderUtilities.isInitialized)
                    ShaderUtilities.GetShaderPropertyIDs();
                
                //Debug.Log("Setting Mask for the first time.");

                m_isScrollRegionSet = true;

                float softnessX = Mathf.Min(Mathf.Min(m_margin.x, m_margin.z), m_sharedMaterial.GetFloat(ShaderUtilities.ID_MaskSoftnessX));
                float softnessY = Mathf.Min(Mathf.Min(m_margin.y, m_margin.w), m_sharedMaterial.GetFloat(ShaderUtilities.ID_MaskSoftnessY));

                softnessX = softnessX > 0 ? softnessX : 0;
                softnessY = softnessY > 0 ? softnessY : 0;

                float width = (m_rectTransform.rect.width - Mathf.Max(m_margin.x, 0) - Mathf.Max(m_margin.z, 0)) / 2 + softnessX;
                float height = (m_rectTransform.rect.height - Mathf.Max(m_margin.y, 0) - Mathf.Max(m_margin.w, 0)) / 2 + softnessY;

                
                Vector2 center = m_rectTransform.localPosition + new Vector3((0.5f - m_rectTransform.pivot.x) * m_rectTransform.rect.width + (Mathf.Max(m_margin.x, 0) - Mathf.Max(m_margin.z, 0)) / 2, (0.5f - m_rectTransform.pivot.y) * m_rectTransform.rect.height + (-Mathf.Max(m_margin.y, 0) + Mathf.Max(m_margin.w, 0)) / 2);                           
        
                //Vector2 center = m_rectTransform.localPosition + new Vector3((0.5f - m_rectTransform.pivot.x) * m_rectTransform.rect.width + (margin.x - margin.z) / 2, (0.5f - m_rectTransform.pivot.y) * m_rectTransform.rect.height + (-margin.y + margin.w) / 2);
                Vector4 mask = new Vector4(center.x, center.y, width, height);
                //Debug.Log(mask);



                //Rect rect = new Rect(0, 0, m_rectTransform.rect.width + margin.x + margin.z, m_rectTransform.rect.height + margin.y + margin.w);
                //int softness = (int)m_sharedMaterial.GetFloat(ShaderUtilities.ID_MaskSoftnessX) / 2;
                m_sharedMaterial.SetVector(ShaderUtilities.ID_ClipRect, mask);
            }
        }



        // Function called internally when a new material is assigned via the fontMaterial property.
        void SetFontMaterial(Material mat)
        {
            // Get Shader PropertyIDs if they haven't been cached already.
            ShaderUtilities.GetShaderPropertyIDs();
            
            // Check in case Object is disabled. If so, we don't have a valid reference to the Renderer.
            // This can occur when the Duplicate Material Context menu is used on an inactive object.
            if (m_uiRenderer == null)
                m_uiRenderer = GetComponent<CanvasRenderer>();

            // Destroy previous instance material.
            if (m_fontMaterial != null) DestroyImmediate(m_fontMaterial);

            // Release masking material
            if (m_maskingMaterial != null)
            {
                MaterialManager.ReleaseStencilMaterial(m_maskingMaterial);
                m_maskingMaterial = null;
            } 

            // Get Masking ID
            m_stencilID = MaterialManager.GetStencilID(gameObject);
  
            // Create Instance Material
            m_fontMaterial = CreateMaterialInstance(mat);
            
            if (m_stencilID > 0)
                m_fontMaterial = MaterialManager.SetStencil(m_fontMaterial, m_stencilID);


            m_sharedMaterial = m_fontMaterial;
            SetShaderDepth(); // Set ZTestMode based on Canvas RenderMode.
            
            m_uiRenderer.SetMaterial(m_sharedMaterial, m_sharedMaterial.mainTexture);
                   
            m_padding = ShaderUtilities.GetPadding(m_sharedMaterial, m_enableExtraPadding, m_isUsingBold);
            //m_alignmentPadding = ShaderUtilities.GetFontExtent(m_sharedMaterial);
        }


        // Function called internally when a new shared material is assigned via the fontSharedMaterial property.
        void SetSharedFontMaterial(Material mat) 
        {
            ShaderUtilities.GetShaderPropertyIDs();

            // Check in case Object is disabled. If so, we don't have a valid reference to the Renderer.
            // This can occur when the Duplicate Material Context menu is used on an inactive object. 
            if (m_uiRenderer == null)
                m_uiRenderer = GetComponent<CanvasRenderer>();

            if (mat == null) { mat = m_baseMaterial; m_isNewBaseMaterial = true; }

     
            // Handle UI Mask
            m_stencilID = MaterialManager.GetStencilID(gameObject);
            if (m_stencilID == 0)
            {
                if (m_maskingMaterial != null)
                {
                    MaterialManager.ReleaseStencilMaterial(m_maskingMaterial);
                    m_maskingMaterial = null;
                }

                m_baseMaterial = mat; // Can Material be a Masking Material?
            }
            else
            {
                if (m_maskingMaterial == null)
                    m_maskingMaterial = MaterialManager.GetStencilMaterial(mat, m_stencilID);
                else if (m_maskingMaterial.GetInt(ShaderUtilities.ID_StencilID) != m_stencilID || m_isNewBaseMaterial)
                {
                    MaterialManager.ReleaseStencilMaterial(m_maskingMaterial);
                    m_maskingMaterial = MaterialManager.GetStencilMaterial(mat, m_stencilID);
                }

                mat = m_maskingMaterial;
            }

            m_isNewBaseMaterial = false;


            m_sharedMaterial = mat;
            SetShaderDepth(); // Set ZTestMode based on Canvas RenderMode.
            
            m_uiRenderer.SetMaterial(m_sharedMaterial, m_sharedMaterial.mainTexture);
            m_padding = ShaderUtilities.GetPadding(m_sharedMaterial, m_enableExtraPadding, m_isUsingBold);
            //m_alignmentPadding = ShaderUtilities.GetFontExtent(m_sharedMaterial);

            //Debug.Log("New Material [" + mat.name + "] with ID " + mat.GetInstanceID() + " has been assigned. Base Material is [" + m_baseMaterial.name + "] with ID " + m_baseMaterial.GetInstanceID());
            
        }


        void SetFontBaseMaterial(Material mat)
        {
            Debug.Log("Changing Base Material from [" + (m_lastBaseMaterial == null ? "Null" : m_lastBaseMaterial.name) + "] to [" + mat.name + "].");
        
            // Remove reference to masking material for this base material if one exists.
            //if (m_maskingMaterial != null)           
            //    MaterialManager.ReleaseMaskingMaterial(m_lastBaseMaterial);
            
            // Assign new Base Material
            m_baseMaterial = mat;
            m_lastBaseMaterial = mat;

            // Check if Masking is enabled and if so assign the masking material.
            //if (m_mask != null && m_mask.enabled)
            //{
                //if (m_maskingMaterial == null)
            //    m_maskingMaterial = MaterialManager.GetMaskingMaterial(mat, 1);
            
            //    fontSharedMaterial = m_maskingMaterial;
            //}

            //m_isBaseMaterialChanged = false;
        }


      
        // This function will create an instance of the Font Material.
        void SetOutlineThickness(float thickness)
        {
            // Use material instance if one exists. Otherwise, create a new instance of the shared material.
            if (m_fontMaterial != null && m_sharedMaterial.GetInstanceID() != m_fontMaterial.GetInstanceID())
            {
                m_sharedMaterial = m_fontMaterial;
                m_uiRenderer.SetMaterial(m_sharedMaterial, m_sharedMaterial.mainTexture);
            }
            else if(m_fontMaterial == null)
            {
                m_fontMaterial = CreateMaterialInstance(m_sharedMaterial);
                m_sharedMaterial = m_fontMaterial;
                m_uiRenderer.SetMaterial(m_sharedMaterial, m_sharedMaterial.mainTexture);
            }

            thickness = Mathf.Clamp01(thickness);
            m_sharedMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, thickness);
            m_padding = ShaderUtilities.GetPadding(m_sharedMaterial, m_enableExtraPadding, m_isUsingBold);
        }


        // This function will create an instance of the Font Material.
        void SetFaceColor(Color32 color)
        {
            // Use material instance if one exists. Otherwise, create a new instance of the shared material.
            if (m_fontMaterial != null && m_sharedMaterial.GetInstanceID() != m_fontMaterial.GetInstanceID())
            {
                m_sharedMaterial = m_fontMaterial;
                m_uiRenderer.SetMaterial(m_sharedMaterial, m_sharedMaterial.mainTexture);
            }
            else if (m_fontMaterial == null)
            {
                m_fontMaterial = CreateMaterialInstance(m_sharedMaterial);
                m_sharedMaterial = m_fontMaterial;
                m_uiRenderer.SetMaterial(m_sharedMaterial, m_sharedMaterial.mainTexture);
            }

            m_sharedMaterial.SetColor(ShaderUtilities.ID_FaceColor, color);
        }


        // This function will create an instance of the Font Material.
        void SetOutlineColor(Color32 color)
        {
            // Use material instance if one exists. Otherwise, create a new instance of the shared material.
            if (m_fontMaterial != null && m_sharedMaterial.GetInstanceID() != m_fontMaterial.GetInstanceID())
            {
                m_sharedMaterial = m_fontMaterial;
                m_uiRenderer.SetMaterial(m_sharedMaterial, m_sharedMaterial.mainTexture);
            }
            else if (m_fontMaterial == null)
            {
                m_fontMaterial = CreateMaterialInstance(m_sharedMaterial);
                m_sharedMaterial = m_fontMaterial;
                m_uiRenderer.SetMaterial(m_sharedMaterial, m_sharedMaterial.mainTexture);
            }

            m_sharedMaterial.SetColor(ShaderUtilities.ID_OutlineColor, color);
        }


        // Function used to create an instance of the material
        Material CreateMaterialInstance(Material source)
        {          
            Material mat = new Material(source);
            mat.shaderKeywords = source.shaderKeywords;
            
            mat.hideFlags = HideFlags.DontSave;
            mat.name += " (Instance)";

            return mat;
        }


        // Sets the Render Queue and Ztest mode 
        void SetShaderDepth()
        {
            if (m_canvas == null || m_sharedMaterial == null)
                return;
            
            if (m_canvas.renderMode == RenderMode.ScreenSpaceOverlay || m_isOverlay)
            {
                m_sharedMaterial.SetFloat(ShaderUtilities.ShaderTag_ZTestMode, 0);
                //m_renderer.material.SetFloat("_ZTestMode", 8);
                //m_renderer.material.renderQueue = 4000;

                //m_sharedMaterial = m_renderer.material;
            }
            else
            {   // TODO: This section needs to be tested.
                m_sharedMaterial.SetFloat(ShaderUtilities.ShaderTag_ZTestMode, 4);
            }
        }


        // Sets the Culling mode of the material
        void SetCulling()
        {
            if (m_isCullingEnabled)
            {                        
                m_uiRenderer.GetMaterial().SetFloat("_CullMode", 2);
            }
            else
            {
                m_uiRenderer.GetMaterial().SetFloat("_CullMode", 0);
            }
        }


        // Set Perspective Correction Mode based on whether Camera is Orthographic or Perspective
        void SetPerspectiveCorrection()
        {
            if (m_isOrthographic)
                m_sharedMaterial.SetFloat(ShaderUtilities.ID_PerspectiveFilter, 0.0f);
            else
                m_sharedMaterial.SetFloat(ShaderUtilities.ID_PerspectiveFilter, 0.875f);
        }


        // Function to allocate the necessary buffers to render the text. This function is called whenever the buffer size needs to be increased.
        void SetMeshArrays(int size)
        {
            m_textInfo.meshInfo.ResizeMeshInfo(size);

            m_uiRenderer.SetMesh(m_textInfo.meshInfo.mesh);
        }


        // Function used in conjunction with SetText()
        void AddIntToCharArray(int number, ref int index, int precision)
        {
            if (number < 0)
            {
                m_input_CharArray[index++] = '-';
                number = -number;
            }

            int i = index;
            do
            {
                m_input_CharArray[i++] = (char)(number % 10 + 48);
                number /= 10;
            } while (number > 0);

            int lastIndex = i;

            // Reverse string
            while (index + 1 < i)
            {
                i -= 1;
                char t = m_input_CharArray[index];
                m_input_CharArray[index] = m_input_CharArray[i];
                m_input_CharArray[i] = t;
                index += 1;
            }
            index = lastIndex;
        }


        // Functions used in conjunction with SetText()
        void AddFloatToCharArray(float number, ref int index, int precision)
        {
            if (number < 0)
            {
                m_input_CharArray[index++] = '-';
                number = -number;
            }

            number += k_Power[Mathf.Min(9, precision)];

            int integer = (int)number;
            AddIntToCharArray(integer, ref index, precision);

            if (precision > 0)
            {
                // Add the decimal point
                m_input_CharArray[index++] = '.';

                number -= integer;
                for (int p = 0; p < precision; p++)
                {
                    number *= 10;
                    int d = (int)(number);

                    m_input_CharArray[index++] = (char)(d + 48);
                    number -= d;
                }
            }
        }


        // Converts a string to a Char[]
        void StringToCharArray(string text, ref int[] chars)
        {
            if (text == null)
            {
                chars[0] = 0;
                return;
            }

            // Check to make sure chars_buffer is large enough to hold the content of the string.
            if (chars.Length <= text.Length)
            {
                int newSize = text.Length > 1024 ? text.Length + 256 : Mathf.NextPowerOfTwo(text.Length + 1);
                //Debug.Log("Resizing the chars_buffer[" + chars.Length + "] to chars_buffer[" + newSize + "].");
                chars = new int[newSize];
            }

            int index = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (m_parseCtrlCharacters && text[i] == 92 && text.Length > i + 1)
                {
                    switch ((int)text[i + 1])
                    {
                        case 85: // \U00000000 for UTF-32 Unicode
                            if (text.Length > i + 9)
                            {
                                chars[index] = GetUTF32(i + 2);
                                i += 9;
                                index += 1;
                                continue;
                            }
                            break;
                        case 92: // \ escape
                            if (text.Length <= i + 2) break;
                            chars[index] = text[i + 1];
                            chars[index + 1] = text[i + 2];
                            i += 2;
                            index += 2;
                            continue;
                        case 110: // \n LineFeed
                            chars[index] = (char)10;
                            i += 1;
                            index += 1;
                            continue;
                        case 114: // \r
                            chars[index] = (char)13;
                            i += 1;
                            index += 1;
                            continue;
                        case 116: // \t Tab
                            chars[index] = (char)9;
                            i += 1;
                            index += 1;
                            continue;
                        case 117: // \u0000 for UTF-16 Unicode
                            if (text.Length > i + 5)
                            {
                                chars[index] = (char)GetUTF16(i + 2);
                                i += 5;
                                index += 1;
                                continue;
                            }
                            break;
                    }
                }

                // Handle UTF-32 in the input text (string).
                if (char.IsHighSurrogate(text[i]) && char.IsLowSurrogate(text[i + 1]))
                {
                    chars[index] = char.ConvertToUtf32(text[i], text[i + 1]);
                    i += 1;
                    index += 1;
                    continue;
                }

                chars[index] = text[i];
                index += 1;
            }
            chars[index] = (char)0;
        }


        // Copies Content of formatted SetText() to charBuffer.
        void SetTextArrayToCharArray(char[] charArray, ref int[] charBuffer)
        {
            //Debug.Log("SetText Array to Char called.");
            if (charArray == null || m_charArray_Length == 0)
                return;

            // Check to make sure chars_buffer is large enough to hold the content of the string.
            if (charBuffer.Length <= m_charArray_Length)
            {
                int newSize = m_charArray_Length > 1024 ? m_charArray_Length + 256 : Mathf.NextPowerOfTwo(m_charArray_Length + 1);
                charBuffer = new int[newSize];
            }

            int index = 0;

            for (int i = 0; i < m_charArray_Length; i++)
            {
                // Handle UTF-32 in the input text (string).
                if (char.IsHighSurrogate(charArray[i]) && char.IsLowSurrogate(charArray[i + 1]))
                {
                    charBuffer[index] = char.ConvertToUtf32(charArray[i], charArray[i + 1]);
                    i += 1;
                    index += 1;
                    continue;
                }

                charBuffer[index] = charArray[i];
                index += 1;
            }
            charBuffer[index] = 0;
        }


        /// <summary>
        /// Function used in conjunction with GetTextInfo to figure out Array allocations.
        /// </summary>
        /// <param name="chars"></param>
        /// <returns></returns>
        int GetArraySizes(int[] chars)
        {
            //Debug.Log("Set Array Size called.");

            int visibleCount = 0;
            int totalCount = 0;
            int tagEnd = 0;
            m_isUsingBold = false;
            m_isParsingText = false;

            m_VisibleCharacters.Clear();

            for (int i = 0; chars[i] != 0; i++)
            {
                int c = chars[i];

                if (m_isRichText && c == 60) // if Char '<'
                {
                    // Check if Tag is Valid
                    if (ValidateHtmlTag(chars, i + 1, out tagEnd))
                    {
                        i = tagEnd;
                        if ((m_style & FontStyles.Underline) == FontStyles.Underline) visibleCount += 3;

                        if ((m_style & FontStyles.Bold) == FontStyles.Bold) m_isUsingBold = true;

                        continue;
                    }
                }

                if (c != 9 && c != 10 && c != 13 && c != 32 && c != 160)
                {
                    visibleCount += 1;
                }

                m_VisibleCharacters.Add((char)c);  
                totalCount += 1;
            }

            return totalCount;
        }




        // This function parses through the Char[] to determine how many characters will be visible. It then makes sure the arrays are large enough for all those characters.
        int SetArraySizes(int[] chars)
        {
            //Debug.Log("Set Array Size called.");

            int visibleCount = 0;
            int totalCount = 0;
            int tagEnd = 0;
            int spriteCount = 0;
            m_isUsingBold = false;
            m_isParsingText = false;
            m_isSprite = false;
            m_fontIndex = 0;

            m_VisibleCharacters.Clear();
            //Array.Clear(m_meshAllocCount, 0, 17);

            for (int i = 0; chars[i] != 0; i++)
            {
                int c = chars[i];

                if (m_isRichText && c == 60) // if Char '<'
                {
                    // Check if Tag is Valid
                    if (ValidateHtmlTag(chars, i + 1, out tagEnd))
                    {
                        i = tagEnd;
                        if ((m_style & FontStyles.Underline) == FontStyles.Underline) visibleCount += 3;

                        if ((m_style & FontStyles.Bold) == FontStyles.Bold) m_isUsingBold = true;

                        if (m_isSprite) { spriteCount += 1; totalCount += 1; m_VisibleCharacters.Add((char)(57344 + m_spriteIndex)); m_isSprite = false; }

                        continue;
                    }
                }

                if (c != 9 && c != 10 && c != 13 && c != 32 && c != 160)
                {
                    visibleCount += 1;
                
                    // Track how many characters per mesh
                    //m_meshAllocCount[m_fontIndex] += 1;

                }

                m_VisibleCharacters.Add((char)c);   
                totalCount += 1;
            }


            // Allocated secondary vertex buffers for InlineGraphic Component if present.
            if (spriteCount > 0)
            {
                if (m_inlineGraphics == null) m_inlineGraphics = GetComponent<InlineGraphicManager>() ?? gameObject.AddComponent<InlineGraphicManager>();

                m_inlineGraphics.AllocatedVertexBuffers(spriteCount);
            }
            else if (m_inlineGraphics != null)
                m_inlineGraphics.ClearUIVertex();
            
           
            m_spriteCount = spriteCount;
                              

            if (m_textInfo.characterInfo == null || totalCount > m_textInfo.characterInfo.Length)
            {
                m_textInfo.characterInfo = new TMP_CharacterInfo[totalCount > 1024 ? totalCount + 256 : Mathf.NextPowerOfTwo(totalCount)];
            }

            // Make sure our Mesh Buffer Allocations can hold these new Quads.

            if (m_textInfo.meshInfo.vertices == null) m_textInfo.meshInfo = new TMP_MeshInfo(m_mesh, visibleCount);
            if (visibleCount * 4 > m_textInfo.meshInfo.vertices.Length)
            {
                // If this is the first allocation, we allocated exactly the number of Quads we need. Otherwise, we allocated more since this text object is dynamic.
                if (m_isFirstAllocation)
                {
                    SetMeshArrays(visibleCount);
                    m_isFirstAllocation = false;
                }
                else
                {
                    SetMeshArrays(visibleCount > 1024 ? visibleCount + 256 : Mathf.NextPowerOfTwo(visibleCount));
                }
            }

            /*
            // Make sure our Mesh Array has enough capacity for the different fonts
            if (m_textInfo.meshInfo.meshArrays == null) m_textInfo.meshInfo.meshArrays = new UIVertex[17][];
            for (int i = 0; i < 17; i++ )
            {
                if (m_textInfo.meshInfo.meshArrays[i] == null || m_textInfo.meshInfo.meshArrays[i].Length < m_meshAllocCount[i])
                {
                    int arraySize = m_meshAllocCount[i] * 4;
                    m_textInfo.meshInfo.meshArrays[i] = new UIVertex[arraySize > 1024 ? arraySize + 256 : Mathf.NextPowerOfTwo(arraySize)];
                }

                if (i > 0 && m_meshAllocCount[i] > 0)
                {
                    if (subObjects[i] == null)
                    {
                        subObjects[i] = new GameObject("Font #" + i, typeof(CanvasRenderer));
                        RectTransform rectTransform = subObjects[i].AddComponent<RectTransform>();
                        rectTransform.SetParent(m_rectTransform);
                        rectTransform.localPosition = Vector3.zero;
                        rectTransform.sizeDelta = Vector2.zero;
                        rectTransform.anchorMin = Vector2.zero;
                        rectTransform.anchorMax = Vector2.one;
                    }
                }
            }
            */

            return totalCount;
        }


        // Added to sort handle the potential issue with OnWillRenderObject() not getting called when objects are not visible by camera.
        //void OnBecameInvisible()
        //{
        //    if (m_mesh != null)
        //        m_mesh.bounds = new Bounds(transform.position, new Vector3(1000, 1000, 0));
        //}


        // Method used to mark layout for rebuild
        void MarkLayoutForRebuild()
        {
            //Debug.Log("MarkLayoutForRebuild() called.");
            
            if (m_rectTransform == null)
                m_rectTransform = GetComponent<RectTransform>();

            LayoutRebuilder.MarkLayoutForRebuild(m_rectTransform);
        }


        // Method to parse the input text based on its source
        void ParseInputText()
        {
            //Debug.Log("Re-parsing Text.");
            isInputParsingRequired = false;

            switch (m_inputSource)
            {
                case TextInputSources.Text:
                    StringToCharArray(m_text, ref m_char_buffer);
                    //isTextChanged = false;
                    break;
                case TextInputSources.SetText:
                    SetTextArrayToCharArray(m_input_CharArray, ref m_char_buffer);
                    //isSetTextChanged = false;
                    break;
                case TextInputSources.SetCharArray:
                    break;
            }

            //TMPro_EventManager.ON_TEXT_CHANGED(this);
        }

      
        void ComputeMarginSize()
        {                      
                      
            if (m_rectTransform != null)
            {              
                //Debug.Log("Computing new margins. Current RectTransform's Width is " + m_rectTransform.rect.width + " and Height is " + m_rectTransform.rect.height); // + "  Preferred Width: " + m_preferredWidth + " Height: " + m_preferredHeight);                            

                //if (m_rectTransform.rect.width == 0) m_marginWidth = Mathf.Infinity;
                //else
                    m_marginWidth = m_rectTransform.rect.width - m_margin.x - m_margin.z;
                
                //if (m_rectTransform.rect.height == 0) m_marginHeight = Mathf.Infinity;
                //else                   
                    m_marginHeight = m_rectTransform.rect.height - m_margin.y - m_margin.w;
                            
            }
        }


        protected override void OnDidApplyAnimationProperties()
        {           
            m_havePropertiesChanged = true;
            //Debug.Log("Animation Properties have changed.");
        }


        protected override void OnRectTransformDimensionsChange()
        {
            //Debug.Log("**** OnRectTransformDimensionsChange() **** isRebuildingLayout = " + (m_isRebuildingLayout ? "True" : "False") + "."); // Rect: " + m_rectTransform.rect); //  called on Object ID " + GetInstanceID());

            SetShaderDepth();
            // Need to add code to figure out if the result of these changes should be processed immediately or the next time the frame is rendered.

            // Make sure object is active in Hierarchy
            if (!this.gameObject.activeInHierarchy)
                return;

            ComputeMarginSize();
         
            if (m_rectTransform != null)
                m_rectTransform.hasChanged = true;
            else
            {
                m_rectTransform = GetComponent<RectTransform>();
                m_rectTransform.hasChanged = true;
            }

            //m_rectTransformDimensionsChanged = true;
            
            //Debug.Log("OnRectTransformDimensionsChange() called. New Width: " + m_rectTransform.rect.width + "  Height: " + m_rectTransform.rect.height);

            if (m_isRebuildingLayout)
                m_isLayoutDirty = true;
            else
                m_havePropertiesChanged = true;
        } 

        

        // Called just before the Canvas is rendered.
        void OnPreRenderCanvas()
        {
            // Early return if text object is disabled.
            if (!this.isActiveAndEnabled || m_fontAsset == null)
                return;

            // Debug Variables
            loopCountA = 0;
            //loopCountB = 0;
            //loopCountC = 0;
            //loopCountD = 0;
            //loopCountE = 0;

            //Debug.Log("***** OnPreRenderCanvas() *****"); // Frame: " + Time.frameCount); // + "  Rect: " + m_rectTransform.rect); // Assigned Material is " + m_uiRenderer.GetMaterial().name); // isInputParsingRequired = " + isInputParsingRequired);

            // Check if Transform has changed since last update.
            if (m_rectTransform.hasChanged || m_marginsHaveChanged)
            {
                //Debug.Log("RectTransform has changed."); // Current Width: " + m_rectTransform.rect.width + " and  Height: " + m_rectTransform.rect.height);

                // Update Pivot of Inline Graphic Component if Pivot has changed.
                // TODO : Should probably also update anchors
                if (m_inlineGraphics != null)
                    m_inlineGraphics.UpdatePivot(m_rectTransform.pivot);
                   
                
                // If Dimension Changed or Margin (Regenerate the Mesh)
                if (m_rectTransformDimensionsChanged || m_marginsHaveChanged)
                {
                    //Debug.Log("RectTransform Dimensions or Margins have changed.");
                    ComputeMarginSize();

                    if (m_marginsHaveChanged)
                       m_isScrollRegionSet = false;


                    m_rectTransformDimensionsChanged = false;
                    m_marginsHaveChanged = false;
                    m_isCalculateSizeRequired = true;

                    m_havePropertiesChanged = true;
                }

                // Update Mask
                if (m_isMaskingEnabled)
                {
                    UpdateMask();
                }

                m_rectTransform.hasChanged = false;


                // We need to regenerate the mesh if the lossy scale has changed.
                Vector3 currentLossyScale = m_rectTransform.lossyScale;
                if (currentLossyScale != m_previousLossyScale)
                {
                    // Update UV2 Scale - only if we don't have to regenerate the text object.
                    if (m_havePropertiesChanged == false && m_previousLossyScale.z != 0 && m_text != string.Empty)
                        UpdateSDFScale(m_previousLossyScale.z, currentLossyScale.z);
                    else
                        m_havePropertiesChanged = true;

                    m_previousLossyScale = currentLossyScale;
                } 
            }


            if (m_havePropertiesChanged || m_fontAsset.propertiesChanged || m_isLayoutDirty)
            {
                //Debug.Log("Properties have changed!"); // Assigned Material is:" + m_sharedMaterial); // New Text is: " + m_text + ".");
                
                // Make sure Text Object is parented to a Canvas.
                if (m_canvas == null) m_canvas = GetComponentInParent<Canvas>();
                if (m_canvas == null) return;


                if (hasFontAssetChanged || m_fontAsset.propertiesChanged)
                {
                    //Debug.Log("Font Asset has changed. Loading new font asset."); 
                    
                    LoadFontAsset();

                    hasFontAssetChanged = false;

                    if (m_fontAsset == null || m_uiRenderer.GetMaterial() == null)
                        return;

                    m_fontAsset.propertiesChanged = false;
                }

                // Re-parse the text if the input has changed.
                if (isInputParsingRequired || m_isTextTruncated)
                    ParseInputText();
                    

                // Reset Font min / max used with Auto-sizing
                if (m_enableAutoSizing)
                    m_fontSize = Mathf.Clamp(m_fontSize, m_fontSizeMin, m_fontSizeMax);

                m_maxFontSize = m_fontSizeMax;
                m_minFontSize = m_fontSizeMin;
                m_lineSpacingDelta = 0;
                m_charWidthAdjDelta = 0;
                m_recursiveCount = 0;

                m_isCharacterWrappingEnabled = false;
                m_isTextTruncated = false;

                m_isLayoutDirty = false;

                GenerateTextMesh();
                m_havePropertiesChanged = false;
            }
        }



        /// <summary>
        /// This is the main function that is responsible for creating / displaying the text.
        /// </summary>
        void GenerateTextMesh()
        {
            //Debug.Log("***** GenerateTextMesh() ***** Frame: " + Time.frameCount); // + ". Point Size: " + m_fontSize + ". Margins are (W) " + m_marginWidth + "  (H) " + m_marginHeight); // ". Iteration Count: " + loopCountA + ".  Min: " + m_minFontSize + "  Max: " + m_maxFontSize + "  Delta: " + (m_maxFontSize - m_minFontSize) + "  Font size is " + m_fontSize); //called for Object with ID " + GetInstanceID()); // Assigned Material is " + m_uiRenderer.GetMaterial().name); // IncludeForMasking " + this.m_IncludeForMasking); // and text is " + m_text);
            //Debug.Log(this.defaultMaterial.GetInstanceID() + "  " + m_sharedMaterial.GetInstanceID() + "  " + m_uiRenderer.GetMaterial().GetInstanceID());
            
            // Early exit if no font asset was assigned. This should not be needed since Arial SDF will be assigned by default.
            if (m_fontAsset.characterDictionary == null)
            {
                Debug.Log("Can't Generate Mesh! No Font Asset has been assigned to Object ID: " + this.GetInstanceID());
                return;
            }

            // Reset TextInfo
            if (m_textInfo != null)
                m_textInfo.Clear();


            // Early exit if we don't have any Text to generate.
            if (m_char_buffer == null || m_char_buffer.Length == 0 || m_char_buffer[0] == (char)0)
            {
                //Debug.Log("Early Out! No Text has been set.");
                //Vector3[] vertices = m_textInfo.meshInfo.vertices;

                m_uiRenderer.SetMesh(null);

                if (m_inlineGraphics != null) m_inlineGraphics.ClearUIVertex();
                
                /*
                if (vertices != null)
                {
                    Array.Clear(vertices, 0, vertices.Length);
                    m_textInfo.meshInfo.mesh.vertices = vertices; 
                }
                */

                m_preferredWidth = 0;
                m_preferredHeight = 0;
                m_renderedWidth = 0;
                m_renderedHeight = 0;

                // This should only be called if there is a layout component attached
                LayoutRebuilder.MarkLayoutForRebuild(m_rectTransform);
                return;
            }

            m_currentFontAsset = m_fontAsset;
            m_currentMaterial = m_sharedMaterial;

            // Determine how many characters will be visible and make the necessary allocations (if needed).
            int totalCharacterCount = SetArraySizes(m_char_buffer);

            // Scale the font to approximately match the point size
            m_fontScale = (m_fontSize / m_currentFontAsset.fontInfo.PointSize);
            float baseScale = m_fontScale; // BaseScale keeps the character aligned vertically since <size=+000> results in font of different scale.
            m_maxFontScale = baseScale;
            float previousLineMaxScale = baseScale;
            float firstVisibleCharacterScale = 0;
            float spriteScale = 1;
            m_currentFontSize = m_fontSize;
            float fontSizeDelta = 0;

            int charCode = 0; // Holds the character code of the currently being processed character.
            //int prev_charCode = 0;
            bool isMissingCharacter; // Used to handle missing characters in the Font Atlas / Definition.

            m_style = m_fontStyle; // Set the default style.
            m_lineJustification = m_textAlignment; // Sets the line justification mode to match editor alignment.

            // GetPadding to adjust the size of the mesh due to border thickness, softness, glow, etc...
            if (checkPaddingRequired)
            {
                checkPaddingRequired = false;
                m_padding = ShaderUtilities.GetPadding(m_uiRenderer.GetMaterial(), m_enableExtraPadding, m_isUsingBold);
                //m_alignmentPadding = ShaderUtilities.GetFontExtent(m_sharedMaterial);
                m_isMaskingEnabled = ShaderUtilities.IsMaskingEnabled(m_sharedMaterial);
            }

            float style_padding = 0; // Extra padding required to accommodate Bold style.
            float xadvance_multiplier = 1; // Used to increase spacing between character when style is bold.

            m_baselineOffset = 0; // Used by subscript characters.

            // Underline
            bool beginUnderline = false;
            Vector3 underline_start = Vector3.zero; // Used to track where underline starts & ends.
            Vector3 underline_end = Vector3.zero;

            // Strike-through
            bool beginStrikethrough = false;
            Vector3 strikethrough_start = Vector3.zero;
            Vector3 strikethrough_end = Vector3.zero;

            m_fontColor32 = m_fontColor;
            Color32 vertexColor;
            m_htmlColor = m_fontColor32;
            m_colorStackIndex = 0;
            Array.Clear(m_colorStack, 0, m_colorStack.Length);

            m_styleStackIndex = 0;
            Array.Clear(m_styleStack, 0, m_styleStack.Length);

            m_lineOffset = 0; // Amount of space between lines (font line spacing + m_linespacing).
            m_lineHeight = 0;

            m_cSpacing = 0; // Amount of space added between characters as a result of the use of the <cspace> tag.
            m_monoSpacing = 0;
            float lineOffsetDelta = 0;
            m_xAdvance = 0; // Used to track the position of each character.
            m_maxXAdvance = 0;

            tag_LineIndent = 0; // Used for indentation of text.
            tag_Indent = 0;
            tag_NoParsing = false;
            m_isIgnoringAlignment = false;

            m_characterCount = 0; // Total characters in the char[]
            m_visibleCharacterCount = 0; // # of visible characters.
            m_visibleSpriteCount = 0;

            // Tracking of line information
            m_firstCharacterOfLine = 0;
            m_lastCharacterOfLine = 0;
            m_firstVisibleCharacterOfLine = 0;
            m_lastVisibleCharacterOfLine = 0;
            m_lineNumber = 0;
            bool isStartOfNewLine = true;

            m_pageNumber = 0;
            int pageToDisplay = Mathf.Clamp(m_pageToDisplay - 1, 0, m_textInfo.pageInfo.Length - 1);

            int ellipsisIndex = 0;

            m_rectTransform.GetLocalCorners(m_rectCorners);
            Vector4 margins = m_margin;
            float marginWidth = m_marginWidth;
            float marginHeight = m_marginHeight;
            m_marginLeft = 0;
            m_marginRight = 0;
            m_width = -1;
            
            // Used by Unity's Auto Layout system.
            m_renderedWidth = 0;
            m_renderedHeight = 0;

            // Initialize struct to track states of word wrapping
            bool isFirstWord = true;
            bool isLastBreakingChar = false;
            //bool isEastAsianLanguage = false;
            m_SavedLineState = new WordWrapState();
            m_SavedWordWrapState = new WordWrapState();
            int wrappingIndex = 0;

            // Need to initialize these Extents structures
            m_meshExtents = new Extents(k_InfinityVector, -k_InfinityVector);

            // Initialize lineInfo
            if (m_textInfo.lineInfo == null) m_textInfo.lineInfo = new TMP_LineInfo[2];
            for (int i = 0; i < m_textInfo.lineInfo.Length; i++)
            {
                m_textInfo.lineInfo[i] = new TMP_LineInfo();
                m_textInfo.lineInfo[i].lineExtents = new Extents(k_InfinityVector, -k_InfinityVector);
                m_textInfo.lineInfo[i].ascender = -k_InfinityVector.x;
                m_textInfo.lineInfo[i].descender = k_InfinityVector.x;
            }


            // Tracking of the highest Ascender
            m_maxAscender = 0;
            m_maxDescender = 0;
            float pageAscender = 0;
            float maxVisibleDescender = 0;
            bool isMaxVisibleDescenderSet = false;
            m_isNewPage = false;

            loopCountA += 1;

            int endTagIndex = 0;
            // Parse through Character buffer to read HTML tags and begin creating mesh.
            for (int i = 0; m_char_buffer[i] != 0; i++)
            {
                charCode = m_char_buffer[i];
                m_isSprite = false;
                spriteScale = 1;

                //Debug.Log("i:" + i + "  Character [" + (char)charCode + "] with ASCII of " + charCode);
                //if (m_characterCount >= m_maxVisibleCharacters || m_lineNumber >= m_maxVisibleLines)
                //    break;

                // Parse Rich Text Tag
                #region Parse Rich Text Tag
                if (m_isRichText && charCode == 60)  // '<'
                {
                    m_isParsingText = true;

                    // Check if Tag is valid. If valid, skip to the end of the validated tag.
                    if (ValidateHtmlTag(m_char_buffer, i + 1, out endTagIndex))
                    {
                        i = endTagIndex;

                        if (m_isRecalculateScaleRequired)
                        {
                            m_fontScale = m_currentFontSize / m_currentFontAsset.fontInfo.PointSize;
                            m_isRecalculateScaleRequired = false;
                        }

                        if (!m_isSprite)
                            continue;
                    }
                }
                #endregion End Parse Rich Text Tag

                m_isParsingText = false;
                isMissingCharacter = false;

                // Check if we should be using a different font asset
                //if (m_fontIndex != 0)
                //{
                //    // Check if we need to load the new font asset
                //    if (m_currentFontAsset == null)
                //    {
                //        Debug.Log("Loading secondary font asset.");
                //        m_currentFontAsset = Resources.Load("Fonts & Materials/Bangers SDF", typeof(TextMeshProFont)) as TextMeshProFont;
                //        //m_sharedMaterials.Add(m_currentFontAsset.material);
                //        //m_renderer.sharedMaterials = new Material[] { m_sharedMaterial, m_currentFontAsset.material }; // m_sharedMaterials.ToArray();
                //    }
                //}               
                //Debug.Log("Char [" + (char)charCode + "] is using FontIndex: " + m_fontIndex);



                // Handle Font Styles like LowerCase, UpperCase and SmallCaps.
                #region Handling of LowerCase, UpperCase and SmallCaps Font Styles
                if ((m_style & FontStyles.UpperCase) == FontStyles.UpperCase)
                {
                    // If this character is lowercase, switch to uppercase.
                    if (char.IsLower((char)charCode))
                        charCode = char.ToUpper((char)charCode);

                }
                else if ((m_style & FontStyles.LowerCase) == FontStyles.LowerCase)
                {
                    // If this character is uppercase, switch to lowercase.
                    if (char.IsUpper((char)charCode))
                        charCode = char.ToLower((char)charCode);
                }
                else if ((m_fontStyle & FontStyles.SmallCaps) == FontStyles.SmallCaps || (m_style & FontStyles.SmallCaps) == FontStyles.SmallCaps)
                {
                    if (char.IsLower((char)charCode))
                    {
                        m_fontScale = m_currentFontSize * 0.8f / m_currentFontAsset.fontInfo.PointSize;
                        charCode = char.ToUpper((char)charCode);
                    }
                    else
                        m_fontScale = m_currentFontSize / m_currentFontAsset.fontInfo.PointSize;

                }
                #endregion


                // Look up Character Data from Dictionary and cache it.
                #region Look up Character Data
                if (m_isSprite)
                {
                    SpriteInfo spriteInfo = m_inlineGraphics.GetSprite(m_spriteIndex);
                    if (spriteInfo == null) continue;

                    // Sprites are assigned in the E000 Private Area + sprite Index
                    charCode = 57344 + m_spriteIndex;

                    m_cached_GlyphInfo = new GlyphInfo(); // Generates 40 bytes

                    m_cached_GlyphInfo.x = spriteInfo.x;
                    m_cached_GlyphInfo.y = spriteInfo.y;
                    m_cached_GlyphInfo.width = spriteInfo.width;
                    m_cached_GlyphInfo.height = spriteInfo.height;
                    m_cached_GlyphInfo.xOffset = spriteInfo.pivot.x + spriteInfo.xOffset;
                    m_cached_GlyphInfo.yOffset = spriteInfo.pivot.y + spriteInfo.yOffset;

                    spriteScale = m_fontAsset.fontInfo.Ascender / spriteInfo.height * spriteInfo.scale;
                   
                    m_cached_GlyphInfo.xAdvance = spriteInfo.xAdvance * spriteScale;

                    m_textInfo.characterInfo[m_characterCount].type = TMP_CharacterType.Sprite;
                }
                else
                {
                    m_currentFontAsset.characterDictionary.TryGetValue(charCode, out m_cached_GlyphInfo);
                    if (m_cached_GlyphInfo == null)
                    {
                        // Character wasn't found in the Dictionary.

                        if (char.IsLower((char)charCode))
                        {
                            if (m_currentFontAsset.characterDictionary.TryGetValue(char.ToUpper((char)charCode), out m_cached_GlyphInfo))
                                charCode = char.ToUpper((char)charCode);
                        }
                        else if (char.IsUpper((char)charCode))
                        {
                            if (m_currentFontAsset.characterDictionary.TryGetValue(char.ToLower((char)charCode), out m_cached_GlyphInfo))
                                charCode = char.ToLower((char)charCode);
                        }

                        // Still don't have a replacement?
                        if (m_cached_GlyphInfo == null)
                        {
                            m_currentFontAsset.characterDictionary.TryGetValue(32, out m_cached_GlyphInfo);
                            if (m_cached_GlyphInfo != null)
                            {
                                Debug.LogWarning("Character with ASCII value of " + charCode + " was not found in the Font Asset Glyph Table.", this);
                                // Replace the missing character by 'space' (if it is found)
								// If TextMeshPro ever gets updated, remember to reapply this change!!!!!!!!!!
								// also the TryGetValue a few lines above should match ^^
	                            charCode = 32;
                                isMissingCharacter = true;
                            }
                            else
                            {  // At this point the character isn't in the Dictionary, the replacement 'space' isn't either so ...
                                Debug.LogWarning("Character with ASCII value of " + charCode + " was not found in the Font Asset Glyph Table.", this);
                                continue;
                            }
                        }
                    }

                    m_textInfo.characterInfo[m_characterCount].type = TMP_CharacterType.Character;
                }
                #endregion

                // Store some of the text object's information
                m_textInfo.characterInfo[m_characterCount].character = (char)charCode;
                m_textInfo.characterInfo[m_characterCount].pointSize = m_currentFontSize;
                m_textInfo.characterInfo[m_characterCount].color = m_htmlColor;
                m_textInfo.characterInfo[m_characterCount].style = m_style;
                m_textInfo.characterInfo[m_characterCount].index = (short)i;
                //m_textInfo.characterInfo[m_characterCount].isIgnoringAlignment = m_isIgnoringAlignment;


                // Handle Kerning if Enabled.
                #region Handle Kerning
                if (m_enableKerning && m_characterCount >= 1)
                {
                    int prev_charCode = m_textInfo.characterInfo[m_characterCount - 1].character;
                    KerningPairKey keyValue = new KerningPairKey(prev_charCode, charCode);

                    KerningPair pair;

                    m_currentFontAsset.kerningDictionary.TryGetValue(keyValue.key, out pair);
                    if (pair != null)
                    {
                        m_xAdvance += pair.XadvanceOffset * m_fontScale;
                    }
                }
                #endregion


                // Handle Mono Spacing
                #region Handle Mono Spacing
                float monoAdvance = 0;
                if (m_monoSpacing != 0)
                {
                    monoAdvance = (m_monoSpacing / 2 - (m_cached_GlyphInfo.width / 2 + m_cached_GlyphInfo.xOffset) * m_fontScale) * (1 - m_charWidthAdjDelta);
                    m_xAdvance += monoAdvance;
                }
                #endregion


                // Set Padding based on selected font style
                #region Handle Style Padding
                if ((m_style & FontStyles.Bold) == FontStyles.Bold || (m_fontStyle & FontStyles.Bold) == FontStyles.Bold) // Checks for any combination of Bold Style.
                {
                    style_padding = m_currentFontAsset.BoldStyle * 2;
                    xadvance_multiplier = 1 + m_currentFontAsset.boldSpacing * 0.01f;
                }
                else
                {
                    style_padding = m_currentFontAsset.NormalStyle * 2;
                    xadvance_multiplier = 1.0f;
                }
                #endregion Handle Style Padding


                // Set padding value if Character or Sprite
                float padding = m_isSprite ? 0 : m_padding;

                // Determine the position of the vertices of the Character or Sprite.
                Vector3 top_left = new Vector3(0 + m_xAdvance + ((m_cached_GlyphInfo.xOffset - padding - style_padding) * m_fontScale * spriteScale * (1 - m_charWidthAdjDelta)), (m_cached_GlyphInfo.yOffset + padding) * m_fontScale * spriteScale - m_lineOffset + m_baselineOffset, 0);
                Vector3 bottom_left = new Vector3(top_left.x, top_left.y - ((m_cached_GlyphInfo.height + padding * 2) * m_fontScale * spriteScale), 0);
                Vector3 top_right = new Vector3(bottom_left.x + ((m_cached_GlyphInfo.width + padding * 2 + style_padding * 2) * m_fontScale * spriteScale * (1 - m_charWidthAdjDelta)), top_left.y, 0);
                Vector3 bottom_right = new Vector3(top_right.x, bottom_left.y, 0);

                // Check if we need to Shear the rectangles for Italic styles
                #region Handle Italic & Shearing
                if ((m_style & FontStyles.Italic) == FontStyles.Italic || (m_fontStyle & FontStyles.Italic) == FontStyles.Italic)
                {
                    // Shift Top vertices forward by half (Shear Value * height of character) and Bottom vertices back by same amount. 
                    float shear_value = m_currentFontAsset.ItalicStyle * 0.01f;
                    Vector3 topShear = new Vector3(shear_value * ((m_cached_GlyphInfo.yOffset + padding + style_padding) * m_fontScale * spriteScale), 0, 0);
                    Vector3 bottomShear = new Vector3(shear_value * (((m_cached_GlyphInfo.yOffset - m_cached_GlyphInfo.height - padding - style_padding)) * m_fontScale * spriteScale), 0, 0);

                    top_left = top_left + topShear;
                    bottom_left = bottom_left + bottomShear;
                    top_right = top_right + topShear;
                    bottom_right = bottom_right + bottomShear;
                }
                #endregion Handle Italics & Shearing


                // Store position of the vertices for the Character or Sprite.
                m_textInfo.characterInfo[m_characterCount].bottomLeft = bottom_left;
                m_textInfo.characterInfo[m_characterCount].topLeft = top_left;
                m_textInfo.characterInfo[m_characterCount].topRight = top_right;
                m_textInfo.characterInfo[m_characterCount].bottomRight = bottom_right;
                m_textInfo.characterInfo[m_characterCount].baseLine = 0 - m_lineOffset + m_baselineOffset;
                m_textInfo.characterInfo[m_characterCount].scale = m_fontScale;


                // Compute MaxAscender & MaxDescender which is used for AutoScaling & other type layout options
                float ascender = m_fontAsset.fontInfo.Ascender * m_fontScale + m_baselineOffset;
                if ((charCode == 10 || charCode == 13) && m_characterCount > m_firstVisibleCharacterOfLine)
                    ascender = m_baselineOffset;

                float descender = m_fontAsset.fontInfo.Descender * m_fontScale - m_lineOffset + m_baselineOffset;

                // Check if Sprite exceeds the Ascender and Descender of the font and if so make the adjustment.
                if (m_isSprite)
                {
                    ascender = Mathf.Max(ascender, top_left.y - padding * m_fontScale * spriteScale);
                    descender = Mathf.Min(descender, bottom_right.y - padding * m_fontScale * spriteScale);
                }

                if (m_lineNumber == 0) m_maxAscender = m_maxAscender > ascender ? m_maxAscender : ascender;
                if (m_lineOffset == 0) pageAscender = pageAscender > ascender ? pageAscender : ascender;

                // Track Line Height
                //maxLineHeight = Mathf.Max(m_lineHeight, maxLineHeight);

                // Used to adjust line spacing when larger fonts or the size tag is used.
                if (m_baselineOffset == 0)
                    m_maxFontScale = Mathf.Max(m_maxFontScale, m_fontScale);

                // Set Characters to not visible by default.
                m_textInfo.characterInfo[m_characterCount].isVisible = false;


                // Setup Mesh for visible characters or sprites. ie. not a SPACE / LINEFEED / CARRIAGE RETURN.
                #region Handle Visible Characters
                if (charCode != 10 && charCode != 13 && charCode != 32 && charCode != 160 || m_isSprite)
                {
                    m_textInfo.characterInfo[m_characterCount].isVisible = true;
                    //if (isStartOfNewLine) { isStartOfNewLine = false; m_firstVisibleCharacterOfLine = m_characterCount; }

                    // Check if Character exceeds the width of the Text Container
                    #region Check for Characters Exceeding Width of Text Container
                    float width = m_width != -1 ? Mathf.Min(marginWidth + 0.0001f - m_marginLeft - m_marginRight, m_width) : marginWidth + 0.0001f - m_marginLeft - m_marginRight;

                    m_textInfo.lineInfo[m_lineNumber].width = width;
                    m_textInfo.lineInfo[m_lineNumber].marginLeft = m_marginLeft;

                    if (m_xAdvance + m_cached_GlyphInfo.xAdvance * (1 - m_charWidthAdjDelta) * m_fontScale > width)
                    {
                        ellipsisIndex = m_characterCount - 1; // Last safely rendered character

                        // Word Wrapping
                        #region Handle Word Wrapping
                        if (enableWordWrapping && m_characterCount != m_firstCharacterOfLine)
                        {

                            if (wrappingIndex == m_SavedWordWrapState.previous_WordBreak || isFirstWord)
                            {
                                // Word wrapping is no longer possible. Shrink size of text if auto-sizing is enabled.
                                if (m_enableAutoSizing && m_fontSize > m_fontSizeMin)
                                {

                                    // Handle Character Width Adjustments
                                    #region Character Width Adjustments
                                    if (m_charWidthAdjDelta < m_charWidthMaxAdj / 100)
                                    {
                                        loopCountA = 0;
                                        m_charWidthAdjDelta += 0.01f;
                                        GenerateTextMesh();
                                        return;
                                    }
                                    #endregion
 
                                    // Adjust Point Size 
                                    m_maxFontSize = m_fontSize;

                                    m_fontSize -= Mathf.Max((m_fontSize - m_minFontSize) / 2, 0.05f);
                                    m_fontSize = (int)(Mathf.Max(m_fontSize, m_fontSizeMin) * 20 + 0.5f) / 20f;

                                    if (loopCountA > 20) return; // Added to debug
                                    GenerateTextMesh();
                                    return;
                                }

                                // Word wrapping is no longer possible, now breaking up individual words.
                                if (m_isCharacterWrappingEnabled == false)
                                {
                                    m_isCharacterWrappingEnabled = true;
                                }
                                else
                                    isLastBreakingChar = true;

                                //Debug.Log("Wrapping Index " + wrappingIndex + ". Recursive Count: " + m_recursiveCount);

                                m_recursiveCount += 1;
                                if (m_recursiveCount > 20)
                                {
                                    //Debug.Log("Recursive count exceeded!");
                                    continue;
                                }

                                //Debug.Log("Line #" + m_lineNumber + " Character [" + (char)charCode + "] cannot be wrapped.  WrappingIndex: " + wrappingIndex + "  Saved Index: " + m_SavedWordWrapState.previous_WordBreak + ". Character Count is " + m_characterCount);
                            }


                            // Restore to previously stored state of last valid (space character or linefeed)
                            i = RestoreWordWrappingState(ref m_SavedWordWrapState);
                            wrappingIndex = i;  // Used to detect when line length can no longer be reduced.

                            //Debug.Log("Last Visible Character of line # " + m_lineNumber + " is [" + m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].character + " Character Count: " + m_characterCount + " Last visible: " + m_lastVisibleCharacterOfLine);

                            // Check if Line Spacing of previous line needs to be adjusted.
                            FaceInfo face = m_currentFontAsset.fontInfo;
                            float gap = m_lineHeight == 0 ? face.LineHeight - (face.Ascender - face.Descender) : m_lineHeight - (face.Ascender - face.Descender);
                            if (m_lineNumber > 0 && m_maxFontScale != 0 && m_lineHeight == 0 && firstVisibleCharacterScale != m_maxFontScale && !m_isNewPage)
                            {
                                float offsetDelta = 0 - face.Descender * previousLineMaxScale + (face.Ascender + gap + m_lineSpacing + m_paragraphSpacing + m_lineSpacingDelta) * m_maxFontScale;
                                m_lineOffset += offsetDelta - lineOffsetDelta;
                                AdjustLineOffset(m_firstCharacterOfLine, m_characterCount - 1, offsetDelta - lineOffsetDelta);
                                m_SavedWordWrapState.lineOffset = m_lineOffset;
                            }
                            m_isNewPage = false;


                            // Calculate lineAscender & make sure if last character is superscript or subscript that we check that as well.
                            float lineAscender = m_fontAsset.fontInfo.Ascender * m_maxFontScale - m_lineOffset;
                            float lineAscender2 = m_fontAsset.fontInfo.Ascender * m_fontScale - m_lineOffset + m_baselineOffset;
                            lineAscender = lineAscender > lineAscender2 ? lineAscender : lineAscender2;

                            // Calculate lineDescender & make sure if last character is superscript or subscript that we check that as well.
                            float lineDescender = m_fontAsset.fontInfo.Descender * m_maxFontScale - m_lineOffset;
                            float lineDescender2 = m_fontAsset.fontInfo.Descender * m_fontScale - m_lineOffset + m_baselineOffset;
                            lineDescender = lineDescender < lineDescender2 ? lineDescender : lineDescender2;

                            // Update maxDescender and maxVisibleDescender
                            m_maxDescender = m_maxDescender < lineDescender ? m_maxDescender : lineDescender;
                            if (!isMaxVisibleDescenderSet)
                                maxVisibleDescender = m_maxDescender;

                            if (m_characterCount >= m_maxVisibleCharacters || m_lineNumber >= m_maxVisibleLines)
                                isMaxVisibleDescenderSet = true;

                            // Track & Store lineInfo for the new line
                            m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex = m_firstCharacterOfLine;
                            m_textInfo.lineInfo[m_lineNumber].firstVisibleCharacterIndex = m_firstVisibleCharacterOfLine;
                            m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex = m_characterCount - 1 > 0 ? m_characterCount - 1 : 0;
                            m_textInfo.lineInfo[m_lineNumber].lastVisibleCharacterIndex = m_lastVisibleCharacterOfLine;
                            m_textInfo.lineInfo[m_lineNumber].characterCount = m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex - m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex + 1;

                            m_textInfo.lineInfo[m_lineNumber].lineExtents.min = new Vector2(m_textInfo.characterInfo[m_firstVisibleCharacterOfLine].bottomLeft.x, lineDescender);
                            m_textInfo.lineInfo[m_lineNumber].lineExtents.max = new Vector2(m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].topRight.x, lineAscender);
                            m_textInfo.lineInfo[m_lineNumber].lineLength = m_textInfo.lineInfo[m_lineNumber].lineExtents.max.x - padding * m_maxFontScale;
                            m_textInfo.lineInfo[m_lineNumber].maxAdvance = m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].xAdvance - m_characterSpacing * m_fontScale;
                            m_textInfo.lineInfo[m_lineNumber].maxScale = m_maxFontScale;

                            m_firstCharacterOfLine = m_characterCount; // Store first character of the next line.

                            // Compute Preferred Width & Height
                            m_renderedWidth += m_xAdvance;
                            if (m_enableWordWrapping)
                                m_renderedHeight = m_maxAscender - m_maxDescender;
                            else
                                m_renderedHeight = Mathf.Max(m_renderedHeight, lineAscender - lineDescender);

                            //Debug.Log("Line # " + m_lineNumber + "  Max Font Scale: " + m_maxFontScale + "  Current Font Scale: " + currentFontScale);
                            //Debug.Log("LineInfo for line # " + (m_lineNumber) + " First character [" + m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex].character + "] at Index: " + m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex +
                            //                                                    " First visible character [" + m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].firstVisibleCharacterIndex].character + "] at Index: " + m_textInfo.lineInfo[m_lineNumber].firstVisibleCharacterIndex +
                            //                                                    " Last character [" + m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex].character + "] at index: " + m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex +
                            //                                                    " Last Visible character [" + m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].lastVisibleCharacterIndex].character + "] at index: " + m_textInfo.lineInfo[m_lineNumber].lastVisibleCharacterIndex +
                            //                                                    " Character Count of " + m_textInfo.lineInfo[m_lineNumber].characterCount /* + " Line Length of " + m_textInfo.lineInfo[m_lineNumber].lineLength +
                            //                                                    "  MinX: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.min.x + "  MinY: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.min.y +
                            //                                                    "  MaxX: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.max.x + "  MaxY: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.max.y +
                            //                                                    "  Line Ascender: " + lineAscender + "  Line Descender: " + lineDescender */ );

                            // Store the state of the line before starting on the new line.
                            SaveWordWrappingState(ref m_SavedLineState, i, m_characterCount - 1);

                            m_lineNumber += 1;
                            isStartOfNewLine = true;

                            // Check to make sure Array is large enough to hold a new line.
                            if (m_lineNumber >= m_textInfo.lineInfo.Length)
                                ResizeLineExtents(m_lineNumber);

                            // Apply Line Spacing based on scale of the last character of the line.
                            FontStyles style = m_textInfo.characterInfo[m_characterCount].style;
                            float scale = (style & FontStyles.Subscript) == FontStyles.Subscript || (style & FontStyles.Superscript) == FontStyles.Superscript ? m_maxFontScale : m_textInfo.characterInfo[m_characterCount].scale;
                            lineOffsetDelta = 0 - face.Descender * m_maxFontScale + (face.Ascender + gap + m_lineSpacing + m_lineSpacingDelta) * scale;
                            m_lineOffset += lineOffsetDelta;


                            previousLineMaxScale = m_maxFontScale;
                            firstVisibleCharacterScale = scale;
                            m_maxFontScale = 0;
                            spriteScale = 1;
                            m_xAdvance = 0 + tag_Indent;

                            continue;
                        }
                        #endregion End Word Wrapping


                        // Text Auto-Sizing (text exceeding Width of container. 
                        #region Handle Text Auto-Sizing
                        if (m_enableAutoSizing && m_fontSize > m_fontSizeMin)
                        {
                            // Handle Character Width Adjustments
                            #region Character Width Adjustments
                            if (m_charWidthAdjDelta < m_charWidthMaxAdj / 100)
                            {
                                loopCountA = 0;
                                m_charWidthAdjDelta += 0.01f;
                                GenerateTextMesh();
                                return;
                            }
                            #endregion

                            // Adjust Point Size
                            m_maxFontSize = m_fontSize;

                            m_fontSize -= Mathf.Max((m_fontSize - m_minFontSize) / 2, 0.05f);
                            m_fontSize = (int)(Mathf.Max(m_fontSize, m_fontSizeMin) * 20 + 0.5f) / 20f;

                            m_recursiveCount = 0;
                            if (loopCountA > 20) return; // Added to debug 
                            GenerateTextMesh();
                            return;
                        }
                        #endregion End Text Auto-Sizing


                        // Handle Text Overflow
                        #region Handle Text Overflow
                        switch (m_overflowMode)
                        {
                            case TextOverflowModes.Overflow:
                                if (m_isMaskingEnabled)
                                    DisableMasking();

                                break;
                            case TextOverflowModes.Ellipsis:
                                if (m_isMaskingEnabled)
                                    DisableMasking();

                                m_isTextTruncated = true;

                                if (m_characterCount < 1)
                                {
                                    m_textInfo.characterInfo[m_characterCount].isVisible = false;
                                    m_visibleCharacterCount -= 1;
                                    break;
                                }

                                m_char_buffer[i - 1] = 8230;
                                m_char_buffer[i] = (char)0;

                                GenerateTextMesh();
                                return;
                            case TextOverflowModes.Masking:
                                if (!m_isMaskingEnabled)
                                    EnableMasking();
                                break;
                            case TextOverflowModes.ScrollRect:
                                if (!m_isMaskingEnabled)
                                    EnableMasking();
                                break;
                            case TextOverflowModes.Truncate:
                                if (m_isMaskingEnabled)
                                    DisableMasking();

                                m_textInfo.characterInfo[m_characterCount].isVisible = false;
                                break;
                        }
                        #endregion End Text Overflow

                    }
                    #endregion End Check for Characters Exceeding Width of Text Container

                    if (charCode != 9)
                    {
                        // Determine Vertex Color
                        if (isMissingCharacter)
                            vertexColor = Color.red;
                        else if (m_overrideHtmlColors)
                            vertexColor = m_fontColor32;
                        else
                            vertexColor = m_htmlColor;


                        // Store Character & Sprite Vertex Information
                        if (!m_isSprite)
                            SaveGlyphVertexInfo(style_padding, vertexColor);
                        else
                            SaveSpriteVertexInfo(vertexColor);
                    }
                    else // If character is Tab
                    {
                        m_textInfo.characterInfo[m_characterCount].isVisible = false;
                        m_lastVisibleCharacterOfLine = m_characterCount;
                        m_textInfo.lineInfo[m_lineNumber].spaceCount += 1;
                        m_textInfo.spaceCount += 1;
                    }

                    // Increase visible count for Characters.
                    if (m_textInfo.characterInfo[m_characterCount].isVisible)
                    {
                        if (m_isSprite)
                            m_visibleSpriteCount += 1;
                        else
                            m_visibleCharacterCount += 1;

                        if (isStartOfNewLine) { isStartOfNewLine = false; m_firstVisibleCharacterOfLine = m_characterCount; }
                        m_lastVisibleCharacterOfLine = m_characterCount;
                    }

                }
                else
                {   // This is a Space, Tab, LineFeed or Carriage Return

                    // Track # of spaces per line which is used for line justification.
                    if (charCode == 9 || charCode == 32 || charCode == 160)
                    {
                        m_textInfo.lineInfo[m_lineNumber].spaceCount += 1;
                        m_textInfo.spaceCount += 1;
                    }
                }
                #endregion Handle Visible Characters


                // Store Rectangle positions for each Character.
                #region Store Character Data
                m_textInfo.characterInfo[m_characterCount].lineNumber = (short)m_lineNumber;
                m_textInfo.characterInfo[m_characterCount].pageNumber = (short)m_pageNumber;

                if (charCode != 10 && charCode != 13 && charCode != 8230 || m_textInfo.lineInfo[m_lineNumber].characterCount == 1)
                    m_textInfo.lineInfo[m_lineNumber].alignment = m_lineJustification;
                #endregion Store Character Data


                // Check if text Exceeds the vertical bounds of the margin area.
                #region Check Vertical Bounds & Auto-Sizing
                if (m_maxAscender - descender > marginHeight + 0.0001f)
                {
                    //Debug.Log((m_maxAscender - descender + (m_alignmentPadding.w * 2 * m_fontScale)).ToString("f6") + "  " + marginHeight.ToString("f6"));
                    //Debug.Log("Character [" + (char)charCode + "] at Index: " + m_characterCount + " has exceeded the Height of the text container. Max Ascender: " + m_maxAscender + "  Max Descender: " + m_maxDescender + "  Margin Height: " + marginHeight + " Bottom Left: " + bottom_left.y);                                              

                    // Handle Line spacing adjustments
                    #region Line Spacing Adjustments
                    if (m_enableAutoSizing && m_lineSpacingDelta > m_lineSpacingMax && m_lineNumber > 0)
                    {
                        m_lineSpacingDelta -= 1;
                        GenerateTextMesh();
                        return;
                    }
                    #endregion


                    // Handle Text Auto-sizing resulting from text exceeding vertical bounds.
                    #region Text Auto-Sizing (Text greater than vertical bounds)
                    if (m_enableAutoSizing && m_fontSize > m_fontSizeMin)
                    {
                        m_maxFontSize = m_fontSize;

                        m_fontSize -= Mathf.Max((m_fontSize - m_minFontSize) / 2, 0.05f);
                        m_fontSize = (int)(Mathf.Max(m_fontSize, m_fontSizeMin) * 20 + 0.5f) / 20f;

                        m_recursiveCount = 0;
                        if (loopCountA > 20) return; // Added to debug 
                        GenerateTextMesh();
                        return;
                    }
                    #endregion Text Auto-Sizing


                    // Handle Text Overflow
                    #region Text Overflow
                    switch (m_overflowMode)
                    {
                        case TextOverflowModes.Overflow:
                            if (m_isMaskingEnabled)
                                DisableMasking();

                            break;
                        case TextOverflowModes.Ellipsis:
                            if (m_isMaskingEnabled)
                                DisableMasking();

                            if (m_lineNumber > 0)
                            {
                                m_char_buffer[m_textInfo.characterInfo[ellipsisIndex].index] = 8230;
                                m_char_buffer[m_textInfo.characterInfo[ellipsisIndex].index + 1] = (char)0;
                                GenerateTextMesh();
                                m_isTextTruncated = true;
                                return;
                            }
                            else
                            {
                                m_char_buffer[0] = (char)0;
                                GenerateTextMesh();
                                m_isTextTruncated = true;
                                return;
                            }
                        case TextOverflowModes.Masking:
                            if (!m_isMaskingEnabled)
                                EnableMasking();
                            break;
                        case TextOverflowModes.ScrollRect:
                            if (!m_isMaskingEnabled)
                                EnableMasking();
                            break;
                        case TextOverflowModes.Truncate:
                         if (m_isMaskingEnabled)
                                DisableMasking();
                    
                            // Alternative Implementation 
                            //if (m_lineNumber > 0)
                            //{
                            //    if (!m_isTextTruncated && m_textInfo.characterInfo[ellipsisIndex + 1].character != 10)
                            //    {
                            //        Debug.Log("Char [" + (char)charCode + "] on line " + m_lineNumber + " exceeds the vertical bounds. Last safe character was " + (int)m_textInfo.characterInfo[ellipsisIndex + 1].character);
                            //        i = RestoreWordWrappingState(ref m_SavedWordWrapState);
                            //        m_lineNumber -= 1;
                            //        m_isTextTruncated = true;
                            //        m_isCharacterWrappingEnabled = true;
                            //        continue;
                            //    }
                            //    else
                            //    {
                            //        //Debug.Log("Char [" + (char)charCode + "] on line " + m_lineNumber + " set to invisible.");
                            //        m_textInfo.characterInfo[m_characterCount].isVisible = false;
                            //    }
                            ////    m_char_buffer[m_textInfo.characterInfo[ellipsisIndex].index + 1] = (char)0;
                            ////    m_isTextTruncated = true;
                            ////    i = RestoreWordWrappingState(ref m_SavedLineState);
                            ////    m_lineNumber -= 1;
                            
                            ////    continue;
                            //}
                            //break;

                            
                            // TODO : Optimize 
                            if (m_lineNumber > 0)
                            {
                                m_char_buffer[m_textInfo.characterInfo[ellipsisIndex].index + 1] = (char)0;
                                GenerateTextMesh();
                                m_isTextTruncated = true;
                                return;
                            }
                            else
                            {
                                m_char_buffer[0] = (char)0;
                                GenerateTextMesh();
                                m_isTextTruncated = true;
                                return;
                            }
                        case TextOverflowModes.Page:
                            if (m_isMaskingEnabled)
                                DisableMasking();

                            // Ignore Page Break, Linefeed or carriage return
                            if (charCode == 13 || charCode == 10)
                                break;

                            //Debug.Log("Character is [" + (char)charCode + "] with ASCII (" + charCode + ") on Page " + m_pageNumber + ". Ascender: " + m_textInfo.pageInfo[m_pageNumber].ascender + "  BaseLine: " + m_textInfo.pageInfo[m_pageNumber].baseLine + "  Descender: " + m_textInfo.pageInfo[m_pageNumber].descender);                          

                            // Go back to previous line and re-layout 
                            i = RestoreWordWrappingState(ref m_SavedLineState);
                            if (i == 0)
                            {
                                m_char_buffer[0] = (char)0;
                                GenerateTextMesh();
                                m_isTextTruncated = true;
                                return;
                            }

                            m_isNewPage = true;
                            m_xAdvance = 0 + tag_Indent;
                            m_lineOffset = 0;
                            m_lineNumber += 1;
                            m_pageNumber += 1;
                            continue;
                    }
                    #endregion End Text Overflow

                }
                #endregion Check Vertical Bounds


                // Handle xAdvance & Tabulation Stops. Tab stops at every 25% of Font Size.
                #region XAdvance, Tabulation & Stops
                if (charCode == 9)
                    m_xAdvance += m_fontAsset.fontInfo.TabWidth * m_fontScale;
                else if (m_monoSpacing != 0)
                    m_xAdvance += (m_monoSpacing - monoAdvance + (m_characterSpacing * m_fontScale) + m_cSpacing) * (1 - m_charWidthAdjDelta);
                else
                    m_xAdvance += ((m_cached_GlyphInfo.xAdvance * xadvance_multiplier + m_characterSpacing) * m_fontScale + m_cSpacing) * (1 - m_charWidthAdjDelta);


                // Store xAdvance information
                m_textInfo.characterInfo[m_characterCount].xAdvance = m_xAdvance;

                #endregion Tabulation & Stops


                // Handle Carriage Return
                #region Carriage Return
                if (charCode == 13)
                {
                    m_maxXAdvance = Mathf.Max(m_maxXAdvance, m_renderedWidth + m_xAdvance);
                    m_renderedWidth = 0;
                    m_xAdvance = 0 + tag_Indent;
                }
                #endregion Carriage Return


                // Handle Line Spacing Adjustments + Word Wrapping & special case for last line.
                #region Check for Line Feed and Last Character
                if (charCode == 10 || m_characterCount == totalCharacterCount - 1)
                {
                    // Check if Line Spacing of previous line needs to be adjusted.
                    FaceInfo face = m_currentFontAsset.fontInfo;
                    float gap = m_lineHeight == 0 ? face.LineHeight - (face.Ascender - face.Descender) : m_lineHeight - (face.Ascender - face.Descender);
                    if (m_lineNumber > 0 && m_maxFontScale != 0 && m_lineHeight == 0 && firstVisibleCharacterScale != m_maxFontScale && !m_isNewPage)
                    {
                        float offsetDelta = 0 - face.Descender * previousLineMaxScale + (face.Ascender + gap + m_lineSpacing + m_paragraphSpacing + m_lineSpacingDelta) * m_maxFontScale;
                        m_lineOffset += offsetDelta - lineOffsetDelta;
                        AdjustLineOffset(m_firstCharacterOfLine, m_characterCount, offsetDelta - lineOffsetDelta);
                    }
                    m_isNewPage = false;

                    // Calculate lineAscender & make sure if last character is superscript or subscript that we check that as well.
                    float lineAscender = m_fontAsset.fontInfo.Ascender * m_maxFontScale - m_lineOffset;
                    float lineAscender2 = m_fontAsset.fontInfo.Ascender * m_fontScale - m_lineOffset + m_baselineOffset;
                    lineAscender = lineAscender > lineAscender2 ? lineAscender : lineAscender2;

                    // Calculate lineDescender & make sure if last character is superscript or subscript that we check that as well.
                    float lineDescender = m_fontAsset.fontInfo.Descender * m_maxFontScale - m_lineOffset;
                    float lineDescender2 = m_fontAsset.fontInfo.Descender * m_fontScale - m_lineOffset + m_baselineOffset;
                    lineDescender = lineDescender < lineDescender2 ? lineDescender : lineDescender2;

                    // Update maxDescender and maxVisibleDescender
                    m_maxDescender = m_maxDescender < lineDescender ? m_maxDescender : lineDescender;
                    if (!isMaxVisibleDescenderSet)
                        maxVisibleDescender = m_maxDescender;

                    if (m_characterCount >= m_maxVisibleCharacters || m_lineNumber >= m_maxVisibleLines)
                        isMaxVisibleDescenderSet = true;

                    // Save Line Information
                    m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex = m_firstCharacterOfLine;
                    m_textInfo.lineInfo[m_lineNumber].firstVisibleCharacterIndex = m_firstVisibleCharacterOfLine;
                    m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex = m_characterCount;
                    m_textInfo.lineInfo[m_lineNumber].lastVisibleCharacterIndex = m_lastVisibleCharacterOfLine >= m_firstVisibleCharacterOfLine ? m_lastVisibleCharacterOfLine : m_firstVisibleCharacterOfLine;
                    m_textInfo.lineInfo[m_lineNumber].characterCount = m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex - m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex + 1;

                    m_textInfo.lineInfo[m_lineNumber].lineExtents.min = new Vector2(m_textInfo.characterInfo[m_firstVisibleCharacterOfLine].bottomLeft.x, lineDescender);
                    m_textInfo.lineInfo[m_lineNumber].lineExtents.max = new Vector2(m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].topRight.x, lineAscender);
                    m_textInfo.lineInfo[m_lineNumber].lineLength = m_textInfo.lineInfo[m_lineNumber].lineExtents.max.x - (padding * m_maxFontScale);
                    m_textInfo.lineInfo[m_lineNumber].maxAdvance = m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].xAdvance - m_characterSpacing * m_fontScale;
                    m_textInfo.lineInfo[m_lineNumber].maxScale = m_maxFontScale;

                    m_firstCharacterOfLine = m_characterCount + 1;

                    // Store PreferredWidth paying attention to linefeed and last character of text.
                    if (charCode == 10 && m_characterCount != totalCharacterCount - 1)
                    {
                        m_maxXAdvance = Mathf.Max(m_maxXAdvance, m_renderedWidth + m_xAdvance);
                        m_renderedWidth = 0;
                    }
                    else
                        m_renderedWidth = Mathf.Max(m_maxXAdvance, m_renderedWidth + m_xAdvance);
               
                    m_renderedHeight = m_maxAscender - m_maxDescender;

                    //Debug.Log("Line # " + m_lineNumber + "  Max Font Scale: " + m_maxFontScale + "  Next line Scale: " + m_fontScale);
                    //Debug.Log("LineInfo for line # " + (m_lineNumber) + " First character [" + m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex].character + "] at Index: " + m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex +
                    //                                                    " First visible character [" + m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].firstVisibleCharacterIndex].character + "] at Index: " + m_textInfo.lineInfo[m_lineNumber].firstVisibleCharacterIndex +
                    //                                                    " Last character [" + m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex].character + "] at index: " + m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex +
                    //                                                    " Last Visible character [" + m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].lastVisibleCharacterIndex].character + "] at index: " + m_textInfo.lineInfo[m_lineNumber].lastVisibleCharacterIndex +
                    //                                                    " Character Count of " + m_textInfo.lineInfo[m_lineNumber].characterCount /* + " Line Length of " + m_textInfo.lineInfo[m_lineNumber].lineLength +
                    //                                                    "  MinX: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.min.x + "  MinY: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.min.y +
                    //                                                    "  MaxX: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.max.x + "  MaxY: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.max.y +
                    //                                                    "  Line Ascender: " + lineAscender + "  Line Descender: " + lineDescender */ );


                    // Add new line if not last lines or character.
                    if (charCode == 10)
                    {
                        // Store the state of the line before starting on the new line.
                        SaveWordWrappingState(ref m_SavedLineState, i, m_characterCount);
                        // Store the state of the last Character before the new line.
                        SaveWordWrappingState(ref m_SavedWordWrapState, i, m_characterCount);

                        m_lineNumber += 1;
                        isStartOfNewLine = true;

                        // Check to make sure Array is large enough to hold a new line.
                        if (m_lineNumber >= m_textInfo.lineInfo.Length)
                            ResizeLineExtents(m_lineNumber);

                        // Apply Line Spacing
                        float scale = (m_style & FontStyles.Subscript) == FontStyles.Subscript || (m_style & FontStyles.Superscript) == FontStyles.Superscript ? m_maxFontScale : m_fontScale;
                        lineOffsetDelta = 0 - face.Descender * m_maxFontScale + (face.Ascender + gap + m_lineSpacing + m_paragraphSpacing + m_lineSpacingDelta) * scale;
                        m_lineOffset += lineOffsetDelta;


                        previousLineMaxScale = m_maxFontScale;
                        firstVisibleCharacterScale = scale;
                        m_maxFontScale = 0;
                        spriteScale = 1;
                        m_xAdvance = 0 + tag_LineIndent + tag_Indent;

                        ellipsisIndex = m_characterCount - 1;
                    }
                }
                #endregion Check for Linefeed or Last Character


                // Store Rectangle positions for each Character and Mesh Extents.
                #region Save CharacterInfo for the current character.
                m_textInfo.characterInfo[m_characterCount].topLine = m_textInfo.characterInfo[m_characterCount].baseLine + m_currentFontAsset.fontInfo.Ascender * m_fontScale; // Ascender
                m_textInfo.characterInfo[m_characterCount].bottomLine = m_textInfo.characterInfo[m_characterCount].baseLine + m_currentFontAsset.fontInfo.Descender * m_fontScale; // Descender
                m_textInfo.characterInfo[m_characterCount].padding = padding * m_fontScale;
                m_textInfo.characterInfo[m_characterCount].aspectRatio = m_cached_GlyphInfo.width / m_cached_GlyphInfo.height;
                //m_textInfo.characterInfo[m_characterCount].scale = m_fontScale;


                // Determine the bounds of the Mesh.
                if (m_textInfo.characterInfo[m_characterCount].isVisible)
                {
                    m_meshExtents.min = new Vector2(Mathf.Min(m_meshExtents.min.x, m_textInfo.characterInfo[m_characterCount].vertex_BL.position.x), Mathf.Min(m_meshExtents.min.y, m_textInfo.characterInfo[m_characterCount].vertex_BL.position.y));
                    m_meshExtents.max = new Vector2(Mathf.Max(m_meshExtents.max.x, m_textInfo.characterInfo[m_characterCount].vertex_TR.position.x), Mathf.Max(m_meshExtents.max.y, m_textInfo.characterInfo[m_characterCount].vertex_TL.position.y));
                }


                // Save pageInfo Data
                if (charCode != 13 && charCode != 10 && m_pageNumber < 16)
                {
                    m_textInfo.pageInfo[m_pageNumber].ascender = pageAscender;
                    m_textInfo.pageInfo[m_pageNumber].descender = descender < m_textInfo.pageInfo[m_pageNumber].descender ? descender : m_textInfo.pageInfo[m_pageNumber].descender;
                    //Debug.Log("Char [" + (char)charCode + "] with ASCII (" + charCode + ") on Page # " + m_pageNumber + " with Ascender: " + m_textInfo.pageInfo[m_pageNumber].ascender + ". Descender: " + m_textInfo.pageInfo[m_pageNumber].descender);

                    if (m_pageNumber == 0 && m_characterCount == 0)
                        m_textInfo.pageInfo[m_pageNumber].firstCharacterIndex = m_characterCount;
                    else if (m_characterCount > 0 && m_pageNumber != m_textInfo.characterInfo[m_characterCount - 1].pageNumber)
                    {
                        m_textInfo.pageInfo[m_pageNumber - 1].lastCharacterIndex = m_characterCount - 1;
                        m_textInfo.pageInfo[m_pageNumber].firstCharacterIndex = m_characterCount;
                    }
                    else if (m_characterCount == totalCharacterCount - 1)
                        m_textInfo.pageInfo[m_pageNumber].lastCharacterIndex = m_characterCount;
                }
                #endregion Saving CharacterInfo


                // Save State of Mesh Creation for handling of Word Wrapping
                #region Save Word Wrapping State
                if (m_enableWordWrapping || m_overflowMode == TextOverflowModes.Truncate || m_overflowMode == TextOverflowModes.Ellipsis)
                {
                    if ((charCode == 9 || charCode == 32) && !m_isNonBreakingSpace)
                    {
                        // We store the state of numerous variables for the most recent Space, LineFeed or Carriage Return to enable them to be restored 
                        // for Word Wrapping.
                        SaveWordWrappingState(ref m_SavedWordWrapState, i, m_characterCount);
                        m_isCharacterWrappingEnabled = false;
                        isFirstWord = false;
                    }
                    else if (charCode > 0x2e80 && charCode < 0x9fff || m_fontAsset.lineBreakingInfo.leadingCharacters.ContainsKey(charCode) || m_fontAsset.lineBreakingInfo.followingCharacters.ContainsKey(charCode))
                    {
                        if (m_characterCount < totalCharacterCount - 1
                                && m_fontAsset.lineBreakingInfo.leadingCharacters.ContainsKey(charCode) == false
                                && m_fontAsset.lineBreakingInfo.followingCharacters.ContainsKey(m_VisibleCharacters[m_characterCount + 1]) == false)
                        {
                            SaveWordWrappingState(ref m_SavedWordWrapState, i, m_characterCount);
                            m_isCharacterWrappingEnabled = false;
                            isFirstWord = false;
                        }
                    }
                    else if ((isFirstWord || m_isCharacterWrappingEnabled == true || isLastBreakingChar))
                        SaveWordWrappingState(ref m_SavedWordWrapState, i, m_characterCount);
                }
                #endregion Save Word Wrapping State

                m_characterCount += 1;
            }


            // Check Auto Sizing and increase font size to fill text container.
            #region Check Auto-Sizing (Upper Font Size Bounds)
            fontSizeDelta = m_maxFontSize - m_minFontSize;
            if (!m_isCharacterWrappingEnabled && m_enableAutoSizing && fontSizeDelta > 0.051f && m_fontSize < m_fontSizeMax)
            {
                m_minFontSize = m_fontSize;
                m_fontSize += Mathf.Max((m_maxFontSize - m_fontSize) / 2, 0.05f);
                m_fontSize = (int)(Mathf.Min(m_fontSize, m_fontSizeMax) * 20 + 0.5f) / 20f;

                if (loopCountA > 20) return; // Added to debug
                GenerateTextMesh();
                return;
            }
            #endregion End Auto-sizing Check


            m_isCharacterWrappingEnabled = false;

            // Adjust Preferred Height to account for Margins.
            m_renderedHeight += m_margin.y > 0 ? m_margin.y : 0;
            
            if (m_renderMode == TextRenderFlags.GetPreferredSizes)
                return;

            if (!IsRectTransformDriven) { m_preferredWidth = m_renderedWidth; m_preferredHeight = m_renderedHeight; }

            // DEBUG & PERFORMANCE CHECKS (0.006ms)
            //Debug.Log("Iteration Count: " + loopCountA + ". Final Point Size: " + m_fontSize); // + "  B: " + loopCountB + "  C: " + loopCountC + "  D: " + loopCountD);

            // If there are no visible characters... no need to continue
            if (m_visibleCharacterCount == 0 && m_visibleSpriteCount == 0)
            {
                m_uiRenderer.SetMesh(null);

                //Vector3[] vertices = m_textInfo.meshInfo.vertices;

                //if (vertices != null)
                //{
                //    Array.Clear(vertices, 0, vertices.Length);
                //    m_mesh.vertices = vertices;
                //}
                return;
            }


            int last_vert_index = m_visibleCharacterCount * 4;
            // Partial clear of the vertices array to mark unused vertices as degenerate.
            Array.Clear(m_textInfo.meshInfo.vertices, last_vert_index, m_textInfo.meshInfo.vertices.Length - last_vert_index);
            // Do we want to clear the sprite array?

            // Handle Text Alignment
            #region Text Alignment
            switch (m_textAlignment)
            {
                // Top Vertically
                case TextAlignmentOptions.Top:
                case TextAlignmentOptions.TopLeft:
                case TextAlignmentOptions.TopJustified:
                case TextAlignmentOptions.TopRight:
                    if (m_overflowMode != TextOverflowModes.Page)
                        m_anchorOffset = m_rectCorners[1] + new Vector3(0 + margins.x, 0 - m_maxAscender - margins.y, 0);
                    else
                        m_anchorOffset = m_rectCorners[1] + new Vector3(0 + margins.x, 0 - m_textInfo.pageInfo[pageToDisplay].ascender - margins.y, 0);
                    break;

                // Middle Vertically
                case TextAlignmentOptions.Left:
                case TextAlignmentOptions.Right:
                case TextAlignmentOptions.Center:
                case TextAlignmentOptions.Justified:
                    if (m_overflowMode != TextOverflowModes.Page)
                        m_anchorOffset = (m_rectCorners[0] + m_rectCorners[1]) / 2 + new Vector3(0 + margins.x, 0 - (m_maxAscender + margins.y + maxVisibleDescender - margins.w) / 2, 0);
                    else
                        m_anchorOffset = (m_rectCorners[0] + m_rectCorners[1]) / 2 + new Vector3(0 + margins.x, 0 - (m_textInfo.pageInfo[pageToDisplay].ascender + margins.y + m_textInfo.pageInfo[pageToDisplay].descender - margins.w) / 2, 0);
                    break;

                // Bottom Vertically
                case TextAlignmentOptions.Bottom:
                case TextAlignmentOptions.BottomLeft:
                case TextAlignmentOptions.BottomRight:
                case TextAlignmentOptions.BottomJustified:
                    if (m_overflowMode != TextOverflowModes.Page)
                        m_anchorOffset = m_rectCorners[0] + new Vector3(0 + margins.x, 0 - maxVisibleDescender + margins.w, 0);
                    else
                        m_anchorOffset = m_rectCorners[0] + new Vector3(0 + margins.x, 0 - m_textInfo.pageInfo[pageToDisplay].descender + margins.w, 0);
                    break;
                    
                // Baseline Vertically
                case TextAlignmentOptions.Baseline:
                case TextAlignmentOptions.BaselineLeft:
                case TextAlignmentOptions.BaselineRight:
                case TextAlignmentOptions.BaselineJustified:
                    m_anchorOffset = (m_rectCorners[0] + m_rectCorners[1]) / 2 + new Vector3(0 + margins.x, 0, 0);
                    break;

                // Midline Vertically 
                case TextAlignmentOptions.MidlineLeft:
                case TextAlignmentOptions.Midline:
                case TextAlignmentOptions.MidlineRight:
                case TextAlignmentOptions.MidlineJustified:
                    m_anchorOffset = (m_rectCorners[0] + m_rectCorners[1]) / 2 + new Vector3(0 + margins.x, 0 - (m_meshExtents.max.y + margins.y + m_meshExtents.min.y - margins.w) / 2, 0);
                    break;
            }
            #endregion Text Alignment


            // Initialization for Second Pass
            Vector3 justificationOffset = Vector3.zero;
            Vector3 offset = Vector3.zero;
            int vert_index_X4 = 0;
            int sprite_index_X4 = 0;
            //Array.Clear(m_meshAllocCount, 0, 17);

            int wordCount = 0;
            int lineCount = 0;
            int lastLine = 0;

            bool isStartOfWord = false;
            int wordFirstChar = 0;
            int wordLastChar = 0;

            // Second Pass : Line Justification, UV Mapping, Character & Line Visibility & more.
            #region Handle Line Justification & UV Mapping & Character Visibility & More
            
            // Variables used to handle Canvas Render Modes and SDF Scaling
            bool isCameraAssigned = m_canvas.worldCamera == null ? false : true;
            float lossyScale = m_rectTransform.lossyScale.z;
            RenderMode canvasRenderMode = m_canvas.renderMode;
            float canvasScaleFactor = m_canvas.scaleFactor;

            int underlineSegmentCount = 0;
            Color32 underlineColor = Color.white;
            Color32 strikethroughColor = Color.white;
            float underlineStartScale = 0;
            float underlineEndScale = 0;
            float underlineMaxScale = 0;
            float underlineBaseLine = Mathf.Infinity;
            int lastPage = 0;

            float strikethroughPointSize = 0;
            float strikethroughScale = 0;
            float strikethroughBaseline = 0;

            TMP_CharacterInfo[] characterInfos = m_textInfo.characterInfo;
            for (int i = 0; i < m_characterCount; i++)
            {
                int currentLine = characterInfos[i].lineNumber;
                char currentCharacter = characterInfos[i].character;
                TMP_LineInfo lineInfo = m_textInfo.lineInfo[currentLine];
                
                TextAlignmentOptions lineAlignment = lineInfo.alignment;
                lineCount = currentLine + 1;

                // Process Line Justification
                #region Handle Line Justification
                //if (!characterInfos[i].isIgnoringAlignment)
                //{
                switch (lineAlignment)
                {
                    case TextAlignmentOptions.TopLeft:
                    case TextAlignmentOptions.Left:
                    case TextAlignmentOptions.BottomLeft:
                    case TextAlignmentOptions.BaselineLeft:
                    case TextAlignmentOptions.MidlineLeft:
                        justificationOffset = new Vector3 (0 + lineInfo.marginLeft, 0, 0);
                        break;

                    case TextAlignmentOptions.Top:
                    case TextAlignmentOptions.Center:
                    case TextAlignmentOptions.Bottom:
                    case TextAlignmentOptions.Baseline:
                    case TextAlignmentOptions.Midline:
                        justificationOffset = new Vector3(lineInfo.marginLeft + lineInfo.width / 2 - lineInfo.maxAdvance / 2, 0, 0);
                        break;

                    case TextAlignmentOptions.TopRight:
                    case TextAlignmentOptions.Right:
                    case TextAlignmentOptions.BottomRight:
                    case TextAlignmentOptions.BaselineRight:
                    case TextAlignmentOptions.MidlineRight:
                        justificationOffset = new Vector3(lineInfo.marginLeft + lineInfo.width - lineInfo.maxAdvance, 0, 0);
                        break;

                    case TextAlignmentOptions.TopJustified:
                    case TextAlignmentOptions.Justified:
                    case TextAlignmentOptions.BottomJustified:
                    case TextAlignmentOptions.BaselineJustified:
                    case TextAlignmentOptions.MidlineJustified:
                        charCode = m_textInfo.characterInfo[i].character;
                        char lastCharOfCurrentLine = m_textInfo.characterInfo[lineInfo.lastCharacterIndex].character;

                        if (/*char.IsWhiteSpace(lastCharOfCurrentLine) &&*/ !char.IsControl(lastCharOfCurrentLine) && currentLine < m_lineNumber)
                        {
                            // All lines are justified accept the last one.
                            float gap = lineInfo.width - lineInfo.maxAdvance;

                            if (currentLine != lastLine || i == 0)
                                justificationOffset = new Vector3(lineInfo.marginLeft, 0, 0);
                            else
                            {
                                if (charCode == 9 || charCode == 32 || charCode == 160)
                                {
                                    int spaces = lineInfo.spaceCount - 1 > 0 ? lineInfo.spaceCount - 1 : 1;
                                    justificationOffset += new Vector3(gap * (1 - m_wordWrappingRatios) / (spaces), 0, 0);
                                }
                                else
                                {
                                    justificationOffset += new Vector3(gap * m_wordWrappingRatios / (lineInfo.characterCount - lineInfo.spaceCount - 1), 0, 0);
                                }
                            }
                        }
                        else
                            justificationOffset = new Vector3(lineInfo.marginLeft, 0, 0); // Keep last line left justified.

                        //Debug.Log("Char [" + (char)charCode + "] Code:" + charCode + "  Line # " + currentLine + "  Offset:" + justificationOffset + "  # Spaces:" + lineInfo.spaceCount + "  # Characters:" + lineInfo.characterCount);
                        break;
                }
                //}
                #endregion End Text Justification

                offset = m_anchorOffset + justificationOffset;

                // Handle Visible Characters
                #region Handle Visible Characters
                if (characterInfos[i].isVisible)
                {
                    TMP_CharacterType type = characterInfos[i].type;
                    switch (type)
                    {
                        // CHARACTERS
                        case TMP_CharacterType.Character:

                            Extents lineExtents = lineInfo.lineExtents;
                            float uvOffset = (m_uvLineOffset * currentLine) % 1 + m_uvOffset.x;

                            // Setup UV2 based on Character Mapping Options Selected
                            #region Handle UV Mapping Options
                            switch (m_horizontalMapping)
                            {
                                case TextureMappingOptions.Character:
                                    characterInfos[i].vertex_BL.uv2.x = 0 + m_uvOffset.x;
                                    characterInfos[i].vertex_TL.uv2.x = 0 + m_uvOffset.x;
                                    characterInfos[i].vertex_TR.uv2.x = 1 + m_uvOffset.x;
                                    characterInfos[i].vertex_BR.uv2.x = 1 + m_uvOffset.x;
                                    break;

                                case TextureMappingOptions.Line:
                                    if (m_textAlignment != TextAlignmentOptions.Justified)
                                    {
                                        characterInfos[i].vertex_BL.uv2.x = (characterInfos[i].vertex_BL.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + uvOffset;
                                        characterInfos[i].vertex_TL.uv2.x = (characterInfos[i].vertex_TL.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + uvOffset;
                                        characterInfos[i].vertex_TR.uv2.x = (characterInfos[i].vertex_TR.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + uvOffset;
                                        characterInfos[i].vertex_BR.uv2.x = (characterInfos[i].vertex_BR.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + uvOffset;
                                        break;
                                    }
                                    else // Special Case if Justified is used in Line Mode.
                                    {
                                        characterInfos[i].vertex_BL.uv2.x = (characterInfos[i].vertex_BL.position.x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                                        characterInfos[i].vertex_TL.uv2.x = (characterInfos[i].vertex_TL.position.x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                                        characterInfos[i].vertex_TR.uv2.x = (characterInfos[i].vertex_TR.position.x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                                        characterInfos[i].vertex_BR.uv2.x = (characterInfos[i].vertex_BR.position.x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                                        break;
                                    }

                                case TextureMappingOptions.Paragraph:
                                    characterInfos[i].vertex_BL.uv2.x = (characterInfos[i].vertex_BL.position.x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                                    characterInfos[i].vertex_TL.uv2.x = (characterInfos[i].vertex_TL.position.x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                                    characterInfos[i].vertex_TR.uv2.x = (characterInfos[i].vertex_TR.position.x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                                    characterInfos[i].vertex_BR.uv2.x = (characterInfos[i].vertex_BR.position.x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                                    break;

                                case TextureMappingOptions.MatchAspect:

                                    switch (m_verticalMapping)
                                    {
                                        case TextureMappingOptions.Character:
                                            characterInfos[i].vertex_BL.uv2.y = 0 + m_uvOffset.y;
                                            characterInfos[i].vertex_TL.uv2.y = 1 + m_uvOffset.y;
                                            characterInfos[i].vertex_TR.uv2.y = 0 + m_uvOffset.y;
                                            characterInfos[i].vertex_BR.uv2.y = 1 + m_uvOffset.y;
                                            break;

                                        case TextureMappingOptions.Line:
                                            characterInfos[i].vertex_BL.uv2.y = (characterInfos[i].vertex_BL.position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + uvOffset;
                                            characterInfos[i].vertex_TL.uv2.y = (characterInfos[i].vertex_TL.position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + uvOffset;
                                            characterInfos[i].vertex_TR.uv2.y = characterInfos[i].vertex_BL.uv2.y;
                                            characterInfos[i].vertex_BR.uv2.y = characterInfos[i].vertex_TL.uv2.y;
                                            break;

                                        case TextureMappingOptions.Paragraph:
                                            characterInfos[i].vertex_BL.uv2.y = (characterInfos[i].vertex_BL.position.y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + uvOffset;
                                            characterInfos[i].vertex_TL.uv2.y = (characterInfos[i].vertex_TL.position.y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + uvOffset;
                                            characterInfos[i].vertex_TR.uv2.y = characterInfos[i].vertex_BL.uv2.y;
                                            characterInfos[i].vertex_BR.uv2.y = characterInfos[i].vertex_TL.uv2.y;
                                            break;

                                        case TextureMappingOptions.MatchAspect:
                                            Debug.Log("ERROR: Cannot Match both Vertical & Horizontal.");
                                            break;
                                    }

                                    //float xDelta = 1 - (_uv2s[vert_index + 0].y * textMeshCharacterInfo[i].AspectRatio); // Left aligned
                                    float xDelta = (1 - ((characterInfos[i].vertex_BL.uv2.y +  characterInfos[i].vertex_TL.uv2.y) * characterInfos[i].aspectRatio)) / 2; // Center of Rectangle

                                    characterInfos[i].vertex_BL.uv2.x = (characterInfos[i].vertex_BL.uv2.y * characterInfos[i].aspectRatio) + xDelta + uvOffset;
                                    characterInfos[i].vertex_TL.uv2.x = characterInfos[i].vertex_BL.uv2.x;
                                    characterInfos[i].vertex_TR.uv2.x = (characterInfos[i].vertex_TL.uv2.y * characterInfos[i].aspectRatio) + xDelta + uvOffset;
                                    characterInfos[i].vertex_BR.uv2.x = characterInfos[i].vertex_TR.uv2.x;
                                    break;
                            }

                            switch (m_verticalMapping)
                            {
                                case TextureMappingOptions.Character:
                                    characterInfos[i].vertex_BL.uv2.y = 0 + m_uvOffset.y;
                                    characterInfos[i].vertex_TL.uv2.y = 1 + m_uvOffset.y;
                                    characterInfos[i].vertex_TR.uv2.y = 1 + m_uvOffset.y;
                                    characterInfos[i].vertex_BR.uv2.y = 0 + m_uvOffset.y;
                                    break;

                                case TextureMappingOptions.Line:
                                    characterInfos[i].vertex_BL.uv2.y = (characterInfos[i].vertex_BL.position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + m_uvOffset.y;
                                    characterInfos[i].vertex_TL.uv2.y = (characterInfos[i].vertex_TL.position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + m_uvOffset.y;
                                    characterInfos[i].vertex_BR.uv2.y = characterInfos[i].vertex_BL.uv2.y;
                                    characterInfos[i].vertex_TR.uv2.y = characterInfos[i].vertex_TL.uv2.y;
                                    break;

                                case TextureMappingOptions.Paragraph:
                                    characterInfos[i].vertex_BL.uv2.y = (characterInfos[i].vertex_BL.position.y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + m_uvOffset.y;
                                    characterInfos[i].vertex_TL.uv2.y = (characterInfos[i].vertex_TL.position.y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + m_uvOffset.y;
                                    characterInfos[i].vertex_BR.uv2.y = characterInfos[i].vertex_BL.uv2.y;
                                    characterInfos[i].vertex_TR.uv2.y = characterInfos[i].vertex_TL.uv2.y;
                                    break;

                                case TextureMappingOptions.MatchAspect:
                                    //float yDelta = 1 - (_uv2s[vert_index + 2].x / textMeshCharacterInfo[i].AspectRatio); // Top Corner
                                    float yDelta = (1 - ((characterInfos[i].vertex_BL.uv2.x + characterInfos[i].vertex_TR.uv2.x) / characterInfos[i].aspectRatio)) / 2; // Center of Rectangle
                                    //float yDelta = 0;

                                    characterInfos[i].vertex_BL.uv2.y = yDelta + (characterInfos[i].vertex_BL.uv2.x / characterInfos[i].aspectRatio) + m_uvOffset.y;
                                    characterInfos[i].vertex_TL.uv2.y = yDelta + (characterInfos[i].vertex_TR.uv2.x / characterInfos[i].aspectRatio) + m_uvOffset.y;
                                    characterInfos[i].vertex_BR.uv2.y = characterInfos[i].vertex_BL.uv2.y;
                                    characterInfos[i].vertex_TR.uv2.y = characterInfos[i].vertex_TL.uv2.y;
                                    break;
                            }
                            #endregion End UV Mapping Options


                            // Pack UV's so that we can pass Xscale needed for Shader to maintain 1:1 ratio.
                            #region Pack Scale into UV2
                            float xScale = characterInfos[i].scale * (1 - m_charWidthAdjDelta);
                            if ((characterInfos[i].style & FontStyles.Bold) == FontStyles.Bold) xScale *= -1;
                        
                            switch (canvasRenderMode)
                            {
                                case RenderMode.ScreenSpaceOverlay:
                                    xScale *= lossyScale / canvasScaleFactor;
                                    break;
                                case RenderMode.ScreenSpaceCamera:
                                    xScale *= isCameraAssigned ? lossyScale : 1;
                                    break;
                                case RenderMode.WorldSpace:
                                    xScale *= lossyScale;
                                    break;
                            }

                            float x0 = characterInfos[i].vertex_BL.uv2.x;
                            float y0 = characterInfos[i].vertex_BL.uv2.y;
                            float x1 = characterInfos[i].vertex_TR.uv2.x;
                            float y1 = characterInfos[i].vertex_TR.uv2.y; 

                            float dx = Mathf.Floor(x0);
                            float dy = Mathf.Floor(y0);

                            x0 = x0 - dx;
                            x1 = x1 - dx;
                            y0 = y0 - dy;
                            y1 = y1 - dy;

                            characterInfos[i].vertex_BL.uv2 = PackUV(x0, y0, xScale);
                            characterInfos[i].vertex_TL.uv2 = PackUV(x0, y1, xScale);
                            characterInfos[i].vertex_TR.uv2 = PackUV(x1, y1, xScale);
                            characterInfos[i].vertex_BR.uv2 = PackUV(x1, y0, xScale);
                            #endregion
                            
                            break;
                        
                        // SPRITES
                        case TMP_CharacterType.Sprite:
                            // Nothing right now
                            break;
                    }


                    // Handle maxVisibleCharacters, maxVisibleLines and Overflow Page Mode.
                    #region Handle maxVisibleCharacters / maxVisibleLines / Page Mode
                    if (i < m_maxVisibleCharacters && currentLine < m_maxVisibleLines && m_overflowMode != TextOverflowModes.Page)
                    {
                        characterInfos[i].vertex_BL.position += offset;
                        characterInfos[i].vertex_TL.position += offset;
                        characterInfos[i].vertex_TR.position += offset;
                        characterInfos[i].vertex_BR.position += offset;
                    }
                    else if (i < m_maxVisibleCharacters && currentLine < m_maxVisibleLines && m_overflowMode == TextOverflowModes.Page && characterInfos[i].pageNumber == pageToDisplay)
                    {
                        characterInfos[i].vertex_BL.position += offset;
                        characterInfos[i].vertex_TL.position += offset;
                        characterInfos[i].vertex_TR.position += offset;
                        characterInfos[i].vertex_BR.position += offset;
                    }
                    else
                    {
                        characterInfos[i].vertex_BL.position *= 0;
                        characterInfos[i].vertex_TL.position *= 0;
                        characterInfos[i].vertex_TR.position *= 0;
                        characterInfos[i].vertex_BR.position *= 0;
                    }
                    #endregion


                    // Fill Vertex Buffers for the various types of element
                    if (type == TMP_CharacterType.Character)
                    {
                        FillCharacterVertexBuffers(i, vert_index_X4);
                        vert_index_X4 += 4;
                    }
                    else if (type == TMP_CharacterType.Sprite)
                    {
                        FillSpriteVertexBuffers(i, sprite_index_X4);
                        sprite_index_X4 += 4;
                    }

                }
                #endregion


                // Apply Alignment and Justification Offset
                m_textInfo.characterInfo[i].bottomLeft += offset;
                m_textInfo.characterInfo[i].topLeft += offset;
                m_textInfo.characterInfo[i].topRight += offset;
                m_textInfo.characterInfo[i].bottomRight += offset;

                // Need to add top left and bottom right.
                m_textInfo.characterInfo[i].topLine += offset.y;
                m_textInfo.characterInfo[i].bottomLine += offset.y;
                m_textInfo.characterInfo[i].baseLine += offset.y;


                // Store Max Ascender & Descender
                if (currentCharacter != 10 && currentCharacter != 13)
                {
                    m_textInfo.lineInfo[currentLine].ascender = m_textInfo.characterInfo[i].topLine > m_textInfo.lineInfo[currentLine].ascender ? m_textInfo.characterInfo[i].topLine : m_textInfo.lineInfo[currentLine].ascender;
                    m_textInfo.lineInfo[currentLine].descender = m_textInfo.characterInfo[i].bottomLine < m_textInfo.lineInfo[currentLine].descender ? m_textInfo.characterInfo[i].bottomLine : m_textInfo.lineInfo[currentLine].descender;
                }


                // Need to recompute lineExtent to account for the offset from justification.
                #region Adjust lineExtents resulting from alignment offset
                if (currentLine != lastLine || i == m_characterCount - 1)
                {
                    // Update the previous line's extents
                    if (currentLine != lastLine)
                    {
                        m_textInfo.lineInfo[lastLine].lineExtents.min = new Vector2(m_textInfo.characterInfo[m_textInfo.lineInfo[lastLine].firstCharacterIndex].bottomLeft.x, m_textInfo.lineInfo[lastLine].descender);
                        m_textInfo.lineInfo[lastLine].lineExtents.max = new Vector2(m_textInfo.characterInfo[m_textInfo.lineInfo[lastLine].lastVisibleCharacterIndex].topRight.x, m_textInfo.lineInfo[lastLine].ascender);
                    }

                    // Update the current line's extents
                    if (i == m_characterCount - 1)
                    {
                        m_textInfo.lineInfo[currentLine].lineExtents.min = new Vector2(m_textInfo.characterInfo[m_textInfo.lineInfo[currentLine].firstCharacterIndex].bottomLeft.x, m_textInfo.lineInfo[currentLine].descender);
                        m_textInfo.lineInfo[currentLine].lineExtents.max = new Vector2(m_textInfo.characterInfo[m_textInfo.lineInfo[currentLine].lastVisibleCharacterIndex].topRight.x, m_textInfo.lineInfo[currentLine].ascender);
                    }
                }
                #endregion


                // Track Word Count per line and for the object
                #region Track Word Count
                if (char.IsLetterOrDigit(currentCharacter) || currentCharacter == 39 || currentCharacter == 8217)
                {
                    if (isStartOfWord == false)
                    {
                        isStartOfWord = true;
                        wordFirstChar = i;
                    }

                    // If last character is a word
                    if (isStartOfWord && i == m_characterCount - 1)
                    {
                        wordLastChar = i;
                        wordCount += 1;
                        m_textInfo.lineInfo[currentLine].wordCount += 1;

                        TMP_WordInfo wordInfo = new TMP_WordInfo();
                        wordInfo.firstCharacterIndex = wordFirstChar;
                        wordInfo.lastCharacterIndex = wordLastChar;
                        wordInfo.characterCount = wordLastChar - wordFirstChar + 1;
                        m_textInfo.wordInfo.Add(wordInfo);
                    }
                }
                else if (isStartOfWord || i == 0 && (char.IsPunctuation(currentCharacter) || char.IsWhiteSpace(currentCharacter) || i == m_characterCount - 1))
                {
                    wordLastChar = i == m_characterCount - 1 && char.IsLetterOrDigit(currentCharacter) ? i : i - 1;
                    isStartOfWord = false;

                    wordCount += 1;
                    m_textInfo.lineInfo[currentLine].wordCount += 1;

                    TMP_WordInfo wordInfo = new TMP_WordInfo();
                    wordInfo.firstCharacterIndex = wordFirstChar;
                    wordInfo.lastCharacterIndex = wordLastChar;
                    wordInfo.characterCount = wordLastChar - wordFirstChar + 1;
                    m_textInfo.wordInfo.Add(wordInfo);
                }
                #endregion


                // Setup & Handle Underline
                #region Underline
                // NOTE: Need to figure out how underline will be handled with multiple fonts and which font will be used for the underline.
                bool isUnderline = (m_textInfo.characterInfo[i].style & FontStyles.Underline) == FontStyles.Underline;
                if (isUnderline)
                {
                    bool isUnderlineVisible = true;
                    int currentPage = m_textInfo.characterInfo[i].pageNumber;

                    if (i > m_maxVisibleCharacters || currentLine > m_maxVisibleLines || (m_overflowMode == TextOverflowModes.Page && currentPage + 1 != m_pageToDisplay))
                        isUnderlineVisible = false;

                    // We only use the scale of visible characters.
                    if (currentCharacter != 10 && currentCharacter != 13 && currentCharacter != 32 && currentCharacter != 160)
                    {
                        underlineMaxScale = Mathf.Max(underlineMaxScale, m_textInfo.characterInfo[i].scale);
                        underlineBaseLine = Mathf.Min(currentPage == lastPage ? underlineBaseLine : Mathf.Infinity, m_textInfo.characterInfo[i].baseLine + font.fontInfo.Underline * underlineMaxScale);
                        lastPage = currentPage; // Need to track pages to ensure we reset baseline for the new pages.
                    }

                    if (beginUnderline == false && isUnderlineVisible == true && i <= lineInfo.lastVisibleCharacterIndex && currentCharacter != 10 && currentCharacter != 13)
                    {
                        if (i == lineInfo.lastVisibleCharacterIndex && (currentCharacter == 32 || currentCharacter == 160))
                        { }
                        else
                        {
                            beginUnderline = true;
                            underlineStartScale = m_textInfo.characterInfo[i].scale;
                            if (underlineMaxScale == 0) underlineMaxScale = underlineStartScale;
                            underline_start = new Vector3(m_textInfo.characterInfo[i].bottomLeft.x, underlineBaseLine, 0);
                            underlineColor = m_textInfo.characterInfo[i].color;
                        }
                    }

                    // End Underline if text only contains one character.
                    if (beginUnderline && m_characterCount == 1)
                    {
                        beginUnderline = false;
                        underline_end = new Vector3(m_textInfo.characterInfo[i].topRight.x, underlineBaseLine, 0);
                        underlineEndScale = m_textInfo.characterInfo[i].scale;

                        DrawUnderlineMesh(underline_start, underline_end, ref last_vert_index, underlineStartScale, underlineEndScale, underlineMaxScale, underlineColor);
                        underlineSegmentCount += 1;
                        underlineMaxScale = 0;
                        underlineBaseLine = Mathf.Infinity;
                    }
                    else if (beginUnderline && (i == lineInfo.lastCharacterIndex || i >= lineInfo.lastVisibleCharacterIndex))
                    {
                        // Terminate underline at previous visible character if space or carriage return.
                        if (currentCharacter == 10 || currentCharacter == 13 || currentCharacter == 32 || currentCharacter == 160)
                        {
                            int lastVisibleCharacterIndex = lineInfo.lastVisibleCharacterIndex;
                            underline_end = new Vector3(m_textInfo.characterInfo[lastVisibleCharacterIndex].topRight.x, underlineBaseLine, 0);
                            underlineEndScale = m_textInfo.characterInfo[lastVisibleCharacterIndex].scale;
                        }
                        else
                        {   // End underline if last character of the line.
                            underline_end = new Vector3(m_textInfo.characterInfo[i].topRight.x, underlineBaseLine, 0);
                            underlineEndScale = m_textInfo.characterInfo[i].scale;
                        }

                        beginUnderline = false;
                        DrawUnderlineMesh(underline_start, underline_end, ref last_vert_index, underlineStartScale, underlineEndScale, underlineMaxScale, underlineColor);
                        underlineSegmentCount += 1;
                        underlineMaxScale = 0;
                        underlineBaseLine = Mathf.Infinity;
                    }
                    else if (beginUnderline && !isUnderlineVisible)
                    {
                        beginUnderline = false;
                        underline_end = new Vector3(m_textInfo.characterInfo[i - 1].topRight.x, underlineBaseLine, 0);
                        underlineEndScale = m_textInfo.characterInfo[i - 1].scale;

                        DrawUnderlineMesh(underline_start, underline_end, ref last_vert_index, underlineStartScale, underlineEndScale, underlineMaxScale, underlineColor);
                        underlineSegmentCount += 1;
                        underlineMaxScale = 0;
                        underlineBaseLine = Mathf.Infinity;
                    }
                }
                else
                {
                    // End Underline
                    if (beginUnderline == true)
                    {
                        beginUnderline = false;
                        underline_end = new Vector3(m_textInfo.characterInfo[i - 1].topRight.x, underlineBaseLine, 0);
                        underlineEndScale = m_textInfo.characterInfo[i - 1].scale;

                        DrawUnderlineMesh(underline_start, underline_end, ref last_vert_index, underlineStartScale, underlineEndScale, underlineMaxScale, underlineColor);
                        underlineSegmentCount += 1;
                        underlineMaxScale = 0;
                        underlineBaseLine = Mathf.Infinity;
                    }
                }
                #endregion


                // Setup & Handle Strikethrough
                #region Strikethrough
                // NOTE: Need to figure out how underline will be handled with multiple fonts and which font will be used for the underline.
                bool isStrikethrough = (m_textInfo.characterInfo[i].style & FontStyles.Strikethrough) == FontStyles.Strikethrough;
                if (isStrikethrough)
                {
                    bool isStrikeThroughVisible = true;

                    if (i > m_maxVisibleCharacters || currentLine > m_maxVisibleLines || (m_overflowMode == TextOverflowModes.Page && m_textInfo.characterInfo[i].pageNumber + 1 != m_pageToDisplay))
                        isStrikeThroughVisible = false;

                    if (beginStrikethrough == false && isStrikeThroughVisible && i <= lineInfo.lastVisibleCharacterIndex && currentCharacter != 10 && currentCharacter != 13)
                    {
                        if (i == lineInfo.lastVisibleCharacterIndex && (currentCharacter == 32 || currentCharacter == 160))
                        { }
                        else
                        {
                            beginStrikethrough = true;
                            strikethroughPointSize = m_textInfo.characterInfo[i].pointSize;
                            strikethroughScale = m_textInfo.characterInfo[i].scale;
                            strikethrough_start = new Vector3(m_textInfo.characterInfo[i].bottomLeft.x, m_textInfo.characterInfo[i].baseLine + (font.fontInfo.Ascender + font.fontInfo.Descender) / 2.75f * strikethroughScale, 0);
                            strikethroughColor = m_textInfo.characterInfo[i].color;
                            strikethroughBaseline = m_textInfo.characterInfo[i].baseLine;
                            //Debug.Log("Char [" + currentCharacter + "] Start Strikethrough POS: " + strikethrough_start);
                        }
                    }

                    // End Strikethrough if text only contains one character.
                    if (beginStrikethrough && m_characterCount == 1)
                    {
                        beginStrikethrough = false;
                        strikethrough_end = new Vector3(m_textInfo.characterInfo[i].topRight.x, m_textInfo.characterInfo[i].baseLine + (font.fontInfo.Ascender + font.fontInfo.Descender) / 2 * strikethroughScale, 0);

                        DrawUnderlineMesh(strikethrough_start, strikethrough_end, ref last_vert_index, strikethroughScale, strikethroughScale, strikethroughScale, strikethroughColor);
                        underlineSegmentCount += 1;
                    }
                    else if (beginStrikethrough && i == lineInfo.lastCharacterIndex)
                    {
                        // Terminate Strikethrough at previous visible character if space or carriage return.
                        if (currentCharacter == 10 || currentCharacter == 13 || currentCharacter == 32 || currentCharacter == 160)
                        {
                            int lastVisibleCharacterIndex = lineInfo.lastVisibleCharacterIndex;
                            strikethrough_end = new Vector3(m_textInfo.characterInfo[lastVisibleCharacterIndex].topRight.x, m_textInfo.characterInfo[lastVisibleCharacterIndex].baseLine + (font.fontInfo.Ascender + font.fontInfo.Descender) / 2 * strikethroughScale, 0);
                        }
                        else
                        {
                            // Terminate Strikethrough at last character of line.
                            strikethrough_end = new Vector3(m_textInfo.characterInfo[i].topRight.x, m_textInfo.characterInfo[i].baseLine + (font.fontInfo.Ascender + font.fontInfo.Descender) / 2 * strikethroughScale, 0);
                        }

                        beginStrikethrough = false;
                        DrawUnderlineMesh(strikethrough_start, strikethrough_end, ref last_vert_index, strikethroughScale, strikethroughScale, strikethroughScale, strikethroughColor);
                        underlineSegmentCount += 1;
                    }
                    else if (beginStrikethrough && i < m_characterCount && (m_textInfo.characterInfo[i + 1].pointSize != strikethroughPointSize || !TMP_Math.Equals(m_textInfo.characterInfo[i + 1].baseLine + offset.y, strikethroughBaseline)))
                    {
                        // Terminate Strikethrough if scale changes.
                        beginStrikethrough = false;

                        int lastVisibleCharacterIndex = lineInfo.lastVisibleCharacterIndex;
                        if (i > lastVisibleCharacterIndex)
                            strikethrough_end = new Vector3(m_textInfo.characterInfo[lastVisibleCharacterIndex].topRight.x, m_textInfo.characterInfo[lastVisibleCharacterIndex].baseLine + (font.fontInfo.Ascender + font.fontInfo.Descender) / 2 * strikethroughScale, 0);
                        else
                            strikethrough_end = new Vector3(m_textInfo.characterInfo[i].topRight.x, m_textInfo.characterInfo[i].baseLine + (font.fontInfo.Ascender + font.fontInfo.Descender) / 2 * strikethroughScale, 0);

                        DrawUnderlineMesh(strikethrough_start, strikethrough_end, ref last_vert_index, strikethroughScale, strikethroughScale, strikethroughScale, strikethroughColor);
                        underlineSegmentCount += 1;
                        //Debug.Log("Char [" + currentCharacter + "] at Index: " + i + "  End Strikethrough POS: " + strikethrough_end + "  Baseline: " + m_textInfo.characterInfo[i].baseLine.ToString("f3"));
                    }
                    else if (beginStrikethrough && !isStrikeThroughVisible)
                    {
                        // Terminate Strikethrough if character is not visible.
                        beginStrikethrough = false;
                        strikethrough_end = new Vector3(m_textInfo.characterInfo[i - 1].topRight.x, m_textInfo.characterInfo[i - 1].baseLine + (font.fontInfo.Ascender + font.fontInfo.Descender) / 2 * strikethroughScale, 0);

                        DrawUnderlineMesh(strikethrough_start, strikethrough_end, ref last_vert_index, strikethroughScale, strikethroughScale, strikethroughScale, strikethroughColor);
                        underlineSegmentCount += 1;
                    }
                }
                else
                {
                    // End Underline
                    if (beginStrikethrough == true)
                    {
                        beginStrikethrough = false;
                        strikethrough_end = new Vector3(m_textInfo.characterInfo[i - 1].topRight.x, m_textInfo.characterInfo[i - 1].baseLine + (font.fontInfo.Ascender + font.fontInfo.Descender) / 2 * m_fontScale, 0);

                        DrawUnderlineMesh(strikethrough_start, strikethrough_end, ref last_vert_index, strikethroughScale, strikethroughScale, strikethroughScale, strikethroughColor);
                        underlineSegmentCount += 1;
                    }
                }
                #endregion


                lastLine = currentLine;
            }
            #endregion


            // METRICS ABOUT THE TEXT OBJECT
            m_textInfo.characterCount = (short)m_characterCount;
            m_textInfo.spriteCount = m_spriteCount;
            m_textInfo.lineCount = (short)lineCount;
            m_textInfo.wordCount = wordCount != 0 && m_characterCount > 0 ? (short)wordCount : (short)1;
            m_textInfo.pageCount = m_pageNumber + 1;


            // If Advanced Layout Component is present, don't upload the mesh.
            if (m_renderMode == TextRenderFlags.Render) // m_isAdvanceLayoutComponentPresent == false || m_advancedLayoutComponent.isEnabled == false)
            {
                //Debug.Log("Uploading Mesh normally.");

                // Upload Mesh Data
                m_mesh.vertices = m_textInfo.meshInfo.vertices;
                m_mesh.uv = m_textInfo.meshInfo.uvs0;
                m_mesh.uv2 = m_textInfo.meshInfo.uvs2;
                m_mesh.colors32 = m_textInfo.meshInfo.colors32;

                m_mesh.RecalculateBounds(); // Replace by manual bound calculation to improve performance.

                m_uiRenderer.SetMesh(m_mesh);


                if (m_inlineGraphics != null)
                    m_inlineGraphics.DrawSprite(m_inlineGraphics.uiVertex, m_visibleSpriteCount);

            }

            // Compute Bounds for the mesh. Manual computation is more efficient then using Mesh.recalcualteBounds.
            m_bounds = new Bounds(new Vector3((m_meshExtents.max.x + m_meshExtents.min.x) / 2, (m_meshExtents.max.y + m_meshExtents.min.y) / 2, 0) + offset, new Vector3(m_meshExtents.max.x - m_meshExtents.min.x, m_meshExtents.max.y - m_meshExtents.min.y, 0));

            // Has Text Container's Width or Height been specified by the user?
            /*
            if (m_rectTransform.sizeDelta.x == 0 || m_rectTransform.sizeDelta.y == 0)
            {
                //Debug.Log("Auto-fitting Text. Default Width:" + m_textContainer.isDefaultWidth + "  Default Height:" + m_textContainer.isDefaultHeight);
                if (marginWidth == 0)
                    m_rectTransform.sizeDelta = new Vector2(m_preferredWidth + margins.x + margins.z, m_rectTransform.sizeDelta.y);

                if (marginHeight == 0)
                    m_rectTransform.sizeDelta = new Vector2(m_rectTransform.sizeDelta.x,  m_preferredHeight + margins.y + margins.w);

                
                Debug.Log("Auto-fitting Text. Default Width:" + m_preferredWidth + "  Default Height:" + m_preferredHeight);
                GenerateTextMesh();
                return;
            }
            */

            //for (int i = 0; i < m_lineNumber + 1; i++)
            //{
            //    Debug.Log("Line: " + (i + 1) + "  Character Count: " + m_textInfo.lineInfo[i].characterCount
            //                                 + "  Word Count: " + m_textInfo.lineInfo[i].wordCount
            //                                 + "  Space Count: " + m_textInfo.lineInfo[i].spaceCount
            //                                 + "  First: [" + m_textInfo.characterInfo[m_textInfo.lineInfo[i].firstCharacterIndex].character + "] at Index: " + m_textInfo.lineInfo[i].firstCharacterIndex
            //                                 + "  First Visible: [" + m_textInfo.characterInfo[m_textInfo.lineInfo[i].firstVisibleCharacterIndex].character + "] at Index: " + m_textInfo.lineInfo[i].firstVisibleCharacterIndex
            //                                 + "  Last [" + m_textInfo.characterInfo[m_textInfo.lineInfo[i].lastCharacterIndex].character + "] at Index: " + m_textInfo.lineInfo[i].lastCharacterIndex
            //                                 + "  Last visible [" + m_textInfo.characterInfo[m_textInfo.lineInfo[i].lastVisibleCharacterIndex].character + "] at Index: " + m_textInfo.lineInfo[i].lastVisibleCharacterIndex
            //                                 + "  Length: " + m_textInfo.lineInfo[i].lineLength
            //                                 + "  Line Extents: " + m_textInfo.lineInfo[i].lineExtents);
            //}

            //Debug.Log("Done rendering text. Character Count is " + m_textInfo.characterCount);
            //Debug.Log("Done rendering text. Preferred Width:" + m_preferredWidth + "  Preferred Height:" + m_preferredHeight);

            // Event indicating the text has been regenerated.
            TMPro_EventManager.ON_TEXT_CHANGED(this);

            //Debug.Log(m_minWidth);
            //Profiler.EndSample();
            //m_StopWatch.Stop();
            //Debug.Log("Done Rendering Text.");

            //Debug.Log("TimeElapsed is:" + (m_StopWatch.ElapsedTicks / 10000f).ToString("f4"));
            //m_StopWatch.Reset();
        }


        // Store Vertex Information for each Character
        void SaveGlyphVertexInfo(float style_padding, Color32 vertexColor)
        {
            // Save the Vertex Position for the Character
            #region Setup Mesh Vertices
            m_textInfo.characterInfo[m_characterCount].vertex_BL.position = m_textInfo.characterInfo[m_characterCount].bottomLeft;
            m_textInfo.characterInfo[m_characterCount].vertex_TL.position = m_textInfo.characterInfo[m_characterCount].topLeft;
            m_textInfo.characterInfo[m_characterCount].vertex_TR.position = m_textInfo.characterInfo[m_characterCount].topRight;
            m_textInfo.characterInfo[m_characterCount].vertex_BR.position = m_textInfo.characterInfo[m_characterCount].bottomRight;

            //Debug.DrawLine(p0, p1, Color.green, 60f);
            //Debug.DrawLine(p1, p2, Color.green, 60f);
            //Debug.DrawLine(p2, p3, Color.green, 60f);
            //Debug.DrawLine(p3, p0, Color.green, 60f);
            #endregion

            // Alpha is the lower of the vertex color or tag color alpha used.
            vertexColor.a = m_fontColor32.a < vertexColor.a ? (byte)(m_fontColor32.a) : (byte)(vertexColor.a);
            
            // Handle Vertex Colors & Vertex Color Gradient
            if (!m_enableVertexGradient)
            {
                m_textInfo.characterInfo[m_characterCount].vertex_BL.color = vertexColor;
                m_textInfo.characterInfo[m_characterCount].vertex_TL.color = vertexColor;
                m_textInfo.characterInfo[m_characterCount].vertex_TR.color = vertexColor;
                m_textInfo.characterInfo[m_characterCount].vertex_BR.color = vertexColor;
            }
            else
            {
                if (!m_overrideHtmlColors && !m_htmlColor.CompareRGB(m_fontColor32))
                {
                    m_textInfo.characterInfo[m_characterCount].vertex_BL.color = vertexColor;
                    m_textInfo.characterInfo[m_characterCount].vertex_TL.color = vertexColor;
                    m_textInfo.characterInfo[m_characterCount].vertex_TR.color = vertexColor;
                    m_textInfo.characterInfo[m_characterCount].vertex_BR.color = vertexColor;
                }
                else
                {
                    m_textInfo.characterInfo[m_characterCount].vertex_BL.color = m_fontColorGradient.bottomLeft;
                    m_textInfo.characterInfo[m_characterCount].vertex_TL.color = m_fontColorGradient.topLeft;
                    m_textInfo.characterInfo[m_characterCount].vertex_TR.color = m_fontColorGradient.topRight;
                    m_textInfo.characterInfo[m_characterCount].vertex_BR.color = m_fontColorGradient.bottomRight;
                }

                m_textInfo.characterInfo[m_characterCount].vertex_BL.color.a = vertexColor.a;
                m_textInfo.characterInfo[m_characterCount].vertex_TL.color.a = vertexColor.a;
                m_textInfo.characterInfo[m_characterCount].vertex_TR.color.a = vertexColor.a;
                m_textInfo.characterInfo[m_characterCount].vertex_BR.color.a = vertexColor.a;
            }
          

            // Apply style_padding only if this is a SDF Shader.
            if (!m_sharedMaterial.HasProperty(ShaderUtilities.ID_WeightNormal))
                style_padding = 0;


            // Setup UVs for the Character
            #region Setup UVs
            Vector2 uv0 = new Vector2((m_cached_GlyphInfo.x - m_padding - style_padding) / m_currentFontAsset.fontInfo.AtlasWidth, 1 - (m_cached_GlyphInfo.y + m_padding + style_padding + m_cached_GlyphInfo.height) / m_currentFontAsset.fontInfo.AtlasHeight);  // bottom left
            Vector2 uv1 = new Vector2(uv0.x, 1 - (m_cached_GlyphInfo.y - m_padding - style_padding) / m_currentFontAsset.fontInfo.AtlasHeight);  // top left
            Vector2 uv2 = new Vector2((m_cached_GlyphInfo.x + m_padding + style_padding + m_cached_GlyphInfo.width) / m_currentFontAsset.fontInfo.AtlasWidth, uv0.y); // bottom right
            Vector2 uv3 = new Vector2(uv2.x, uv1.y); // top right

            // Store UV Information
            m_textInfo.characterInfo[m_characterCount].vertex_BL.uv = uv0;
            m_textInfo.characterInfo[m_characterCount].vertex_TL.uv = uv1;
            m_textInfo.characterInfo[m_characterCount].vertex_TR.uv = uv3;
            m_textInfo.characterInfo[m_characterCount].vertex_BR.uv = uv2;   
            #endregion Setup UVs


            // Normal
            #region Setup Normals & Tangents
            Vector3 normal = new Vector3(0, 0, -1);
            m_textInfo.characterInfo[m_characterCount].vertex_BL.normal = normal;
            m_textInfo.characterInfo[m_characterCount].vertex_TL.normal = normal;
            m_textInfo.characterInfo[m_characterCount].vertex_TR.normal = normal;
            m_textInfo.characterInfo[m_characterCount].vertex_BR.normal = normal;

            // Tangents
            Vector4 tangent = new Vector4(-1, 0, 0, 1);
            m_textInfo.characterInfo[m_characterCount].vertex_BL.tangent = tangent;
            m_textInfo.characterInfo[m_characterCount].vertex_TL.tangent = tangent;
            m_textInfo.characterInfo[m_characterCount].vertex_TR.tangent = tangent;
            m_textInfo.characterInfo[m_characterCount].vertex_BR.tangent = tangent;
            #endregion end Normals & Tangents
        }


        // Store Vertex Information for each Sprite
        void SaveSpriteVertexInfo(Color32 vertexColor)
        {
            //int padding = m_enableExtraPadding ? 4 : 0;
            // Determine UV for the Sprite
            Vector2 uv0 = new Vector2(m_cached_GlyphInfo.x / m_inlineGraphics.spriteAsset.spriteSheet.width, m_cached_GlyphInfo.y / m_inlineGraphics.spriteAsset.spriteSheet.height);  // bottom left
            Vector2 uv1 = new Vector2(uv0.x, (m_cached_GlyphInfo.y + m_cached_GlyphInfo.height) / m_inlineGraphics.spriteAsset.spriteSheet.height);  // top left
            Vector2 uv2 = new Vector2((m_cached_GlyphInfo.x + m_cached_GlyphInfo.width) / m_inlineGraphics.spriteAsset.spriteSheet.width, uv0.y); // bottom right
            Vector2 uv3 = new Vector2(uv2.x, uv1.y); // top right


            // Vertex Color Alpha
            Color32 spriteColor = Color.white; 
            spriteColor.a = m_fontColor32.a < vertexColor.a ? m_fontColor32.a : vertexColor.a;

            TMP_Vertex vertex = new TMP_Vertex();
            // Bottom Left Vertex
            vertex.position = m_textInfo.characterInfo[m_characterCount].bottomLeft;
            vertex.uv = uv0;
            vertex.color = spriteColor;
            m_textInfo.characterInfo[m_characterCount].vertex_BL = vertex;
            
            // Top Left Vertex
            vertex.position = m_textInfo.characterInfo[m_characterCount].topLeft;
            vertex.uv = uv1;
            vertex.color = spriteColor;
            m_textInfo.characterInfo[m_characterCount].vertex_TL = vertex;
            
            // Top Right Vertex
            vertex.position = m_textInfo.characterInfo[m_characterCount].topRight;
            vertex.uv = uv3;
            vertex.color = spriteColor;
            m_textInfo.characterInfo[m_characterCount].vertex_TR = vertex;
            
            // Bottom Right Vertex
            vertex.position = m_textInfo.characterInfo[m_characterCount].bottomRight;
            vertex.uv = uv2;
            vertex.color = spriteColor;
            m_textInfo.characterInfo[m_characterCount].vertex_BR = vertex;
        }


        // Fill Vertex Buffers for Characters
        void FillCharacterVertexBuffers(int i, int index_X4)
        {
            //int meshIndex = m_textInfo.characterInfo[index].meshIndex;
            //int index2 = m_meshAllocCount[meshIndex];
            TMP_CharacterInfo[] characterInfoArray = m_textInfo.characterInfo;
            m_textInfo.characterInfo[i].vertexIndex = (short)(index_X4);

            // Setup Vertices for Characters
            m_textInfo.meshInfo.vertices[0 + index_X4] = characterInfoArray[i].vertex_BL.position;
            m_textInfo.meshInfo.vertices[1 + index_X4] = characterInfoArray[i].vertex_TL.position;
            m_textInfo.meshInfo.vertices[2 + index_X4] = characterInfoArray[i].vertex_TR.position;
            m_textInfo.meshInfo.vertices[3 + index_X4] = characterInfoArray[i].vertex_BR.position;


            // Setup UVS0
            m_textInfo.meshInfo.uvs0[0 + index_X4] = characterInfoArray[i].vertex_BL.uv;
            m_textInfo.meshInfo.uvs0[1 + index_X4] = characterInfoArray[i].vertex_TL.uv;
            m_textInfo.meshInfo.uvs0[2 + index_X4] = characterInfoArray[i].vertex_TR.uv;
            m_textInfo.meshInfo.uvs0[3 + index_X4] = characterInfoArray[i].vertex_BR.uv;


            // Setup UVS2
            m_textInfo.meshInfo.uvs2[0 + index_X4] = characterInfoArray[i].vertex_BL.uv2;
            m_textInfo.meshInfo.uvs2[1 + index_X4] = characterInfoArray[i].vertex_TL.uv2;
            m_textInfo.meshInfo.uvs2[2 + index_X4] = characterInfoArray[i].vertex_TR.uv2;
            m_textInfo.meshInfo.uvs2[3 + index_X4] = characterInfoArray[i].vertex_BR.uv2;


            // setup Vertex Colors
            m_textInfo.meshInfo.colors32[0 + index_X4] = characterInfoArray[i].vertex_BL.color;
            m_textInfo.meshInfo.colors32[1 + index_X4] = characterInfoArray[i].vertex_TL.color;
            m_textInfo.meshInfo.colors32[2 + index_X4] = characterInfoArray[i].vertex_TR.color;
            m_textInfo.meshInfo.colors32[3 + index_X4] = characterInfoArray[i].vertex_BR.color;
        }


        // Fill Vertex Buffers for Sprites
        void FillSpriteVertexBuffers(int i, int spriteIndex_X4)
        {
            m_textInfo.characterInfo[i].vertexIndex = (short)(spriteIndex_X4);
            TMP_CharacterInfo[] characterInfoArray = m_textInfo.characterInfo;

            //Debug.Log(m_visibleSpriteCount);
            UIVertex[] spriteVertices = m_inlineGraphics.uiVertex;
            //m_textInfo.characterInfo[i].uiVertices = spriteVertices;

            UIVertex uiVertex = new UIVertex();

            uiVertex.position = characterInfoArray[i].vertex_BL.position;
            uiVertex.uv0 = characterInfoArray[i].vertex_BL.uv;
            uiVertex.color = characterInfoArray[i].vertex_BL.color;
            spriteVertices[spriteIndex_X4 + 0] = uiVertex;

            uiVertex.position = characterInfoArray[i].vertex_TL.position;
            uiVertex.uv0 = characterInfoArray[i].vertex_TL.uv;
            uiVertex.color = characterInfoArray[i].vertex_TL.color;
            spriteVertices[spriteIndex_X4 + 1] = uiVertex;

            uiVertex.position = characterInfoArray[i].vertex_TR.position;
            uiVertex.uv0 = characterInfoArray[i].vertex_TR.uv;
            uiVertex.color = characterInfoArray[i].vertex_TR.color;
            spriteVertices[spriteIndex_X4 + 2] = uiVertex;

            uiVertex.position = characterInfoArray[i].vertex_BR.position;
            uiVertex.uv0 = characterInfoArray[i].vertex_BR.uv;
            uiVertex.color = characterInfoArray[i].vertex_BR.color;
            spriteVertices[spriteIndex_X4 + 3] = uiVertex;  

            m_inlineGraphics.SetUIVertex(spriteVertices);
        }


        // Draws the Underline
        void DrawUnderlineMesh(Vector3 start, Vector3 end, ref int index, float startScale, float endScale, float maxScale, Color32 underlineColor)
        {
            if (m_cached_Underline_GlyphInfo == null)
            {
                Debug.LogWarning("Unable to add underline since the Font Asset doesn't contain the underline character.", this);
                return;
            }

            int verticesCount = index + 12;
            // Check to make sure our current mesh buffer allocations can hold these new Quads.
            if (verticesCount > m_textInfo.meshInfo.vertices.Length)
            {
                // Resize Mesh Buffers
                m_textInfo.meshInfo.ResizeMeshInfo(verticesCount / 4);
            }

            // Adjust the position of the underline based on the lowest character. This matters for subscript character.
            start.y = Mathf.Min(start.y, end.y);
            end.y = Mathf.Min(start.y, end.y);

            float segmentWidth = m_cached_Underline_GlyphInfo.width / 2 * maxScale;

            if (end.x - start.x < m_cached_Underline_GlyphInfo.width * maxScale)
            {
                segmentWidth = (end.x - start.x) / 2f;
            }

            float startPadding = m_padding * startScale / maxScale;
            float endPadding = m_padding * endScale / maxScale;

            float underlineThickness = m_cached_Underline_GlyphInfo.height;

            // VERTICES
            Vector3[] vertices = m_textInfo.meshInfo.vertices;
            
            // Front Part of the Underline
            vertices[index + 0] = start + new Vector3(0, 0 - (underlineThickness + m_padding) * maxScale, 0); // BL
            vertices[index + 1] = start + new Vector3(0, m_padding * maxScale, 0); // TL
            vertices[index + 2] = vertices[index + 1] + new Vector3(segmentWidth, 0, 0); // TR
            vertices[index + 3] = vertices[index + 0] + new Vector3(segmentWidth, 0, 0); // BR

            // Middle Part of the Underline
            vertices[index + 4] = vertices[index + 3]; // BL
            vertices[index + 5] = vertices[index + 2]; // TL
            vertices[index + 6] = end + new Vector3(-segmentWidth, m_padding * maxScale, 0);  // TR
            vertices[index + 7] = end + new Vector3(-segmentWidth, -(underlineThickness + m_padding) * maxScale, 0); // BR

            // End Part of the Underline
            vertices[index + 8] = vertices[index + 7]; // BL
            vertices[index + 9] = vertices[index + 6]; // TL
            vertices[index + 10] = end + new Vector3(0, m_padding * maxScale, 0); // TR
            vertices[index + 11] = end + new Vector3(0, -(underlineThickness + m_padding) * maxScale, 0); // BR


            // UVS

            // Calculate UV required to setup the 3 Quads for the Underline.
            Vector2 uv0 = new Vector2((m_cached_Underline_GlyphInfo.x - startPadding) / m_fontAsset.fontInfo.AtlasWidth, 1 - (m_cached_Underline_GlyphInfo.y + m_padding + m_cached_Underline_GlyphInfo.height) / m_fontAsset.fontInfo.AtlasHeight);  // bottom left
            Vector2 uv1 = new Vector2(uv0.x, 1 - (m_cached_Underline_GlyphInfo.y - m_padding) / m_fontAsset.fontInfo.AtlasHeight);  // top left
            Vector2 uv2 = new Vector2((m_cached_Underline_GlyphInfo.x - startPadding + m_cached_Underline_GlyphInfo.width / 2) / m_fontAsset.fontInfo.AtlasWidth, uv1.y); // Mid Top Left
            Vector2 uv3 = new Vector2(uv2.x, uv0.y); // Mid Bottom Left
            Vector2 uv4 = new Vector2((m_cached_Underline_GlyphInfo.x + endPadding + m_cached_Underline_GlyphInfo.width / 2) / m_fontAsset.fontInfo.AtlasWidth, uv1.y); // Mid Top Right
            Vector2 uv5 = new Vector2(uv4.x, uv0.y); // Mid Bottom right
            Vector2 uv6 = new Vector2((m_cached_Underline_GlyphInfo.x + endPadding + m_cached_Underline_GlyphInfo.width) / m_fontAsset.fontInfo.AtlasWidth, uv1.y); // End Part - Bottom Right
            Vector2 uv7 = new Vector2(uv6.x, uv0.y); // End Part - Top Right

            Vector2[] uvs0 = m_textInfo.meshInfo.uvs0;

            // Left Part of the Underline
            uvs0[0 + index] = uv0; // BL
            uvs0[1 + index] = uv1; // TL
            uvs0[2 + index] = uv2; // TR
            uvs0[3 + index] = uv3; // BR

            // Middle Part of the Underline
            uvs0[4 + index] = new Vector2(uv2.x - uv2.x * 0.001f, uv0.y);
            uvs0[5 + index] = new Vector2(uv2.x - uv2.x * 0.001f, uv1.y);
            uvs0[6 + index] = new Vector2(uv2.x + uv2.x * 0.001f, uv1.y);
            uvs0[7 + index] = new Vector2(uv2.x + uv2.x * 0.001f, uv0.y);

            // Right Part of the Underline
            uvs0[8 + index] = uv5;
            uvs0[9 + index] = uv4;
            uvs0[10 + index] = uv6;
            uvs0[11 + index] = uv7;


            // UV1 contains Face / Border UV layout.
            float min_UvX = 0;
            float max_UvX = (vertices[index + 2].x - start.x) / (end.x - start.x);

            //Calculate the xScale or how much the UV's are getting stretched on the X axis for the middle section of the underline.
            float xScale = maxScale * m_rectTransform.lossyScale.z; // *m_canvas.planeDistance / 100;
            float xScale2 = xScale;

            Vector2[] uvs2 = m_textInfo.meshInfo.uvs2;

            uvs2[0 + index] = PackUV(0, 0, xScale);
            uvs2[1 + index] = PackUV(0, 1, xScale);
            uvs2[2 + index] = PackUV(max_UvX, 1, xScale);
            uvs2[3 + index] = PackUV(max_UvX, 0, xScale);

            min_UvX = (vertices[index + 4].x - start.x) / (end.x - start.x);
            max_UvX = (vertices[index + 6].x - start.x) / (end.x - start.x);

            uvs2[4 + index] = PackUV(min_UvX, 0, xScale2);
            uvs2[5 + index] = PackUV(min_UvX, 1, xScale2);
            uvs2[6 + index] = PackUV(max_UvX, 1, xScale2);
            uvs2[7 + index] = PackUV(max_UvX, 0, xScale2);

            min_UvX = (vertices[index + 8].x - start.x) / (end.x - start.x);
            max_UvX = (vertices[index + 6].x - start.x) / (end.x - start.x);

            uvs2[8 + index] = PackUV(min_UvX, 0, xScale);
            uvs2[9 + index] = PackUV(min_UvX, 1, xScale);
            uvs2[10 + index] = PackUV(1, 1, xScale);
            uvs2[11 + index] = PackUV(1, 0, xScale);


            //underlineColor.a /= 2; // Since bold is encoded in the alpha, we need to adjust this.

            Color32[] colors = m_textInfo.meshInfo.colors32;
            // Set Underline Color
            colors[0 + index] = underlineColor;
            colors[1 + index] = underlineColor;
            colors[2 + index] = underlineColor;
            colors[3 + index] = underlineColor;

            colors[4 + index] = underlineColor;
            colors[5 + index] = underlineColor;
            colors[6 + index] = underlineColor;
            colors[7 + index] = underlineColor;

            colors[8 + index] = underlineColor;
            colors[9 + index] = underlineColor;
            colors[10 + index] = underlineColor;
            colors[11 + index] = underlineColor;


            index += 12;
        }


        /// <summary>
        /// Method to Update Scale in UV2
        /// </summary>
        void UpdateSDFScale(float prevScale, float newScale)
        {
            Vector2[] uvs2 = m_textInfo.meshInfo.uvs2;

            for (int i = 0; i < uvs2.Length; i++)
            {
                uvs2[i].y = (uvs2[i].y / prevScale) * newScale; 
            }

            m_mesh.uv2 = uvs2;
            m_uiRenderer.SetMesh(m_mesh);
        }


        // Function to offset vertices position to account for line spacing changes.
        void AdjustLineOffset(int startIndex, int endIndex, float offset)
        {
            Vector3 vertexOffset = new Vector3(0, offset, 0);

            for (int i = startIndex; i <= endIndex; i++)
            {
                m_textInfo.characterInfo[i].bottomLeft -= vertexOffset;
                m_textInfo.characterInfo[i].topLeft -= vertexOffset;
                m_textInfo.characterInfo[i].topRight -= vertexOffset;
                m_textInfo.characterInfo[i].bottomRight -= vertexOffset;

                m_textInfo.characterInfo[i].topLine -= vertexOffset.y;
                m_textInfo.characterInfo[i].baseLine -= vertexOffset.y;
                m_textInfo.characterInfo[i].bottomLine -= vertexOffset.y;

                if (m_textInfo.characterInfo[i].isVisible)
                {
                    m_textInfo.characterInfo[i].vertex_BL.position -= vertexOffset;
                    m_textInfo.characterInfo[i].vertex_TL.position -= vertexOffset;
                    m_textInfo.characterInfo[i].vertex_TR.position -= vertexOffset;
                    m_textInfo.characterInfo[i].vertex_BR.position -= vertexOffset;
                }
            }
        }


        // Save the State of various variables used in the mesh creation loop in conjunction with Word Wrapping 
        void SaveWordWrappingState(ref WordWrapState state, int index, int count)
        {
            state.previous_WordBreak = index;
            state.total_CharacterCount = count;
            state.visible_CharacterCount = m_visibleCharacterCount;
            state.firstCharacterIndex = m_firstCharacterOfLine;
            state.visible_SpriteCount = m_visibleSpriteCount;
            state.visible_LinkCount = m_textInfo.linkCount;
            state.firstVisibleCharacterIndex = m_firstVisibleCharacterOfLine;
            state.lastVisibleCharIndex = m_lastVisibleCharacterOfLine;
            
            state.xAdvance = m_xAdvance;
            state.maxAscender = m_maxAscender;
            state.maxDescender = m_maxDescender;
            state.preferredWidth = m_preferredWidth;
            state.preferredHeight = m_preferredHeight;
            state.fontScale = m_fontScale;
            state.maxFontScale = m_maxFontScale; 
            //state.previousLineScale = m_previousFontScale;
                    
            state.currentFontSize = m_currentFontSize;
            state.lineNumber = m_lineNumber; 
            state.lineOffset = m_lineOffset;
            state.baselineOffset = m_baselineOffset;
            state.fontStyle = m_style;
            //state.alignment = m_lineJustification;
            state.vertexColor = m_htmlColor;
            state.colorStackIndex = m_colorStackIndex;
            state.meshExtents = m_meshExtents;
            state.lineInfo = m_textInfo.lineInfo[m_lineNumber];
            //state.textInfo = m_textInfo;
        }

        // Restore the State of various variables used in the mesh creation loop.
        int RestoreWordWrappingState(ref WordWrapState state)
        {
            m_textInfo.lineInfo[m_lineNumber] = state.lineInfo;
            //m_textInfo = state.textInfo != null ? state.textInfo : m_textInfo;
            m_currentFontSize = state.currentFontSize;
            m_fontScale = state.fontScale;
            m_baselineOffset = state.baselineOffset;
            m_style = state.fontStyle;
            //m_lineJustification = state.alignment;
            m_htmlColor = state.vertexColor;
            m_colorStackIndex = state.colorStackIndex;

            m_characterCount = state.total_CharacterCount + 1;
            m_visibleCharacterCount = state.visible_CharacterCount;
            m_visibleSpriteCount = state.visible_SpriteCount;
            m_textInfo.linkCount = state.visible_LinkCount;
            m_firstCharacterOfLine = state.firstCharacterIndex;
            m_firstVisibleCharacterOfLine = state.firstVisibleCharacterIndex;
            m_lastVisibleCharacterOfLine = state.lastVisibleCharIndex;
            m_meshExtents = state.meshExtents;
            m_xAdvance = state.xAdvance;
            m_maxAscender = state.maxAscender;
            m_maxDescender = state.maxDescender;
            m_preferredWidth = state.preferredWidth;
            m_preferredHeight = state.preferredHeight;
            m_lineNumber = state.lineNumber;
            m_lineOffset = state.lineOffset;
            //m_previousFontScale = state.previousLineScale;
            m_maxFontScale = state.maxFontScale;

            int index = state.previous_WordBreak;

            return index;
        }


        // Function to pack scale information in the UV2 Channel.
        Vector2 PackUV(float x, float y, float scale)
        {
            x = (x % 5) / 5;
            y = (y % 5) / 5;

            //return new Vector2((x * 4096) + y, scale);
            return new Vector2(Mathf.Round(x * 4096) + y, scale);
        }


        // Function to increase the size of the Line Extents Array.
        void ResizeLineExtents(int size)
        {
            size = size > 1024 ? size + 256 : Mathf.NextPowerOfTwo(size + 1);

            TMP_LineInfo[] temp_lineInfo = new TMP_LineInfo[size];
            for (int i = 0; i < size; i++)
            {
                if (i < m_textInfo.lineInfo.Length)
                    temp_lineInfo[i] = m_textInfo.lineInfo[i];
                else
                {
                    temp_lineInfo[i].lineExtents = new Extents(k_InfinityVector, -k_InfinityVector);
                    temp_lineInfo[i].ascender = -k_InfinityVector.x;
                    temp_lineInfo[i].descender = k_InfinityVector.x;
                }
            }

            m_textInfo.lineInfo = temp_lineInfo;
        }


        // Convert HEX to INT
        int HexToInt(char hex)
        {
            switch (hex)
            {
                case '0': return 0;
                case '1': return 1;
                case '2': return 2;
                case '3': return 3;
                case '4': return 4;
                case '5': return 5;
                case '6': return 6;
                case '7': return 7;
                case '8': return 8;
                case '9': return 9;
                case 'A': return 10;
                case 'B': return 11;
                case 'C': return 12;
                case 'D': return 13;
                case 'E': return 14;
                case 'F': return 15;
                case 'a': return 10;
                case 'b': return 11;
                case 'c': return 12;
                case 'd': return 13;
                case 'e': return 14;
                case 'f': return 15;
            }
            return 15;
        }


        /// <summary>
        /// Convert UTF-16 Hex to Char
        /// </summary>
        /// <returns>The Unicode hex.</returns>
        /// <param name="i">The index.</param>
        int GetUTF16(int i)
        {
            int unicode = HexToInt (text [i]) * 4096;
            unicode += HexToInt (text [i + 1]) * 256;
            unicode += HexToInt (text [i + 2]) * 16;
            unicode += HexToInt (text [i + 3]);
            return unicode;
        }


        /// <summary>
        /// Convert UTF-32 Hex to Char
        /// </summary>
        /// <returns>The Unicode hex.</returns>
        /// <param name="i">The index.</param>
        int GetUTF32(int i)
        {
            int unicode = 0;
            unicode += HexToInt(text[i]) * 268435456;
            unicode += HexToInt(text[i + 1]) * 16777216;
            unicode += HexToInt(text[i + 2]) * 1048576;
            unicode += HexToInt(text[i + 3]) * 65536;
            unicode += HexToInt(text[i + 4]) * 4096;
            unicode += HexToInt(text[i + 5]) * 256;
            unicode += HexToInt(text[i + 6]) * 16;
            unicode += HexToInt(text[i + 7]);
            return unicode;
        }


        Color32 HexCharsToColor(char[] hexChars, int tagCount)
        {
            if (tagCount == 7)
            {
                byte r = (byte)(HexToInt(hexChars[1]) * 16 + HexToInt(hexChars[2]));
                byte g = (byte)(HexToInt(hexChars[3]) * 16 + HexToInt(hexChars[4]));
                byte b = (byte)(HexToInt(hexChars[5]) * 16 + HexToInt(hexChars[6]));

                return new Color32(r, g, b, 255);
            }
            else if (tagCount == 9)
            {
                byte r = (byte)(HexToInt(hexChars[1]) * 16 + HexToInt(hexChars[2]));
                byte g = (byte)(HexToInt(hexChars[3]) * 16 + HexToInt(hexChars[4]));
                byte b = (byte)(HexToInt(hexChars[5]) * 16 + HexToInt(hexChars[6]));
                byte a = (byte)(HexToInt(hexChars[7]) * 16 + HexToInt(hexChars[8]));

                return new Color32(r, g, b, a);
            }
            else if (tagCount == 13)
            {
                byte r = (byte)(HexToInt(hexChars[7]) * 16 + HexToInt(hexChars[8]));
                byte g = (byte)(HexToInt(hexChars[9]) * 16 + HexToInt(hexChars[10]));
                byte b = (byte)(HexToInt(hexChars[11]) * 16 + HexToInt(hexChars[12]));

                return new Color32(r, g, b, 255);
            }
            else if (tagCount == 15)
            {
                byte r = (byte)(HexToInt(hexChars[7]) * 16 + HexToInt(hexChars[8]));
                byte g = (byte)(HexToInt(hexChars[9]) * 16 + HexToInt(hexChars[10]));
                byte b = (byte)(HexToInt(hexChars[11]) * 16 + HexToInt(hexChars[12]));
                byte a = (byte)(HexToInt(hexChars[13]) * 16 + HexToInt(hexChars[14]));

                return new Color32(r, g, b, a);
            }

            return new Color32(255, 255, 255, 255);
        }


        /// <summary>
        /// Extracts a float value from char[] assuming we know the position of the start, end and decimal point.
        /// </summary>
        /// <param name="chars"></param> The Char[] containing the numerical sequence.
        /// <param name="startIndex"></param> The index of the start of the numerical sequence.
        /// <param name="endIndex"></param> The index of the last number in the numerical sequence.
        /// <param name="decimalPointIndex"></param> The index of the decimal point if any.
        /// <returns></returns>
        float ConvertToFloat(char[] chars, int startIndex, int endIndex, int decimalPointIndex)
        {
            if (startIndex == 0) return 0;

            float v = 0;
            float sign = 1;
            decimalPointIndex = decimalPointIndex > 0 ? decimalPointIndex : endIndex + 1; // Check in case we don't have any decimal point

            // Check if negative value
            if (chars[startIndex] == 45) // '-'
            {
                startIndex += 1;
                sign = -1;
            }

            if (chars[startIndex] == 43 || chars[startIndex] == 37) startIndex += 1; // '+'


            for (int i = startIndex; i < endIndex + 1; i++)
            {
                if (!char.IsDigit(chars[i]) && chars[i] != 46) return -9999;

                switch (decimalPointIndex - i)
                {
                    case 4:
                        v += (chars[i] - 48) * 1000;
                        break;
                    case 3:
                        v += (chars[i] - 48) * 100;
                        break;
                    case 2:
                        v += (chars[i] - 48) * 10;
                        break;
                    case 1:
                        v += (chars[i] - 48);
                        break;
                    case -1:
                        v += (chars[i] - 48) * 0.1f;
                        break;
                    case -2:
                        v += (chars[i] - 48) * 0.01f;
                        break;
                    case -3:
                        v += (chars[i] - 48) * 0.001f;
                        break;
                }
            }
            return v * sign;
        }


        // Function to identify and validate the rich tag. Returns the position of the > if the tag was valid.
        bool ValidateHtmlTag(int[] chars, int startIndex, out int endIndex)
        {
            Array.Clear(m_htmlTag, 0, m_htmlTag.Length);
            int tagCharCount = 0;
            int tagHashCode = 0;

            TagAttribute attribute_1 = new TagAttribute();
            TagAttribute attribute_2 = new TagAttribute();
            byte attributeFlag = 0;

            TagUnits tagUnits = TagUnits.Pixels;

            int numSequenceStart = 0;
            int numSequenceEnd = 0;
            int numSequenceDecimalPos = 0;
            int numSequenceUnitPos = 0;

            endIndex = startIndex;

            bool isValidHtmlTag = false;
            bool hasNumericalValue = false;

            for (int i = startIndex; i < chars.Length && chars[i] != 0 && tagCharCount < m_htmlTag.Length && chars[i] != 60; i++)
            {
                if (chars[i] == 62) // ASCII Code of End HTML tag '>'
                {
                    isValidHtmlTag = true;
                    endIndex = i;
                    m_htmlTag[tagCharCount] = (char)0;
                    if (numSequenceEnd == 0) numSequenceEnd = tagCharCount - 1;
                    break;
                }

                m_htmlTag[tagCharCount] = (char)chars[i];
                tagCharCount += 1;


                // Compute HashCode for 1st attribute
                if (attributeFlag == 1)
                {
                    if (chars[i] != 34) // Exclude quotes from the HashCode.
                    {
                        if (attribute_1.startIndex == 0) attribute_1.startIndex = tagCharCount - 1;

                        attribute_1.hashCode = (attribute_1.hashCode << 5) - attribute_1.hashCode + chars[i];
                        attribute_1.length += 1;
                    }
                    else
                        if (attribute_1.startIndex != 0) attributeFlag = 2;
                }

                // Compute HashCode for 2st attribute
                if (attributeFlag == 3)
                {
                    if (chars[i] != 34) // Exclude quotes from the HashCode.
                    {
                        if (attribute_2.startIndex == 0) attribute_2.startIndex = tagCharCount - 1;

                        attribute_2.hashCode = (attribute_2.hashCode << 5) - attribute_2.hashCode + chars[i];
                        attribute_2.length += 1;
                    }
                    else
                        if (attribute_2.startIndex != 0) attributeFlag = 0;
                }


                // Extract numerical value and unit type (px, em, %)
                if (chars[i] == 61)  // '='
                {
                    numSequenceStart = tagCharCount;
                    attributeFlag += 1;
                }
                else if (chars[i] == 46) // '.'
                    numSequenceDecimalPos = tagCharCount - 1;
                else if (numSequenceStart != 00 && !hasNumericalValue && char.IsDigit((char)chars[i]))
                    hasNumericalValue = true;
                else if (numSequenceStart != 0 && numSequenceUnitPos == 0 && (chars[i] == 112 || chars[i] == 101 || chars[i] == 37))
                {
                    numSequenceEnd = tagCharCount - 2;
                    numSequenceUnitPos = tagCharCount - 1;
                    if (chars[i] == 101) tagUnits = TagUnits.FontUnits;
                    else if (chars[i] == 37) tagUnits = TagUnits.Percentage;
                }

                // Compute HashCode for the <tag>
                if (numSequenceStart == 0)
                    tagHashCode = (tagHashCode << 3) - tagHashCode + chars[i];

            }

            if (!isValidHtmlTag)
            {
                return false;
            }


            //Debug.Log("Tag is [" + m_htmlTag.ArrayToString() + "].  Tag HashCode: " + tagHashCode + "  Attribute HashCode: " + attribute_1.hashCode);

            // Special handling of the NoParsing tag
            if (tag_NoParsing && tagHashCode != 53822163)
                return false;
            else if (tagHashCode == 53822163)
            {
                tag_NoParsing = false;
                return true;
            }

            // Color <#FF00FF>
            if (m_htmlTag[0] == 35 && tagCharCount == 7) // if Tag begins with # and contains 7 characters. 
            {
                m_htmlColor = HexCharsToColor(m_htmlTag, tagCharCount);
                m_colorStack[m_colorStackIndex] = m_htmlColor;
                m_colorStackIndex += 1;
                return true;
            }
            // Color <#FF00FF00> with alpha
            else if (m_htmlTag[0] == 35 && tagCharCount == 9) // if Tag begins with # and contains 9 characters. 
            {
                m_htmlColor = HexCharsToColor(m_htmlTag, tagCharCount);
                m_colorStack[m_colorStackIndex] = m_htmlColor;
                m_colorStackIndex += 1;
                return true;
            }
            else
            {
                float value = 0;

                switch (tagHashCode)
                {
                    case 98: // <b>
                        m_style |= FontStyles.Bold;
                        return true;
                    case 427: // </b>
                        if ((m_fontStyle & FontStyles.Bold) != FontStyles.Bold)
                            m_style &= ~FontStyles.Bold;
                        return true;
                    case 105: // <i>
                        m_style |= FontStyles.Italic;
                        return true;
                    case 434: // </i>
                        m_style &= ~FontStyles.Italic;
                        return true;
                    case 115: // <s>
                        m_style |= FontStyles.Strikethrough;
                        return true;
                    case 444: // </s>
                        if ((m_fontStyle & FontStyles.Strikethrough) != FontStyles.Strikethrough)
                            m_style &= ~FontStyles.Strikethrough;
                        return true;
                    case 117: // <u>
                        m_style |= FontStyles.Underline;
                        return true;
                    case 446: // </u>
                        if ((m_fontStyle & FontStyles.Underline) != FontStyles.Underline)
                            m_style &= ~FontStyles.Underline;
                        return true;

                    case 6552: // <sub>
                        m_currentFontSize *= m_fontAsset.fontInfo.SubSize > 0 ? m_fontAsset.fontInfo.SubSize : 1; // Subscript characters are half size.
                        m_fontScale = m_currentFontSize / m_fontAsset.fontInfo.PointSize;
                        m_baselineOffset = m_fontAsset.fontInfo.SubscriptOffset * m_fontScale;
                        m_style |= FontStyles.Subscript;
                        //m_isRecalculateScaleRequired = true;
                        return true;
                    case 22673: // </sub>
                        m_currentFontSize /= m_fontAsset.fontInfo.SubSize > 0 ? m_fontAsset.fontInfo.SubSize : 1; //m_fontSize / m_fontAsset.FontInfo.PointSize * .1f;
                        m_baselineOffset = 0;
                        m_fontScale = m_currentFontSize / m_fontAsset.fontInfo.PointSize;
                        m_style &= ~FontStyles.Subscript;
                        //m_isRecalculateScaleRequired = true;
                        return true;
                    case 6566: // <sup>
                        m_currentFontSize *= m_fontAsset.fontInfo.SubSize > 0 ? m_fontAsset.fontInfo.SubSize : 1;
                        m_fontScale = m_currentFontSize / m_fontAsset.fontInfo.PointSize;
                        m_baselineOffset = m_fontAsset.fontInfo.SuperscriptOffset * m_fontScale;
                        m_style |= FontStyles.Superscript;
                        //m_isRecalculateScaleRequired = true;
                        return true;
                    case 22687: // </sup>
                        m_currentFontSize /= m_fontAsset.fontInfo.SubSize > 0 ? m_fontAsset.fontInfo.SubSize : 1; //m_fontSize / m_fontAsset.FontInfo.PointSize * .1f;
                        m_baselineOffset = 0;
                        m_fontScale = m_currentFontSize / m_fontAsset.fontInfo.PointSize;
                        m_style &= ~FontStyles.Superscript;
                        //m_isRecalculateScaleRequired = true;
                        return true;
                    case 6380: // <pos=000.00px> <pos=0em> <pos=50%>
                        value = ConvertToFloat(m_htmlTag, numSequenceStart, numSequenceEnd, numSequenceDecimalPos);
                        if (value == -9999 || value == 0) return false;

                        switch (tagUnits)
                        {
                            case TagUnits.Pixels:
                                m_xAdvance = value;
                                //m_isIgnoringAlignment = true;
                                return true;
                            case TagUnits.FontUnits:
                                m_xAdvance = value * m_fontScale * m_fontAsset.fontInfo.TabWidth / m_fontAsset.TabSize;
                                //m_isIgnoringAlignment = true;
                                return true;
                            case TagUnits.Percentage:
                                m_xAdvance = m_marginWidth * value / 100;
                                //m_isIgnoringAlignment = true;
                                return true;
                        }
                        return false;
                    case 22501: // </pos>
                        m_isIgnoringAlignment = false;
                        return true;
                    case 16034505: // <voffset>
                        value = ConvertToFloat(m_htmlTag, numSequenceStart, numSequenceEnd, numSequenceDecimalPos);
                        if (value == -9999 || value == 0) return false;

                        switch (tagUnits)
                        {
                            case TagUnits.Pixels:
                                m_baselineOffset = value;
                                return true;
                            case TagUnits.FontUnits:
                                m_baselineOffset = value * m_fontScale * m_fontAsset.fontInfo.Ascender;
                                return true;
                            case TagUnits.Percentage:
                                //m_baselineOffset = m_marginHeight * val / 100;
                                return false;
                        }
                        return false;
                    case 54741026: // </voffset>
                        m_baselineOffset = 0;
                        return true;
                    case 43991: // <page>
                        // This tag only works when Overflow - Page mode is used.
                        if (m_overflowMode == TextOverflowModes.Page)
                        {
                            m_xAdvance = 0 + tag_LineIndent + tag_Indent;
                            //m_textInfo.lineInfo[m_lineNumber].marginLeft = m_xAdvance;
                            m_lineOffset = 0;
                            m_pageNumber += 1;
                            m_isNewPage = true;
                        }
                        return true;

                    case 43969: // <nobr>
                        m_isNonBreakingSpace = true;
                        return true;
                    case 156816: // </nobr>
                        m_isNonBreakingSpace = false;
                        return true;
                    case 45545: // <size=>
                        value = ConvertToFloat(m_htmlTag, numSequenceStart, numSequenceEnd, numSequenceDecimalPos);
                        if (value == -9999 || value == 0) return false;

                        switch (tagUnits)
                        {
                            case TagUnits.Pixels:
                                if (m_htmlTag[5] == 43) // <size=+00>
                                {
                                    m_currentFontSize = m_fontSize + value;
                                    m_isRecalculateScaleRequired = true;
                                    return true;
                                }
                                else if (m_htmlTag[5] == 45) // <size=-00>
                                {
                                    m_currentFontSize = m_fontSize + value;
                                    m_isRecalculateScaleRequired = true;
                                    return true;
                                }
                                else // <size=00.0>
                                {
                                    m_currentFontSize = value;
                                    m_isRecalculateScaleRequired = true;
                                    return true;
                                }
                            case TagUnits.FontUnits:
                                m_currentFontSize *= value;
                                m_isRecalculateScaleRequired = true;
                                return true;
                            case TagUnits.Percentage:
                                m_currentFontSize = m_fontSize * value / 100;
                                m_isRecalculateScaleRequired = true;
                                return true;
                        }
                        return false;
                    case 158392: // </size>
                        m_currentFontSize = m_fontSize;
                        m_isRecalculateScaleRequired = true;
                        //m_fontScale = m_fontSize / m_fontAsset.fontInfo.PointSize * .1f;
                        return true;
                    case 41311: // <font=xx>
                        //Debug.Log("Font name: \"" + new string(m_htmlTag, attribute_1.startIndex, attribute_1.length) + "\"   HashCode: " + attribute_1.hashCode + "   Material Name: \"" + new string(m_htmlTag, attribute_2.startIndex, attribute_2.length) + "\"   Hashcode: " + attribute_2.hashCode);

                        int fontHashCode = attribute_1.hashCode;
                        int materialHashCode = attribute_2.hashCode;

                        TextMeshProFont tempFont;
                        Material tempMaterial;

                        // HANDLE NEW FONT ASSET
                        if (m_fontAsset_Dict.TryGetValue(fontHashCode, out tempFont))
                        {
                            if (tempFont != m_currentFontAsset)
                            {
                                //Debug.Log("Assigning Font Asset: " + tempFont.name);
                                m_currentFontAsset = m_fontAsset_Dict[fontHashCode];
                                m_isRecalculateScaleRequired = true;
                            }
                        }
                        else
                        {
                            // Load new font asset
                            tempFont = Resources.Load("Fonts & Materials/" + new string(m_htmlTag, attribute_1.startIndex, attribute_1.length), typeof(TextMeshProFont)) as TextMeshProFont;
                            if (tempFont != null)
                            {
                                //Debug.Log("Loading and Assigning Font Asset: " + tempFont.name);
                                m_fontAsset_Dict.Add(fontHashCode, tempFont);
                                m_currentFontAsset = tempFont;
                                m_isRecalculateScaleRequired = true;
                            }
                            else
                                return false;
                        }


                        // HANDLE NEW MATERIAL
                        if (materialHashCode == 0)
                        {
                            if (!m_fontMaterial_Dict.TryGetValue(m_currentFontAsset.materialHashCode, out tempMaterial))
                                m_fontMaterial_Dict.Add(m_currentFontAsset.materialHashCode, m_currentFontAsset.material);

                            if (m_currentMaterial != m_currentFontAsset.material)
                            {
                                //Debug.Log("Assigning Default Font Asset Material: " + m_currentFontAsset.material.name);
                                m_currentMaterial = m_currentFontAsset.material;
                            }

                        }
                        else if (m_fontMaterial_Dict.TryGetValue(materialHashCode, out tempMaterial))
                        {
                            if (tempMaterial != m_currentMaterial)
                            {
                                //Debug.Log("Assigning Material: " + tempMaterial.name);
                                m_currentMaterial = tempMaterial;
                            }
                        }
                        else
                        {
                            // Load new material
                            tempMaterial = Resources.Load("Fonts & Materials/" + new string(m_htmlTag, attribute_2.startIndex, attribute_2.length), typeof(Material)) as Material;
                            if (tempMaterial != null)
                            {
                                //Debug.Log("Loading and Assigning Material: " + tempMaterial.name);
                                m_fontMaterial_Dict.Add(materialHashCode, tempMaterial);
                                m_currentMaterial = tempMaterial;
                            }
                            else
                                return false;
                        }

                        return true;
                    case 320078: // <space=000.00>
                        value = ConvertToFloat(m_htmlTag, numSequenceStart, numSequenceEnd, numSequenceDecimalPos);
                        if (value == -9999 || value == 0) return false;

                        switch (tagUnits)
                        {
                            case TagUnits.Pixels:
                                m_xAdvance += value;
                                return true;
                            case TagUnits.FontUnits:
                                m_xAdvance += value * m_fontScale * m_fontAsset.fontInfo.TabWidth / m_fontAsset.TabSize;
                                return true;
                            case TagUnits.Percentage:
                                // Not applicable
                                return false;
                        }
                        return false;
                    case 276254: // <alpha=#FF>
                        m_htmlColor.a = (byte)(HexToInt(m_htmlTag[7]) * 16 + HexToInt(m_htmlTag[8]));
                        return true;

                    case 1750458: // <a name=" ">
                        return true;
                    case 426: // </a>
                        return true;
                    case 43066: // <link="name">
                        if (m_isParsingText)
                        {
                            tag_LinkInfo.hashCode = attribute_1.hashCode;
                            tag_LinkInfo.firstCharacterIndex = m_characterCount;
                            //Debug.Log("Link begin at Character # " + m_characterCount);
                        }
                        return true;
                    case 155913: // </link>
                        if (m_isParsingText)
                        {
                            tag_LinkInfo.lastCharacterIndex = m_characterCount - 1;
                            tag_LinkInfo.characterCount = m_characterCount - tag_LinkInfo.firstCharacterIndex;
                            m_textInfo.linkInfo.Add(tag_LinkInfo);
                            m_textInfo.linkCount += 1;

                            //Debug.Log("*** LinkInfo Element Added ***\nHashCode: " + tag_LinkInfo.hashCode + "  First Index: " + tag_LinkInfo.firstCharacterIndex + "  Last Index: " + tag_LinkInfo.lastCharacterIndex + "  Link Count: " + m_textInfo.linkCount);
                        }
                        return true;
                    case 275917: // <align=>
                        switch (attribute_1.hashCode)
                        {
                            case 3317767: // <align=left>
                                m_lineJustification = TextAlignmentOptions.Left;
                                return true;
                            case 108511772: // <align=right>
                                m_lineJustification = TextAlignmentOptions.Right;
                                return true;
                            case -1364013995: // <align=center>
                                m_lineJustification = TextAlignmentOptions.Center;
                                return true;
                            case 1838536479: // <align=justified>
                                m_lineJustification = TextAlignmentOptions.Justified;
                                return true;
                        }
                        return false;
                    case 1065846: // </align>
                        m_lineJustification = m_textAlignment;
                        return true;
                    case 327550: // <width=xx>
                        value = ConvertToFloat(m_htmlTag, numSequenceStart, numSequenceEnd, numSequenceDecimalPos);
                        if (value == -9999 || value == 0) return false;

                        switch (tagUnits)
                        {
                            case TagUnits.Pixels:
                                m_width = value;
                                break;
                            case TagUnits.FontUnits:
                                return false;
                            //break;
                            case TagUnits.Percentage:
                                m_width = m_marginWidth * value / 100;
                                break;
                        }
                        return true;
                    case 1117479: // </width>
                        m_width = -1;
                        return true;
                    case 322689: // <style="name">
                        TMP_Style style = TMP_StyleSheet.Instance.GetStyle(attribute_1.hashCode);

                        if (style == null) return false;

                        m_styleStack[m_styleStackIndex] = style.hashCode;
                        m_styleStackIndex += 1;

                        //// Parse Style Macro
                        for (int i = 0; i < style.styleOpeningTagArray.Length; i++)
                        {
                            if (style.styleOpeningTagArray[i] == 60)
                                ValidateHtmlTag(style.styleOpeningTagArray, i + 1, out i);
                        }
                        return true;
                    case 1112618: // </style>
                        style = TMP_StyleSheet.Instance.GetStyle(attribute_1.hashCode);

                        if (style == null)
                        {
                            // Get style from the Style Stack
                            m_styleStackIndex = m_styleStackIndex > 0 ? m_styleStackIndex - 1 : 0;
                            style = TMP_StyleSheet.Instance.GetStyle(m_styleStack[m_styleStackIndex]);

                        }

                        if (style == null) return false;
                        //// Parse Style Macro
                        for (int i = 0; i < style.styleClosingTagArray.Length; i++)
                        {
                            if (style.styleClosingTagArray[i] == 60)
                                ValidateHtmlTag(style.styleClosingTagArray, i + 1, out i);
                        }
                        return true;
                    case 281955: // <color=#FF00FF> or <color=#FF00FF00>
                        // <color=#FF00FF>
                        if (m_htmlTag[6] == 35 && tagCharCount == 13)
                        {
                            m_htmlColor = HexCharsToColor(m_htmlTag, tagCharCount);
                            m_colorStack[m_colorStackIndex] = m_htmlColor;
                            m_colorStackIndex += 1;
                            return true;
                        }
                        // <color=#FF00FF00>
                        else if (m_htmlTag[6] == 35 && tagCharCount == 15)
                        {
                            m_htmlColor = HexCharsToColor(m_htmlTag, tagCharCount);
                            m_colorStack[m_colorStackIndex] = m_htmlColor;
                            m_colorStackIndex += 1;
                            return true;
                        }

                        // <color=name>
                        switch (attribute_1.hashCode)
                        {
                            case 112785: // <color=red>
                                m_htmlColor = Color.red;
                                m_colorStack[m_colorStackIndex] = m_htmlColor;
                                m_colorStackIndex += 1;
                                return true;
                            case 3027034: // <color=blue>
                                m_htmlColor = Color.blue;
                                m_colorStack[m_colorStackIndex] = m_htmlColor;
                                m_colorStackIndex += 1;
                                return true;
                            case 93818879: // <color=black>
                                m_htmlColor = Color.black;
                                m_colorStack[m_colorStackIndex] = m_htmlColor;
                                m_colorStackIndex += 1;
                                return true;
                            case 98619139: // <color=green>
                                m_htmlColor = Color.green;
                                m_colorStack[m_colorStackIndex] = m_htmlColor;
                                m_colorStackIndex += 1;
                                return true;
                            case 113101865: // <color=white>
                                m_htmlColor = Color.white;
                                m_colorStack[m_colorStackIndex] = m_htmlColor;
                                m_colorStackIndex += 1;
                                return true;
                            case -1008851410: // <color=orange>
                                m_htmlColor = new Color32(255, 128, 0, 255);
                                m_colorStack[m_colorStackIndex] = m_htmlColor;
                                m_colorStackIndex += 1;
                                return true;
                            case -976943172: // <color=purple>
                                m_htmlColor = new Color32(160, 32, 240, 255);
                                m_colorStack[m_colorStackIndex] = m_htmlColor;
                                m_colorStackIndex += 1;
                                return true;
                            case -734239628: // <color=yellow>
                                m_htmlColor = Color.yellow;
                                m_colorStack[m_colorStackIndex] = m_htmlColor;
                                m_colorStackIndex += 1;
                                return true;
                        }
                        return false;
                    case 1983971: // <cspace=xx.x>
                        value = ConvertToFloat(m_htmlTag, numSequenceStart, numSequenceEnd, numSequenceDecimalPos);
                        if (value == -9999 || value == 0) return false;

                        switch (tagUnits)
                        {
                            case TagUnits.Pixels:
                                m_cSpacing = value;
                                break;
                            case TagUnits.FontUnits:
                                m_cSpacing = value;
                                m_cSpacing *= m_fontScale * m_fontAsset.fontInfo.TabWidth / m_fontAsset.TabSize;
                                break;
                            case TagUnits.Percentage:
                                return false;
                        }
                        return true;
                    case 7513474: // </cspace>
                        m_cSpacing = 0;
                        return true;
                    case 2152041: // <mspace=xx.x>
                        value = ConvertToFloat(m_htmlTag, numSequenceStart, numSequenceEnd, numSequenceDecimalPos);
                        if (value == -9999 || value == 0) return false;

                        switch (tagUnits)
                        {
                            case TagUnits.Pixels:
                                m_monoSpacing = value;
                                break;
                            case TagUnits.FontUnits:
                                m_monoSpacing = value;
                                m_monoSpacing *= m_fontScale * m_fontAsset.fontInfo.TabWidth / m_fontAsset.TabSize;
                                break;
                            case TagUnits.Percentage:
                                return false;
                        }
                        return true;
                    case 7681544: // </mspace>
                        m_monoSpacing = 0;
                        return true;
                    case 280416: // <class="name">
                        return false;
                    case 1071884: // </color>
                        m_colorStackIndex -= 1;

                        if (m_colorStackIndex <= 0)
                        {
                            m_htmlColor = m_fontColor32;
                            m_colorStackIndex = 0;
                        }
                        else
                        {
                            m_htmlColor = m_colorStack[m_colorStackIndex - 1];
                        }

                        return true;
                    case 2068980: // <indent=10px> <indent=10em> <indent=50%>
                        value = ConvertToFloat(m_htmlTag, numSequenceStart, numSequenceEnd, numSequenceDecimalPos);
                        if (value == -9999 || value == 0) return false;

                        switch (tagUnits)
                        {
                            case TagUnits.Pixels:
                                tag_Indent = value;
                                break;
                            case TagUnits.FontUnits:
                                tag_Indent *= m_fontScale * m_fontAsset.fontInfo.TabWidth / m_fontAsset.TabSize;
                                break;
                            case TagUnits.Percentage:
                                tag_Indent = m_marginWidth * tag_Indent / 100;
                                break;
                        }

                        m_xAdvance = tag_Indent;
                        return true;
                    case 7598483: // </indent>
                        tag_Indent = 0;
                        return true;
                    case 1109386397: // <line-indent>
                        value = ConvertToFloat(m_htmlTag, numSequenceStart, numSequenceEnd, numSequenceDecimalPos);
                        if (value == -9999 || value == 0) return false;

                        switch (tagUnits)
                        {
                            case TagUnits.Pixels:
                                tag_LineIndent = value;
                                break;
                            case TagUnits.FontUnits:
                                tag_LineIndent = value;
                                tag_LineIndent *= m_fontScale * m_fontAsset.fontInfo.TabWidth / m_fontAsset.TabSize;
                                break;
                            case TagUnits.Percentage:
                                tag_LineIndent = m_marginWidth * tag_LineIndent / 100;
                                break;
                        }

                        m_xAdvance += tag_LineIndent;
                        return true;
                    case -445537194: // </line-indent>
                        tag_LineIndent = 0;
                        return true;
                    case 2246877: // <sprite=x>
                        if (m_inlineGraphics == null) m_inlineGraphics = GetComponent<InlineGraphicManager>() ?? gameObject.AddComponent<InlineGraphicManager>();

                        if (char.IsDigit(m_htmlTag[7]))
                        {
                            int index = (int)ConvertToFloat(m_htmlTag, numSequenceStart, numSequenceEnd, numSequenceDecimalPos);
                            m_spriteIndex = m_inlineGraphics.GetSpriteIndexByIndex(index);
                            if (m_spriteIndex == -1)
                                return false;
                        }
                        else
                        {
                            // Get sprite index by looking it up by name.
                            m_spriteIndex = m_inlineGraphics.GetSpriteIndexByHashCode(attribute_1.hashCode);
                            if (m_spriteIndex == -1)
                                return false;
                            //Debug.Log("Sprite name is: \"" + new string(m_htmlTag, attribute_1.startIndex, attribute_1.length) + "\" with HashCode: " + attribute_1.hashCode);
                        }
                        m_isSprite = true;
                        return true;
                    case 13526026: // <allcaps>
                        m_style |= FontStyles.UpperCase;
                        return true;
                    case 52232547: // </allcaps>
                        m_style &= ~FontStyles.UpperCase;
                        return true;
                    case 766244328: // <smallcaps>
                        m_style |= FontStyles.SmallCaps;
                        return true;
                    case -1632103439: // </smallcaps>
                        m_style &= ~FontStyles.SmallCaps;
                        m_isRecalculateScaleRequired = true;
                        return true;
                    case 2109854: // <margin=00.0> <margin=00em> <margin=50%>
                        value = ConvertToFloat(m_htmlTag, numSequenceStart, numSequenceEnd, numSequenceDecimalPos); // px
                        if (value == -9999 || value == 0) return false;

                        m_marginLeft = value;
                        switch (tagUnits)
                        {
                            case TagUnits.Pixels:
                                // Default behavior
                                break;
                            case TagUnits.FontUnits:
                                m_marginLeft *= m_fontScale * m_fontAsset.fontInfo.TabWidth / m_fontAsset.TabSize;
                                break;
                            case TagUnits.Percentage:
                                m_marginLeft = (m_marginWidth - (m_width != -1 ? m_width : 0)) * m_marginLeft / 100;
                                break;
                        }
                        m_marginLeft = m_marginLeft >= 0 ? m_marginLeft : 0;
                        m_marginRight = m_marginLeft;
                        return true;
                    case 7639357: // </margin>
                        m_marginLeft = 0;
                        m_marginRight = 0;
                        return true;
                    case 1100728678: // <margin-left=xx.x>
                        value = ConvertToFloat(m_htmlTag, numSequenceStart, numSequenceEnd, numSequenceDecimalPos); // px
                        if (value == -9999 || value == 0) return false;

                        m_marginLeft = value;
                        switch (tagUnits)
                        {
                            case TagUnits.Pixels:
                                // Default behavior
                                break;
                            case TagUnits.FontUnits:
                                m_marginLeft *= m_fontScale * m_fontAsset.fontInfo.TabWidth / m_fontAsset.TabSize;
                                break;
                            case TagUnits.Percentage:
                                m_marginLeft = (m_marginWidth - (m_width != -1 ? m_width : 0)) * m_marginLeft / 100;
                                break;
                        }
                        m_marginLeft = m_marginLeft >= 0 ? m_marginLeft : 0;
                        return true;
                    case -884817987: // <margin-right=xx.x>
                        value = ConvertToFloat(m_htmlTag, numSequenceStart, numSequenceEnd, numSequenceDecimalPos); // px
                        if (value == -9999 || value == 0) return false;

                        m_marginRight = value;
                        switch (tagUnits)
                        {
                            case TagUnits.Pixels:
                                // Default behavior
                                break;
                            case TagUnits.FontUnits:
                                m_marginRight *= m_fontScale * m_fontAsset.fontInfo.TabWidth / m_fontAsset.TabSize;
                                break;
                            case TagUnits.Percentage:
                                m_marginRight = (m_marginWidth - (m_width != -1 ? m_width : 0)) * m_marginRight / 100;
                                break;
                        }
                        m_marginRight = m_marginRight >= 0 ? m_marginRight : 0;
                        return true;
                    case 1109349752: // <line-height=xx.x>
                        value = ConvertToFloat(m_htmlTag, numSequenceStart, numSequenceEnd, numSequenceDecimalPos);
                        if (value == -9999 || value == 0) return false;

                        m_lineHeight = value;
                        switch (tagUnits)
                        {
                            case TagUnits.Pixels:
                                m_lineHeight /= m_fontScale;
                                break;
                            case TagUnits.FontUnits:
                                m_lineHeight *= m_fontAsset.fontInfo.LineHeight;
                                break;
                            case TagUnits.Percentage:
                                m_lineHeight = m_fontAsset.fontInfo.LineHeight * m_lineHeight / 100;
                                break;
                        }
                        return true;
                    case -445573839: // </line-height>
                        m_lineHeight = 0;
                        return true;
                    case 15115642: // <noparse>
                        tag_NoParsing = true;
                        return true;
                }
            }
            return false;
        }
    }
}

#endif