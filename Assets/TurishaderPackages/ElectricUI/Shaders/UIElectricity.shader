// Upgrade NOTE: upgraded instancing buffer 'TurishaderUIElectricity' to new syntax.

// Made with Amplify Shader Editor v1.9.8.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Turishader/UIElectricity"
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

        [Header(Color)][Header((Color intensity for tint color))]_ColorIntensity("ColorIntensity", Float) = 1
        [Header(Ray parameters)]_Frequency("Frequency", Float) = 1
        _Width("Width", Range( 0 , 1)) = 0.5
        _Borderfade("Border fade", Range( 0 , 1)) = 0.5
        _Amplitude("Amplitude", Range( 0 , 1)) = 1
        [Toggle]_Tileable("Tileable", Float) = 0
        _Variationseed("Variation seed", Float) = 0

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
            #pragma multi_compile_instancing


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
            uniform float _Tileable;
            UNITY_INSTANCING_BUFFER_START(TurishaderUIElectricity)
            	UNITY_DEFINE_INSTANCED_PROP(float, _Width)
#define _Width_arr TurishaderUIElectricity
            	UNITY_DEFINE_INSTANCED_PROP(float, _Borderfade)
#define _Borderfade_arr TurishaderUIElectricity
            	UNITY_DEFINE_INSTANCED_PROP(float, _Variationseed)
#define _Variationseed_arr TurishaderUIElectricity
            	UNITY_DEFINE_INSTANCED_PROP(float, _Frequency)
#define _Frequency_arr TurishaderUIElectricity
            	UNITY_DEFINE_INSTANCED_PROP(float, _Amplitude)
