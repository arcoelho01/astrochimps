Shader "ElectricityVertAnim"
{
	Properties 
	{
_Color("_Color", Color) = (0.9253731,0.03452887,0.03452887,1)
_Brightness_Min("_Brightness_Min", Range(0,3) ) = 0.7
_Brightness_Max("_Brightness_Max", Range(0,3) ) = 2.5
_MotionRateGlobal_Pixel("_MotionRateGlobal_Pixel", Float) = 1
_ElectricitySharpness("_ElectricitySharpness", Range(0,10) ) = 6
_MiddleOpacityMult("_MiddleOpacityMult", Float) = 1
_MiddleOpacityFalloff("_MiddleOpacityFalloff", Float) = 2
_MotionRateGlobal("_MotionRateGlobal", Float) = 1
_NoiseMotionRate("_NoiseMotionRate", Range(0,2) ) = 0.65
_NoiseFrequency("_NoiseFrequency", Range(0,2) ) = 1
_NoiseScale("_NoiseScale", Range(0,3) ) = 1

	}
	
	SubShader 
	{
		Tags
		{
"Queue"="Transparent"
"IgnoreProjector"="False"
"RenderType"="Transparent"

		}

		
Cull Off
ZWrite Off
ZTest LEqual
ColorMask RGBA
Fog{
}


		CGPROGRAM
#pragma surface surf BlinnPhongEditor  addshadow alpha decal:add vertex:vert
#pragma target 3.0


float4 _Color;
float _Brightness_Min;
float _Brightness_Max;
float _MotionRateGlobal_Pixel;
float _ElectricitySharpness;
float _MiddleOpacityMult;
float _MiddleOpacityFalloff;
float _MotionRateGlobal;
float _NoiseMotionRate;
float _NoiseFrequency;
float _NoiseScale;

			struct EditorSurfaceOutput {
				half3 Albedo;
				half3 Normal;
				half3 Emission;
				half3 Gloss;
				half Specular;
				half Alpha;
			};
			
			inline half4 LightingBlinnPhongEditor_PrePass (EditorSurfaceOutput s, half4 light)
			{
half3 spec = light.a * s.Gloss;
half4 c;
c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
c.a = s.Alpha;
return c;

			}

			inline half4 LightingBlinnPhongEditor (EditorSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
			{
				half3 h = normalize (lightDir + viewDir);
				
				half diff = max (0, dot (s.Normal, lightDir));
				
				float nh = max (0, dot (s.Normal, h));
				float spec = pow (nh, s.Specular*128.0);
				
				half4 res;
				res.rgb = _LightColor0.rgb * diff * atten * 2.0;
				res.w = spec * Luminance (_LightColor0.rgb);

				return LightingBlinnPhongEditor_PrePass( s, res );
			}
			
			struct Input {
				float4 meshUV;

			};


			void vert (inout appdata_full v, out Input o) {
float4 Mask8=float4(0.0,v.texcoord.y,0.0,0.0);
float4 Multiply19=float4( 0.01) * float4( 0);
float4 Multiply28=_Time * float4( 0.05);
float4 Frac2=frac(Multiply28);
float4 Add21=Multiply19 + Frac2;
float4 Multiply59=float4( 25) * float4(_MotionRateGlobal);
float4 Multiply58=Add21 * Multiply59;
float4 Multiply20=Multiply58 * float4( 1.123);
float4 Floor0=floor(Multiply20);
float4 Multiply22=Multiply58 * float4( 3);
float4 Floor1=floor(Multiply22);
float4 Add15=Floor0 + Floor1;
float4 Multiply21=Add15 * float4( 8.93);
float4 Add11=Multiply58 + Multiply21;
float4 Multiply35=float4( 3) * _NoiseMotionRate.xxxx;
float4 Multiply30=Add11 * Multiply35;
float4 Add17=Mask8 + Multiply30;
float4 Multiply57=float4( 193.1759) * _NoiseFrequency.xxxx;
float4 Multiply29=Add17 * Multiply57;
float4 Sin7=sin(Multiply29);
float4 Multiply34=float4( 0.25) * _NoiseScale.xxxx;
float4 Multiply27=Sin7 * Multiply34;
float4 Mask0=float4(0.0,v.texcoord.y,0.0,0.0);
float4 Multiply36=float4( 0.15) * _NoiseMotionRate.xxxx;
float4 Multiply3=Add11 * Multiply36;
float4 Add1=Mask0 + Multiply3;
float4 Multiply37=float4( 8.567) * _NoiseFrequency.xxxx;
float4 Multiply1=Add1 * Multiply37;
float4 Sin0=sin(Multiply1);
float4 Multiply40=float4( 5) * _NoiseScale.xxxx;
float4 Multiply2=Sin0 * Multiply40;
float4 Mask2=float4(0.0,v.texcoord.y,0.0,0.0);
float4 Multiply39=float4( 0.15) * _NoiseMotionRate.xxxx;
float4 Multiply5=Add11 * Multiply39;
float4 Add2=Mask2 + Multiply5;
float4 Multiply38=float4( 93.15) * _NoiseFrequency.xxxx;
float4 Multiply0=Add2 * Multiply38;
float4 Sin1=sin(Multiply0);
float4 Multiply41=float4( 0.5) * _NoiseScale.xxxx;
float4 Multiply7=Sin1 * Multiply41;
float4 Add4=Multiply2 + Multiply7;
float4 Add18=Multiply27 + Add4;
float4 Mask3=float4(0.0,v.texcoord.y,0.0,0.0);
float4 Multiply44=float4( -0.1051) * _NoiseMotionRate.xxxx;
float4 Multiply6=Add11 * Multiply44;
float4 Add3=Mask3 + Multiply6;
float4 Multiply43=float4( 25.31) * _NoiseFrequency.xxxx;
float4 Multiply4=Add3 * Multiply43;
float4 Sin2=sin(Multiply4);
float4 Multiply42=float4( 1.5) * _NoiseScale.xxxx;
float4 Multiply8=Sin2 * Multiply42;
float4 Add5=Add18 + Multiply8;
float4 Splat0=Add5.y;
float4 Mask1=float4(0.0,0.0,Splat0.z,0.0);
float4 Mask4=float4(0.0,v.texcoord.y,0.0,0.0);
float4 Add14=Add11 + float4( 2);
float4 Multiply47=float4( 0.256) * _NoiseMotionRate.xxxx;
float4 Multiply9=Add14 * Multiply47;
float4 Add6=Mask4 + Multiply9;
float4 Multiply46=float4( 7.071) * _NoiseFrequency.xxxx;
float4 Multiply16=Add6 * Multiply46;
float4 Sin5=sin(Multiply16);
float4 Multiply45=float4( 5) * _NoiseScale.xxxx;
float4 Multiply24=Multiply58 * float4( 3);
float4 Frac0=frac(Multiply24);
float4 Splat3=Frac0.y;
float4 Multiply23=Multiply58 * float4( 1.123);
float4 Frac1=frac(Multiply23);
float4 Splat4=Frac1.y;
float4 Multiply25=Splat3 * Splat4;
float4 Lerp0=lerp(float4( 0.5),float4( 1),Multiply25);
float4 Multiply26=Multiply45 * Lerp0;
float4 Multiply17=Sin5 * Multiply26;
float4 Mask5=float4(0.0,v.texcoord.y,0.0,0.0);
float4 Multiply50=float4( 0.190731) * _NoiseMotionRate.xxxx;
float4 Multiply10=Add14 * Multiply50;
float4 Add7=Mask5 + Multiply10;
float4 Multiply49=float4( 79.533) * _NoiseFrequency.xxxx;
float4 Multiply11=Add7 * Multiply49;
float4 Sin4=sin(Multiply11);
float4 Multiply48=float4( 0.5) * _NoiseScale.xxxx;
float4 Multiply14=Sin4 * Multiply48;
float4 Add9=Multiply17 + Multiply14;
float4 Mask9=float4(0.0,v.texcoord.y,0.0,0.0);
float4 Multiply56=float4( 2.705931) * _NoiseMotionRate.xxxx;
float4 Multiply33=Add14 * Multiply56;
float4 Add19=Mask9 + Multiply33;
float4 Multiply55=float4( 179.5317) * _NoiseFrequency.xxxx;
float4 Multiply32=Add19 * Multiply55;
float4 Sin8=sin(Multiply32);
float4 Multiply54=float4( 0.25) * _NoiseScale.xxxx;
float4 Multiply31=Sin8 * Multiply54;
float4 Add20=Add9 + Multiply31;
float4 Mask6=float4(0.0,v.texcoord.y,0.0,0.0);
float4 Multiply53=float4( -0.107335) * _NoiseMotionRate.xxxx;
float4 Multiply12=Add14 * Multiply53;
float4 Add8=Mask6 + Multiply12;
float4 Multiply52=float4( 23.0917) * _NoiseFrequency.xxxx;
float4 Multiply13=Add8 * Multiply52;
float4 Sin3=sin(Multiply13);
float4 Multiply51=float4( 1.5) * _NoiseScale.xxxx;
float4 Add16=Multiply51 + Lerp0;
float4 Multiply15=Sin3 * Add16;
float4 Add10=Add20 + Multiply15;
float4 Splat1=Add10.y;
float4 Mask7=float4(0.0,Splat1.y,0.0,0.0);
float4 Add12=Mask1 + Mask7;
float4 Splat5=v.texcoord.y;
float4 Multiply18=Splat5 * float4( 2);
float4 Add13=float4( -1) + Multiply18;
float4 Abs0=abs(Add13);
float4 Lerp1=lerp(Add12,float4( 0.0 ),Abs0);
float4 Add0=Lerp1 + v.vertex;
float4 VertexOutputMaster0_1_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_2_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_3_NoInput = float4(0,0,0,0);
v.vertex = Add0;

o.meshUV.xy = v.texcoord.xy;
o.meshUV.zw = v.texcoord1.xy;

			}
			

			void surf (Input IN, inout EditorSurfaceOutput o) {
				o.Normal = float3(0.0,0.0,1.0);
				o.Alpha = 1.0;
				o.Albedo = 0.0;
				o.Emission = 0.0;
				o.Gloss=0.0;
				o.Specular=0.0;
				
float4 Multiply0=(IN.meshUV.xyxy) * float4( 3.33);
float4 Add0=Multiply0 + float4( 1.5);
float4 Sin0=sin(Add0);
float4 Abs0=abs(Sin0);
float4 Splat0=Abs0.x;
float4 Invert0= float4(1.0) - Splat0;
float4 Multiply11=float4( 0.01) * float4( 0);
float4 Multiply10=_Time * float4( 0.05);
float4 Frac2=frac(Multiply10);
float4 Add3=Multiply11 + Frac2;
float4 Multiply13=float4( 25) * float4(_MotionRateGlobal_Pixel);
float4 Multiply12=Add3 * Multiply13;
float4 Multiply8=Multiply12 * float4( 3);
float4 Frac0=frac(Multiply8);
float4 Splat2=Frac0.y;
float4 Multiply7=Multiply12 * float4( 1.123);
float4 Frac1=frac(Multiply7);
float4 Splat3=Frac1.y;
float4 Multiply6=Splat2 * Splat3;
float4 Lerp0=lerp(_Brightness_Min.xxxx,_Brightness_Max.xxxx,Multiply6);
float4 Multiply9=Invert0 * Lerp0;
float4 Pow1=pow(Multiply9,_ElectricitySharpness.xxxx);
float4 Multiply4=(IN.meshUV.xyxy) * float4( 3.11);
float4 Add1=Multiply4 + float4( 0);
float4 Sin1=sin(Add1);
float4 Splat1=Sin1.y;
float4 Multiply3=Pow1 * Splat1;
float4 Multiply1=_Color * Multiply3;
float4 Splat4=(IN.meshUV.xyxy).y;
float4 Multiply2=Splat4 * float4( 2);
float4 Add2=float4( -1) + Multiply2;
float4 Abs1=abs(Add2);
float4 Pow0=pow(Abs1,float4(_MiddleOpacityFalloff));
float4 Lerp1=lerp(float4(_MiddleOpacityMult),float4( 1),Pow0);
float4 Multiply5=Multiply1 * Lerp1;
float4 Master0_0_NoInput = float4(0,0,0,0);
float4 Master0_1_NoInput = float4(0,0,1,1);
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_4_NoInput = float4(0,0,0,0);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Emission = Multiply5;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}