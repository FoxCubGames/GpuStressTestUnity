

﻿// Copyright (C) 2014 - 2015 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using UnityEngine;
using System;
using System.Collections.Generic;


namespace TMPro
{
    public struct TMP_MeshInfo
    {
        private static readonly Color32 s_DefaultColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
        private static readonly Vector3 s_DefaultNormal = new Vector3(0.0f, 0.0f, -1f);
        private static readonly Vector4 s_DefaultTangent = new Vector4(-1f, 0.0f, 0.0f, 1f);

        public Mesh mesh;
        public int vertexCount;

        public Vector3[] vertices;
        public Vector3[] normals;
        public Vector4[] tangents;

        public Vector2[] uvs0;
        public Vector2[] uvs2;
        public Vector2[] uvs4;
        public Color32[] colors32;
        public int[] triangles;


        /// <summary>
        /// Function to pre-allocate vertex attributes for a mesh of size X.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="size"></param>
        public TMP_MeshInfo (Mesh mesh, int size)
        {
            // Clear existing mesh data
            mesh.Clear();

            this.mesh = mesh;

            int sizeX4 = size * 4;
            int sizeX6 = size * 6;

            this.vertexCount = 0;

            this.vertices = new Vector3[sizeX4];
            this.uvs0 = new Vector2[sizeX4];
            this.uvs2 = new Vector2[sizeX4];
            this.uvs4 = new Vector2[sizeX4]; // SDF scale data
            this.colors32 = new Color32[sizeX4];

            this.normals = new Vector3[sizeX4];
            this.tangents = new Vector4[sizeX4];

            this.triangles = new int[sizeX6];

            int index_X6 = 0;
            int index_X4 = 0;
            while (index_X4 / 4 < size)
            {
                for (int i = 0; i < 4; i++)
                {
                    this.vertices[index_X4 + i] = Vector3.zero;
                    this.uvs0[index_X4 + i] = Vector2.zero;
                    this.uvs2[index_X4 + i] = Vector2.zero;
                    this.uvs4[index_X4 + i] = Vector2.zero;
                    this.colors32[index_X4 + i] = s_DefaultColor;
                    this.normals[index_X4 + i] = s_DefaultNormal;
                    this.tangents[index_X4 + i] = s_DefaultTangent;
                }

                this.triangles[index_X6 + 0] = index_X4 + 0;
                this.triangles[index_X6 + 1] = index_X4 + 1;
                this.triangles[index_X6 + 2] = index_X4 + 2;
                this.triangles[index_X6 + 3] = index_X4 + 2;
                this.triangles[index_X6 + 4] = index_X4 + 3;
                this.triangles[index_X6 + 5] = index_X4 + 0;

                index_X4 += 4;
                index_X6 += 6;
            }

            // Pre-assign base vertex attributes.
            this.mesh.vertices = this.vertices;
            this.mesh.normals = this.normals;
            this.mesh.tangents = this.tangents;
            this.mesh.triangles = this.triangles;
        } 


        /// <summary>
        /// Function to resized the content of MeshData and re-assign normals, tangents and triangles.
        /// </summary>
        /// <param name="meshData"></param>
        /// <param name="size"></param>
        public void ResizeMeshInfo(int size)
        {
            int size_X4 = size * 4;
            int size_X6 = size * 6;

            int previousSize = this.vertices.Length / 4;

            Array.Resize(ref this.vertices, size_X4);
            Array.Resize(ref this.normals, size_X4);
            Array.Resize(ref this.tangents, size_X4);

            Array.Resize(ref this.uvs0, size_X4);
            Array.Resize(ref this.uvs2, size_X4);
            Array.Resize(ref this.uvs4, size_X4);

            Array.Resize(ref this.colors32, size_X4);

            Array.Resize(ref this.triangles, size_X6);


            // Re-assign Normals, Tangents and Triangles
            if (size <= previousSize) return;

            for (int i = previousSize; i < size; i++)
            {
                int index_X4 = i * 4;
                int index_X6 = i * 6;

                this.normals[0 + index_X4] = new Vector3(0, 0, -1);
                this.normals[1 + index_X4] = new Vector3(0, 0, -1);
                this.normals[2 + index_X4] = new Vector3(0, 0, -1);
                this.normals[3 + index_X4] = new Vector3(0, 0, -1);

                this.tangents[0 + index_X4] = new Vector4(-1, 0, 0, 1);
                this.tangents[1 + index_X4] = new Vector4(-1, 0, 0, 1);
                this.tangents[2 + index_X4] = new Vector4(-1, 0, 0, 1);
                this.tangents[3 + index_X4] = new Vector4(-1, 0, 0, 1);

                // Setup Triangles
                this.triangles[0 + index_X6] = 0 + index_X4;
                this.triangles[1 + index_X6] = 1 + index_X4;
                this.triangles[2 + index_X6] = 2 + index_X4;
                this.triangles[3 + index_X6] = 2 + index_X4;
                this.triangles[4 + index_X6] = 3 + index_X4;
                this.triangles[5 + index_X6] = 0 + index_X4;
            }

            this.mesh.vertices = this.vertices;
            this.mesh.normals = this.normals;
            this.mesh.tangents = this.tangents;
            this.mesh.triangles = this.triangles;
        }

    }


