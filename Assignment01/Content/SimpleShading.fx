float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;
float4 AmbientColor;
float AmbientIntensity;
float4 DiffuseColor;
float DiffuseIntensity;
// *** Lab 4
float3 LightPosition;
float3 CameraPosition;
float Shininess;
float4 SpecularColor;
float SpecularIntensity;

struct VertexInput {
	float4 Position: POSITION;
	float4 Normal: NORMAL;
};

struct VertexShaderOutput {
	float4 Position : POSITION;
	float4 Color : COLOR;
	float4 Normal : TEXCOORD0;
	float4 WorldPosition: TEXCOORD1;
};

//F1-Gourand-vs
VertexShaderOutput GourandVertexShaderFunction(VertexInput input) {
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.WorldPosition = 0;
	output.Normal = 0;
	float3 N = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
	float3 V = normalize(CameraPosition - worldPosition.xyz);
	float3 L = normalize(LightPosition);
	float3 R = reflect(-L, N);
	float4 ambient = AmbientColor * AmbientIntensity;
	float4 diffuse = DiffuseIntensity * DiffuseColor * max(0, dot(N, L));
	float4 specular = SpecularIntensity * SpecularColor * pow( max(0, dot(V, R)), Shininess);
	//if (dot(N, L) <  0) specular = 0;
	output.Color = saturate(ambient + diffuse +specular);
	output.Color.w = 1;
	return output;
}
//F2-Phong-
VertexShaderOutput PhongVertexShader(VertexInput input) {
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.WorldPosition = worldPosition;
	output.Normal = mul(input.Normal, WorldInverseTranspose);
	output.Color = 0;
	return output;
}

VertexShaderOutput PhongBlinnVertexShader(VertexInput input) {
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.WorldPosition = worldPosition;
	output.Normal = mul(input.Normal, WorldInverseTranspose);

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR
{
	return input.Color;
}
float4 PhongPixelShaderFunction(VertexShaderOutput input) : COLOR
{
	float3 N = normalize(input.Normal.xyz);
	float3 V = normalize(CameraPosition - input.WorldPosition.xyz);
	float3 L = normalize(LightPosition);
	float3 R = reflect(-L, N);
	float4 ambient = AmbientColor * AmbientIntensity;
	float4 diffuse = DiffuseIntensity * DiffuseColor * max(0, dot(N, L));
	float4 specular = SpecularIntensity * SpecularColor * pow(max(0, dot(V, R)), Shininess);
	float4 color = saturate(ambient + diffuse + specular);
	color.a = 1;
	return color;
}

float4 PhongBlinnShaderFunction(VertexShaderOutput input) : COLOR
{
	float4 color = float4(0,0,0,0);
	float3 N = normalize(input.Normal.xyz);
	float3 V = normalize(CameraPosition - input.WorldPosition.xyz);
	float3 L = normalize(LightPosition);
	float3 R = reflect(-L, N);
	float4 ambient = AmbientColor * AmbientIntensity;
	float4 diffuse = DiffuseIntensity * DiffuseColor * max(0, dot(N, L));

	color += ambient + diffuse;
	float3 H = normalize(L + V);
	color += SpecularColor * SpecularIntensity * max(0, pow(max(0, dot(H, N)), Shininess));
	return color;
}

float4 SchlickShaderFunction(VertexShaderOutput input) : COLOR
{
	float4 color = float4(0,0,0,0);
	float3 N = normalize(input.Normal.xyz);
	float3 V = normalize(CameraPosition - input.WorldPosition.xyz);
	float3 L = normalize(LightPosition);
	float3 R = reflect(-L, N);
	float4 ambient = AmbientColor * AmbientIntensity;
	float4 diffuse = DiffuseIntensity * DiffuseColor * max(0, dot(N, L));

	color += ambient + diffuse;
	float4 t = max(0, dot(V, R));
	color += SpecularColor * SpecularIntensity * t / (Shininess - t * Shininess + t);
	return color;
}

float4 ToonPixelShaderFunction(VertexShaderOutput input) : COLOR
{
	float3 N = normalize(input.Normal.xyz);
	float3 V = normalize(CameraPosition - input.WorldPosition.xyz);
	float3 L = normalize(LightPosition);
	float3 R = reflect(-L, N);
	float D = dot(V, R);
	if (D < -0.7)
	{
		return float4(0, 0, 0, 1);
	}
	else if (D < 0.2)
	{
		return float4(0.25, 0.25, 0.25, 1);
	}
	else if (D < 0.97)
	{
		return float4(0.5, 0.5, 0.5, 1);
	}
	else
	{
		return float4(1, 1, 1, 1);
	}
}

float4 HalfLifeShaderFunction(VertexShaderOutput input) : COLOR
{
	float4 color = float4(0,0,0,0);
	float3 N = normalize(input.Normal.xyz);
	float3 V = normalize(CameraPosition - input.WorldPosition.xyz);
	float3 L = normalize(LightPosition);
	float3 R = reflect(-L, N);
	float4 ambient = AmbientColor * AmbientIntensity;
	float4 diffuse = DiffuseIntensity * DiffuseColor * (0.5 * pow(dot(N, L), 2));

	color += ambient + diffuse;
	float4 t = max(0, dot(V, R));
	color += SpecularColor * SpecularIntensity * t / (Shininess - t * Shininess + t);
	return color;
}



technique Gouraud
{
	pass Pass1 {
		VertexShader = compile vs_4_0 GourandVertexShaderFunction();
		PixelShader = compile ps_4_0 PixelShaderFunction();
	}
}
technique Phong
{
	pass Pass1 {
		VertexShader = compile vs_4_0 PhongVertexShader();
		PixelShader = compile ps_4_0 PhongPixelShaderFunction();
	}
}
technique PhongBlinn
{
	pass Pass1 {
		VertexShader = compile vs_4_0 PhongBlinnVertexShader();
		PixelShader = compile ps_4_0 PhongBlinnShaderFunction();
	}
}
technique Schlick
{
	pass Pass1 {
		VertexShader = compile vs_4_0 PhongVertexShader();
		PixelShader = compile ps_4_0 SchlickShaderFunction();
	}
}
technique Toon
{
	pass Pass1 {
		VertexShader = compile vs_4_0 PhongVertexShader();
		PixelShader = compile ps_4_0 ToonPixelShaderFunction();
	}
}
technique HalfLife
{
	pass Pass1 {
		VertexShader = compile vs_4_0 PhongVertexShader();
		PixelShader = compile ps_4_0 HalfLifeShaderFunction();
	}
}


