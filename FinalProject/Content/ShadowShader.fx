float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;
float4x4 LightViewMatrix;
float4x4 LightProjectionMatrix;
float3 CameraPosition;
float3 LightPosition;

float4 ShadowColor;

texture ShadowMap;

sampler ShadowMapSampler = sampler_state
{
	Texture = <ShadowMap>;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	MipFilter = none;
	AddressU = border;
	AddressV = border;
};

texture ModelTexture;

sampler ModelTextureSampler = sampler_state
{
	Texture = <ModelTexture>;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
	AddressU = WRAP;
	AddressV = WRAP;
};


struct ShadowedSceneVertexShaderInput
{
	float4 Position : POSITION0;
	float4 Normal : NORMAL0;
	float2 TexCoord : TEXCOORD0;
};

struct ShadowedSceneVertexShaderOutput
{
	float4 Position: POSITION0;
	float4 Pos2DAsSeenByLight: TEXCOORD0;
	float3 Normal: TEXCOORD1;
	float2 TexCoord: TEXCOORD2;
	float4 WorldPosition: TEXCOORD3;
};



ShadowedSceneVertexShaderOutput ShadowedSceneVertexShader(ShadowedSceneVertexShaderInput input)
{
	ShadowedSceneVertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	output.Position = mul(mul(worldPosition, View), Projection);
	output.Pos2DAsSeenByLight = mul(mul(worldPosition, LightViewMatrix), LightProjectionMatrix);
	output.Normal = normalize(mul(input.Normal, WorldInverseTranspose));
	output.WorldPosition = worldPosition;
	output.TexCoord = input.TexCoord;
	return output;
}

float4 ShadowedScenePixelShader(ShadowedSceneVertexShaderOutput  input) : COLOR
{
	float4 projTexCoord = input.Pos2DAsSeenByLight / input.Pos2DAsSeenByLight.w;
	projTexCoord.xy = 0.5 * projTexCoord.xy + float2(0.5, 0.5);
	projTexCoord.y = 1.0 - projTexCoord.y;
	float realDistance = 1 - projTexCoord.z;

	float3 N = normalize(input.Normal);
	float3 L = normalize(LightPosition - input.WorldPosition.xyz);

	float4 diffuseLightingFactor = 0;

	if (projTexCoord.x >= 0 && projTexCoord.x <= 1 &&
		projTexCoord.y >= 0 && projTexCoord.y <= 1 &&
		saturate(projTexCoord).x == projTexCoord.x &&
		saturate(projTexCoord).y == projTexCoord.y)
	{
		float depthStoredInShadowMap = tex2D(ShadowMapSampler, projTexCoord.xy).r;

			if (realDistance + (1.0f / 100.0f) > depthStoredInShadowMap)
			{
				diffuseLightingFactor = max(0, dot(N,L)); //Gray
			}
			else
			{
				diffuseLightingFactor = ShadowColor;
			}
	}
	float4 texColor = tex2D(ModelTextureSampler, input.TexCoord);

	diffuseLightingFactor *= texColor;

	return diffuseLightingFactor;
}

struct VertexShaderInput
{
	float4 Position : POSITION0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float4 Position2D: TEXCOORD2;
};

VertexShaderOutput ShadowMapVertexShader(VertexShaderInput input)
{
	VertexShaderOutput output;
	output.Position = mul(mul(mul(input.Position, World), LightViewMatrix), LightProjectionMatrix);
	output.Position2D = output.Position;
	return output;
}

float4 ShadowMapPixelShader(VertexShaderOutput input) : COLOR0
{
	float4 projTexCoord = input.Position2D / input.Position2D.w;
	projTexCoord.xy = 0.5 * projTexCoord.xy + float2(0.5, 0.5);
	projTexCoord.y = 1.0 - projTexCoord.y;
	float depth = 1.0 - projTexCoord.z;
	float4 color = (depth > 0) ? depth : 0;

	return color;
}

technique ShadowMapShader
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 ShadowMapVertexShader();
		PixelShader = compile ps_4_0 ShadowMapPixelShader();
	}
}

technique ShadowedScene
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 ShadowedSceneVertexShader();
		PixelShader = compile ps_4_0  ShadowedScenePixelShader();
	}
}