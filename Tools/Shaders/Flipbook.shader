Shader "_UdonVR/Toolkit/Flipbook"
{
    Properties
    {
        [HideInInspector][MaterialEnum(Opaque,0,Cutout,1,Fade,2)]_BlendMode("Blend Mode", Int) = 0 // allow switching transparancy options
        [HideInInspector]_SrcBlend("SrcBlend", Int) = 1
        [HideInInspector]_DstBlend("DstBlend", Int) = 0
        [HideInInspector]_ZWrite("ZWrite", Int) = 1
        [HideInInspector][MaterialEnum(Off,0,Front,1,Back,2)]_Cull("Cull", Int) = 2 // allow switching culling options
        [HideInInspector][ToggleOff]_SpecularHighlights("Specular Highlights", Int) = 0 // allow toggling of specular highlights

        [HideInInspector]_MainTex("Main Tex", 2D) = "white" {}
        [HideInInspector]_EnableEmission("Enable Emission", Float) = 1
        [HideInInspector]_EmissionMap("Emission Map", 2D) = "white" {}
        [HideInInspector][MaterialToggle]_EmissionMapIsFlipbook("Emission Map Is Flipbook", Float) = 0
        [HideInInspector]_Color("Color", Color) = (1, 1, 1, 1)
        [HideInInspector][MaterialToggle]_EnableLoop("Enable Loop", Float) = 0
        [HideInInspector]_Percent("Percent", Range(0, 1)) = 0
        [HideInInspector]_Index("Index", Int) = 0
        [HideInInspector][MaterialEnum(Sawtooth,0,Triangle,1)] _WaveType("Wave Type", Int) = 0
        [HideInInspector]_Speed("Speed", Range(0.005, 5)) = 0.15
        [HideInInspector]_Columns("Columns", Int) = 1
        [HideInInspector]_Rows("Rowss", Int) = 1

        [HideInInspector]_Glossiness("Smoothness", Range(0,1)) = 0
        [HideInInspector]_Metallic("Metallic", Range(0,1)) = 0
        [HideInInspector]_AlphaCutout("Alpha Cutout", Range(0, 1.01)) = 0.5 // the alpha cutout thresh hold
        [HideInInspector]_AlphaFade("Alpha Fade", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "PreviewType" = "plane" }
        Cull [_Cull]
        Blend [_SrcBlend][_DstBlend]
        ZWrite [_ZWrite]

        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows keepalpha
        #pragma target 3.0
        #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF // needs to be defined in shader

        #include "Assets/_UdonVR/Tools/Shaders/ShaderImports/UsefulFunctions.cginc"

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed _AlphaCutout;
        fixed _AlphaFade;
        #pragma shader_feature_local _ALPHACUTOUT
        #pragma shader_feature_local _ALPHAFADE
        #pragma alphatest:_AlphaCutout alpha:fade

        sampler2D _MainTex;
        fixed _EnableEmission;
        sampler2D _EmissionMap;
        fixed _EmissionMapIsFlipbook;
        fixed4 _Color;

        fixed _EnableLoop;
        half _Percent;
        int _Index;
        fixed _WaveType;
        half _Speed;
        int _Columns;
        int _Rows;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
        // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            _Speed = clampMin(_Speed, 0.005);
            _Columns = clampMin(_Columns, 1);
            _Rows = clampMin(_Rows, 1);

            fixed t = _Time.y * _Speed;
            fixed sawtoothWave = frac(2 * (t - floor(0.5 + t))); // get sawtooth wave
            fixed triangleWave = abs(2 * (t - floor(0.5 + t))); // get triangle wave
            fixed waveInput = _WaveType == 0 ? sawtoothWave
            : _WaveType == 1 ? triangleWave
            : 0;

            float index = _EnableLoop ? floor(Remap(waveInput, 0, 1, 0, (_Columns * _Rows) - 0.5)) : Remap(_Percent, 0, 1, 0, (_Columns * _Rows) - 1);
            index = _Index > 0 ? _Index : index;
            index = clampMax(index, (_Columns * _Rows) - 1);

            float2 output = Flipbook(IN.uv_MainTex, _Columns, _Rows, index);
            fixed4 c = tex2D(_MainTex, output) * _Color;
            fixed4 e = tex2D(_EmissionMap, _EmissionMapIsFlipbook ? output : IN.uv_MainTex);

            o.Albedo = c.rgb;
            o.Emission = _EnableEmission == 1 ? c.rgb * e.rgb: float3(0, 0, 0);
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;

            #if defined(_ALPHACUTOUT)
                clip(c.a - _AlphaCutout);
            #endif

            #if defined(_ALPHAFADE)
                o.Alpha = _AlphaFade;
                clip(c.a - 0.5);
            #endif
        }
        ENDCG
    }
    FallBack "Diffuse"
    CustomEditor "Flipbook_Editor"
}
