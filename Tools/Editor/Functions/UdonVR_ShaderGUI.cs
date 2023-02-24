using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

using UdonVR.EditorUtility;

namespace UdonVR.EditorUtility.ShaderGUI
{
    public static class UdonVR_ShaderGUI
    {
        #region Cull Blend Specular
        public enum CullMode
        {
            Off, Front, Back
        };

        public enum BlendMode
        {
            Opaque, Cutout, Fade
        };

        public struct CullBlendSpecularData
        {
            public UdonVR_ShaderGUI.BlendMode? blendMode;
            public UdonVR_ShaderGUI.CullMode? cullMode;
            public bool? specularHighlights;
        };

        /// <summary>
        /// Displays the necessary fields in the inspector.
        /// </summary>
        /// <param name="targetMat"> Reference to the Material currently selected. </param>
        /// <returns> CullBlendSpecularData </returns>
        public static CullBlendSpecularData ShowCullBlendSpecular(Material targetMat)
        {
            CullBlendSpecularData data = new CullBlendSpecularData();

            GUILayout.BeginVertical(EditorStyles.helpBox);
            if (targetMat.HasProperty("_BlendMode"))
            {
                data.blendMode = (UdonVR_ShaderGUI.BlendMode)EditorGUILayout.EnumPopup(new GUIContent("Blend Mode"), (UdonVR_ShaderGUI.BlendMode)targetMat.GetInt("_BlendMode"));
            }
            if (targetMat.HasProperty("_Cull"))
            {
                data.cullMode = (UdonVR_ShaderGUI.CullMode)EditorGUILayout.EnumPopup(new GUIContent("Cull"), (UdonVR_ShaderGUI.CullMode)targetMat.GetInt("_Cull"));
            }
            if (targetMat.HasProperty("_SpecularHighlights"))
            {
                data.specularHighlights = UdonVR_GUI.ToggleButton(new GUIContent("Specular Highlights"), System.Convert.ToBoolean(targetMat.GetInt("_SpecularHighlights")));
            }
            GUILayout.EndVertical();

            return data;
        }

        /// <summary>
        /// Applies the value changes to the Material.
        /// </summary>
        /// <param name="targetMat"> Reference to the Material currently selected. </param>
        /// <param name="data"> Reference to the struct containing the values. </param>
        public static void SaveCullBlendSpecular(Material targetMat, CullBlendSpecularData data)
        {
            if (data.blendMode.HasValue)
            {
                targetMat.SetInt("_BlendMode", (int)data.blendMode.Value);
                #region Blend Mode Keywords & Tags
                switch (data.blendMode.Value)
                {
                    case UdonVR_ShaderGUI.BlendMode.Opaque:
                        targetMat.SetOverrideTag("RenderType", "");
                        targetMat.SetOverrideTag("Queue", "Geometry");
                        targetMat.renderQueue = (int)RenderQueue.Geometry;
                        targetMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        targetMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                        targetMat.SetInt("_ZWrite", 1);
                        targetMat.DisableKeyword("_ALPHACUTOUT");
                        targetMat.DisableKeyword("_ALPHAFADE");

                        targetMat.DisableKeyword("_ALPHATEST_ON");
                        targetMat.DisableKeyword("_ALPHABLEND_ON");
                        targetMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        break;

                    case UdonVR_ShaderGUI.BlendMode.Cutout:
                        targetMat.SetOverrideTag("RenderType", "TransparentCutout");
                        targetMat.SetOverrideTag("Queue", "AlphaTest");
                        targetMat.renderQueue = (int)RenderQueue.Transparent; // should be AlphaTest but that would require lighting passes to be added to the shader
                        targetMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        targetMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                        targetMat.SetInt("_ZWrite", 1);
                        targetMat.EnableKeyword("_ALPHACUTOUT");
                        targetMat.DisableKeyword("_ALPHAFADE");

                        targetMat.EnableKeyword("_ALPHATEST_ON");
                        targetMat.DisableKeyword("_ALPHABLEND_ON");
                        targetMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        break;

                    case UdonVR_ShaderGUI.BlendMode.Fade:
                        targetMat.SetOverrideTag("RenderType", "Transparent");
                        targetMat.SetOverrideTag("Queue", "Transparent");
                        targetMat.renderQueue = (int)RenderQueue.Transparent;
                        targetMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        targetMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        targetMat.SetInt("_ZWrite", 0);
                        targetMat.DisableKeyword("_ALPHACUTOUT");
                        targetMat.EnableKeyword("_ALPHAFADE");

                        targetMat.DisableKeyword("_ALPHATEST_ON");
                        targetMat.EnableKeyword("_ALPHABLEND_ON");
                        targetMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        break;
                }
                #endregion
            }
            if (data.cullMode.HasValue)
            {
                targetMat.SetInt("_Cull", (int)data.cullMode.Value);
            }
            if (data.specularHighlights.HasValue)
            {
                targetMat.SetInt("_SpecularHighlights", (int)System.Convert.ToSingle(data.specularHighlights.Value));
                #region Specular Highlights Keywords
                if (data.specularHighlights.Value)
                {
                    targetMat.DisableKeyword("_SPECULARHIGHLIGHTS_OFF");
                }
                else
                {
                    targetMat.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
                }
                #endregion
            }
        }
        #endregion

