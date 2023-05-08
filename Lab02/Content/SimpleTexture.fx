// *** Lab02 HLSL: 2D/3D Simple Shader ****
texture MyTexture;
float3  offset;

float4x4 World;
float4x4 View;
float4x4 Projection;


sampler mySampler = sampler_state {
	Texture = <MyTexture>;
};
struct VertexPositionTexture {
	float4 Position: POSITION;
	float2 TextureCoordinate : TEXCOORD;
};
VertexPositionTexture MyVertexShader(VertexPositionTexture input) {
	//input.Position.xyz += offset;
	//return input;
	VertexPositionTexture output;
	float4 worldPos = mul(input.Position, World);
	float4 viewPos = mul(worldPos, View);
	float4 projPos = mul(viewPos, Projection);
	output.Position = projPos;
	output.TextureCoordinate = input.TextureCoordinate;
	return output;
}
float4 MyPixelShader(VertexPositionTexture input) : COLOR
{
	return tex2D(mySampler, input.TextureCoordinate);
}
technique MyTechnique {
	pass Pass1 {
		VertexShader = compile vs_4_0 MyVertexShader();
		PixelShader = compile ps_4_0 MyPixelShader();
	}
}