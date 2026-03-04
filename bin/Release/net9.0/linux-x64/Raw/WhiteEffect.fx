//======================================================
// MonoGame White Override Shader
// Turns a sprite fully white while preserving alpha
//======================================================

// Required texture input
texture Texture;

// Sampler for SpriteBatch
sampler TextureSampler = sampler_state
{
    Texture = <Texture>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

// Controls whether we force white (1 = on, 0 = off)
float MakeWhite = 1.0f;

// SpriteBatch provides this automatically
float4x4 MatrixTransform;


//====================
// Vertex Shader
//====================
struct VertexInput
{
    float4 Position : POSITION0;
    float4 Color    : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

struct VertexOutput
{
    float4 Position : SV_POSITION;
    float4 Color    : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

VertexOutput MainVS(VertexInput input)
{
    VertexOutput output;

    output.Position = mul(input.Position, MatrixTransform);
    output.Color = input.Color;
    output.TexCoord = input.TexCoord;

    return output;
}


//====================
// Pixel Shader
//====================
float4 MainPS(VertexOutput input) : COLOR0
{
    float4 texColor = tex2D(TextureSampler, input.TexCoord);

    // Multiply by SpriteBatch color
    texColor *= input.Color;

    // If MakeWhite is enabled, force RGB to white
    if (MakeWhite > 0.5f)
    {
        return float4(1.0f, 1.0f, 1.0f, texColor.a);
    }

    return texColor;
}


//====================
// Technique
//====================
technique SpriteWhite
{
    pass Pass1
    {
        VertexShader = compile vs_4_0_level_9_1 MainVS();
        PixelShader  = compile ps_4_0_level_9_1 MainPS();
    }
}