        #region Glossiness Metallic AlphaCut AlphaFade
        public static float AlphaCutout(Material targetMat)
        {
            float _alphaCutout = targetMat.GetFloat("_AlphaCutout");

            if (targetMat.HasProperty("_BlendMode"))
            {
                if (targetMat.IsKeywordEnabled("_ALPHACUTOUT"))
                {
                    _alphaCutout = EditorGUILayout.Slider(new GUIContent("Alpha Cutout"), _alphaCutout, 0, 1.01f);
                }
            }
            else
            {
                _alphaCutout = EditorGUILayout.Slider(new GUIContent("Alpha Cutout"), _alphaCutout, 0, 1.01f);
            }

            return _alphaCutout;
        }

        public static float AlphaFade(Material targetMat)
        {
            float _alphaFade = targetMat.GetFloat("_AlphaFade");

            if (targetMat.HasProperty("_BlendMode"))
            {
                if (targetMat.IsKeywordEnabled("_ALPHAFADE"))
                {
                    _alphaFade = EditorGUILayout.Slider(new GUIContent("Alpha Fade"), _alphaFade, 0, 1.0f);
                }
            }
            else
            {
                _alphaFade = EditorGUILayout.Slider(new GUIContent("Alpha Fade"), _alphaFade, 0, 1.0f);
            }

            return _alphaFade;
        }

        public struct GlossMetalAlphaData
        {
            public float? glossiness;
            public float? metallic;
            public float? alphaCutout;
            public float? alphaFade;
        };

        /// <summary>
        /// Shows the necessary fields in the inspector.
        /// </summary>
        /// <param name="targetMat"> Reference to the Material currently selected. </param>
        /// <returns> GlossMetalAlphaData </returns>
        public static GlossMetalAlphaData ShowGlossMetalAlpha(Material targetMat)
        {
            GlossMetalAlphaData data = new GlossMetalAlphaData();

            GUILayout.BeginVertical(EditorStyles.helpBox);
            if (targetMat.HasProperty("_Glossiness")) data.glossiness = EditorGUILayout.Slider(new GUIContent("Smoothness"), targetMat.GetFloat("_Glossiness"), 0, 1);
            if (targetMat.HasProperty("_Metallic")) data.metallic = EditorGUILayout.Slider(new GUIContent("Metallic"), targetMat.GetFloat("_Metallic"), 0, 1);
            if (targetMat.HasProperty("_AlphaCutout")) data.alphaCutout = UdonVR_ShaderGUI.AlphaCutout(targetMat);
            if (targetMat.HasProperty("_AlphaFade")) data.alphaFade = UdonVR_ShaderGUI.AlphaFade(targetMat);
            GUILayout.EndVertical();

            return data;
        }

