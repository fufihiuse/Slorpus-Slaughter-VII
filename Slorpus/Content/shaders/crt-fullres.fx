/*
Split of CRT shader for effects that should be applied proportional to the size
of the final screen. IE. on the 1920x1080 scaled rendertexture.
*/

// taken from: https://github.com/Hammster/windows-terminal-shaders/blob/main/crt.hlsl

// vars ------------------------------------------

#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif

float gameTime;
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

// configuration ----------------------------------

#define REFRESHLINE_DISTANCE 0.005
#define ENABLE_CURVE 1
#define CURVE_INTENSITY 0.7 // if you change this: also change it in Screen.cs :(
#define ENABLE_SCANLINES 1
#define ENABLE_REFRESHLINE 1
#define REFRESHLINE_TINT float3(0.05, 0.05, 0.05)
#define REFRESHLINE_OFFSET 0.005
#define SCANLINE_TINT float4(0.2f, 0.2f, 0.2f, 0.0f)

// actual shader ----------------------------------

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
	if (xy.x < -0.025f || xy.y < -0.025f) return float4(0, 0, 0, 0);
	if (xy.x > 1.025f || xy.y > 1.025f)  return float4(0, 0, 0, 0);
	// Bazel
	if (xy.x < -0.015f || xy.y < -0.015f) return float4(0.03f, 0.03f, 0.03f, 0.0f);
	if (xy.x > 1.015f || xy.y > 1.015f)  return float4(0.03f, 0.03f, 0.03f, 0.0f);
	// Screen Border
	if (xy.x < 0.001f || xy.y < 0.001f)  return float4(0.0f, 0.0f, 0.0f, 0.0f);
	if (xy.x > 0.999f || xy.y > 0.999f)  return float4(0.0f, 0.0f, 0.0f, 0.0f);
	#endif

	float4 color = tex2D(TextureSampler, xy);

	#if ENABLE_REFRESHLINE
	if (abs(xy.y - gameTime*2) < REFRESHLINE_DISTANCE) {
		xy.x += REFRESHLINE_OFFSET;
		color = tex2D(TextureSampler, xy);
		color.rgb += REFRESHLINE_TINT;
	}
	#endif

	#if ENABLE_SCANLINES
	// TODO: fixing the precision issue so that scanlines are always 1px
	if (floor(xy.y * 1000) % 4) color -= SCANLINE_TINT;
	#endif

	return color;
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