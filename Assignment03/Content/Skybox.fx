﻿float4x4 World;
float4x4 View;
float4x4 Projection;
float3 CameraPosition;
Texture2D SkyBoxTexture;

samplerCUBE SkyBoxSampler = sampler_state
{
	texture = <SkyBoxTexture>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Mirror;
	AddressV = Mirror;
};

struct VertexInput {
	float4 Position: POSITION0;
};

struct VertexOutput {
	float4 Position: POSITION0;
	float3 TextureCoordinate: TEXCOORD0;
};

VertexOutput VS(VertexInput input)
{
	VertexOutput output;
	float4 worldPos = mul(input.Position, World);
	float4 viewPos = mul(worldPos, View);
	float4 projPos = mul(viewPos, Projection);
	output.Position = projPos;

	float4 vertexPos = mul(input.Position, World);
	output.TextureCoordinate = vertexPos.xyz - CameraPosition;

	return output;
}

float4 PS(VertexOutput input) : COLOR0
{
	return texCUBE(SkyBoxSampler,normalize(input.TextureCoordinate));
}

technique SkyBox {
	pass Pass1 {
		VertexShader = compile vs_4_0 VS();
		PixelShader = compile ps_4_0 PS();
	}
}