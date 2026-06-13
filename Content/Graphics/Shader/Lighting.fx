#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

//	This is the texture that will be supplied by the SpriteBatch.Draw() command
//	It should be the texture of the RenderTarget2D of the game render.
Texture2D RenderTargetTexture;

//	This texture is the texture of the RenderTarget2D where we rendered all 
//	the light masks on.
Texture2D MaskTexture;

//	This is the sampler for the render target texture.
sampler2D RenderTargetSampler = sampler_state
{
	Texture = <RenderTargetTexture>;
};

//	This is the sampler for the mask texture.
sampler2D MaskSampler = sampler_state
{
	Texture = <MaskTexture>;
};

//	No structs defined in this one.  We only need to know about
//	the texture coordinates.  So instead of defining a struct and
//	using that, we can just change the signature of our pixel shader
//	so the parameter is a float2 that uses the ': TEXCOORD0' to 
//	semantically link the paramter as the texture coordinate value.
float4 MainPS(float2 textureCoords : TEXCOORD0) : COLOR
{
    float4 pixelColor = tex2D(RenderTargetSampler, textureCoords);
    float4 lightColor = tex2D(MaskSampler, textureCoords);

    // Multiply the lightColor by 2. This means:
    // A neutral light color of 0.5 becomes 1.0 (no change to the base image)
    // A bright light color of 1.0 becomes 2.0 (doubles the brightness of the base image)
    float4 finalColor = pixelColor * (lightColor * 2.0);

    // Clamp the final result so it stays within valid rendering bounds
    return saturate(finalColor);
}

technique LightDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
