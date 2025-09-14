// Made with Amplify Shader Editor v1.9.8.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Turishader/ElectricText"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

        [Header(Color)][Header((Color intensity for tint color))]_ColorIntensity("Color Intensity", Float) = 1
        [Header(Ray parameters)]_Scale("Scale", Float) = 1
        _Raythreshold("Ray threshold", Range( 0 , 1)) = 0.95
        _Innetraythreshold("Innet ray threshold", Range( 0 , 1)) = 0.7
        [KeywordEnum(OuterOnly,InnerOnly,All)] _InnerFill("InnerFill", Float) = 0
        [HideInInspector] _texcoord( "", 2D ) = "white" {}

    }

    SubShader
    {
		LOD 0

        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }

        Stencil
        {
        	Ref [_Stencil]
        	ReadMask [_StencilReadMask]
        	WriteMask [_StencilWriteMask]
        	Comp [_StencilComp]
        	Pass [_StencilOp]
        }


        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend One OneMinusSrcAlpha
        ColorMask [_ColorMask]

        
        Pass
        {
            Name "Default"
        CGPROGRAM
            #define ASE_VERSION 19801

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.5

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "UnityShaderVariables.cginc"
            #define ASE_NEEDS_FRAG_COLOR
            #pragma shader_feature_local _INNERFILL_OUTERONLY _INNERFILL_INNERONLY _INNERFILL_ALL


            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float4  mask : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
                
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            float _UIMaskSoftnessX;
            float _UIMaskSoftnessY;

            uniform float _ColorIntensity;
            uniform float _Raythreshold;
            uniform float _Scale;
            uniform float _Innetraythreshold;
            float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
            float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
            float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
            float snoise( float2 v )
            {
            	const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
            	float2 i = floor( v + dot( v, C.yy ) );
            	float2 x0 = v - i + dot( i, C.xx );
            	float2 i1;
            	i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
            	float4 x12 = x0.xyxy + C.xxzz;
            	x12.xy -= i1;
            	i = mod2D289( i );
            	float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
            	float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
            	m = m * m;
            	m = m * m;
            	float3 x = 2.0 * frac( p * C.www ) - 1.0;
            	float3 h = abs( x ) - 0.5;
            	float3 ox = floor( x + 0.5 );
            	float3 a0 = x - ox;
            	m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
            	float3 g;
            	g.x = a0.x * x0.x + h.x * x0.y;
            	g.yz = a0.yz * x12.xz + h.yz * x12.yw;
            	return 130.0 * dot( m, g );
            }
            


            v2f vert(appdata_t v )
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                

                v.vertex.xyz +=  float3( 0, 0, 0 ) ;

                float4 vPosition = UnityObjectToClipPos(v.vertex);
                OUT.worldPosition = v.vertex;
                OUT.vertex = vPosition;

                float2 pixelSize = vPosition.w;
                pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

                float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                float2 maskUV = (v.vertex.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);
                OUT.texcoord = v.texcoord;
                OUT.mask = float4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));

                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN ) : SV_Target
            {
                //Round up the alpha color coming from the interpolator (to 1.0/256.0 steps)
                //The incoming alpha could have numerical instability, which makes it very sensible to
                //HDR color transparency blend, when it blends with the world's texture.
                const half alphaPrecision = half(0xff);
                const half invAlphaPrecision = half(1.0/alphaPrecision);
                IN.color.a = round(IN.color.a * alphaPrecision)*invAlphaPrecision;

                float2 uv_MainTex = IN.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                float4 tex2DNode404 = tex2D( _MainTex, uv_MainTex );
                float temp_output_2_0_g1 = 10.0;
                float temp_output_534_0 = ( round( ( _Time.y * temp_output_2_0_g1 ) ) / temp_output_2_0_g1 );
                float2 texCoord528 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
                float2 panner532 = ( temp_output_534_0 * float2( 1,1 ) + texCoord528);
                float simplePerlin2D527 = snoise( panner532*( _Scale * 40.0 ) );
                simplePerlin2D527 = simplePerlin2D527*0.5 + 0.5;
                float smoothstepResult526 = smoothstep( _Raythreshold , 1.0 , ( 1.0 - abs( (-1.0 + (( tex2DNode404.a - ( pow( simplePerlin2D527 , 2.0 ) * 0.3 ) ) - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) ) ));
                float simplePerlin2D538 = snoise( panner532*( _Scale * 80.0 ) );
                simplePerlin2D538 = simplePerlin2D538*0.5 + 0.5;
                float smoothstepResult543 = smoothstep( 0.975 , 1.0 , ( 1.0 - abs( (-1.0 + (( tex2DNode404.a - ( pow( simplePerlin2D538 , 3.0 ) * 0.2 ) ) - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) ) ));
                float temp_output_560_0 = ( smoothstepResult526 + smoothstepResult543 );
                float2 texCoord555 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
                float2 texCoord3_g2 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
                float2 panner1_g2 = ( temp_output_534_0 * float2( 0.1,0 ) + texCoord3_g2);
                float temp_output_13_0_g2 = ( _Scale * 50.0 );
                float simplePerlin2D7_g2 = snoise( ( panner1_g2 + float2( 0.01,0 ) )*temp_output_13_0_g2 );
                simplePerlin2D7_g2 = simplePerlin2D7_g2*0.5 + 0.5;
                float simplePerlin2D2_g2 = snoise( panner1_g2*temp_output_13_0_g2 );
                simplePerlin2D2_g2 = simplePerlin2D2_g2*0.5 + 0.5;
                float simplePerlin2D8_g2 = snoise( ( panner1_g2 + float2( 0,0.01 ) )*temp_output_13_0_g2 );
                simplePerlin2D8_g2 = simplePerlin2D8_g2*0.5 + 0.5;
                float4 appendResult9_g2 = (float4(( simplePerlin2D7_g2 - simplePerlin2D2_g2 ) , ( simplePerlin2D8_g2 - simplePerlin2D2_g2 ) , 0.0 , 0.0));
                float smoothstepResult548 = smoothstep( _Innetraythreshold , 1.0 , tex2D( _MainTex, ( float4( texCoord555, 0.0 , 0.0 ) + ( appendResult9_g2 * 0.002 ) ).xy ).a);
                float temp_output_549_0 = saturate( smoothstepResult548 );
                #if defined( _INNERFILL_OUTERONLY )
                float staticSwitch559 = temp_output_560_0;
                #elif defined( _INNERFILL_INNERONLY )
                float staticSwitch559 = temp_output_549_0;
                #elif defined( _INNERFILL_ALL )
                float staticSwitch559 = ( temp_output_560_0 + temp_output_549_0 );
                #else
                float staticSwitch559 = temp_output_560_0;
                #endif
                float4 appendResult497 = (float4(( IN.color * _ColorIntensity ).rgb , ( IN.color.a * saturate( staticSwitch559 ) )));
                

                half4 color = appendResult497;

                #ifdef UNITY_UI_CLIP_RECT
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
                color.a *= m.x * m.y;
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                color.rgb *= color.a;

                return color;
            }
        ENDCG
        }
    }
    CustomEditor "AmplifyShaderEditor.MaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19801
