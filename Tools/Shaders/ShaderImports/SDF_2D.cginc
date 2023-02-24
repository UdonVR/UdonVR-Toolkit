/*

A collection of different 2D sdf shapes
Modified by Hamster9090901

*/

// include guards that keep the functions from being included more than once
#ifndef SDF_2D
    #define SDF_2D

    #ifndef PIHalf
        #define PIHalf 1.5707963268
    #endif
    #ifndef PI
        #define PI 3.14159265359
    #endif
    #ifndef PI2
        #define PI2 6.28318530718
    #endif

    #ifndef RhombusScale
        #define RhombusScale float2(1.7320508, 1)
    #endif

    float sdCircle(float2 p, float r)
    {
        return length(p) - r;
    }

    float sdBox(in float2 p, in float2 b)
    {
        float2 d = abs(p) - b;
        return length(max(d, 0)) + min(max(d.x, d.y), 0);
    }

    float sdSegment(in float2 p, in float2 a, in float2 b)
    {
        // to set width subtract output by desired thickness
        float2 pa = p - a;
        float2 ba = b - a;
        float h = clamp(dot(pa, ba) / dot(ba, ba), 0, 1);
        return length(pa - ba * h);
    }

    float ndot(in float2 a, in float2 b) { return a.x * b.x - a.y * b.y; }
    float sdRhombus(in float2 p, in float2 b)
    {
        p = abs(p);
        float h = clamp(ndot(b - 2 * p, b) / dot(b, b), -1, 1);
        float d = length(p - 0.5 * b * float2(1 - h, 1 + h));
        return d * sign(p.x * b.y + p.y * b.x - b.x * b.y);
    }

    float sdEquilateralTriangle( in float2 p )
    {
        const float k = sqrt(3);
        p.x = abs(p.x) - 1;
        p.y = p.y + 1 / k;
        if (p.x + k * p.y > 0 ) p = float2(p.x - k * p.y,-k * p.x - p.y) *  0.5;
        p.x -= clamp(p.x, -2, 0);
        return -length(p) * sign(p.y);
    }

    float sdTriangleIsosceles(in float2 p, in float2 q)
    {
        p.x = abs(p.x);
        float2 a = p - q * clamp(dot(p, q) / dot(q, q), 0, 1);
        float2 b = p - q * float2(clamp(p.x / q.x, 0, 1), 1);
        float s = -sign(q.y);
        float2 d = min(float2(dot(a, a), s * (p.x * q.y - p.y * q.x)), float2(dot(b, b), s * (p.y-q.y)));
        return -sqrt(d.x) * sign(d.y);
    }



    float sdArc(in float2 p, in float a0, in float a1, in float r)
    {
        float a = fmod(atan2(p.y, p.x), PI2);

        float ap = a - a0;
        if (ap < 0)
        {
            ap += PI2;
        }
        float a1p = a1 - a0;
        if (a1p < 0)
        {
            a1p += PI2;
        }

        if (ap >= a1p)
        {
            float2 q0 = float2(r * cos(a0), r * sin(a0));
            float2 q1 = float2(r * cos(a1), r * sin(a1));
            return min(length(p - q0), length(p - q1));
        }

        return abs(length(p) - r);
    }

#endif
