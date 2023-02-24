/*

Writen by Hamster9090901 after jokingly talking about how it would be easy to make.

*/

Shader "_UdonVR/Toolkit/BowlingCarpet"
{
    Properties
    {
        [HideInInspector][ToggleOff]_SpecularHighlights("Specular Highlights", Int) = 0 // allow toggling of specular highlights

        [HideInInspector]_Glossiness("Smoothness", Range(0,1)) = 0
        [HideInInspector]_Metallic("Metallic", Range(0,1)) = 0

        [HideInInspector]_MainTex("Main Tex", 2D) = "white" {}
        [HideInInspector][MaterialToggle]_GreyscaleMainTex("Greyscale Main Tex", Float) = 1
        [HideInInspector]_Color("Color", Color) = (0.1255, 0.008, 0.33725, 1)
        [HideInInspector]_EmissionBrightness("Emission Brightness", Range(0, 1)) = 0.5

        [HideInInspector]_ShapeBrightnessMultiplier("Shape Brightness Multiplier", Float) = 1

        [HideInInspector][MaterialToggle]_IsFrame("Frame", Float) = 1
        [HideInInspector][MaterialToggle]_IsFrameRandom("Frame Random", Float) = 1

        [HideInInspector][MaterialToggle]_RemoveRandomShapes("Remove Random Shapes", Float) = 1

        [HideInInspector][MaterialToggle]_IsTopOnly("Top Only", Float) = 0

        [HideInInspector]_ColorCount("Color Count", Int) = 5
        [HideInInspector]_ShapeColor1("Shape Color 1", Color) = (0.067, 1, 0.467, 1) // teal
        [HideInInspector]_ShapeColor2("Shape Color 2", Color) = (0.996, 0.949, 0, 1) // yellow
        [HideInInspector]_ShapeColor3("Shape Color 3", Color) = (0.333, 0.765, 0.996, 1) // light blue
        [HideInInspector]_ShapeColor4("Shape Color 4", Color) = (0.953, 0.302, 0.922, 1) // pink
        [HideInInspector]_ShapeColor5("Shape Color 5", Color) = (0.469, 0, 0.714, 1) // purple
        [HideInInspector]_ShapeColor6("Shape Color 6", Color) = (0, 0, 0, 1)
        [HideInInspector]_ShapeColor7("Shape Color 7", Color) = (0, 0, 0, 1)
        [HideInInspector]_ShapeColor8("Shape Color 8", Color) = (0, 0, 0, 1)

        [HideInInspector]_IsWindows("Is Windows", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        LOD 100

        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows //vertex:vert
        #pragma target 3.0
        #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF // needs to be defined in shader

        #include "Assets/_UdonVR/Tools/Shaders/ShaderImports/UsefulFunctions.cginc"
        #include "Assets/_UdonVR/Tools/Shaders/ShaderImports/SDF_2D.cginc"
        #include "Assets/_UdonVR/Tools/Shaders/ShaderImports/SDF_2D_Operations.cginc"

        struct Input
        {
            float3 worldPos;
            float3 worldNormal;

            //float3 localPos;
            //float3 localNormal;
        };

        half _Glossiness;
        half _Metallic;

        sampler2D _MainTex;
        const fixed _GreyscaleMainTex;
        fixed4 _Color;
        half _EmissionBrightness;

        half _ShapeBrightnessMultiplier;

        const fixed _IsFrame;
        const fixed _IsFrameRandom;

        const fixed _RemoveRandomShapes;

        const fixed _IsTopOnly;

        int _ColorCount;
        fixed4 _ShapeColor1;
        fixed4 _ShapeColor2;
        fixed4 _ShapeColor3;
        fixed4 _ShapeColor4;
        fixed4 _ShapeColor5;
        fixed4 _ShapeColor6;
        fixed4 _ShapeColor7;
        fixed4 _ShapeColor8;

        fixed _IsWindows;

        /*
        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.localPos = v.vertex;
            o.localNormal = v.normal;
        }
        */


        //
        // CUSTOM OPERATIONS BEGIN
        //
        float3 opFixDist(in float3 distance)
        {
            distance.x = opFixDist(distance.x);
            distance.y = opFixDist(distance.y);
            distance.z = opFixDist(distance.z);
            return distance;
        }

        float3 opRound(in float3 d, in float r)
        {
            return float3(opRound(d.x, r), opRound(d.y, r), opRound(d.z, r));
        }

        float3 opOnion(in float3 d, in float r)
        {
            return float3(opOnion(d.x, r), opOnion(d.y, r), opOnion(d.z, r));
        }
        //
        // CUSTOM OPERATIONS END
        //


        //
        // CUSTOM SHAPES BEGIN
        //
        float3 isoCube(in float2 p, in float s)
        {
            float2 pos = float2(0, 0);

            // top
            pos = opTxRxScale(p, float2(0, 1) * s, 0, s);
            float top = sdRhombus(pos, float2(1, 1) * RhombusScale);

            // left
            pos = opTxRxScale(p, float2(-0.86, -0.485) * s, opRemapRx(60), s);
            float left = sdRhombus(pos, float2(1, 1) * RhombusScale);

            // right
            pos = opTxRxScale(p, float2(0.86, -0.485) * s, opRemapRx(120), s);
            float right = sdRhombus(pos, float2(1, 1) * RhombusScale);

            return float3(top, left, right);
        }

        float3 isoFry(in float2 p, in float s)
        {
            float2 pos = float2(0, 0);
            float3 fry = float3(0, 0, 0);

            pos = opTx(p, float2(-1.725, 2.975) * 2 * s);
            fry = isoCube(pos, s);

            pos = opTx(p, float2(-1.725, 2.975) * s);
            fry = min(fry, isoCube(pos, s));

            pos = opTx(p, float2(0, 0));
            fry = min(fry, isoCube(pos, s));

            pos = opTx(p, float2(1.725, -2.975) * s);
            fry = min(fry, isoCube(pos, s));

            pos = opTx(p, float2(1.725, -2.975) * 2 * s);
            fry = min(fry, isoCube(pos, s));

            return fry;
        }

        float sdTriangleIsoscelesCentered(in float2 p, in float2 q)
        {
            // approximately moves the origin of the triangle to the center of it
            p = opTx(p, float2(0, -(q.y * 0.55)));
            return sdTriangleIsosceles(p, q);
        }

        float lightning(in float2 p, in float2 s)
        {
            float2 pos = float2(0, 0);
            float d = 0;

            pos = opTx(p, float2(-0.55, 1.715) * s);
            d = sdTriangleIsoscelesCentered(pos, float2(1, -1.5 * 3) * s);
            pos = opTx(p, float2(0.55, -1.715) * s);
            d = min(d, sdTriangleIsoscelesCentered(pos, float2(1, 1.5 * 3) * s));

            return d;
        }
        //
        // CUSTOM SHAPES END
        //



        //
        // RANDOM VALUES FUNCTION BEGIN
        //
        struct RandValue
        {
            float2 uvOffset[2];
            float rand[2];
            float pos[2];
            float rot[2];
            float multi[2];
        };
        RandValue GetRandValue(in float2 uv, in float min, in float max, in float offset1, in float offset2)
        {
            float2 uvOffset[2];
            float rand[2];
            float pos[2];
            float rot[2];
            float multi[2];

            uvOffset[0] = uv + offset1 * 10;
            uvOffset[1] = uv + offset2 * 10;
            for(int i = 0; i < 2; i++){
                rand[i] = Random(floor(uvOffset[i]));
                pos[i] = Remap(rand[i], 0, 1, min, max);
                rot[i] = Remap(rand[i], 0, 1, 0, PI * 2);
                multi[i] = Remap(rand[i], 0, 1, 0.75, 1.25);
            }

            RandValue rv = (RandValue)0;
            rv.uvOffset = uvOffset;
            rv.rand = rand;
            rv.pos = pos;
            rv.rot = rot;
            rv.multi = multi;
            return rv;
        }
        //
        // RANDOM VALUES FUNCTION END
        //

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed3 c = fixed3(0, 0, 0);
            fixed3 e = fixed3(0, 0, 0);

            //
            // COLORS BEGIN
            //
            fixed3 _Colors[8];
            _Colors[0] = _ShapeColor1.rgb;
            _Colors[1] = _ShapeColor2.rgb;
            _Colors[2] = _ShapeColor3.rgb;
            _Colors[3] = _ShapeColor4.rgb;
            _Colors[4] = _ShapeColor5.rgb;
            _Colors[5] = _ShapeColor6.rgb;
            _Colors[6] = _ShapeColor7.rgb;
            _Colors[7] = _ShapeColor8.rgb;
            //
            // COLORS END
            //

            //
            // POSITION BEGIN
            //
            float2 posRaw = float2(0, 0);
            fixed3 axis = float3(0, 0, 0);
            fixed3 isPositive = float3(0, 0, 0);

            // get the position (uv) for the axis were on
            posRaw = abs(IN.worldNormal.x) > 0.5 ? IN.worldPos.zy
            : abs(IN.worldNormal.y) > 0.5 ? IN.worldPos.xz
            : abs(IN.worldNormal.z) > 0.5 ? IN.worldPos.xy
            : float2(0, 0);

            // get what axis were on
            axis.x = IN.worldNormal.x ? 1 : 0;
            axis.y = IN.worldNormal.y ? 1 : 0;
            axis.z = IN.worldNormal.z ? 1 : 0;

            // check if the axis is positive or negative
            isPositive.x = IN.worldNormal.x > 0 ? 1 : 0;
            isPositive.y = IN.worldNormal.y > 0 ? 1 : 0;
            isPositive.z = IN.worldNormal.z > 0 ? 1 : 0;
            //
            // POSITION END
            //

            //
            // DRAW OBJECTS BEGIN
            //
            fixed3 carpetTex = tex2D(_MainTex, frac(posRaw)).rgb;
            if (_GreyscaleMainTex) carpetTex = (carpetTex.r + carpetTex.g + carpetTex.b) / 3;
            c = carpetTex * _Color;

            float2 pos = float2(0, 0);
            float2 rp = float2(0, 0);
            fixed obj = 0;

            float min = -0.3275;
            float max = 0.3275;

            RandValue rv = (RandValue)0;
            RandValue rvExtra = (RandValue)0;

            if (!_GreyscaleMainTex) carpetTex = (carpetTex.r + carpetTex.g + carpetTex.b) / 3;
            //carpetTex = lerp(carpetTex, float3(1, 1, 1), _ShapeBrightness);
            carpetTex *= _ShapeBrightnessMultiplier;
            carpetTex = clamp(carpetTex, 0, 1);

            // box
            pos = frac(posRaw);
            pos -= 0.5;
            rv = GetRandValue(posRaw, min, max, 7, 0);

            rp = opTxRxScale(pos, float2(rv.pos[0], rv.pos[1]), opRemapRx(rv.rot[1]), 0.0625 * rv.multi[1]);
            obj = sdBox(rp, float2(1, 2));
            if(_IsFrame) obj = (_IsFrameRandom ? rv.rand[0] : 1) > 0.5 ? opOnion(obj, 0.125) : obj;
            obj = opFixDist(obj);
            if(_RemoveRandomShapes) obj *= rv.rand[0] > 0.1 ? 1 : 0;
            c = obj > 0.5 ? 0 : c;
            //e = obj > 0.5 ? 0 : e;
            e += obj * carpetTex * _Colors[Remap(rv.rand[1], 0, 1, 0, _ColorCount)];

            // circle
            pos = frac(posRaw + 0.125);
            pos -= 0.5;
            rv = GetRandValue(posRaw + 0.125, min, max, 6, 1);

            rp = opTxRxScale(pos, float2(rv.pos[0], rv.pos[1]), opRemapRx(rv.rot[1]), 0.125 * rv.multi[1]);
            obj = sdCircle(rp, 1);
            if(_IsFrame) obj = (_IsFrameRandom ? rv.rand[0] : 1) > 0.5 ? opOnion(obj, 0.125) : obj;
            obj = opFixDist(obj);
            if(_RemoveRandomShapes) obj *= rv.rand[0] > 0.1 ? 1 : 0;
            c = obj > 0.5 ? 0 : c;
            //e = obj > 0.5 ? 0 : e;
            e += obj * carpetTex * _Colors[Remap(rv.rand[1], 0, 1, 0, _ColorCount)];

            // triangle
            if (_IsWindows)
            {
                pos = frac(posRaw + 0.25);
                pos -= 0.5;
                rv = GetRandValue(posRaw + 0.25, min + 0.1, max - 0.1, 5, 2);

                rp = opTxRxScale(pos, float2(rv.pos[0], rv.pos[1]), opRemapRx(rv.rot[1]), 0.125 * rv.multi[1]);
                obj = sdTriangleIsoscelesCentered(rp, float2(1 * rv.multi[0], 1.5 * 2 * rv.multi[1]));
                if(_IsFrame) obj = (_IsFrameRandom ? rv.rand[0] : 1) > 0.5 ? opOnion(obj, 0.125) : obj;
                obj = opFixDist(obj);
                if(_RemoveRandomShapes) obj *= rv.rand[0] > 0.1 ? 1 : 0;
                c = obj > 0.5 ? 0 : c;
                //e = obj > 0.5 ? 0 : e;
                e += obj * carpetTex * _Colors[Remap(rv.rand[1], 0, 1, 0, _ColorCount)];
            }

            // round box
            pos = frac(posRaw + 0.375);
            pos -= 0.5;
            rv = GetRandValue(posRaw + 0.375, min, max, 4, 3);

            rp = opTxRxScale(pos, float2(rv.pos[0], rv.pos[1]), opRemapRx(rv.rot[1]), 0.0625 * rv.multi[1]);
            obj = opRound(sdBox(rp, float2(1, 2)), 0.25);
            if(_IsFrame) obj = (_IsFrameRandom ? rv.rand[0] : 1) > 0.5 ? opOnion(obj, 0.125) : obj;
            obj = opFixDist(obj);
            if(_RemoveRandomShapes) obj *= rv.rand[0] > 0.1 ? 1 : 0;
            c = obj > 0.5 ? 0 : c;
            //e = obj > 0.5 ? 0 : e;
            e += obj * carpetTex * _Colors[Remap(rv.rand[1], 0, 1, 0, _ColorCount)];

            // lightning
            if (_IsWindows)
            {
                pos = frac(posRaw + 0.45);
                pos -= 0.5;
                rv = GetRandValue(posRaw + 0.45, min + 0.115, max - 0.115, 3, 4);

                rp = opTxRxScale(pos, float2(rv.pos[0], rv.pos[1]), opRemapRx(rv.rot[1]), 0.0625 * rv.multi[1]);
                obj = lightning(rp, float2(rv.multi[0], rv.multi[1]));
                if(_IsFrame) obj = (_IsFrameRandom ? rv.rand[0] : 1) > 0.5 ? opOnion(obj, 0.125) : obj;
                obj = opFixDist(obj);
                if(_RemoveRandomShapes) obj *= rv.rand[0] > 0.1 ? 1 : 0;
                c = obj > 0.5 ? 0 : c;
                //e = obj > 0.5 ? 0 : e;
                e += obj * carpetTex * _Colors[Remap(rv.rand[1], 0, 1, 0, _ColorCount)];
            }

            // iso cube
            pos = frac(posRaw + 0.5);
            pos -= 0.5;
            rv = GetRandValue(posRaw + 0.5, min, max, 2, 5);
            rvExtra = GetRandValue(posRaw + 0.5, min, max, 4, 6);

            rp = opTxRxScale(pos, float2(rv.pos[0], rv.pos[1]), opRemapRx(rv.rot[1]), 1);
            float3 cube = isoCube(rp, 0.0625 * rv.multi[1]);
            if(_IsFrame) cube = (_IsFrameRandom ? rv.rand[0] : 1) > 0.5 ? opOnion(cube, 0.125) : cube;
            cube = opFixDist(cube);
            if(_RemoveRandomShapes) cube *= rv.rand[0] > 0.1 ? 1 : 0;
            c = cube.x > 0.5 ? 0 : c;
            c = cube.y > 0.5 ? 0 : c;
            c = cube.z > 0.5 ? 0 : c;
            e += cube.x * carpetTex * _Colors[Remap(rv.rand[0], 0, 1, 0, _ColorCount)];
            e += cube.y * carpetTex * _Colors[Remap(rvExtra.rand[0], 0, 1, 0, _ColorCount)];
            e += cube.z * carpetTex * _Colors[Remap(rvExtra.rand[1], 0, 1, 0, _ColorCount)];

            // iso fry
            pos = frac(posRaw + 0.625);
            pos -= 0.5;
            rv = GetRandValue(posRaw + 0.625, min + 0.05, max - 0.05, 1, 6);
            rvExtra = GetRandValue(posRaw + 0.625, min + 0.05, max - 0.05, 3, 5);

            rp = opTxRxScale(pos, float2(rv.pos[0], rv.pos[1]), opRemapRx(rv.rot[1]), 1);
            float3 fry = isoFry(rp, 0.0625 / 3 * rv.multi[1]);
            if(_IsFrame) fry = (_IsFrameRandom ? rv.rand[0] : 1) > 0.5 ? opOnion(fry, 0.125) : fry;
            fry = opFixDist(fry);
            if(_RemoveRandomShapes) fry *= rv.rand[0] > 0.1 ? 1 : 0;
            c = fry.x > 0.5 ? 0 : c;
            c = fry.y > 0.5 ? 0 : c;
            c = fry.z > 0.5 ? 0 : c;
            e += fry.x * carpetTex * _Colors[Remap(rv.rand[0], 0, 1, 0, _ColorCount)];
            e += fry.y * carpetTex * _Colors[Remap(rvExtra.rand[0], 0, 1, 0, _ColorCount)];
            e += fry.z * carpetTex * _Colors[Remap(rvExtra.rand[1], 0, 1, 0, _ColorCount)];

            // squiggly
            pos = frac(posRaw + 0.75);
            pos -= 0.5;
            rv = GetRandValue(posRaw + 0.75, min, max, 0, 7);

            rp = opTxRxScale(pos, float2(rv.pos[0], rv.pos[1]), opRemapRx(rv.rot[1]), 0.125 * rv.multi[1]);
            rp = opWobble(rp, float2(5 * rv.multi[0], 5 * rv.multi[1]), 0.2);
            float segmentThickness = 0.125;
            obj = sdSegment(rp, float2(0, -1.35), float2(0, 1.35)) - segmentThickness;
            if(_IsFrame) obj = (_IsFrameRandom ? rv.rand[0] : 1) > 0.5 ? opOnion(obj, segmentThickness / 4) : obj;
            obj = opFixDist(obj);
            obj *= rv.rand[0] > 0.1 ? 1 : 0;
            c = obj > 0.5 ? 0 : c;
            e += obj * carpetTex * _Colors[Remap(rv.rand[1], 0, 1, 0, _ColorCount)];

            //
            // DRAW OBJECTS END
            //

            //e.rg += posRaw;

            // only draw color on the top of the object
            if(_IsTopOnly)
            {
                c = isPositive.y ? c : 0;
                e = isPositive.y ? e : 0;
            }

            o.Albedo = c + e;
            o.Emission = e * _EmissionBrightness;
            o.Smoothness = _Glossiness;
            o.Metallic = _Metallic;
            o.Alpha = _Color.a;
        }
        ENDCG
    }
    CustomEditor "BowlingCarpet_Editor"
}