Node;AmplifyShaderEditor.SimpleTimeNode;533;-1408,-496;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;535;-1376,-400;Inherit;False;Constant;_Float0;Float 0;8;0;Create;True;0;0;0;False;0;False;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;534;-1200,-480;Inherit;False;SectionsRemap;-1;;1;e07d5d5a126b6f243ac17cc867746f65;0;2;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;528;-1120,-640;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;564;-1696,-96;Inherit;False;Property;_Scale;Scale;1;1;[Header];Create;True;1;Ray parameters;0;0;False;0;False;1;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;532;-880,-544;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;1,1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;565;-1440,-272;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;40;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;566;-1440,-176;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;80;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;527;-640,-576;Inherit;False;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;40;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;538;-640,-464;Inherit;False;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;80;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;536;-400,-576;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;540;-400,-464;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;403;-1008,-32;Inherit;False;0;0;_MainTex;Shader;False;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;530;-224,-576;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;541;-224,-464;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;404;-352,-320;Inherit;True;Property;_TextureSample0;Texture Sample 0;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;567;-1520,-32;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;50;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;557;-1056,512;Inherit;False;Constant;_Float1;Float 1;1;0;Create;True;0;0;0;False;0;False;0.002;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;553;-1216,288;Inherit;False;NoiseNormal;-1;;2;d7e016d03fc9f6c48aec9a4d96d9b9cb;0;4;15;FLOAT;0;False;14;FLOAT2;0.1,0;False;13;FLOAT;50;False;12;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;531;16,-576;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;539;16,-464;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;556;-864,256;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;555;-1280,96;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;522;240,-656;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;542;240,-464;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;554;-653.4877,158.9694;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.AbsOpNode;544;432,-464;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;523;432,-560;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;524;560,-560;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;545;560,-464;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;558;464,-352;Inherit;False;Property;_Raythreshold;Ray threshold;2;0;Create;True;0;0;0;False;0;False;0.95;0.95;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;552;-352,-128;Inherit;True;Property;_TextureSample1;Texture Sample 0;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;563;-336,80;Inherit;False;Property;_Innetraythreshold;Innet ray threshold;3;0;Create;True;0;0;0;False;0;False;0.7;0.95;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;526;736,-608;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.95;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;543;736,-464;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.975;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;548;-16,0;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.7;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;560;944,-544;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;549;224,0;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;562;1104,-208;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;559;1264,-368;Inherit;False;Property;_InnerFill;InnerFill;4;0;Create;True;0;0;0;False;0;False;0;0;0;True;;KeywordEnum;3;OuterOnly;InnerOnly;All;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;515;1536,-864;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;517;1536,-688;Inherit;False;Property;_ColorIntensity;Color Intensity;0;1;[Header];Create;True;2;Color;(Color intensity for tint color);0;0;False;0;False;1;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;547;1600,-464;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;516;1840,-704;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;568;1808,-544;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;497;2000,-656;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;397;2208,-656;Float;False;True;-1;3;AmplifyShaderEditor.MaterialInspector;0;3;Turishader/ElectricText;5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;False;True;3;1;False;;10;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;True;True;True;True;True;0;True;_ColorMask;False;False;False;False;False;False;False;True;True;0;True;_Stencil;255;True;_StencilReadMask;255;True;_StencilWriteMask;0;True;_StencilComp;0;True;_StencilOp;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;0;True;unity_GUIZTestMode;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;False;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;534;1;533;0
WireConnection;534;2;535;0
WireConnection;532;0;528;0
WireConnection;532;1;534;0
WireConnection;565;0;564;0
WireConnection;566;0;564;0
WireConnection;527;0;532;0
WireConnection;527;1;565;0
WireConnection;538;0;532;0
WireConnection;538;1;566;0
WireConnection;536;0;527;0
WireConnection;540;0;538;0
WireConnection;530;0;536;0
WireConnection;541;0;540;0
WireConnection;404;0;403;0
WireConnection;567;0;564;0
WireConnection;553;15;534;0
WireConnection;553;13;567;0
WireConnection;531;0;404;4
WireConnection;531;1;530;0
WireConnection;539;0;404;4
WireConnection;539;1;541;0
WireConnection;556;0;553;0
WireConnection;556;1;557;0
WireConnection;522;0;531;0
WireConnection;542;0;539;0
WireConnection;554;0;555;0
WireConnection;554;1;556;0
WireConnection;544;0;542;0
WireConnection;523;0;522;0
WireConnection;524;0;523;0
WireConnection;545;0;544;0
WireConnection;552;0;403;0
WireConnection;552;1;554;0
WireConnection;526;0;524;0
WireConnection;526;1;558;0
WireConnection;543;0;545;0
WireConnection;548;0;552;4
WireConnection;548;1;563;0
WireConnection;560;0;526;0
WireConnection;560;1;543;0
WireConnection;549;0;548;0
WireConnection;562;0;560;0
WireConnection;562;1;549;0
WireConnection;559;1;560;0
WireConnection;559;0;549;0
WireConnection;559;2;562;0
WireConnection;547;0;559;0
WireConnection;516;0;515;0
WireConnection;516;1;517;0
WireConnection;568;0;515;4
WireConnection;568;1;547;0
WireConnection;497;0;516;0
WireConnection;497;3;568;0
WireConnection;397;0;497;0
ASEEND*/
//CHKSM=3D929E5DB4D39AB48FEE04884629FB2031CCEFE0