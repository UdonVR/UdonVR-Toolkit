/*

A collection of different 2D sdf operations
Modified by Hamster9090901

*/

#include "UsefulFunctions.cginc"

// include guards that keep the functions from being included more than once
#ifndef SDF_2D_Operations
    #define SDF_2D_Operations

    #ifndef PI2
        #define PI2 6.28318530718
    #endif

    // positioning
    float2 opTx(in float2 samplePosition, in float2 offset)
    {
        return samplePosition - offset;
    }

    float2 opRx(in float2 samplePosition, in float rotation)
    {
        // expects rotation to be between (0 - 1)
        float angle = rotation * PI2 * -1;
        float sine, cosine;
        sincos(angle, sine, cosine);
        return float2(cosine * samplePosition.x + sine * samplePosition.y, cosine * samplePosition.y - sine * samplePosition.x);
    }

    float2 opScale(in float2 samplePosition, in float scale)
    {
        return samplePosition / scale;
    }

    float opRemapRx(in float rotation)
    {
        // remaps rotation from (0 - 6.28) or (0 - 360) to (0 - 1)
        rotation = rotation <= PI2 ? Remap(rotation, 0, PI2, 0, 1) : Remap(rotation, 0, 360, 0, 1);
        return rotation;
    }

    float2 opTxRxScale(in float2 samplePosition, in float2 offset, in float rotation, in float scale)
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



    // weird effects
    float2 opRepeat(in float2 p, in float s)
    {
        return fmod(p + s * 0.5, s) - s * 0.5;
    }

    float2 opWobble(in float2 position, in float2 frequency, in float2 amount)
    {
        float2 wobble = sin(position.yx * frequency) * amount;
        return position + wobble;
    }

#endif

