/*

A collection of different 3D sdf shapes
Modified by Hamster9090901

*/

// include guards that keep the functions from being included more than once
#ifndef SDF_3D
    #define SDF_3D

    float smin(float a, float b, float k) {
        float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
        return lerp(b, a, h) - k * h * (1.0 - h);
    }

    float dot2(in float2 v) {
        return dot(v, v); 
    }

    float dot2(in float3 v) { 
        return dot(v, v); 
    }

    float ndot(in float2 a, in float2 b) { 
        return a.x * b.x - a.y * b.y; 
    }

    // shapes
    float sdSphere(float3 p, float r) {
        return length(p) - r;
    }

    float sdBox(float3 p, float3 s) {
        p = abs(p) - s;
        return length(max(p, 0.0)) + min(max(p.x, max(p.y, p.z)), 0.0);
    }

    float sdRoundBox(float3 p, float3 b, float r) {
        float3 q = abs(p) - b;
        return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0) - r;
    }

    float sdBoxFrame(float3 p, float3 s, float e){
        p = abs(p) - s;
        float3 q = abs(p + e) - e;
        return min(min(
        length(max(float3(p.x, q.y, q.z), 0.0)) + min(max(p.x, max(q.y, q.z)), 0.0),
        length(max(float3(q.x, p.y, q.z), 0.0)) + min(max(q.x, max(p.y, q.z)), 0.0)),
        length(max(float3(q.x, q.y, p.z), 0.0)) + min(max(q.x, max(q.y, p.z)), 0.0));
    }

    float sdTorus(float3 p, float2 r) {
        float x = length(p.xz) - r.x;
        return length(float2(x, p.y)) - r.y;
    }

    float sdCappedTorus(in float3 p, in float2 sc, in float ra, in float rb){
        p.x = abs(p.x);
        float k = (sc.y * p.x > sc.x * p.y) ? dot(p.xy, sc) : length(p.xy);
        return sqrt(dot(p, p) + ra * ra - 2.0 * ra * k) - rb;
    }

    float sdCapsule(float3 p, float3 a, float3 b, float r) {
        float3 ab = b - a;
        float3 ap = p - a;

        float t = dot(ab, ap) / dot(ab, ab);
        t = clamp(t, 0.0, 1.0);

        float3 c = a + t * ab;

        return length(p - c) - r;
    }

    float sdCone(in float3 p, in float2 c, float h)
    {
        // c is the sin/cos of the angle, h is height
        // Alternatively pass q instead of (c,h),
        // which is the point at the base in 2D
        float2 q = h * float2(c.x / c.y, -1.0);

        float2 w = float2(length(p.xz), p.y);
        float2 a = w - q * clamp(dot(w, q) / dot(q, q), 0.0, 1.0);
        float2 b = w - q * float2(clamp(w.x / q.x, 0.0, 1.0), 1.0);
        float k = sign(q.y);
        float d = min(dot(a, a), dot(b, b));
        float s = max(k * (w.x * q.y - w.y * q.x), k * (w.y - q.y));
        return sqrt(d) * sign(s);
    }

    float sdCappedCylinder(float3 p, float3 a, float3 b, float r)
    {
        float3  ba = b - a;
        float3  pa = p - a;
        float baba = dot(ba, ba);
        float paba = dot(pa, ba);
        float x = length(pa * baba - ba * paba) - r * baba;
        float y = abs(paba - baba * 0.5) - baba * 0.5;
        float x2 = x * x;
        float y2 = y * y * baba;
        float d = (max(x, y) < 0.0) ? -min(x2, y2) : (((x > 0.0) ? x2 : 0.0) + ((y > 0.0) ? y2 : 0.0));
        return sign(d) * sqrt(abs(d)) / baba;
    }

    float udQuad(float3 p, float3 a, float3 b, float3 c, float3 d)
    {
        float3 ba = b - a; 
        float3 pa = p - a;

        float3 cb = c - b; 
        float3 pb = p - b;

        float3 dc = d - c; 
        float3 pc = p - c;

        float3 ad = a - d; 
        float3 pd = p - d;

        float3 nor = cross(ba, ad);

        return sqrt(
        (sign(dot(cross(ba, nor), pa)) +
        sign(dot(cross(cb, nor), pb)) +
        sign(dot(cross(dc, nor), pc)) +
        sign(dot(cross(ad, nor), pd)) < 3.0)
        ?
        min(min(min(
        dot2(ba * clamp(dot(ba, pa) / dot2(ba), 0.0, 1.0) - pa),
        dot2(cb * clamp(dot(cb, pb) / dot2(cb), 0.0, 1.0) - pb)),
        dot2(dc * clamp(dot(dc, pc) / dot2(dc), 0.0, 1.0) - pc)),
        dot2(ad * clamp(dot(ad, pd) / dot2(ad), 0.0, 1.0) - pd))
        :
        dot(nor, pa) * dot(nor, pa) / dot2(nor));
    }

#endif