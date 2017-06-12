#if !UNITY_EDITOR || FCLOG
using Debug = FC.Debug;
#else
using Debug = UnityEngine.Debug;
#endif

// Copyright (C) 2014 - 2015 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms
// Beta Release 0.1.52 Beta 2b


#if UNITY_4_6 || UNITY_5

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;
using UnityEngine.EventSystems;

#pragma warning disable 0414 // Disabled a few warnings related to serialized variables not used in this script but used in the editor.

namespace TMPro
{

    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasRenderer))]	
    [AddComponentMenu("UI/TextMeshPro Text", 12)]
    //[SelectionBase]
    public partial class TextMeshProUGUI : MaskableGraphic, ILayoutElement
    {
        // Public Properties & Serializable Properties
        
        /// <summary>
        /// A string containing the text to be displayed.
        /// </summary>
        public string text
        {
            get { return m_text; }
            set { m_inputSource = TextInputSources.Text; m_havePropertiesChanged = true; m_isCalculateSizeRequired = true; isInputParsingRequired = true; MarkLayoutForRebuild(); m_text = value; /* ScheduleUpdate(); */ }
        }


        /// <summary>
        /// The TextMeshPro font asset to be assigned to this text object.
        /// </summary>
        public TextMeshProFont font
        {
            get { return m_fontAsset; }
            set { if (m_fontAsset != value) { m_fontAsset = value; LoadFontAsset(); m_havePropertiesChanged = true; m_isCalculateSizeRequired = true; MarkLayoutForRebuild();/* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// The material to be assigned to this text object. An instance of the material will be assigned to the object's renderer.
        /// </summary>
        public Material fontMaterial
        {
            // Return a new Instance of the Material if none exists. Otherwise return the current Material Instance.
            get
            {
                if (m_fontMaterial == null)
                {
                    SetFontMaterial(m_sharedMaterial);
                    return m_sharedMaterial;
                }
                return m_sharedMaterial;
            }

            // Assigning fontMaterial always returns an instance of the material.
            set { SetFontMaterial(value); m_havePropertiesChanged = true; /* ScheduleUpdate(); */  }
        }


        ///// <summary>
        ///// 
        ///// </summary>
        //public override Material material
        //{
        //    get
        //    {
        //        Debug.Log("material property called.");
        //        return m_sharedMaterial;
        //    }

        //    set
        //    {
        //        m_sharedMaterial = value;
        //    }
        //}



        /// <summary>
        /// The material to be assigned to this text object.
        /// </summary>
        public Material fontSharedMaterial
        {
			get { return m_uiRenderer.GetMaterial(); }
            set { if (m_uiRenderer.GetMaterial() != value) { m_isNewBaseMaterial = true; SetSharedFontMaterial(value); m_havePropertiesChanged = true; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// The material to be assigned to this text object.
        /// </summary>
        protected Material fontBaseMaterial
        {
            get { return m_baseMaterial; }
            set { if (m_baseMaterial != value) { SetFontBaseMaterial(value); m_havePropertiesChanged = true; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Sets the RenderQueue along with Ztest to force the text to be drawn last and on top of scene elements.
        /// </summary>
        public bool isOverlay
        {
            get { return m_isOverlay; }
            set { if (m_isOverlay != value) { m_isOverlay = value; SetShaderDepth(); m_havePropertiesChanged = true; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// This is the default vertex color assigned to each vertices. Color tags will override vertex colors unless the overrideColorTags is set.
        /// </summary>
        public new Color color
        {
            get { return m_fontColor; }
            set { if (m_fontColor != value) { m_havePropertiesChanged = true; m_fontColor = value;/* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Sets the vertex color alpha value.
        /// </summary>
        public float alpha
        {
            get { return m_fontColor.a; }
            set { Color c = m_fontColor; c.a = value; m_fontColor = c; m_havePropertiesChanged = true; }
        }


        /// <summary>
        /// Sets the vertex colors for each of the 4 vertices of the character quads.
        /// </summary>
        /// <value>The color gradient.</value>
        public VertexGradient colorGradient
        {
            get { return m_fontColorGradient;}
            set { m_havePropertiesChanged = true; m_fontColorGradient = value; }
        }


        /// <summary>
        /// Determines if Vertex Color Gradient should be used
        /// </summary>
        /// <value><c>true</c> if enable vertex gradient; otherwise, <c>false</c>.</value>
        public bool enableVertexGradient
        {
            get { return m_enableVertexGradient; }
            set { m_havePropertiesChanged = true; m_enableVertexGradient = value; }
        }


        /// <summary>
        /// Sets the color of the _FaceColor property of the assigned material. Changing face color will result in an instance of the material.
        /// </summary>
        public Color32 faceColor
        {
            get { return m_faceColor; }
            set { /* if (m_faceColor.Compare(value) == false) { */ SetFaceColor(value); m_havePropertiesChanged = true; m_faceColor = value; /* ScheduleUpdate(); */ }
        }


        /// <summary>
        /// Sets the color of the _OutlineColor property of the assigned material. Changing outline color will result in an instance of the material.
        /// </summary>
        public Color32 outlineColor
        {
            get { return m_outlineColor; }
            set { /* if (m_outlineColor.Compare(value) == false) { */ SetOutlineColor(value); m_havePropertiesChanged = true; m_outlineColor = value; /* ScheduleUpdate(); */ }
        }


        /// <summary>
        /// Sets the thickness of the outline of the font. Setting this value will result in an instance of the material.
        /// </summary>
        public float outlineWidth
        {
            get { return m_outlineWidth; }
            set { SetOutlineThickness(value); m_havePropertiesChanged = true; m_outlineWidth = value; /* ScheduleUpdate(); */ }
        }


        /// <summary>
        /// The size of the font.
        /// </summary>
        public float fontSize
        {
            get { return m_fontSize; }
            set { m_havePropertiesChanged = true; m_isCalculateSizeRequired = true; MarkLayoutForRebuild(); m_fontSize = value; if (!m_enableAutoSizing) m_fontSizeBase = m_fontSize; }
        }


        /// <summary>
        /// The style of the text
        /// </summary>
        public FontStyles fontStyle
        {
            get { return m_fontStyle; }
            set { m_fontStyle = value; m_havePropertiesChanged = true; checkPaddingRequired = true; }
        }


        /// <summary>
        /// The amount of additional spacing between characters.
        /// </summary>
        public float characterSpacing
        {
            get { return m_characterSpacing; }
            set { if (m_characterSpacing != value) { m_havePropertiesChanged = true; m_isCalculateSizeRequired = true; MarkLayoutForRebuild(); m_characterSpacing = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// The amount of additional spacing to add between each lines of text.
        /// </summary>
        public float lineSpacing
        {
            get { return m_lineSpacing; }
            set { if (m_lineSpacing != value) { m_havePropertiesChanged = true; m_isCalculateSizeRequired = true; MarkLayoutForRebuild(); m_lineSpacing = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// The amount of additional spacing to add between each lines of text.
        /// </summary>
        public float paragraphSpacing
        {
            get { return m_paragraphSpacing; }
            set { if (m_paragraphSpacing != value) { m_havePropertiesChanged = true; m_isCalculateSizeRequired = true; MarkLayoutForRebuild(); m_paragraphSpacing = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Enables or Disables Rich Text Tags
        /// </summary>
        public bool richText
        {
            get { return m_isRichText; }
            set { m_isRichText = value; m_havePropertiesChanged = true; m_isCalculateSizeRequired = true; MarkLayoutForRebuild(); isInputParsingRequired = true; }
        }


        /// <summary>
        /// Enables or Disables parsing of CTRL characters in input text.
        /// </summary>
        public bool parseCtrlCharacters
        {
            get { return m_parseCtrlCharacters; }
            set { m_parseCtrlCharacters = value; m_havePropertiesChanged = true; m_isCalculateSizeRequired = true; MarkLayoutForRebuild(); isInputParsingRequired = true; }
        }


        /// <summary>
        /// Controls the Text Overflow Mode
        /// </summary>
        public TextOverflowModes OverflowMode
        {
            get { return m_overflowMode; }
            set { m_overflowMode = value; m_havePropertiesChanged = true; m_isRecalculateScaleRequired = true; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Texture texture;
        public override Texture mainTexture
        {
            get
            {
                if ((UnityEngine.Object)this.texture == (UnityEngine.Object)null)
                    return (Texture)Graphic.s_WhiteTexture;
                else
                    return this.texture;
            }
        }


        /// <summary>
        /// Contains the bounds of the text object.
        /// </summary>
        public Bounds bounds
        {
            get { if (m_mesh != null) return m_bounds; return new Bounds(); }
            //set { if (_meshExtents != value) havePropertiesChanged = true; _meshExtents = value; }
        }


        //public Mesh mesh
        //{
        //    get { return m_textInfo.meshInfo.uiVertices; }
        //}

        
        /// <summary>
        /// Determines the anchor position of the text object.  
        /// </summary>
        //public AnchorPositions anchor
        //{
        //    get { return m_anchor; }
        //    set { if (m_anchor != value) { havePropertiesChanged = true; m_anchor = value; /* ScheduleUpdate(); */ } }
        //}

              
        /// <summary>
        /// Text alignment options
        /// </summary>
        public TextAlignmentOptions alignment
        {
            get { return m_textAlignment; } 
            set { if (m_textAlignment != value) { m_havePropertiesChanged = true; m_textAlignment = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Determines if kerning is enabled or disabled.
        /// </summary>
        public bool enableKerning
        {
            get { return m_enableKerning; }
            set { if (m_enableKerning != value) { m_havePropertiesChanged = true; m_isCalculateSizeRequired = true; MarkLayoutForRebuild(); m_enableKerning = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Anchor dampening prevents the anchor position from being adjusted unless the positional change exceeds about 40% of the width of the underline character. This essentially stabilizes the anchor position.
        /// </summary>
        //public bool anchorDampening
        //{
        //    get { return m_anchorDampening; }
        //    set { if (m_anchorDampening != value) { havePropertiesChanged = true; m_anchorDampening = value; /* ScheduleUpdate(); */ } }
        //}


        /// <summary>
        /// This overrides the color tags forcing the vertex colors to be the default font color.
        /// </summary>
        public bool overrideColorTags
        {
            get { return m_overrideHtmlColors; }
            set { if (m_overrideHtmlColors != value) { m_havePropertiesChanged = true; m_overrideHtmlColors = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Adds extra padding around each character. This may be necessary when the displayed text is very small to prevent clipping.
        /// </summary>
        public bool extraPadding
        {
            get { return m_enableExtraPadding; }
            set { if (m_enableExtraPadding != value) { m_havePropertiesChanged = true; checkPaddingRequired = true; m_enableExtraPadding = value; m_isCalculateSizeRequired = true; MarkLayoutForRebuild();/* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Controls whether or not word wrapping is applied. When disabled, the text will be displayed on a single line.
        /// </summary>
        public bool enableWordWrapping
        {
            get { return m_enableWordWrapping; }
            set { if (m_enableWordWrapping != value) { m_havePropertiesChanged = true; isInputParsingRequired = true; m_isRecalculateScaleRequired = true; m_enableWordWrapping = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Controls how the face and outline textures will be applied to the text object.
        /// </summary>
        public TextureMappingOptions horizontalMapping
        {
            get { return m_horizontalMapping; }
            set { if (m_horizontalMapping != value) { m_havePropertiesChanged = true; m_horizontalMapping = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Controls how the face and outline textures will be applied to the text object.
        /// </summary>
        public TextureMappingOptions verticalMapping
        {
            get { return m_verticalMapping; }
            set { if (m_verticalMapping != value) { m_havePropertiesChanged = true; m_verticalMapping = value; /* ScheduleUpdate(); */ } }
        }

        /// <summary>
        /// Forces objects that are not visible to get refreshed.
        /// </summary>
        public bool ignoreVisibility
        {
            get { return m_ignoreCulling; }
            set { if (m_ignoreCulling != value) { m_havePropertiesChanged = true; m_ignoreCulling = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Sets Perspective Correction to Zero for Orthographic Camera mode & 0.875f for Perspective Camera mode.
        /// </summary>
        public bool isOrthographic
        {
            get { return m_isOrthographic; }
            set { m_havePropertiesChanged = true; m_isOrthographic = value; /* ScheduleUpdate(); */ }
        }


        /// <summary>
        /// Sets the culling on the shaders. Note changing this value will result in an instance of the material.
        /// </summary>
        public bool enableCulling
        {
            get { return m_isCullingEnabled; }
            set { m_isCullingEnabled = value; SetCulling(); m_havePropertiesChanged = true; }
        }


        /// <summary>
        /// Sets the Renderer's sorting Layer ID
        /// </summary>
        //public int sortingLayerID
        //{
        //    get { return m_sortingLayerID; }
        //    set { m_sortingLayerID = value; /*m_renderer.sortingLayerID = value;*/ }
        //}


        /// <summary>
        /// Sets the Renderer's sorting order within the assigned layer.
        /// </summary>
        //public int sortingOrder
        //{
        //    get { return m_sortingOrder; }
        //    set { m_sortingOrder = value; /*m_renderer.sortingOrder = value;*/ }
        //}


        /// <summary>
        /// Determines if the Mesh will be uploaded.
        /// </summary>
        public TextRenderFlags renderMode
        {
            get { return m_renderMode; }
            set { m_renderMode = value; m_havePropertiesChanged = true; }
        }

        /// <summary>
        /// Property tracking if any of the text properties have changed. Flag is set before the text is regenerated.
        /// </summary>
        public bool havePropertiesChanged
        {
            get { return m_havePropertiesChanged; }
            set { m_havePropertiesChanged = value; }
        }


        ///// <summary>
        ///// Property tracking if the text object has been re-generated since last frame.
        ///// </summary>
        //public bool hasChanged
        //{
        //    get { return m_hasChanged; }
        //}


        /// <summary>
        /// <para>Sets the margin for the text inside the Rect Transform.</para>
        /// <para>Vector4 (left, top, right, bottom);</para>
        /// </summary>
        public Vector4 margin
        {
            get { return m_margin; }
            set { /* if (m_margin != value) */ { m_margin = value; /* Debug.Log("Margin is " + margin); ComputeMarginSize();*/ m_havePropertiesChanged = true; m_marginsHaveChanged = true; } }
        }      



        /// <summary>
        /// Allows to control how many characters are visible from the input.
        /// </summary>
        public int maxVisibleCharacters
        {
            get { return m_maxVisibleCharacters; }
            set { if (m_maxVisibleCharacters != value) { m_havePropertiesChanged = true; m_maxVisibleCharacters = value; } }
        }

        /// <summary>
        /// Allows to control how many words are visible from the input.
        /// </summary>
        public int maxVisibleWords
        {
            get { return m_maxVisibleWords; }
            set { if (m_maxVisibleWords != value) { m_havePropertiesChanged = true; m_maxVisibleWords = value; } }
        }

        /// <summary>
        /// Allows control over how many lines of text are displayed.
        /// </summary>
        public int maxVisibleLines
        {
            get { return m_maxVisibleLines; }
            set { if (m_maxVisibleLines != value) { m_havePropertiesChanged = true; isInputParsingRequired = true; m_maxVisibleLines = value; } }
        }


        /// <summary>
        /// Controls which page of text is shown
        /// </summary>
        public int pageToDisplay
        {
            get { return m_pageToDisplay; }
            set { m_havePropertiesChanged = true; m_pageToDisplay = value; }
        }


        /// <summary>
        /// Returns are reference to the RectTransform
        /// </summary>
        public new RectTransform rectTransform
        {
            get
            {
                if (m_rectTransform == null)
                    m_rectTransform = GetComponent<RectTransform>();
                return m_rectTransform;
            }
        }


        //public TMPro_TextMetrics metrics
        //{
        //    get { return m_textMetrics; }
        //}


        //public int characterCount
        //{
        //    get { return m_textInfo.characterCount; }
        //}

        //public int lineCount
        //{
        //    get { return m_textInfo.lineCount; }
        //}


        //public Vector2[] spacePositions
        //{
        //    get { return m_spacePositions; }
        //}


        public bool enableAutoSizing
        {
            get { return m_enableAutoSizing; }
            set { m_enableAutoSizing = value; }
        }

        public float fontSizeMin
        {
            get { return m_fontSizeMin; }
            set { m_fontSizeMin = value; }
        }

        public float fontSizeMax
        {
            get { return m_fontSizeMax; }
            set { m_fontSizeMax = value; }
        }


        // ILayoutElement Implementation
        public float flexibleHeight { get { return m_flexibleHeight; } }
        private float m_flexibleHeight;

        public float flexibleWidth { get { return m_flexibleWidth; } }
        private float m_flexibleWidth;

        public float minHeight { get { return m_minHeight; } }
        private float m_minHeight;

        public float minWidth { get { return m_minWidth; } }
        private float m_minWidth;

        public float preferredWidth { get { return m_preferredWidth == 9999 ? m_renderedWidth : m_preferredWidth; } }
        
        private float m_preferredWidth = 9999;

        public float preferredHeight { get { return m_preferredHeight == 9999 ? m_renderedHeight : m_preferredHeight; } }
        private float m_preferredHeight = 9999;

        // Used to track actual rendered size of the text object.
        private float m_renderedWidth;
        private float m_renderedHeight;

        public int layoutPriority { get { return m_layoutPriority; } }
        private int m_layoutPriority = 0;

        private bool m_isRebuildingLayout = false;
        private bool m_isLayoutDirty = false;
        private enum AutoLayoutPhase { Horizontal, Vertical };
        private AutoLayoutPhase m_LayoutPhase;

        //private TextOverflowModes m_currentOverflowMode;
        private bool m_currentAutoSizeMode;
      

        private float GetPreferredWidth()
        {
            TextOverflowModes currentOverflowMode = m_overflowMode;
            m_overflowMode = TextOverflowModes.Overflow;
            
            m_renderMode = TextRenderFlags.GetPreferredSizes;

            GenerateTextMesh();

            m_renderMode = TextRenderFlags.Render;
            m_overflowMode = currentOverflowMode;

            Debug.Log("GetPreferredWidth() Called. Returning width of " + m_preferredWidth);

            return m_preferredWidth;
        }


        private float GetPreferredHeight()
        {
            TextOverflowModes currentOverflowMode = m_overflowMode;
            m_overflowMode = TextOverflowModes.Overflow;

            m_renderMode = TextRenderFlags.GetPreferredSizes;

            GenerateTextMesh();

            m_renderMode = TextRenderFlags.Render;
            m_overflowMode = currentOverflowMode;

            Debug.Log("GetPreferredHeight() Called. Returning height of " + m_preferredHeight);

            return m_preferredHeight;
        }



        public void CalculateLayoutInputHorizontal()
        {
            //Debug.Log("CalculateLayoutHorizontal() at Frame: " + Time.frameCount); // called on Object ID " + GetInstanceID());
            
            // Check if object is active
            if (!this.gameObject.activeInHierarchy)
                return;

            IsRectTransformDriven = true;
            // Get a Reference to the Driving Layout Controller
            //if ((m_layoutController as UIBehaviour) == null) 
            //{
            //    m_layoutController = GetComponent(typeof(ILayoutController)) as ILayoutController ?? (transform.parent != null ? transform.parent.GetComponent(typeof(ILayoutController)) as ILayoutController : null);               
            //    if (m_layoutController != null)
            //        IsRectTransformDriven = true;
            //    else
            //    {
            //        IsRectTransformDriven = false;
            //        return;
            //    }
            //}

            m_currentAutoSizeMode = m_enableAutoSizing;

            //if (m_isCalculateSizeRequired || m_rectTransform.hasChanged)
            //{

                m_LayoutPhase = AutoLayoutPhase.Horizontal;
                m_isRebuildingLayout = true;

                m_minWidth = 0;
                m_flexibleWidth = 0;

                m_renderMode = TextRenderFlags.GetPreferredSizes; // Set Text to not Render and exit early once we have new width values.
                
                if (m_enableAutoSizing)
                {
                    m_fontSize = m_fontSizeMax;   
                }
                
                // Set Margins to Infinity
                m_marginWidth = Mathf.Infinity;
                m_marginHeight = Mathf.Infinity;
                
                if (isInputParsingRequired || m_isTextTruncated)
                    ParseInputText();

                GenerateTextMesh();

                m_renderMode = TextRenderFlags.Render;

                m_preferredWidth = m_renderedWidth; // (int)(m_renderedWidth + 1f);

                ComputeMarginSize();

                //Debug.Log("Preferred Width: " + m_preferredWidth + "  Margin Width: " + m_marginWidth + "  Preferred Height: " + m_preferredHeight + "  Margin Height: " + m_marginHeight + "  Rendered Width: " + m_renderedWidth + "  Height: " + m_renderedHeight + "  RectTransform Width: " + m_rectTransform.rect);

                m_isLayoutDirty = true;
            //}

        }


        public void CalculateLayoutInputVertical()
        {
            // Check if object is active
            if (!this.gameObject.activeInHierarchy) // || IsRectTransformDriven == false)
                return;

            IsRectTransformDriven = true;
            //Debug.Log("CalculateLayoutInputVertical() at Frame: " + Time.frameCount); // called on Object ID " + GetInstanceID());

            //if (m_isCalculateSizeRequired || m_rectTransform.hasChanged)
            //{

                m_LayoutPhase = AutoLayoutPhase.Vertical;
                m_isRebuildingLayout = true;

                m_minHeight = 0;
                m_flexibleHeight = 0;

                m_renderMode = TextRenderFlags.GetPreferredSizes;

                if (m_enableAutoSizing)
                {
                    m_currentAutoSizeMode = true;
                    m_enableAutoSizing = false;
                }
                
                m_marginHeight = Mathf.Infinity;
                
                GenerateTextMesh();

                m_enableAutoSizing = m_currentAutoSizeMode;

                m_renderMode = TextRenderFlags.Render;

                ComputeMarginSize();

                m_preferredHeight = m_renderedHeight; // (int)(m_renderedHeight + 1f);

                //Debug.Log("Preferred Height: " + m_preferredHeight + "  Margin Height: " + m_marginHeight + "  Preferred Width: " + m_preferredWidth + "  Margin Width: " + m_marginWidth + "  Rendered Width: " + m_renderedWidth + "  Height: " + m_renderedHeight + "  RectTransform Width: " + m_rectTransform.rect);

                m_isLayoutDirty = true;
            //}
 
            m_isCalculateSizeRequired = false;    
        }


        // ICanvasElement
        //public void Rebuild(CanvasUpdate update)
        //{
        //    Debug.Log("Rebuild()");
        //    LayoutRebuilder.MarkLayoutForRebuild(m_rectTransform);
        //}



        //public bool IsDestroyed()
        //{
        //    return true;
        //}


        //public override bool Raycast(Vector2 sp, Camera eventCamera)
        //{
        //    //Debug.Log("Raycast Event. ScreenPoint: " + sp);
        //    return base.Raycast(sp, eventCamera);
        //}


        // MASKING RELATED PROPERTIES
        /// <summary>
        /// Sets the masking offset from the bounds of the object
        /// </summary>
        public Vector4 maskOffset
        {
            get { return m_maskOffset; }
            set { m_maskOffset = value; UpdateMask(); m_havePropertiesChanged = true; }
        }


        //public override Material defaultMaterial 
        //{
        //    get { Debug.Log("Default Material called."); return m_sharedMaterial; }
        //}



        //protected override void OnCanvasHierarchyChanged()
        //{
        //    //Debug.Log("OnCanvasHierarchyChanged...");
        //}


        /// <summary>
        /// Method called when the state of a parent changes.
        /// </summary>
        public override void RecalculateClipping()
        {
            //Debug.Log("***** RecalculateClipping() *****");

            base.RecalculateClipping();
        }


        /// <summary>
        /// Method called when Stencil Mask needs to be updated on this element and parents.
        /// </summary>
        public override void RecalculateMasking()
        {
            //Debug.Log("***** RecalculateMasking() *****");

            if (m_fontAsset == null) return;

            //if (m_canvas == null) m_canvas = GetComponentInParent<Canvas>();


            if (!m_isAwake)
                return;

            m_stencilID = MaterialManager.GetStencilID(gameObject);
            //m_stencilID = MaskUtilities.GetStencilDepth(this.transform, m_canvas.transform);
            //Debug.Log("Stencil ID: " + m_stencilID + "  Stencil ID (2): " + MaskUtilities.GetStencilDepth(this.transform, m_canvas.transform));

            if (m_stencilID == 0)
            {
                if (m_maskingMaterial != null)
                {
                    MaterialManager.ReleaseStencilMaterial(m_maskingMaterial);
                    m_maskingMaterial = null;

                    m_sharedMaterial = m_baseMaterial;
                }
                else if (m_fontMaterial != null)
                    m_sharedMaterial = MaterialManager.SetStencil(m_fontMaterial, 0);
                else
                    m_sharedMaterial = m_baseMaterial;
            }
            else
            {
                ShaderUtilities.GetShaderPropertyIDs();

                if (m_fontMaterial != null)
                {
                    m_sharedMaterial = MaterialManager.SetStencil(m_fontMaterial, m_stencilID);
                }
                else if (m_maskingMaterial == null)
                {
                   m_maskingMaterial = MaterialManager.GetStencilMaterial(m_baseMaterial, m_stencilID);
                   m_sharedMaterial = m_maskingMaterial;
                }
                else if (m_maskingMaterial.GetInt(ShaderUtilities.ID_StencilID) != m_stencilID || m_isNewBaseMaterial)
                {
                    MaterialManager.ReleaseStencilMaterial(m_maskingMaterial);
                    
                    m_maskingMaterial = MaterialManager.GetStencilMaterial(m_baseMaterial, m_stencilID);
                    m_sharedMaterial = m_maskingMaterial;
                }


                if (m_isMaskingEnabled)
                    EnableMasking();

                //Debug.Log("Masking Enabled. Assigning " + m_maskingMaterial.name + " with ID " + m_maskingMaterial.GetInstanceID());
            }

            m_uiRenderer.SetMaterial(m_sharedMaterial, m_sharedMaterial.mainTexture);
            m_padding = ShaderUtilities.GetPadding(m_sharedMaterial, m_enableExtraPadding, m_isUsingBold);
            //m_alignmentPadding = ShaderUtilities.GetFontExtent(m_sharedMaterial);
           
        }


        //public override void SetVerticesDirty()
        //{
        //    //Debug.Log("Set Vertices Dirty called.");
        //    //if (m_fontColor != base.color) this.color = base.color;
        //    base.SetVerticesDirty();
        //}

        protected override void UpdateGeometry()
        {
            //Debug.Log("UpdateGeometry");
            //base.UpdateGeometry();
        }


        protected override void UpdateMaterial()
        {
            //Debug.Log("UpdateMaterial called.");
        //    base.UpdateMaterial();
        }


        /*
        /// <summary>
        /// Sets the mask type 
        /// </summary>
        public MaskingTypes mask
        {
            get { return m_mask; }
            set { m_mask = value; havePropertiesChanged = true; isMaskUpdateRequired = true; }
        }

        /// <summary>
        /// Set the masking offset mode (as percentage or pixels)
        /// </summary>
        public MaskingOffsetMode maskOffsetMode
        {
            get { return m_maskOffsetMode; }
            set { m_maskOffsetMode = value; havePropertiesChanged = true; isMaskUpdateRequired = true; }
        }
        */



        /*
        /// <summary>
        /// Sets the softness of the mask
        /// </summary>
        public Vector2 maskSoftness
        {
            get { return m_maskSoftness; }
            set { m_maskSoftness = value; havePropertiesChanged = true; isMaskUpdateRequired = true; }
        }

        /// <summary>
        /// Allows to move / offset the mesh vertices by a set amount
        /// </summary>
        public Vector2 vertexOffset
        {
            get { return m_vertexOffset; }
            set { m_vertexOffset = value; havePropertiesChanged = true; isMaskUpdateRequired = true; }
        }
        */

        public TMP_TextInfo textInfo
        {
            get { return m_textInfo; }
        }


        //public TMPro_MeshInfo meshInfo
        //{
        //    get { return m_meshInfo; }
        //}

        
        public Mesh mesh
        {
            get { return m_mesh; }
        }


        public new CanvasRenderer canvasRenderer
        {
            get { return m_uiRenderer; }
        }


        public InlineGraphicManager inlineGraphicManager
        {
            get { return m_inlineGraphics; }
        }


        //public TMP_CharacterInfo[] characterInfo
        //{
        //    get { return m_textInfo.characterInfo; }
        //
        //}

  

        /// <summary>
        /// Function to be used to force recomputing of character padding when Shader / Material properties have been changed via script.
        /// </summary>
        public void UpdateMeshPadding()
        {
            m_padding = ShaderUtilities.GetPadding(new Material[] {m_uiRenderer.GetMaterial()}, m_enableExtraPadding, m_isUsingBold);
            m_havePropertiesChanged = true;
            /* ScheduleUpdate(); */
        }


        /// <summary>
        /// Function to force regeneration of the mesh before its normal process time. This is useful when changes to the text object properties need to be applied immediately.
        /// </summary>
        public void ForceMeshUpdate()
        {
            //Debug.Log("ForceMeshUpdate() called.");
            //havePropertiesChanged = true;
            OnPreRenderCanvas();
        }


        public void UpdateFontAsset()
        {        
            LoadFontAsset();
        }


        /// <summary>
        /// Function used to evaluate the length of a text string.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public TMP_TextInfo GetTextInfo(string text)
        {
            //TextInfo temp_textInfo = new TextInfo();

            StringToCharArray(text, ref m_char_buffer);
            m_renderMode = TextRenderFlags.DontRender;
           
            GenerateTextMesh();

            m_renderMode = TextRenderFlags.Render;
            //this.text = string.Empty;

            return this.textInfo;
        }


        //public Vector2[] SetTextWithSpaces(string text, int numPositions)
        //{
        //    m_spacePositions = new Vector2[numPositions];

        //    this.text = text;

        //    return m_spacePositions;
        //}


        /// <summary>
        /// <para>Formatted string containing a pattern and a value representing the text to be rendered.</para>
        /// <para>ex. TextMeshPro.SetText ("Number is {0:1}.", 5.56f);</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text">String containing the pattern."</param>
        /// <param name="arg0">Value is a float.</param>
        public void SetText (string text, float arg0)
        {
            SetText(text, arg0, 255, 255);
        }

        /// <summary>
        /// <para>Formatted string containing a pattern and a value representing the text to be rendered.</para>
        /// <para>ex. TextMeshPro.SetText ("First number is {0} and second is {1:2}.", 10, 5.756f);</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text">String containing the pattern."</param>
        /// <param name="arg0">Value is a float.</param>
        /// <param name="arg1">Value is a float.</param>
        public void SetText (string text, float arg0, float arg1)
        {
            SetText(text, arg0, arg1, 255);
        }

        /// <summary>
        /// <para>Formatted string containing a pattern and a value representing the text to be rendered.</para>
        /// <para>ex. TextMeshPro.SetText ("A = {0}, B = {1} and C = {2}.", 2, 5, 7);</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text">String containing the pattern."</param>
        /// <param name="arg0">Value is a float.</param>
        /// <param name="arg1">Value is a float.</param>
        /// <param name="arg2">Value is a float.</param>
        public void SetText (string text, float arg0, float arg1, float arg2)
        {
            // Early out if nothing has been changed from previous invocation.
            if (text == old_text && arg0 == old_arg0 && arg1 == old_arg1 && arg2 == old_arg2)
            {
                return;
            }

            old_text = text;
            old_arg1 = 255;
            old_arg2 = 255;

            int decimalPrecision = 0;
            int index = 0;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (c == 123) // '{'
                {
                    // Check if user is requesting some decimal precision. Format is {0:2}
                    if (text[i + 2] == 58) // ':'
                    {
                        decimalPrecision = text[i + 3] - 48;
                    }

                    switch (text[i + 1] - 48)
                    {
                        case 0: // 1st Arg                        
                            old_arg0 = arg0;
                            AddFloatToCharArray(arg0, ref index, decimalPrecision);
                            break;                        
                        case 1: // 2nd Arg
                            old_arg1 = arg1;
                            AddFloatToCharArray(arg1, ref index, decimalPrecision);
                            break;                       
                        case 2: // 3rd Arg
                            old_arg2 = arg2;
                            AddFloatToCharArray(arg2, ref index, decimalPrecision);
                            break;                       
                    }

                    if (text[i + 2] == 58)
                        i += 4;
                    else
                        i += 2;

                    continue;
                }
                m_input_CharArray[index] = c;
                index += 1;
            }

            m_input_CharArray[index] = (char)0;
            m_charArray_Length = index; // Set the length to where this '0' termination is.

#if UNITY_EDITOR
            // Create new string to be displayed in the Input Text Box of the Editor Panel.
            m_text = new string(m_input_CharArray, 0, index);           
#endif

            m_inputSource = TextInputSources.SetText;
            isInputParsingRequired = true;
            m_havePropertiesChanged = true;
            /* ScheduleUpdate(); */
        }




        /// <summary>
        /// Character array containing the text to be displayed.
        /// </summary>
        /// <param name="charArray"></param>
        public void SetCharArray(char[] charArray)
        {
            if (charArray == null || charArray.Length == 0)
                return;

            // Check to make sure chars_buffer is large enough to hold the content of the string.
            if (m_char_buffer.Length <= charArray.Length)
            {
                int newSize = Mathf.NextPowerOfTwo(charArray.Length + 1);
                m_char_buffer = new int[newSize];
            }

            int index = 0;

            for (int i = 0; i < charArray.Length; i++)
            {
                if (charArray[i] == 92 && i < charArray.Length - 1)
                {
                    switch ((int)charArray[i + 1])
                    {
                        case 110: // \n LineFeed
                            m_char_buffer[index] = (char)10;
                            i += 1;
                            index += 1;
                            continue;
                        case 114: // \r LineFeed
                            m_char_buffer[index] = (char)13;
                            i += 1;
                            index += 1;
                            continue;
                        case 116: // \t Tab
                            m_char_buffer[index] = (char)9;
                            i += 1;
                            index += 1;
                            continue;
                    }
                }

                m_char_buffer[index] = charArray[i];
                index += 1;
            }
            m_char_buffer[index] = (char)0;

            m_inputSource = TextInputSources.SetCharArray;
            m_havePropertiesChanged = true;
            isInputParsingRequired = true;
        }

    }
}

#endif