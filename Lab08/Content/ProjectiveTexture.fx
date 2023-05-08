
float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;
float4x4 LightViewMatrix;
float4x4 LightProjectionMatrix;
float3 CameraPosition;
float3 LightPosition;
texture ProjectiveTexture;

sampler TextureSampler = sampler_state
{
	Texture = <ProjectiveTexture>;
	MinFilter = none;
	MagFilter = none;
	MipFilter = none;
	AddressU = border;
	AddressV = border;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Normal : NORMAL0;
	float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float3 Normal	: TEXCOORD0;
	float2 TexCoord : TEXCOORD1;
	float3 WorldPosition : TEXCOORD2;
};

VertexShaderOutput VSFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	float4 worldPos = mul(input.Position, World);
	float4 viewPosition = mul(worldPos, View);
	output.Position = mul(viewPosition, Projection);
	output.WorldPosition = worldPos.xyz;
	output.Normal = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
	output.TexCoord = input.TexCoord;
	return output;
}

float4 PSFunction(VertexShaderOutput input) : COLOR0
{
	//float4 color = float4(1, 1, 1, 1);

	//step1
	float4 projTexCoord = mul(mul(float4(input.WorldPosition, 1), LightViewMatrix), LightProjectionMatrix);
	//step2
	projTexCoord = projTexCoord / projTexCoord.w;
	//step3
	projTexCoord.xy = 0.5 * projTexCoord.xy + float2(0.5, 0.5);
	//step4
	projTexCoord.y = 1.0 - projTexCoord.y;
	//step5
	float depth = 1.0 - projTexCoord.z;
	//step6
	float4 color = tex2D(TextureSampler, projTexCoord.xy);
	//step7
	if (color.x == 0 && color.y == 1 && color.z == 1)
			color.xyz = float3(0, 0, 0);
	//step8(Option)
	float3 N = normalize(input.Normal);
	float3 L = normalize(LightPosition - input.WorldPosition);
	if (dot(L, N) < 0) color = 0;
	return color;
}
technique Technique1
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 VSFunction();
		PixelShader = compile ps_4_0 PSFunction();
	}
}