    public enum TMP_CharacterType { Character, Sprite };


    // Structure containing information about each Character & related Mesh info for the text object.
    public struct TMP_CharacterInfo
    {   
        public char character;
        public TMP_CharacterType type;
        public float pointSize;
        
        //public short wordNumber;
        public short lineNumber;
        //public short charNumber;
        public short pageNumber;
        
        public short index; // Index of character in the input text.

        //public UIVertex[] uiVertices;
        public short vertexIndex;
        public TMP_Vertex vertex_TL;
        public TMP_Vertex vertex_BL;
        public TMP_Vertex vertex_TR;
        public TMP_Vertex vertex_BR;
        
        public Vector3 topLeft;
        public Vector3 bottomLeft;
        public Vector3 topRight;
        public Vector3 bottomRight;
        public float topLine;
        public float baseLine;
        public float bottomLine;
        
        public float xAdvance;
        public float aspectRatio;
        public float padding;
        public float scale;
        public Color32 color;
        public FontStyles style;
        public bool isVisible;
        public bool isIgnoringAlignment;
    }


    public struct TMP_Vertex
    {      
        public Vector3 position;
        public Vector2 uv;
        public Vector2 uv2;
        public Color32 color;

        public Vector3 normal;
        public Vector4 tangent;
    }


    //public struct TMP_VertexInfo
    //{      
    //    public TMP_Vertex topLeft;
    //    public TMP_Vertex bottomLeft;
    //    public TMP_Vertex topRight;
    //    public TMP_Vertex bottomRight;
    //}


    [Serializable]
    public struct VertexGradient
    {
        public Color topLeft;
        public Color topRight;
        public Color bottomLeft;
        public Color bottomRight;

        public VertexGradient (Color color)
        {
            this.topLeft = color;
            this.topRight = color;
            this.bottomLeft = color;
            this.bottomRight = color;
        }

        public VertexGradient(Color color0, Color color1, Color color2, Color color3)
        {
            this.topLeft = color0;
            this.topRight = color1;
            this.bottomLeft = color2;
            this.bottomRight = color3;
        }
    }


    [Serializable]
    public class TMP_TextInfo
    {          
        public int characterCount;
        public int spriteCount;
        public int spaceCount;
        public int wordCount;
        public int linkCount;
        public int lineCount;
        public int pageCount;

        public TMP_CharacterInfo[] characterInfo;
        public List<TMP_WordInfo> wordInfo;
        public List<TMP_LinkInfo> linkInfo;
        public TMP_LineInfo[] lineInfo;
        public TMP_PageInfo[] pageInfo;
        public TMP_MeshInfo meshInfo;
      

        // Default Constructor
        public TMP_TextInfo()
        {
            characterInfo = new TMP_CharacterInfo[0];
            wordInfo = new List<TMP_WordInfo>(32);
            linkInfo = new List<TMP_LinkInfo>(4);
            lineInfo = new TMP_LineInfo[16];
            pageInfo = new TMP_PageInfo[16];

            meshInfo = new TMP_MeshInfo();
        }


        // Method to clear the information of the text object;
        public void Clear()
        {
            characterCount = 0;
            spaceCount = 0;
            wordCount = 0;
            linkCount = 0;
            lineCount = 0;
            pageCount = 0;
            spriteCount = 0;
                 
            Array.Clear(characterInfo, 0, characterInfo.Length);
            wordInfo.Clear();
            linkInfo.Clear();
            Array.Clear(lineInfo, 0, lineInfo.Length);
            Array.Clear(pageInfo, 0, pageInfo.Length);
        }
    }


    public struct TMP_PageInfo
    {
        public int firstCharacterIndex;
        public int lastCharacterIndex;
        public float ascender;
        public float baseLine;
        public float descender;
    }


    public struct TMP_LinkInfo
    {
        public int hashCode;
        public int firstCharacterIndex;
        public int lastCharacterIndex;
        public int characterCount;
    }


