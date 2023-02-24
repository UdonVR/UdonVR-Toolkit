Shader "_UdonVR/Toolkit/Unlit/LavaLamp"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Scale("Scale", Vector) = (1, 1, 1, 0)
        _Position1 ("Position1", Vector) = (0, 0, 0, 0.125)
        _Position2 ("Position2", Vector) = (0, 0, 0, 0.125)
        _Position3 ("Position3", Vector) = (0, 0, 0, 0.125)
        _Position4 ("Position4", Vector) = (0, 0, 0, 0.125)
        _Position5 ("Position5", Vector) = (0, 0, 0, 0.125)
    }
    SubShader
    {
        Tags { "RenderType"="Opawue" }
        LOD 100

        GrabPass { "_GrabpassTex" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #include "Assets/_UdonVR/Tools/Shaders/ShaderImports/SDF_3D.cginc"
            #include "Assets/_UdonVR/Tools/Shaders/ShaderImports/SDF_3D_Operations.cginc"
            #include "Assets/_UdonVR/Tools/Shaders/ShaderImports/UsefulFunctions.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

                float4 pos : POSITION;
                half3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 ro : TEXCOORD1;
                float3 hitPos : TEXCOORD2;

                float4 grapPassUV : TEXCOORD3;
            };

            sampler2D _GrabpassTex;

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float3 _Scale;

            float4 _Position1;
            float4 _Position2;
            float4 _Position3;
            float4 _Position4;
            float4 _Position5;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.ro = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1));// object space
                o.hitPos = v.vertex; // object space

                o.grapPassUV = ComputeGrabScreenPos(o.vertex);

                return o;
            }

            #define MAX_STEPS 100
            #define MAX_DIST 100.0
            #define SURF_DIST 0.001

            float GetDist(float3 p){
                float d = MAX_DIST;
                float3 rp = 0;

                rp = opTxRxScale(p, _Position1.xyz, 0, _Scale);
                d = opSmoothUnion(d, sdSphere(rp, _Position1.w), 0.35);

                rp = opTxRxScale(p, _Position2.xyz, 0, _Scale);
                d = opSmoothUnion(d, sdSphere(rp, _Position2.w), 0.35);

                rp = opTxRxScale(p, _Position3.xyz, 0, _Scale);
                d = opSmoothUnion(d, sdSphere(rp, _Position3.w), 0.35);

                rp = opTxRxScale(p, _Position4.xyz, 0, _Scale);
                d = opSmoothUnion(d, sdSphere(rp, _Position4.w), 0.35);

                rp = opTxRxScale(p, _Position5.xyz, 0, _Scale);
                d = opSmoothUnion(d, sdSphere(rp, _Position5.w), 0.35);

                return d;
            }

            float RayMarch(float3 ro, float3 rd) {
                float dO = 0.0;
                for (int i = 0; i < MAX_STEPS; i++) {
                    float3 p = ro + rd * dO;
                    float dS = GetDist(p);
                    dO += dS;
                    if (dO > MAX_DIST || dS < SURF_DIST) break;
                }
                return dO;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = float4(1, 1, 1, 1);
                float2 uv = i.uv;

                float3 ro = i.ro;
                float3 rd = normalize(i.hitPos - ro);

                float d = RayMarch(ro, rd);

                float4 bgCol = tex2Dproj(_GrabpassTex, i.grapPassUV);

                if(d >= MAX_DIST){
                    discard;
                }
                else{
                    float3 p = ro + rd * d;

                    float3 colR = 0.5 + 0.5 * cos(_Time.y + (p.xyz * 2 + 0.5) + float3(0, 2, 4));
                    col.rgb *= colR;
                }

                return col;
            }
            ENDCG
        }
    }
}