        /// <summary>
        /// Saves the value changes to the Material.
        /// </summary>
        /// <param name="targetMat"> Reference to the Material currently selected. </param>
        /// <param name="data"> Reference to the struct containing the values. </param>
        public static void SaveGlossMetalAlpha(Material targetMat, GlossMetalAlphaData data)
        {
            if (data.glossiness.HasValue) targetMat.SetFloat("_Glossiness", data.glossiness.Value);
            if (data.glossiness.HasValue) targetMat.SetFloat("_Metallic", data.metallic.Value);
            if (data.alphaCutout.HasValue) targetMat.SetFloat("_AlphaCutout", data.alphaCutout.Value);
            if (data.alphaFade.HasValue) targetMat.SetFloat("_AlphaFade", data.alphaFade.Value);
        }
        #endregion

        #region Emission
        public struct EmissionData
        {
            public bool? lightOnly;
            public float? intensity;
            public Color? color;
            public Texture emissionMap;
        };

        /// <summary>
        /// Shows the necessary fields in the inspector.
        /// </summary>
        /// <param name="targetMat"> Reference to the Material currently selected. </param>
        /// <param name="materialEditor"> Reference to the MaterialEditor. </param>
        /// <returns> RandomAxisFlipData </returns>
        public static EmissionData ShowEmission(Material targetMat, MaterialEditor materialEditor)
        {
            EmissionData _data = new EmissionData();
            if (targetMat.HasProperty("_EmissionLightOnly")) _data.lightOnly = System.Convert.ToBoolean(targetMat.GetFloat("_EmissionLightOnly"));
            if (targetMat.HasProperty("_EmissionIntensity")) _data.intensity = targetMat.GetFloat("_EmissionIntensity");
            if (targetMat.HasProperty("_EmissionColor")) _data.color = targetMat.GetColor("_EmissionColor");
            if (targetMat.HasProperty("_EmissionMap")) _data.emissionMap = targetMat.GetTexture("_EmissionMap");

            GUILayout.BeginVertical(EditorStyles.helpBox);
            UdonVR_GUI.Header(new GUIContent("Realtime GI does not work unless you use a script update it when there are changes."), UdonVR_GUIOption.FontSize(12), UdonVR_GUIOption.TextAnchor(TextAnchor.MiddleCenter));
            materialEditor.LightmapEmissionFlagsProperty(0, true);
            if (_data.lightOnly.HasValue) _data.lightOnly = UdonVR_GUI.ToggleButton(new GUIContent("Emission Light Only", "Only emits GI light."), _data.lightOnly.Value);
            if (_data.intensity.HasValue) _data.intensity = EditorGUILayout.Slider(new GUIContent("Emission Intensity", "Unused if 'Global Illumination' is set to 'None'."), _data.intensity.Value, 0, 4096);
            if (_data.color.HasValue) _data.color = EditorGUILayout.ColorField(new GUIContent("Emission Color"), _data.color.Value, true, true, true);
            if (targetMat.HasProperty("_EmissionMap")) _data.emissionMap = (Texture)EditorGUILayout.ObjectField(new GUIContent("Emission Map"), _data.emissionMap, typeof(Texture), false);
            GUILayout.EndVertical();

            return _data;
        }

        /// <summary>
        /// Saves the value changes to the Material.
        /// </summary>
        /// <param name="targetMat"> Reference to the Material currently selected. </param>
        /// <param name="data"> Reference to the struct containing the values. </param>
        public static void SaveEmission(Material targetMat, EmissionData data)
        {
            if (data.lightOnly.HasValue) targetMat.SetFloat("_EmissionLightOnly", System.Convert.ToSingle(data.lightOnly));
            if (data.intensity.HasValue) targetMat.SetFloat("_EmissionIntensity", data.intensity.Value);
            if (data.color.HasValue) targetMat.SetColor("_EmissionColor", data.color.Value);
            if (targetMat.HasProperty("_EmissionMap")) targetMat.SetTexture("_EmissionMap", data.emissionMap);
            targetMat.SetShaderPassEnabled("META", true);
        }
        #endregion
    }
}
