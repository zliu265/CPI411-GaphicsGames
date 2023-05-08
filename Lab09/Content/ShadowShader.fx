float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;
float4x4 LightViewMatrix;
float4x4 LightProjectionMatrix;
float3 CameraPosition;

//lab07****************
float3 LightPosition;
float4 AmbientColor;
float AmbientIntensity;
float4 DiffuseColor;
float DiffuseIntensity;
float4 SpecularColor;
float SpecularIntensity;
float Shininess;
texture normalMap;
//**********************
texture DiffuseTexture;

sampler DiffuseSampler = sampler_state
{
	Texture = <DiffuseTexture>;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
	AddressU = WRAP;
	AddressV = WRAP;
};


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

//Lab 08****************
//texture ProjectiveTexture;
//
//sampler TextureSampler = sampler_state
//{
//	Texture = <ProjectiveTexture>;
//	MinFilter = none;
//	MagFilter = none;
//	MipFilter = none;
//	AddressU = border;
//	AddressV = border;
//};

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

	float4 textureColor = tex2D(DiffuseSampler, input.TexCoord);

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
				diffuseLightingFactor = float4(1, 0, 0, 1); //Red
			}
	}
	//return diffuseLightingFactor;
	return textureColor * diffuseLightingFactor;

}

struct SVVertexShaderInput
{
	float4 Position : POSITION0;
};

struct SVVertexShaderOutput
{
	float4 Position : POSITION0;
	float4 Position2D: TEXCOORD3;

};

SVVertexShaderOutput ShadowMapVertexShader(SVVertexShaderInput input)
{
	SVVertexShaderOutput output;
	output.Position = mul(mul(mul(input.Position, World), LightViewMatrix), LightProjectionMatrix);
	output.Position2D = output.Position;
	return output;
}

float4 ShadowMapPixelShader(SVVertexShaderOutput input) : COLOR0
{
	float4 projTexCoord = input.Position2D / input.Position2D.w;
	projTexCoord.xy = 0.5 * projTexCoord.xy + float2(0.5, 0.5);
	projTexCoord.y = 1.0 - projTexCoord.y;
	float depth = 1.0 - projTexCoord.z;
	float4 color = (depth > 0) ? depth : 0;

	return color;

}

//lab08******************************
//struct VertexShaderInput
//{
//	float4 Position : POSITION0;
//	float4 Normal : NORMAL0;
//	float2 TexCoord : TEXCOORD0;
//};
//
//struct VertexShaderOutput
//{
//	float4 Position : POSITION0;
//	float3 Normal	: TEXCOORD0;
//	float2 TexCoord : TEXCOORD1;
//	float3 WorldPosition : TEXCOORD2;
//};
//
//VertexShaderOutput VSFunction(VertexShaderInput input)
//{
//	VertexShaderOutput output;
//	float4 worldPos = mul(input.Position, World);
//	float4 viewPosition = mul(worldPos, View);
//	output.Position = mul(viewPosition, Projection);
//	output.WorldPosition = worldPos.xyz;
//	output.Normal = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
//	output.TexCoord = input.TexCoord;
//	return output;
//}
//
//float4 PSFunction(VertexShaderOutput input) : COLOR0
//{
//	//float4 color = float4(1, 1, 1, 1);
//
//	//step1
//	float4 projTexCoord = mul(mul(float4(input.WorldPosition, 1), LightViewMatrix), LightProjectionMatrix);
//	//step2
//	projTexCoord = projTexCoord / projTexCoord.w;
//	//step3
//	projTexCoord.xy = 0.5 * projTexCoord.xy + float2(0.5, 0.5);
//	//step4
//	projTexCoord.y = 1.0 - projTexCoord.y;
//	//step5
//	float depth = 1.0 - projTexCoord.z;
//	//step6
//	float4 color = tex2D(TextureSampler, projTexCoord.xy);
//	//step7
//	if (color.x == 0 && color.y == 1 && color.z == 1)
//			color.xyz = float3(0, 0, 0);
//	//step8(Option)
//	float3 N = normalize(input.Normal);
//	float3 L = normalize(LightPosition - input.WorldPosition);
//	if (dot(L, N) < 0) color = 0;
//	return color;
//}
//************************************
sampler tsampler1 = sampler_state {
	texture = <normalMap>;
	magfilter = LINEAR; // None, POINT, LINEAR, Anisotropic
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Wrap; // Clamp, Mirror, MirrorOnce, Wrap, Border
	AddressV = Wrap;
};
struct VertexInput {
	float4 Position: POSITION;
	float2 TexCoord: TEXCOORD;
	float4 Normal: NORMAL;
	float4 Tangent: TANGENT0;
	float4 Binormal: BINORMAL0;
};
struct VertexShaderOutput {
	float4 Position: POSITION;
	float3 Normal : TEXCOORD0;
	float3 Tangent: TEXCOORD1;
	float3 Binormal: TEXCOORD2;
	float2 TexCoord: TEXCOORD3;
	float3 Position3D : TEXCOORD4; //WorldPosition
};
VertexShaderOutput BumpMapVertexShader(VertexInput input)
{
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.Position3D = worldPosition.xyz;
	output.Normal = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
	output.Tangent = normalize(mul(input.Tangent, WorldInverseTranspose).xyz);
	output.Binormal = normalize(mul(input.Binormal, WorldInverseTranspose).xyz);
	output.TexCoord = input.TexCoord;
	return output;
}
float4 BumpMapPixelShader(VertexShaderOutput input) : COLOR0
{
	float3 N = input.Normal;
	float3 V = normalize(CameraPosition - input.Position3D.xyz);
	float3 L = normalize(LightPosition - input.Position3D.xyz);
	float3 R = reflect(-L, N);
	// *** Lab 7 ***
	float3 T = input.Tangent;
	float3 B = input.Binormal;
	float3 H = normalize(L + V);

	float3 normalTex = (tex2D(tsampler1, input.TexCoord).xyz
		- float3(0.5, 0.5, 0.5)) * 2.0;
	// *** Lab7 Version ***
	float3 bumpNormal = N + normalTex.x * T + normalTex.y * B;
	// *******************

	float4 ambient = AmbientColor * AmbientIntensity;
	float4 diffuse = DiffuseIntensity * DiffuseColor * max(0, (dot(bumpNormal, L)));
	float4 specular = pow(max(0, dot(H, bumpNormal)), Shininess) * SpecularColor * SpecularIntensity;

	float4 color = saturate(ambient + diffuse + specular);
	color.a = 1;

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
//Lab08******************
//technique Technique1
//{
//	pass Pass1
//	{
//		VertexShader = compile vs_4_0 VSFunction();
//		PixelShader = compile ps_4_0 PSFunction();
//	}
//}

technique BumpMap {
	pass Pass1
	{
		VertexShader = compile vs_4_0 BumpMapVertexShader();
		PixelShader = compile ps_4_0 BumpMapPixelShader();
	}
};