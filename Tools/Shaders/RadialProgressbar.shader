Shader "_UdonVR/Toolkit/Unlit/RadialProgressbar"
{
    Properties
    {
        [HideInInspector]_Percent("Percent", Range(0, 1)) = 1
        [HideInInspector][MaterialToggle]_FlipProgressBar("Flip Progress Bar", Float) = 1
        [HideInInspector][MaterialToggle]_IsBillboard("Billboard", Float) = 0

        [HideInInspector][MaterialToggle]_IsGradient("Gradient", Float) = 1
        [HideInInspector]_Color1("Color1", Color) = (0, 0.75, 1, 1)
        [HideInInspector]_Color2("Color2", Color) = (1, 0.75, 0, 1)

        [HideInInspector][MaterialToggle]_IsHollow("Hollow", Float) = 1

        [HideInInspector]_Thickness("Thickness", Range(0.005, 0.15)) = 0.03125
        [HideInInspector]_Radius("Radius", Range(0.05, 0.475)) = 0.375

        [HideInInspector]_Rotation("Rotation", Range(0, 360)) = 0

        [HideInInspector]_StartAngle("Start Angle", Range(0, 179.9)) = 0
        [HideInInspector]_EndAngle("End Angle", Range(180, 360)) = 360
        [HideInInspector]_Seperation("Seperation", Range(0, 90)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="true" "PreviewType" = "plane" }
        LOD 100
        CULL Off
        ZWrite Off
        Lighting Off

        Blend SrcAlpha OneMinusSrcAlpha 

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #include "Assets/_UdonVR/Tools/Shaders/ShaderImports/UsefulFunctions.cginc"
            #include "Assets/_UdonVR/Tools/Shaders/ShaderImports/SDF_2D.cginc"
            #include "Assets/_UdonVR/Tools/Shaders/ShaderImports/SDF_2D_Operations.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float _Percent;
            const fixed _FlipProgressBar;
            const fixed _IsBillboard;

            const fixed _IsGradient;
            fixed4 _Color1;
            fixed4 _Color2;

            const fixed _IsHollow;

            fixed _Thickness;
            fixed _Radius;

            float _Rotation;

            float _StartAngle;
            float _EndAngle;
            float _Seperation;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                if(_IsBillboard) o.vertex = Billboard(v.vertex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                if (_FlipProgressBar) uv.x = Remap(uv.x, 0, 1, 1, 0);
                uv -= 0.5;

                uv = opRx(uv, 0.75);
                if (_Rotation > 0) uv = opRx(uv, Remap(_Rotation, 0, 360, 0, 1));

                float a = 0;
                float s = 0;
                if (_IsGradient)
                {
                    a = atan2(uv.x, uv.y);
                    s = fmod(a + PI2 - PIHalf, PI2) / PI2;
                }

                _Thickness = (_IsHollow ? _Thickness * 0.8 : _Thickness);
                if (_IsGradient) _Seperation += _Thickness * (_IsHollow ? 200 : 160);
                _StartAngle = clamp(_StartAngle + _Seperation, 0, 180);
                _EndAngle = clamp(_EndAngle - _Seperation, 180, 360);

                float startPercent = Remap(_StartAngle, 0, 180, 0, 0.5);
                float endPercent = Remap(_EndAngle, 180, 360, 0.5, 1);

                _Percent = clamp(_Percent, 0.0001, 1);
                _Percent = Remap(_Percent, 0, 1, startPercent, endPercent);
                float d = sdArc(uv, Remap(startPercent, 0, 0.5, 0, PI), Remap(_Percent, 0, 1, 0, PI2), _Radius) - _Thickness;
                if (_IsHollow) d = opOnion(d, _Thickness * 0.25);
                d = opFixDist(d);

                //d = (d == 0 ? 0.5 : d);
                clip(d - 0.5); // remove black areas

                fixed4 c = fixed4(0, 0, 0, 0);
                if (_IsGradient)
                {
                    c = d * lerp(_Color2, _Color1, clamp(s, 0, 1));
                }
                else
                {
                    c = d * _Color1;
                }

                return c;
            }
            ENDCG
        }
    }
    CustomEditor "RadialProgressBar_Editor"
}
