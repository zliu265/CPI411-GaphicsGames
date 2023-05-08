float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 InverseCamera; //Inverse Cam Matrix
float4x4 WorldInverseTranspose;

sampler ParticleSampler : register(s0) = sampler_state 
{
	Texture = <Texture>;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
};

struct VertexShaderInput 
{
	float4 Position: POSITION0;
	float2 TexCoord: TEXCOORD0;
	float4 ParticlePosition: POSITION1;
	float4 ParticleParameter: POSITION2;
};

struct VertexShaderOutput
{
	float4 Position: POSITION0;
	float2 TexCoord: TEXCOORD0;
	float4 Color: COLOR0;
};

struct VertexInput
{
	float4 Position: POSITION;
	float4 Normal: NORMAL;
	//float2 UV: TEXCOORD0;
};

VertexShaderOutput ParticleVS(VertexShaderInput input)
{
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, InverseCamera); // Rotating Camera
	worldPosition.xyz = worldPosition.xyz * sqrt(input.ParticleParameter.x / input.ParticleParameter.y);
	worldPosition += input.ParticlePosition; //Center is moved
	output.Position = mul(mul(mul(worldPosition, World), View), Projection);
	output.TexCoord = input.TexCoord;
	output.Color = 1 - input.ParticleParameter.x / input.ParticleParameter.y;
	return output;
}

float4 ParticlePS(VertexShaderOutput input) : COLOR
{
	float4 color = tex2D(ParticleSampler, input.TexCoord);
	color *= input.Color;
	return color;
}


technique particle
{
	pass Pass0 
	{
		VertexShader = compile vs_4_0 ParticleVS();
		PixelShader = compile ps_4_0 ParticlePS();
	}
}
