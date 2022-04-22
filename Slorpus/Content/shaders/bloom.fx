// taken from https://github.com/Kosmonaut3d/BloomFilter-for-Monogame-and-XNA/blob/master/Bloom%20Sample/Content/Shaders/BloomFilter/BloomCrossPlatform.fx

#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif

float4x4 view_projection;
sampler TextureSampler : register(s0);

struct VertexInput {
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float4 TexCoord : TEXCOORD0;
};
struct PixelInput {
	float4 Position : SV_Position0;
	float4 Color : COLOR0;
	float4 TexCoord : TEXCOORD0;
};

//Needed for pixel offset
float2 InverseResolution;
//The threshold of pixels that are brighter than that.
float Threshold = 0.1f;
//MODIFIED DURING RUNTIME, CHANGING HERE MAKES NO DIFFERENCE;
float Radius;
float Strength;
//How far we stretch the pixels
float StreakLength = 1;

//Just an average of 4 values.
float4 Box4(float4 p0, float4 p1, float4 p2, float4 p3)
{
    return (p0 + p1 + p2 + p3) * 0.25f;
}

//Extracts the pixels we want to blur
float4 ExtractPS(float4 pos : SV_POSITION, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
    float2 halfPixel = InverseResolution / 2;
    float4 color = tex2D(TextureSampler, texCoord + halfPixel);

    float avg = (color.r + color.g + color.b) / 3;

    if (avg > Threshold)
    {
        return color * (avg - Threshold) / (1 - Threshold);// * (avg - Threshold);
    }

    return float4(0, 0, 0, 0);
}

//Extracts the pixels we want to blur, but considers luminance instead of average rgb
float4 ExtractLuminancePS(float4 pos : SV_POSITION, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
    float2 halfPixel = InverseResolution / 2;
    float4 color = tex2D(TextureSampler, texCoord + halfPixel);

    float luminance = color.r * 0.21f + color.g * 0.72f + color.b * 0.07f;

    if (luminance > Threshold)
    {
        return color * (luminance - Threshold) / (1 - Threshold);// *(luminance - Threshold);
        //return saturate((color - Threshold) / (1 - Threshold));
    }

    return float4(0, 0, 0, 0);
}

//Downsample to the next mip, blur in the process
float4 DownsamplePS(float4 pos : SV_POSITION, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
    float2 offset = float2(StreakLength * InverseResolution.x, 1 * InverseResolution.y);

    float2 halfPixel = InverseResolution / 2;

    float4 c0 = tex2D(TextureSampler, texCoord + float2(-2, -2) * offset + halfPixel);
    float4 c1 = tex2D(TextureSampler, texCoord + float2(0,-2) * offset + halfPixel);
    float4 c2 = tex2D(TextureSampler, texCoord + float2(2, -2) * offset + halfPixel);
    float4 c3 = tex2D(TextureSampler, texCoord + float2(-1, -1) * offset + halfPixel);
    float4 c4 = tex2D(TextureSampler, texCoord + float2(1, -1) * offset + halfPixel);
    float4 c5 = tex2D(TextureSampler, texCoord + float2(-2, 0) * offset + halfPixel);
    float4 c6 = tex2D(TextureSampler, texCoord + halfPixel);
    float4 c7 = tex2D(TextureSampler, texCoord + float2(2, 0) * offset + halfPixel);
    float4 c8 = tex2D(TextureSampler, texCoord + float2(-1, 1) * offset + halfPixel);
    float4 c9 = tex2D(TextureSampler, texCoord + float2(1, 1) * offset + halfPixel);
    float4 c10 = tex2D(TextureSampler, texCoord + float2(-2, 2) * offset + halfPixel);
    float4 c11 = tex2D(TextureSampler, texCoord + float2(0, 2) * offset + halfPixel);
    float4 c12 = tex2D(TextureSampler, texCoord + float2(2, 2) * offset + halfPixel);

    return Box4(c0, c1, c5, c6) * 0.125f +
    Box4(c1, c2, c6, c7) * 0.125f +
    Box4(c5, c6, c10, c11) * 0.125f +
    Box4(c6, c7, c11, c12) * 0.125f +
    Box4(c3, c4, c8, c9) * 0.5f;
}

