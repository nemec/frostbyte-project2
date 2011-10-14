float4x4 xWorldViewProjection;
float4x4 xWorld;
float xCurrentTime;
Texture xTexture1;
Texture xTexture2;
float xFadeStartPercent;
float xChangePicPercent;

sampler TextureSampler1 = sampler_state { texture = <xTexture1> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};
sampler TextureSampler2 = sampler_state { texture = <xTexture2> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

//This is the struct that will be passed from the vertex shader to the pixel shader
struct FadeAtXPercentVTP
{
    float4 Position     : POSITION;
    float2 TexCoord     : TEXCOORD0;
	float FadeAmount    : COLOR0;
};

//This is the struct that will be passed from the pixel shader to be displayed on the screen
struct PixelToFrame
{
    float4 Color        : COLOR0;
};

//This is the vertex shader for ParticleFadeAt20Percent
//This function takes a position and normal from the application
FadeAtXPercentVTP FadeAtXPercentVS( float3 inPos : POSITION,
                                      float3 inVel : TEXCOORD0,
									  float3 inAcc : TEXCOORD1,
									  float2 inTex : TEXCOORD2,
									  float startTime: TEXCOORD3,
									  float timeToLive: TEXCOORD4)
{
	//initilize output object
    FadeAtXPercentVTP Output = (FadeAtXPercentVTP)0;
    
	//Time Since Creating of this Particle in Seconds
	float deltaT = (xCurrentTime - startTime)/1000.0;

	//Calculate Position
	float3 newPos = inPos + inVel * deltaT + .5 * inAcc * deltaT * deltaT;
    Output.Position = mul(float4(newPos,1), xWorldViewProjection);

	Output.TexCoord = inTex;

	float percentTimeRemaining = (timeToLive - (xCurrentTime - startTime))/timeToLive;
	Output.FadeAmount = (1 - percentTimeRemaining) - xFadeStartPercent;

    return Output;
}

//This is the pixel shader for ParticleFadeAt20Percent
//This function takes the interpolated output from the vertex shader 
PixelToFrame FadeAtXPercentPS(FadeAtXPercentVTP PSIn)
{
	//initilize output object
    PixelToFrame Output = (PixelToFrame)0;

	//store color for outputing to the screen
	Output.Color = tex2D(TextureSampler1, PSIn.TexCoord);
	Output.Color.a -= PSIn.FadeAmount * (1 / ((1 - xFadeStartPercent) + .001));

    return Output;
}


technique FadeAtXPercent
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 FadeAtXPercentVS();
        PixelShader = compile ps_2_0 FadeAtXPercentPS();
    }
}











//This is the struct that will be passed from the vertex shader to the pixel shader
struct FadeAtXPercentAndChangeAtYPercentVTP
{
    float4 Position     : POSITION;
    float2 TexCoord     : TEXCOORD0;
	float FadeAmount    : COLOR0;
	float ChangePic     : COLOR1;
};


//This is the vertex shader for StartFadeAtXPercentAndChangePictureAtYPercent
//This function takes a position and normal from the application
FadeAtXPercentAndChangeAtYPercentVTP ChangePicAndFadeAtPercentVS( float3 inPos : POSITION,
                                      float3 inVel : TEXCOORD0,
									  float3 inAcc : TEXCOORD1,
									  float2 inTex : TEXCOORD2,
									  float startTime: TEXCOORD3,
									  float timeToLive: TEXCOORD4)
{
	//initilize output object
    FadeAtXPercentAndChangeAtYPercentVTP Output = (FadeAtXPercentAndChangeAtYPercentVTP)0;
    
	//Time Since Creating of this Particle in Seconds
	float deltaT = (xCurrentTime - startTime)/1000.0;

	//Calculate Position
	float3 newPos = inPos + inVel * deltaT + .5 * inAcc * deltaT * deltaT;
    Output.Position = mul(float4(newPos,1), xWorldViewProjection);

	Output.TexCoord = inTex;

	float percentTimeRemaining = (timeToLive - (xCurrentTime - startTime))/timeToLive;
	Output.FadeAmount = (1 - percentTimeRemaining) - xFadeStartPercent;
	Output.ChangePic = (1 - percentTimeRemaining) - xChangePicPercent;

    return Output;
}

//This is the pixel shader for StartFadeAtXPercentAndChangePictureAtYPercent
//This function takes the interpolated output from the vertex shader 
PixelToFrame ChangePicAndFadeAtPercentPS(FadeAtXPercentAndChangeAtYPercentVTP PSIn)
{
	//initilize output object
    PixelToFrame Output = (PixelToFrame)0;


	if (PSIn.ChangePic)
	{
		Output.Color = tex2D(TextureSampler2, PSIn.TexCoord);
	}
	else
	{
		Output.Color = tex2D(TextureSampler1, PSIn.TexCoord);
	}
	Output.Color.a -= PSIn.FadeAmount * (1 / ((1 - xFadeStartPercent) + .001));

    return Output;
}


technique ChangePicAndFadeAtPercent
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 ChangePicAndFadeAtPercentVS();
        PixelShader = compile ps_2_0 ChangePicAndFadeAtPercentPS();
    }
}