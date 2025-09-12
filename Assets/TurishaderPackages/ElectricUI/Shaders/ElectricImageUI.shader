// Upgrade NOTE: upgraded instancing buffer 'TurishaderElectricImageUI' to new syntax.

// Made with Amplify Shader Editor v1.9.8.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Turishader/ElectricImageUI"
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
        [Header(Mip level aura (requires mip level enabled on texture ))][IntRange]_MipLevel("MipLevel", Range( 0 , 10)) = 1
        [Header(Ray parameters)]_Scale("Scale", Float) = 10
        _Displacement("Displacement", Float) = 0.05
        [KeywordEnum(OnlyRays,RaysAndImage)] _InnerFill("InnerFill", Float) = 1
        _RayThreshold("RayThreshold", Range( 0 , 1)) = 0.9
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
            #pragma shader_feature_local _INNERFILL_ONLYRAYS _INNERFILL_RAYSANDIMAGE
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
            uniform float _RayThreshold;
            uniform float _Scale;
            uniform float _Displacement;
            UNITY_INSTANCING_BUFFER_START(TurishaderElectricImageUI)
            	UNITY_DEFINE_INSTANCED_PROP(float, _MipLevel)
#define _MipLevel_arr TurishaderElectricImageUI
            UNITY_INSTANCING_BUFFER_END(TurishaderElectricImageUI)
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

                float4 temp_output_253_0 = ( IN.color * _ColorIntensity );
                float2 texCoord227 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
                float mulTime229 = _Time.y * 10.0;
                float temp_output_2_0_g3 = 1.0;
                float2 panner1_g5 = ( ( round( ( mulTime229 * temp_output_2_0_g3 ) ) / temp_output_2_0_g3 ) * float2( 0.1,0 ) + texCoord227);
                float temp_output_13_0_g5 = _Scale;
                float simplePerlin2D7_g5 = snoise( ( panner1_g5 + float2( 0.01,0 ) )*temp_output_13_0_g5 );
                simplePerlin2D7_g5 = simplePerlin2D7_g5*0.5 + 0.5;
                float simplePerlin2D2_g5 = snoise( panner1_g5*temp_output_13_0_g5 );
                simplePerlin2D2_g5 = simplePerlin2D2_g5*0.5 + 0.5;
                float simplePerlin2D8_g5 = snoise( ( panner1_g5 + float2( 0,0.01 ) )*temp_output_13_0_g5 );
                simplePerlin2D8_g5 = simplePerlin2D8_g5*0.5 + 0.5;
                float4 appendResult9_g5 = (float4(( simplePerlin2D7_g5 - simplePerlin2D2_g5 ) , ( simplePerlin2D8_g5 - simplePerlin2D2_g5 ) , 0.0 , 0.0));
                float _MipLevel_Instance = UNITY_ACCESS_INSTANCED_PROP(_MipLevel_arr, _MipLevel);
                float smoothstepResult212 = smoothstep( _RayThreshold , 1.0 , ( 1.0 - abs( (-1.0 + (tex2Dlod( _MainTex, float4( ( float4( texCoord227, 0.0 , 0.0 ) + ( appendResult9_g5 * _Displacement ) ).xy, 0, _MipLevel_Instance) ).a - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) ) ));
                float2 texCoord238 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
                float mulTime236 = _Time.y * 10.0;
                float temp_output_2_0_g4 = 1.0;
                float2 panner1_g6 = ( ( round( ( mulTime236 * temp_output_2_0_g4 ) ) / temp_output_2_0_g4 ) * float2( 0.1,0 ) + texCoord238);
                float temp_output_13_0_g6 = ( _Scale * 1.5 );
                float simplePerlin2D7_g6 = snoise( ( panner1_g6 + float2( 0.01,0 ) )*temp_output_13_0_g6 );
                simplePerlin2D7_g6 = simplePerlin2D7_g6*0.5 + 0.5;
                float simplePerlin2D2_g6 = snoise( panner1_g6*temp_output_13_0_g6 );
                simplePerlin2D2_g6 = simplePerlin2D2_g6*0.5 + 0.5;
                float simplePerlin2D8_g6 = snoise( ( panner1_g6 + float2( 0,0.01 ) )*temp_output_13_0_g6 );
                simplePerlin2D8_g6 = simplePerlin2D8_g6*0.5 + 0.5;
                float4 appendResult9_g6 = (float4(( simplePerlin2D7_g6 - simplePerlin2D2_g6 ) , ( simplePerlin2D8_g6 - simplePerlin2D2_g6 ) , 0.0 , 0.0));
                float smoothstepResult246 = smoothstep( _RayThreshold , 1.0 , ( 1.0 - abs( (-1.0 + (tex2Dlod( _MainTex, float4( ( float4( texCoord238, 0.0 , 0.0 ) + ( appendResult9_g6 * _Displacement ) ).xy, 0, _MipLevel_Instance) ).a - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) ) ));
                float temp_output_215_0 = saturate( ( smoothstepResult212 + smoothstepResult246 ) );
                float4 appendResult250 = (float4(temp_output_253_0.rgb , temp_output_215_0));
                float4 _MainTex_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_MainTex_ST_arr, _MainTex_ST);
                float2 uv_MainTex = IN.texcoord.xy * _MainTex_ST_Instance.xy + _MainTex_ST_Instance.zw;
                float4 tex2DNode151 = tex2D( _MainTex, uv_MainTex );
                float4 appendResult232 = (float4(temp_output_253_0.rgb , ( temp_output_215_0 + tex2DNode151.a )));
                float4 lerpResult233 = lerp( tex2DNode151 , appendResult232 , temp_output_215_0);
                #if defined( _INNERFILL_ONLYRAYS )
                float4 staticSwitch249 = appendResult250;
                #elif defined( _INNERFILL_RAYSANDIMAGE )
                float4 staticSwitch249 = lerpResult233;
                #else
                float4 staticSwitch249 = lerpResult233;
                #endif
                float4 break257 = staticSwitch249;
                float4 appendResult258 = (float4(break257.x , break257.y , break257.z , ( IN.color.a * break257.w )));
                

                half4 color = appendResult258;

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
Node;AmplifyShaderEditor.SimpleTimeNode;229;-3760,-464;Inherit;False;1;0;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;228;-3728,-400;Inherit;False;Constant;_Float2;Float 0;8;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;236;-3792,-112;Inherit;False;1;0;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;237;-3760,-48;Inherit;False;Constant;_Float3;Float 0;8;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;241;-3504,-192;Inherit;False;Property;_Scale;Scale;2;1;[Header];Create;True;1;Ray parameters;0;0;False;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;230;-3504,-448;Inherit;False;SectionsRemap;-1;;3;e07d5d5a126b6f243ac17cc867746f65;0;2;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;227;-3440,-352;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;235;-3536,-96;Inherit;False;SectionsRemap;-1;;4;e07d5d5a126b6f243ac17cc867746f65;0;2;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;238;-3504,160;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;252;-3408,64;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;226;-3168,-416;Inherit;False;NoiseNormal;-1;;5;d7e016d03fc9f6c48aec9a4d96d9b9cb;0;4;15;FLOAT;0;False;14;FLOAT2;0.1,0;False;13;FLOAT;10;False;12;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;239;-3200,-64;Inherit;False;NoiseNormal;-1;;6;d7e016d03fc9f6c48aec9a4d96d9b9cb;0;4;15;FLOAT;0;False;14;FLOAT2;0.1,0;False;13;FLOAT;15;False;12;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;223;-3168,-192;Inherit;False;Property;_Displacement;Displacement;3;0;Create;True;0;0;0;False;0;False;0.05;-0.035;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;224;-2864,-352;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;240;-2896,0;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;149;-2288,-624;Inherit;False;0;0;_MainTex;Shader;False;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;225;-2704,-336;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;242;-2736,16;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;170;-2352,-272;Inherit;False;InstancedProperty;_MipLevel;MipLevel;1;2;[Header];[IntRange];Create;True;1;Mip level aura (requires mip level enabled on texture );0;0;False;0;False;1;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;168;-1952,-416;Inherit;True;Property;_TextureSample1;Texture Sample 0;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;MipLevel;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode;234;-1952,-208;Inherit;True;Property;_TextureSample2;Texture Sample 0;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;MipLevel;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.TFHCRemapNode;206;-992,-1168;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;243;-1008,-912;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;209;-800,-1072;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;244;-816,-816;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;210;-672,-1072;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;245;-688,-816;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;251;-832,-960;Inherit;False;Property;_RayThreshold;RayThreshold;5;0;Create;True;0;0;0;False;0;False;0.9;0.622;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;212;-496,-1120;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.9;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;246;-512,-864;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.9;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;247;-304,-896;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;151;-1920,-672;Inherit;True;Property;_TextureSample0;Texture Sample 0;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;254;-211.3838,-1191.939;Inherit;False;Property;_ColorIntensity;Color Intensity;0;1;[Header];Create;True;2;Color;(Color intensity for tint color);0;0;False;0;False;1;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;215;-176,-768;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;255;-211.3838,-1367.939;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;220;48,-592;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;253;64,-1200;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;232;192,-752;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;233;400,-640;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;250;192,-848;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StaticSwitch;249;528,-768;Inherit;False;Property;_InnerFill;InnerFill;4;0;Create;True;0;0;0;False;0;False;0;1;1;True;;KeywordEnum;2;OnlyRays;RaysAndImage;Create;True;True;All;9;1;FLOAT4;0,0,0,0;False;0;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;5;FLOAT4;0,0,0,0;False;6;FLOAT4;0,0,0,0;False;7;FLOAT4;0,0,0,0;False;8;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.BreakToComponentsNode;257;832,-928;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;259;992,-816;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCGrayscale;182;-640,-656;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;122;-7710.87,-977.833;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;258;1200,-880;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;138;1504,-784;Float;False;True;-1;3;AmplifyShaderEditor.MaterialInspector;0;3;Turishader/ElectricImageUI;5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;False;True;3;1;False;;10;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;True;True;True;True;True;0;True;_ColorMask;False;False;False;False;False;False;False;True;True;0;True;_Stencil;255;True;_StencilReadMask;255;True;_StencilWriteMask;0;True;_StencilComp;0;True;_StencilOp;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;0;True;unity_GUIZTestMode;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;False;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;230;1;229;0
WireConnection;230;2;228;0
WireConnection;235;1;236;0
WireConnection;235;2;237;0
WireConnection;252;0;241;0
WireConnection;226;15;230;0
WireConnection;226;13;241;0
WireConnection;226;12;227;0
WireConnection;239;15;235;0
WireConnection;239;13;252;0
WireConnection;239;12;238;0
WireConnection;224;0;226;0
WireConnection;224;1;223;0
WireConnection;240;0;239;0
WireConnection;240;1;223;0
WireConnection;225;0;227;0
WireConnection;225;1;224;0
WireConnection;242;0;238;0
WireConnection;242;1;240;0
WireConnection;168;0;149;0
WireConnection;168;1;225;0
WireConnection;168;2;170;0
WireConnection;234;0;149;0
WireConnection;234;1;242;0
WireConnection;234;2;170;0
WireConnection;206;0;168;4
WireConnection;243;0;234;4
WireConnection;209;0;206;0
WireConnection;244;0;243;0
WireConnection;210;0;209;0
WireConnection;245;0;244;0
WireConnection;212;0;210;0
WireConnection;212;1;251;0
WireConnection;246;0;245;0
WireConnection;246;1;251;0
WireConnection;247;0;212;0
WireConnection;247;1;246;0
WireConnection;151;0;149;0
WireConnection;215;0;247;0
WireConnection;220;0;215;0
WireConnection;220;1;151;4
WireConnection;253;0;255;0
WireConnection;253;1;254;0
WireConnection;232;0;253;0
WireConnection;232;3;220;0
WireConnection;233;0;151;0
WireConnection;233;1;232;0
WireConnection;233;2;215;0
WireConnection;250;0;253;0
WireConnection;250;3;215;0
WireConnection;249;1;250;0
WireConnection;249;0;233;0
WireConnection;257;0;249;0
WireConnection;259;0;255;4
WireConnection;259;1;257;3
WireConnection;182;0;151;5
WireConnection;258;0;257;0
WireConnection;258;1;257;1
WireConnection;258;2;257;2
WireConnection;258;3;259;0
WireConnection;138;0;258;0
ASEEND*/
//CHKSM=E188F8257C19482FA67A3AA5989027E9767DB92B