//Upsample to the former MIP, blur in the process
float4 UpsamplePS(float4 pos : SV_POSITION, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
    float2 offset = float2(StreakLength * InverseResolution.x, 1 * InverseResolution.y) * Radius;

    float2 halfPixel = InverseResolution / 2;

    float4 c0 = tex2D(TextureSampler, texCoord + float2(-1, -1) * offset + halfPixel);
    float4 c1 = tex2D(TextureSampler, texCoord + float2(0, -1) * offset + halfPixel);
    float4 c2 = tex2D(TextureSampler, texCoord + float2(1, -1) * offset + halfPixel);
    float4 c3 = tex2D(TextureSampler, texCoord + float2(-1, 0) * offset + halfPixel);
    float4 c4 = tex2D(TextureSampler, texCoord + halfPixel);
    float4 c5 = tex2D(TextureSampler, texCoord + float2(1, 0) * offset + halfPixel);
    float4 c6 = tex2D(TextureSampler, texCoord + float2(-1,1) * offset + halfPixel);
    float4 c7 = tex2D(TextureSampler, texCoord + float2(0, 1) * offset + halfPixel);
    float4 c8 = tex2D(TextureSampler, texCoord + float2(1, 1) * offset + halfPixel);

    //Tentfilter  0.0625f    
    return 0.0625f * (c0 + 2 * c1 + c2 + 2 * c3 + 4 * c4 + 2 * c5 + c6 + 2 * c7 + c8) * Strength + float4(0, 0,0,0); //+ 0.5f * tex2D(c_texture, texCoord);

}

//Upsample to the former MIP, blur in the process, change offset depending on luminance
float4 UpsampleLuminancePS(float4 pos : SV_POSITION, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
    float2 halfPixel = InverseResolution / 2;

    float4 c4 = tex2D(TextureSampler, texCoord + halfPixel); //middle one

    /*float luminance = c4.r * 0.21f + c4.g * 0.72f + c4.b * 0.07f;
    luminance = max(luminance, 0.4f);
*/
    float2 offset = float2(StreakLength * InverseResolution.x, 1 * InverseResolution.y) * Radius; /// luminance;

    float4 c0 = tex2D(TextureSampler, texCoord + float2(-1, -1) * offset + halfPixel);
    float4 c1 = tex2D(TextureSampler, texCoord + float2(0, -1) * offset + halfPixel);
    float4 c2 = tex2D(TextureSampler, texCoord + float2(1, -1) * offset + halfPixel);
    float4 c3 = tex2D(TextureSampler, texCoord + float2(-1, 0) * offset + halfPixel);
    float4 c5 = tex2D(TextureSampler, texCoord + float2(1, 0) * offset + halfPixel);
    float4 c6 = tex2D(TextureSampler, texCoord + float2(-1, 1) * offset + halfPixel);
    float4 c7 = tex2D(TextureSampler, texCoord + float2(0, 1) * offset + halfPixel);
    float4 c8 = tex2D(TextureSampler, texCoord + float2(1, 1) * offset + halfPixel);

    return 0.0625f * (c0 + 2 * c1 + c2 + 2 * c3 + 4 * c4 + 2 * c5 + c6 + 2 * c7 + c8) * Strength + float4(0, 0, 0, 0); //+ 0.5f * tex2D(c_texture, texCoord);

}


PixelInput SpriteVertexShader(VertexInput v) {
	PixelInput output;

	output.Position = mul(v.Position, view_projection);
	output.Color = v.Color;
	output.TexCoord = v.TexCoord;
	return output;
}


/*

techniques --------------------------------

*/
technique Extract
{
    pass Pass1
    {
        VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
        PixelShader = compile PS_SHADERMODEL ExtractPS();
    }
}

technique ExtractLuminance
{
    pass Pass1
    {
        VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
        PixelShader = compile PS_SHADERMODEL ExtractLuminancePS();
    }
}

technique Downsample
{
    pass Pass1
    {
        VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
        PixelShader = compile PS_SHADERMODEL DownsamplePS();
    }
}

technique Upsample
{
    pass Pass1
    {
        VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
        PixelShader = compile PS_SHADERMODEL UpsamplePS();
    }
}


technique UpsampleLuminance
{
    pass Pass1
    {
        VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
        PixelShader = compile PS_SHADERMODEL UpsampleLuminancePS();
    }
}