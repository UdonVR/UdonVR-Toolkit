/*

A collection of functions by Hamster9090901 and is letting being used and also uses input some of their other shaders not just those associated with UdonVR

*/

// include guards that keep the functions from being included more than once
#ifndef Hamster9090901_Functions
    #define Hamster9090901_Functions

    float3 HueRotationRadians(float3 input, float offset)
    {
        float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
        float4 P = lerp(float4(input.bg, K.wz), float4(input.gb, K.xy), step(input.b, input.g));
        float4 Q = lerp(float4(P.xyw, input.r), float4(input.r, P.yzx), step(P.x, input.r));
        float D = Q.x - min(Q.w, Q.y);
        float E = 1e-10;
        float3 hsv = float3(abs(Q.z + (Q.w - Q.y)/(6.0 * D + E)), D / (Q.x + E), Q.x);

        float hue = hsv.x + offset;
        hsv.x = (hue < 0) ? hue + 1 : (hue > 1) ? hue - 1 : hue;

        float4 K2 = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
        float3 P2 = abs(frac(hsv.xxx + K2.xyz) * 6.0 - K2.www);
        return hsv.z * lerp(K2.xxx, saturate(P2 - K2.xxx), hsv.y);
    }
    float3 HueRotationDegrees(float3 input, float offset)
    {
        return HueRotationRadians(input, offset / 360);
    }

    float Fresnel(float4 vertPos, float vertNormal, float bias, float scale, float power)
    {
        return bias + scale * pow(1.0 + dot(normalize(ObjSpaceViewDir(vertPos)), vertNormal), power);
    }

#endif