    public struct TMP_WordInfo
    {
        public int firstCharacterIndex;
        public int lastCharacterIndex;
        public int characterCount;
        public float length;
        //public char[] word;
        //public string word;

        public string GetWord(TMP_CharacterInfo[] charInfo)
        {
            string word = string.Empty;
            
            for (int i = firstCharacterIndex; i < lastCharacterIndex + 1; i++)
            {
                word += charInfo[i].character;
            }

            return word;
        }
    }


    public struct TMP_SpriteInfo
    {
        public int spriteIndex; // Index of the sprite in the sprite atlas.
        public int characterIndex; // The characterInfo index which holds the key information about this sprite.
        public int vertexIndex;
    }


    public struct TMP_LineInfo
    {
        public int characterCount;
        public int spaceCount;
        public int wordCount;
        public int firstCharacterIndex;
        public int firstVisibleCharacterIndex;
        public int lastCharacterIndex;
        public int lastVisibleCharacterIndex;
        public float lineLength;
        public float lineHeight;
        public float ascender;
        public float descender;
        public float maxAdvance;
        public float width;
        public float marginLeft;
        public float marginRight;
        public float maxScale;

        public TextAlignmentOptions alignment;
        public Extents lineExtents;

    }


    //public struct SpriteInfo
    //{
    //    
    //}


    public struct Extents
    {
        public Vector2 min;
        public Vector2 max;

        public Extents(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;
        }

        public override string ToString()
        {
            string s = "Min (" + min.x.ToString("f2") + ", " + min.y.ToString("f2") + ")   Max (" + max.x.ToString("f2") + ", " + max.y.ToString("f2") + ")";           
            return s;
        }
    }


    [Serializable]
    public struct Mesh_Extents
    {
        public Vector2 min;
        public Vector2 max;
      
     
        public Mesh_Extents(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;           
        }

        public override string ToString()
        {
            string s = "Min (" + min.x.ToString("f2") + ", " + min.y.ToString("f2") + ")   Max (" + max.x.ToString("f2") + ", " + max.y.ToString("f2") + ")";
            //string s = "Center: (" + ")" + "  Extents: (" + ((max.x - min.x) / 2).ToString("f2") + "," + ((max.y - min.y) / 2).ToString("f2") + ").";
            return s;
        }
    }


    // Structure used for Word Wrapping which tracks the state of execution when the last space or carriage return character was encountered. 
    public struct WordWrapState
    {
        public int previous_WordBreak;
        public int total_CharacterCount;
        public int visible_CharacterCount;
        public int visible_SpriteCount;
        public int visible_LinkCount;
        public int firstCharacterIndex;
        public int firstVisibleCharacterIndex;
        public int lastCharacterIndex;
        public int lastVisibleCharIndex;
        public int lineNumber;
        public float maxAscender;
        public float maxDescender;
        public float xAdvance;
        public float preferredWidth;
        public float preferredHeight;
        public float maxFontScale;
        public float previousLineScale;
      
        public int wordCount;
        public FontStyles fontStyle;
        public float fontScale;
      
        public float currentFontSize;
        public float baselineOffset;
        public float lineOffset;

        public TMP_TextInfo textInfo;
        //public TMPro_CharacterInfo[] characterInfo;
        public TMP_LineInfo lineInfo;
        
        public Color32 vertexColor;
        public int colorStackIndex;
        public Extents meshExtents;
        //public Mesh_Extents lineExtents;
    }


    /// <summary>
    /// Structure used to store retrieve the name and hashcode of the font and material
    /// </summary>
    public struct TagAttribute
    {
        public int startIndex;
        public int length;
        public int hashCode;
    }


    // Structure used to track & restore state of previous line which is used to adjust linespacing.
    //public struct LineWrapState
    //{
    //    public int previous_LineBreak;
    //    public int total_CharacterCount;
    //    public int visible_CharacterCount;
    //    public int visible_SpriteCount;
    //    public float maxAscender;
    //    public float maxDescender;
    //    public float maxFontScale;

    //    //public float maxLineLength;
    //    public int wordCount;
    //    public FontStyles fontStyle;
    //    public float fontScale;
    //    public float xAdvance;
    //    public float currentFontSize;
    //    public float baselineOffset;
    //    public float lineOffset;

    //    public TMP_TextInfo textInfo;
    //    public TMP_LineInfo lineInfo;

    //    public Color32 vertexColor;
    //    public Extents meshExtents;
    //    //public Mesh_Extents lineExtents;    
    //}

}
