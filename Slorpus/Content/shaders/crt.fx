// taken from: https://github.com/Hammster/windows-terminal-shaders/blob/main/crt.hlsl

#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif

static const float REFRESHLINE_DISTANCE = 0.005;

float gameTime;
float4x4 view_projection;
sampler TextureSampler : register(s0);

/*
BLOOM STUFF --------------
*/
//Needed for pixel offset
float2 InverseResolution;
//The threshold of pixels that are brighter than that.
float Threshold = 0.8f;
//MODIFIED DURING RUNTIME, CHANGING HERE MAKES NO DIFFERENCE;
float Radius;
float Strength;
//How far we stretch the pixels
float StreakLength = 1;

// structs (boilerplate :/)
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

// Settings
#define GRAIN_INTENSITY 0.02
#define ENABLE_GRAIN 1
#define TINT_COLOR float4(1, 0.7f, 0, 0)
#define ENABLE_SCANLINES 1
#define ENABLE_REFRESHLINE 1
#define ENABLE_NOISE 1
#define ENABLE_CURVE 1
#define CURVE_INTENSITY 0.7
#define ENABLE_TINT 0
#define DEBUG 0

// Grain Lookup Table
#define a0  0.151015505647689
#define a1 -0.5303572634357367
#define a2  1.365020122861334
#define b0  0.132089632343748
#define b1 -0.7607324991323768

static const float4 tint = TINT_COLOR;
static const float4 scanlineTint = float4(0.6f, 0.6f, 0.6f, 0.0f);

float permute(float x)
{
	x *= (34 * x + 1);
	return 289 * frac(x * 1 / 289.0f);
}

float rand(inout float state)
{
	state = permute(state);
	return frac(state / 41.0f);
}

float4 mainImage(float2 tex) : TARGET
{
	float2 xy = tex.xy;
	
	#if ENABLE_CURVE
	// TODO: add control variable for transform intensity
	xy -= 0.5f;				// offcenter screen
	float r = xy.x * xy.x + xy.y * xy.y; // get ratio, x^2 + y^2
	xy *= (4.2f / CURVE_INTENSITY) + r; // apply ratio (curves the screen)
	xy *= 0.245f * CURVE_INTENSITY;				// zoom
	xy += 0.5f;				// move back to center

	// TODO: add monitor visuals and make colors static consts
	// Outer Box
	if(xy.x < -0.025f || xy.y < -0.025f) return float4(0, 0, 0, 0); 
	if(xy.x > 1.025f  || xy.y > 1.025f)  return float4(0, 0, 0, 0); 
	// Bazel
	if(xy.x < -0.015f || xy.y < -0.015f) return float4(0.03f, 0.03f, 0.03f, 0.0f);
	if(xy.x > 1.015f  || xy.y > 1.015f)  return float4(0.03f, 0.03f, 0.03f, 0.0f);
	// Screen Border
	if(xy.x < 0.001f  || xy.y < 0.001f)  return float4(0.0f, 0.0f, 0.0f, 0.0f);
	if(xy.x > 0.999f  || xy.y > 0.999f)  return float4(0.0f, 0.0f, 0.0f, 0.0f);
	#endif
	
	float4 color = tex2D(TextureSampler, xy);

	#if DEBUG
	if(xy.x < 0.5f) return color;
	#endif

	#if ENABLE_REFRESHLINE
    if (abs(xy.y - gameTime) < REFRESHLINE_DISTANCE) {
		color.rgb += float3(0.5, 0.2, 0.2);
	} 
	#endif

	#if ENABLE_SCANLINES
	// TODO: fixing the precision issue so that scanlines are always 1px
	if(floor(xy.y * 1000) % 4) color *= scanlineTint;
	#endif	

	#if ENABLE_TINT
	float grayscale = (color.r + color.g + color.b) / 3.f;
	color = float4(grayscale, grayscale, grayscale, 0);
	color *= tint;
	#endif

	#if ENABLE_GRAIN
	float3 m = float3(tex, gameTime % 5 / 5) + 1.;
	float state = permute(permute(m.x) + m.y) + m.z;

	float p = 0.95 * rand(state) + 0.025;
	float q = p - 0.5;
	float r2 = q * q;

	float grain = q * (a2 + (a1 * r2 + a0) / (r2 * r2 + b1 * r2 + b0));
	color.rgb += GRAIN_INTENSITY * grain;
	#endif

	return color;
}

// taken from https://github.com/Kosmonaut3d/BloomFilter-for-Monogame-and-XNA/blob/master/Bloom%20Sample/Content/Shaders/BloomFilter/BloomCrossPlatform.fx

//Just an average of 4 values.
float4 Box4(float4 p0, float4 p1, float4 p2, float4 p3)
{
	return (p0 + p1 + p2 + p3) * 0.25f;
}

//Extracts the pixels we want to blur
float4 ExtractPS(float4 pos : SV_POSITION,  float2 texCoord : TEXCOORD0) : SV_TARGET0
{
	float2 halfPixel = InverseResolution / 2;
	float4 color = tex2D(TextureSampler, texCoord + halfPixel);

	float avg = (color.r + color.g + color.b) / 3;

	if (avg>Threshold)
	{
		return color * (avg - Threshold) / (1 - Threshold);// * (avg - Threshold);
	}

	return float4(0, 0, 0, 0);
}

//Extracts the pixels we want to blur, but considers luminance instead of average rgb
float4 ExtractLuminancePS(float4 pos : SV_POSITION,  float2 texCoord : TEXCOORD0) : SV_TARGET0
{
	float2 halfPixel = InverseResolution / 2;
	float4 color = tex2D(TextureSampler, texCoord + halfPixel);

    float luminance = color.r * 0.21f + color.g * 0.72f + color.b * 0.07f;

    if(luminance>Threshold)
    {
		return color * (luminance - Threshold) / (1 - Threshold);// *(luminance - Threshold);
        //return saturate((color - Threshold) / (1 - Threshold));
    }

    return float4(0, 0, 0, 0);
}

//Downsample to the next mip, blur in the process
float4 DownsamplePS(float4 pos : SV_POSITION,  float2 texCoord : TEXCOORD0) : SV_TARGET0
{
    float2 offset = float2(StreakLength * InverseResolution.x, 1 * InverseResolution.y);

	float2 halfPixel = InverseResolution / 2;
        
    float4 c0 = tex2D(TextureSampler, texCoord + float2(-2, -2) * offset + halfPixel);
    float4 c1 = tex2D(TextureSampler, texCoord + float2(0,-2)*offset + halfPixel);
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
float4 UpsamplePS(float4 pos : SV_POSITION,  float2 texCoord : TEXCOORD0) : SV_TARGET0
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
float4 UpsampleLuminancePS(float4 pos : SV_POSITION,  float2 texCoord : TEXCOORD0) : SV_TARGET0
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
float4 SpritePixelShader(PixelInput p) : COLOR0{
	float4 diffuse = mainImage(p.TexCoord.xy);
	return diffuse * p.Color;
}

technique SpriteBatch {
	pass {
		VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
		PixelShader = compile PS_SHADERMODEL SpritePixelShader();
	}
}

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