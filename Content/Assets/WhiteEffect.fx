#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0
    #define PS_SHADERMODEL ps_4_0
#endif

float enabled;

Texture2D SpriteTexture;
sampler2D SpriteTextureSampler = sampler_state { Texture = <SpriteTexture>; };

float4 overlayColor;

struct ShaderInput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(ShaderInput input) : COLOR
{
    float4 color = tex2D(SpriteTextureSampler, input.TextureCoordinates) * input.Color;

    if (enabled < 0.5)
        return color;

    if (color.a > 0)
        return float4(overlayColor.rgb, color.a);

    return color;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}