#define _Amplitude_arr TurishaderUIElectricity
            UNITY_INSTANCING_BUFFER_END(TurishaderUIElectricity)
            		float2 voronoihash60( float2 p )
            		{
            			
            			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
            			return frac( sin( p ) *43758.5453);
            		}
            
            		float voronoi60( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
            		{
            			float2 n = floor( v );
            			float2 f = frac( v );
            			float F1 = 8.0;
            			float F2 = 8.0; float2 mg = 0;
            			for ( int j = -2; j <= 2; j++ )
            			{
            				for ( int i = -2; i <= 2; i++ )
            			 	{
            			 		float2 g = float2( i, j );
            			 		float2 o = voronoihash60( n + g );
            					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
            					float d = 0.5 * dot( r, r );
            			 		if( d<F1 ) {
            			 			F2 = F1;
            			 			F1 = d; mg = g; mr = r; id = o;
            			 		} else if( d<F2 ) {
            			 			F2 = d;
            			
            			 		}
            			 	}
            			}
            			
            F1 = 8.0;
            for ( int j = -2; j <= 2; j++ )
            {
            for ( int i = -2; i <= 2; i++ )
            {
            float2 g = mg + float2( i, j );
            float2 o = voronoihash60( n + g );
            		o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
            float d = dot( 0.5 * ( r + mr ), normalize( r - mr ) );
            F1 = min( F1, d );
            }
            }
            return F1;
            		}
            
            		float2 voronoihash14( float2 p )
            		{
            			p = p - 6 * floor( p / 6 );
            			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
            			return frac( sin( p ) *43758.5453);
            		}
            
            		float voronoi14( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
            		{
            			float2 n = floor( v );
            			float2 f = frac( v );
            			float F1 = 8.0;
            			float F2 = 8.0; float2 mg = 0;
            			for ( int j = -2; j <= 2; j++ )
            			{
            				for ( int i = -2; i <= 2; i++ )
            			 	{
            			 		float2 g = float2( i, j );
            			 		float2 o = voronoihash14( n + g );
            					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
            					float d = 0.5 * dot( r, r );
            			 		if( d<F1 ) {
            			 			F2 = F1;
            			 			F1 = d; mg = g; mr = r; id = o;
            			 		} else if( d<F2 ) {
            			 			F2 = d;
            			
            			 		}
            			 	}
            			}
            			
            F1 = 8.0;
            for ( int j = -2; j <= 2; j++ )
            {
            for ( int i = -2; i <= 2; i++ )
            {
            float2 g = mg + float2( i, j );
            float2 o = voronoihash14( n + g );
            		o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
            float d = dot( 0.5 * ( r + mr ), normalize( r - mr ) );
            F1 = min( F1, d );
            }
            }
            return F1;
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

                float4 break64 = ( IN.color * _ColorIntensity * IN.color.a );
                float _Width_Instance = UNITY_ACCESS_INSTANCED_PROP(_Width_arr, _Width);
                float lerpResult47 = lerp( 0.99 , 0.8 , _Width_Instance);
                float2 texCoord11 = IN.texcoord.xy * float2( 1,2 ) + float2( 0,-1 );
                float lerpResult37 = lerp( 1.0 , lerpResult47 , ( min( texCoord11.x , ( 1.0 - texCoord11.x ) ) * 2.0 ));
                float _Borderfade_Instance = UNITY_ACCESS_INSTANCED_PROP(_Borderfade_arr, _Borderfade);
                float lerpResult55 = lerp( lerpResult47 , lerpResult37 , _Borderfade_Instance);
                float mulTime20 = _Time.y * -3.0;
                float temp_output_2_0_g5 = 5.0;
                float _Variationseed_Instance = UNITY_ACCESS_INSTANCED_PROP(_Variationseed_arr, _Variationseed);
                float temp_output_52_0 = ( ( round( ( mulTime20 * temp_output_2_0_g5 ) ) / temp_output_2_0_g5 ) + _Variationseed_Instance );
                float time60 = temp_output_52_0;
                float2 voronoiSmoothId60 = 0;
                float _Frequency_Instance = UNITY_ACCESS_INSTANCED_PROP(_Frequency_arr, _Frequency);
                float mulTime25 = _Time.y * -5.0;
                float temp_output_2_0_g4 = 2.0;
                float temp_output_24_0 = ( ( ( texCoord11.x + 0.0 ) * _Frequency_Instance ) + ( round( ( mulTime25 * temp_output_2_0_g4 ) ) / temp_output_2_0_g4 ) + _Variationseed_Instance );
                float2 temp_cast_0 = (temp_output_24_0).xx;
                float2 coords60 = temp_cast_0 * 1.0;
                float2 id60 = 0;
                float2 uv60 = 0;
                float fade60 = 0.5;
                float voroi60 = 0;
                float rest60 = 0;
                for( int it60 = 0; it60 <2; it60++ ){
                voroi60 += fade60 * voronoi60( coords60, time60, id60, uv60, 0,voronoiSmoothId60 );
                rest60 += fade60;
                coords60 *= 2;
                fade60 *= 0.5;
                }//Voronoi60
                voroi60 /= rest60;
                float time14 = temp_output_52_0;
                float2 voronoiSmoothId14 = 0;
                float2 temp_cast_1 = (temp_output_24_0).xx;
                float2 coords14 = temp_cast_1 * 6.0;
                float2 id14 = 0;
                float2 uv14 = 0;
                float fade14 = 0.5;
                float voroi14 = 0;
                float rest14 = 0;
                for( int it14 = 0; it14 <2; it14++ ){
                voroi14 += fade14 * voronoi14( coords14, time14, id14, uv14, 0,voronoiSmoothId14 );
                rest14 += fade14;
                coords14 *= 2;
                fade14 *= 0.5;
                }//Voronoi14
                voroi14 /= rest14;
                float _Amplitude_Instance = UNITY_ACCESS_INSTANCED_PROP(_Amplitude_arr, _Amplitude);
                float lerpResult57 = lerp( 0.0 , 0.5 , _Amplitude_Instance);
                float4 appendResult63 = (float4(break64.r , break64.g , break64.b , ( IN.color.a * step( lerpResult55 , ( 1.0 - abs( ( (( lerpResult57 * -1.0 ) + (saturate( ( (( _Tileable )?( voroi14 ):( voroi60 )) * 2.0 ) ) - 0.0) * (lerpResult57 - ( lerpResult57 * -1.0 )) / (1.0 - 0.0)) + texCoord11.y ) ) ) ) )));
                

                half4 color = appendResult63;

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
Node;AmplifyShaderEditor.TextureCoordinatesNode;11;-2496,256;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,2;False;1;FLOAT2;0,-1;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;25;-2320,-192;Inherit;False;1;0;FLOAT;-5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-2304,-80;Inherit;False;Constant;_Float1;Float 0;0;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;48;-2208,144;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;20;-2352,-592;Inherit;False;1;0;FLOAT;-3;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-2336,-464;Inherit;False;Constant;_Float0;Float 0;0;0;Create;True;0;0;0;False;0;False;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-2288,48;Inherit;False;InstancedProperty;_Frequency;Frequency;1;1;[Header];Create;True;1;Ray parameters;0;0;False;0;False;1;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;28;-2048,-224;Inherit;True;SectionsRemap;-1;;4;e07d5d5a126b6f243ac17cc867746f65;0;2;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-2048,32;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;22;-2064,-640;Inherit;True;SectionsRemap;-1;;5;e07d5d5a126b6f243ac17cc867746f65;0;2;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;61;-2656,176;Inherit;False;InstancedProperty;_Variationseed;Variation seed;7;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;24;-1680,-144;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;52;-1696,-496;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;14;-1456,-416;Inherit;True;1;0;1;4;2;True;6;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;6;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.VoronoiNode;60;-1472,-800;Inherit;True;1;0;1;4;2;False;2;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.RangedFloatNode;56;-1472,-96;Inherit;False;InstancedProperty;_Amplitude;Amplitude;5;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;59;-1232,-672;Inherit;False;Property;_Tileable;Tileable;6;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-1200,-384;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;57;-1136,-112;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;19;-1024,-384;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;30;-1856,368;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-1040,-272;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;15;-848,-368;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-0.5;False;4;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMinOpNode;34;-1680,256;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;46;-1600,64;Inherit;False;InstancedProperty;_Width;Width;3;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-1456,256;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;47;-1040,224;Inherit;False;3;0;FLOAT;0.99;False;1;FLOAT;0.8;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;13;-560,0;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;16;-320,0;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;37;-864,528;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0.9;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-944,416;Inherit;False;InstancedProperty;_Borderfade;Border fade;4;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;32;-101.1201,225.555;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;55;-608,224;Inherit;False;3;0;FLOAT;0.99;False;1;FLOAT;0.8;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;39;-464,-384;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;43;-480,-144;Inherit;False;Property;_ColorIntensity;ColorIntensity;0;1;[Header];Create;True;2;Color;(Color intensity for tint color);0;0;False;0;False;1;40;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;26;96,32;Inherit;True;2;0;FLOAT;0.9;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;-48,-272;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;320,-64;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;64;400,-208;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.ObjectToWorldTransfNode;49;-3008,16;Inherit;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;50;-2752,32;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-2560,48;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;7;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;301.3322,233.8092;Inherit;False;Property;_Clipvalue;Clip value;2;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;53;-624,368;Inherit;False;Inverse Lerp;-1;;6;09cbe79402f023141a4dc1fddd4c9511;0;3;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;63;576,-176;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;62;752,-176;Float;False;True;-1;3;AmplifyShaderEditor.MaterialInspector;0;3;Turishader/UIElectricity;5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;False;True;3;1;False;;10;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;True;True;True;True;True;0;True;_ColorMask;False;False;False;False;False;False;False;True;True;0;True;_Stencil;255;True;_StencilReadMask;255;True;_StencilWriteMask;0;True;_StencilComp;0;True;_StencilOp;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;0;True;unity_GUIZTestMode;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;False;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;48;0;11;1
WireConnection;28;1;25;0
WireConnection;28;2;29;0
WireConnection;44;0;48;0
WireConnection;44;1;45;0
WireConnection;22;1;20;0
WireConnection;22;2;23;0
WireConnection;24;0;44;0
WireConnection;24;1;28;0
WireConnection;24;2;61;0
WireConnection;52;0;22;0
WireConnection;52;1;61;0
WireConnection;14;0;24;0
WireConnection;14;1;52;0
WireConnection;60;0;24;0
WireConnection;60;1;52;0
WireConnection;59;0;60;0
WireConnection;59;1;14;0
WireConnection;18;0;59;0
WireConnection;57;2;56;0
WireConnection;19;0;18;0
WireConnection;30;0;11;1
WireConnection;58;0;57;0
WireConnection;15;0;19;0
WireConnection;15;3;58;0
WireConnection;15;4;57;0
WireConnection;34;0;11;1
WireConnection;34;1;30;0
WireConnection;35;0;34;0
WireConnection;47;2;46;0
WireConnection;13;0;15;0
WireConnection;13;1;11;2
WireConnection;16;0;13;0
WireConnection;37;1;47;0
WireConnection;37;2;35;0
WireConnection;32;0;16;0
WireConnection;55;0;47;0
WireConnection;55;1;37;0
WireConnection;55;2;54;0
WireConnection;26;0;55;0
WireConnection;26;1;32;0
WireConnection;42;0;39;0
WireConnection;42;1;43;0
WireConnection;42;2;39;4
WireConnection;40;0;39;4
WireConnection;40;1;26;0
WireConnection;64;0;42;0
WireConnection;50;0;49;1
WireConnection;50;1;49;2
WireConnection;50;2;49;3
WireConnection;51;0;50;0
WireConnection;53;1;47;0
WireConnection;53;2;37;0
WireConnection;53;3;54;0
WireConnection;63;0;64;0
WireConnection;63;1;64;1
WireConnection;63;2;64;2
WireConnection;63;3;40;0
WireConnection;62;0;63;0
ASEEND*/
//CHKSM=93DB5CCC64E160C81A9803D3EDC91B14F4A7B9FF