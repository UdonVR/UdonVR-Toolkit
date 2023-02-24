/*

A collection of useful functions
By Hamster9090901

*/

// include guards that keep the functions from being included more than once
#ifndef UsefulFunctions
    #define UsefulFunctions

    float Random(in float2 xy)
    {
        return frac(sin(dot(xy, float2(12.9898, 78.233))) * 43758.5453123);
    }

    float Remap(float value, float low1, float high1, float low2, float high2)
    {
        return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
    }

    float3 FromRGB(in float r, in float g, in float b)
    {
        r = Remap(r, 0, 255, 0, 1);
        g = Remap(g, 0, 255, 0, 1);
        b = Remap(b, 0, 255, 0, 1);
        return float3(r, g, b);
    }
    float3 FromRGB(in float3 rgb)
    {
        return FromRGB(rgb.r, rgb.g, rgb.b);
    }

    float clampMin(in float value, in float min)
    {
        return value < min ? min : value;
    }

    float clampMax(in float value, in float max)
    {
        return value > max ? max : value;
    }

    // returns the UV position of a sprite in a sprite sheet by index based on number of rows and columns
    float2 Flipbook(in float2 uv, in uint columns, in uint rows, in uint index)
    {
        // get single sprite size
        float2 size = float2(1.0 / columns, 1.0 / rows);
        int totalFrames = columns * rows;

        // wrap x and y indexes
        int indexX = index % rows;
        int indexY = floor((index % totalFrames) / columns);

        // get offset to sprite index
        float2 offset = float2(size.x * indexX, -size.y * indexY);

        float2 newUV = uv * size; // get single sprite UV
        newUV.y = newUV.y + size.y * (rows - 1); // flip Y (to start 0 from top)

        return newUV + offset; // return UV with offset
    }

    // passes color though with the applied flipbook color if there is a sprite sheet
    fixed4 ApplyFlipbookColor(sampler2D spriteSheet, float2 uv, fixed4 colIn, fixed4 flipbookColMultiplier, int index, int columns, int rows, fixed cutoutVal)
    {
        // spriteSheet | the texture sampler for the spritesheet
        // index | index of sprite in the sprite sheet
        // rows | number of rows in sprite sheet
        // columns | number of columns in sprite sheet
        // cutoutVal | used to remove black when under that value

        float2 output = Flipbook(frac(uv), columns, rows, index); // get the UV of the index
        fixed4 c = tex2D(spriteSheet, output); // get the sprite
        c *= flipbookColMultiplier; // add the flipbook color

        //fixed alpha = colIn.a; // get alpha
        //c.a = alpha; // copy alpha over

        return (c.r <= cutoutVal && c.g <= cutoutVal && c.b <= cutoutVal) ? colIn : c;
    }

    // billboards a mesh towards the camera
    float4 Billboard(float4 vertex)
    {
        float3 vpos = mul((float3x3)unity_ObjectToWorld, vertex.xyz);
        float4 worldCoord = float4(unity_ObjectToWorld._m03, unity_ObjectToWorld._m13, unity_ObjectToWorld._m23, 1);
        float4 viewPos = mul(UNITY_MATRIX_V, worldCoord) + float4(vpos, 0);
        float4 outPos = mul(UNITY_MATRIX_P, viewPos);
        return outPos;
    }

    float3 Checkerboard(float2 uv, float3 colorA, float3 colorB, float2 frequency)
    {
        uv = (uv.xy + 0.5) * frequency;
        float4 derivatives = float4(ddx(uv), ddy(uv));
        float2 duv_length = sqrt(float2(dot(derivatives.xz, derivatives.xz), dot(derivatives.yw, derivatives.yw)));
        float width = 1.0;
        float2 distance3 = 4.0 * abs(frac(uv + 0.25) - 0.5) - width;
        float2 scale = 0.35 / duv_length.xy;
        float freqLimiter = sqrt(clamp(1.1f - max(duv_length.x, duv_length.y), 0.0, 1.0));
        float2 vector_alpha = clamp(distance3 * scale.xy, -1.0, 1.0);
        float alpha = saturate(0.5f + 0.5f * vector_alpha.x * vector_alpha.y * freqLimiter);
        return lerp(colorA, colorB, alpha.xxx);
    }

#endif