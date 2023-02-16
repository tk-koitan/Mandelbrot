#ifndef SAMPLE_OUTLINE_INCLUDED
#define SAMPLE_OUTLINE_INCLUDED

void GaussianFilter_float(Texture2D tex, float2 uv, SamplerState sam, out float4 color)
{
    color = float4(0, 0, 0, 1);
    
    color += (1.0 / 16.0) * SAMPLE_TEXTURE2D(tex, sam, uv + int2(-1, -1));
    color += (2.0 / 16.0) * SAMPLE_TEXTURE2D(tex, sam, uv + int2(-1, 0));
    color += (1.0 / 16.0) * SAMPLE_TEXTURE2D(tex, sam, uv + int2(-1, 1));
    color += (2.0 / 16.0) * SAMPLE_TEXTURE2D(tex, sam, uv + int2(0, -1));
    color += (4.0 / 16.0) * SAMPLE_TEXTURE2D(tex, sam, uv + int2(0, 0));
    color += (2.0 / 16.0) * SAMPLE_TEXTURE2D(tex, sam, uv + int2(0, 1));
    color += (1.0 / 16.0) * SAMPLE_TEXTURE2D(tex, sam, uv + int2(1, -1));
    color += (2.0 / 16.0) * SAMPLE_TEXTURE2D(tex, sam, uv + int2(1, 0));
    color += (1.0 / 16.0) * SAMPLE_TEXTURE2D(tex, sam, uv + int2(1, 1));
}

#endif    