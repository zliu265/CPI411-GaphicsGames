float4x4 World;
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
	float4 Position: POSITION;
};

struct VertexOutput {
	float4 Position: POSITION;
	float3 TextureCoordinate : TEXCOORD;
};

VertexOutput GourandVertexShaderFunction(VertexInput input)
{
	VertexOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.TextureCoordinate = mul(input.Position, World).xyz - CameraPosition;

	return output;
}

float4 MyPixelShader(VertexOutput input) : COLOR
{
	return texCUBE(SkyBoxSampler, normalize(input.TextureCoordinate));
}

technique MyTechnique {
	pass Pass1 {
		VertexShader = compile vs_4_0 GourandVertexShaderFunction();
		PixelShader = compile ps_4_0 MyPixelShader();
	}
}