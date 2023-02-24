/*

A collection of different 3D sdf operations
Modified by Hamster9090901

*/

// include guards that keep the functions from being included more than once
#ifndef SDF_3D_Operations
    #define SDF_3D_Operations

    #ifndef PI2
        #define PI2 6.28318530718
    #endif

    float2x2 Rot(float a) {
        float s = sin(a);
        float c = cos(a);
        return float2x2(c, -s, s, c);
    }

    // positioning
    float3 opTx(in float3 samplePosition, in float3 offset)
    {
        return samplePosition - offset;
    }

    float3 opRx(in float3 samplePosition, in float3 rotation)
    {
        // expects rotation to be between (0 - 1)
        float angle = 0;
        if(rotation.x){
            angle = rotation.x * PI2 * -1;
            samplePosition.zy = mul(samplePosition.zy, Rot(angle));
        }
        if(rotation.y){
            angle = rotation.y * PI2 * -1;
            samplePosition.xz = mul(samplePosition.xz, Rot(angle));
        }
        if(rotation.z){
            angle = rotation.z * PI2 * -1;
            samplePosition.xy = mul(samplePosition.xy, Rot(angle));
        }
        return samplePosition;
    }

    float3 opScale(in float3 samplePosition, in float3 scale)
    {
        return samplePosition / scale;
    }

    float3 opTxRxScale(in float3 samplePosition, in float3 offset, in float rotation, in float3 scale)
    {
        samplePosition = opTx(samplePosition, offset);
        samplePosition = opRx(samplePosition, rotation);
        samplePosition = opScale(samplePosition, scale);
        return samplePosition;
    }

    // effects
    float opFixDist(in float distance)
    {
        // fixes the distance output from the sdfs to have a hard edge
        return lerp(1, 0, step(0, distance));
    }

    float opRound(in float d, in float r)
    {
        return d - r;
    }

    float opOnion(in float d, in float r)
    {
        return abs(d) - r;
    }

    float opSmoothUnion(float d1, float d2, float k) {
        float h = clamp(0.5 + 0.5 * (d2 - d1) / k, 0.0, 1.0);
        return lerp(d2, d1, h) - k * h * (1.0 - h);
    }



    // weird effects
    float3 opRepeat(in float3 p, in float s)
    {
        return fmod(p + s * 0.5, s) - s * 0.5;
    }

    float3 opWobble(in float3 position, in float3 frequency, in float3 amount)
    {
        float3 wobble = sin(position.xyz * frequency) * amount;
        return position + wobble;
    }

#endif