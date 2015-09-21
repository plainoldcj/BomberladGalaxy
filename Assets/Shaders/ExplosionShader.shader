Shader "Custom/ExplosionShader" {
	Properties {
		_MappingDomain ("mapping domain", Float) = 10.0
		_SphereRadius ("sphere radius", Float) = 5.0
        _DisplacementTex ("displacement texture", 2D) = "white" {}
        _DisplacementFactor ("displacement factor", Float) = 1.0
        _ColorRamp ("color ramp", 2D) = "white" {}
		_RampInterval ("color ramp interval", Vector) = (0, 1, 0)
        _DisplacementCoeffs ("displacement coefficients", Color) = (0.0, 0.0, 0.0, 1.0)
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }	
		Cull Off
		CGPROGRAM
		
		#pragma surface surf Lambert vertex:vert
		
		#include "UnityCG.cginc"
		
		float 		_MappingDomain;
		float		_SphereRadius;
		float4x4	_LocalToWorld;
        
        sampler2D   _DisplacementTex;
        float       _DisplacementFactor;
        float4      _DisplacementCoeffs;
		
		// general idea: sphere is "texture-mapped with vertices"
		
		// assumption: view matrix does only translate on z-axis
		
		static const float PI = 3.14159265;
		
		float3 Warp(float3 pos) {
			if(pos.x > _MappingDomain) pos.x -= 2.0 * _MappingDomain;
			if(pos.y > _MappingDomain) pos.y -= 2.0 * _MappingDomain;
			if(pos.x < -_MappingDomain) pos.x += 2.0 * _MappingDomain;
			if(pos.y < -_MappingDomain) pos.y += 2.0 * _MappingDomain;
			return pos;
		}
        
        float3 MapOnSphere(float3 pos) {
            float zOff = pos.z;
            
            float2 pos2 = (1.0 / _MappingDomain) * pos.xy;
            
            float r = min(length(pos2), 1.0);
            
            float phi = atan2(pos2.y, pos2.x);
            float theta = PI * r;
            
            return (_SphereRadius + zOff) * float3(
                sin(theta) * cos(phi), sin(theta) * sin(phi), -cos(theta));
        }
		
		void vert(inout appdata_full input) {
            float3 pos = mul(_LocalToWorld, float4(input.vertex.xyz, 1.0)).xyz;
            pos = Warp(pos);

			float3 p0 = MapOnSphere(pos);
            float3 p1 = MapOnSphere(pos + float3(1, 0, 0));
            float3 p2 = MapOnSphere(pos + float3(0, 1, 0));
                
            float3 N = normalize(p0);
            float3 T = normalize(p1 - p0);
            float3 B = normalize(p2 - p0);

            float3 n = normalize(mul(_LocalToWorld, float4(input.normal, 0.0)));

            float3 normal = n.x * T + n.y * B + n.z * N;
            
            float disp = dot(_DisplacementCoeffs, tex2Dlod(_DisplacementTex, input.texcoord).rgb);
            
            p0 += _DisplacementFactor * disp * normal;
            
			input.vertex.xyz = p0;
            input.normal.xyz = normal;
		}
		
		struct Input {
			float2 uv_DisplacementTex;
		};
        
        sampler2D	_ColorRamp;
		float		_DispCutoff;
		float2		_RampInterval;

		static const float DispCutoffWindow = 1.0;
        
		void surf(Input IN, inout SurfaceOutput OUT) {
            float disp = dot(_DisplacementCoeffs, tex2D(_DisplacementTex, IN.uv_DisplacementTex).rgb);

			clip(_DispCutoff - disp);
            
			float v = float2(disp, 0.5) * (_RampInterval.y - _RampInterval.x) + _RampInterval.x;
            float3 color = tex2D(_ColorRamp, float2(v, 0.5));
        
			OUT.Emission = color;
		}
		
		ENDCG
	}
	Fallback "Unlit/Color "
}
