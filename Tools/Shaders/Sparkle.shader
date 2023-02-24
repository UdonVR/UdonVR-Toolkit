// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "_UdonVR/Toolkit/Unlit/Sparkle"
{
    Properties
    {
        [HideInInspector]_MainTex ("Noise Tex", 2D) = "white" {}
        [HideInInspector]_NoiseRotationSpeed ("Noise Rotation Speed", Range(1, 100)) = 25
        [HideInInspector]_SparkleOffset ("Sparkle Offset", Range(0, 1)) = 0.5
        [HideInInspector]_Color ("Color", Color) = (0.6941177, 1, 0.9647059, 1)
        [HideInInspector]_NoiseScale ("Noise Scale", Vector) = (1, 1, 0, 0)

        // fresnel
        /*
        [MaterialToggle] _EnableFresnel("Enable Fresnel", Float) = 0
        _FresnelBias("Fresnel Bias", Float) = -0.35
        _FresnelScale("Fresnel Scale", Float) = 1
        _FresnelPower("Fresnel Power", Float) = 0.85
        */
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #include "Assets/_UdonVR/Tools/Shaders/ShaderImports/Hamster9090901_Functions.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

                float4 pos : POSITION;
                half3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;

                float3 viewDir : TEXCOORD1;
                //float fresnel : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _NoiseRotationSpeed;
            float _SparkleOffset;

            half4 _Color;

            float4 _NoiseScale;

            float _EnableFresnel;
            float _FresnelBias;
            float _FresnelScale;
            float _FresnelPower;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.viewDir = _WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, v.vertex).xyz;

                //o.fresnel = Fresnel(v.pos, v.normal, _FresnelBias, _FresnelScale, _FresnelPower);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 noise = tex2D(_MainTex, i.uv * _NoiseScale.xy);
                noise = HueRotationDegrees(noise, _NoiseRotationSpeed * _Time.y);
                noise = normalize(noise - _SparkleOffset);

                float3 viewDir = 1.0 - normalize(i.viewDir);

                float d = dot(noise, viewDir);
                d = saturate(d);

                float4 col = float4(0, 0, 0, 1);
                col.rgb = d * _Color;
                //if (_EnableFresnel) col.rgb += (1.0 - i.fresnel) * _Color.rgb;

                return col;
            }
            ENDCG
        }
    }
    CustomEditor "Sparkle_Editor"
}
