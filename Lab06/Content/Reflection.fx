


float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;
float3 CameraPosition;
texture decalMap;
texture environmentMap;

sampler tsampler1 = sampler_state{
	texture = <decalMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

samplerCUBE SkyBoxSampler = sampler_state{
	texture = <environmentMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Mirror;
	AddressV = Mirror;
};

struct VertexShaderInput
{
	float4 Position: POSITION0;
	float2 TexCoord: TEXCOORD0;
	float4 Normal: NORMAL0;
};

struct VertexShaderOutput
{
	float4 Position: POSITION0;
	float2 TexCoord: TEXCOORD0;
	float3 R : TEXCOORD1;
};

VertexShaderOutput ReflectionVertexShader(VertexShaderInput input) {
	VertexShaderOutput output;

	float4 worldPos = mul(input.Position, World);
	float4 viewPos = mul(worldPos, View);
	output.Position = mul(viewPos, Projection);

	float3 N = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
	float3 I = normalize(worldPos.xyz - CameraPosition);
	output.R = reflect(I, N);
	output.TexCoord = input.TexCoord;

	return output;
}

float4 ReflectionPixelShader(VertexShaderOutput input) : COLOR0
{
	float4 reflectedColor = texCUBE(SkyBoxSampler, input.R);
	float4 decalColor = tex2D(tsampler1, input.TexCoord);
	return lerp(decalColor, reflectedColor, 0.5);

	//return texCUBE(SkyBoxSampler, input.R);
}

technique Reflection {
	pass Pass1
	{
		VertexShader = compile vs_4_0 ReflectionVertexShader();
		PixelShader = compile ps_4_0 ReflectionPixelShader();
	}
};