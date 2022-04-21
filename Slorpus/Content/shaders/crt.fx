// taken from: https://github.com/Hammster/windows-terminal-shaders/blob/main/crt.hlsl

#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif

static const float REFRESHLINE_DISTANCE = 30;

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
    if (xy.y == gameTime) color.rgb = 1;
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