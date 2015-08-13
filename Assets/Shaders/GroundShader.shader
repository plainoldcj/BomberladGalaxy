Shader "Custom/GroundShader" {
	Properties {
		_MappingDomain ("mapping domain", Float) = 10.0
		_SphereRadius ("sphere radius", Float) = 5.0
		_DiffuseTex ("diffuse texture", 2D) = "white" {}
        _Color ("diffuse color", Color) = (1.0, 1.0, 1.0, 1.0)
        _RimColor ("Rim Color", Color) = (0.26, 0.19, 0.16, 0.0)
        _RimPower ("Rim Power", Range(0.5, 8.0)) = 3.0
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }	
		CGPROGRAM
		
		#pragma surface surf Lambert vertex:vert addshadow
        #pragma multi_compile ___ ENABLE_RIM_LIGHTING
		
		#include "UnityCG.cginc"
		
		float 		_MappingDomain;
		float		_SphereRadius;
		float4x4	_LocalToWorld;
		
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
            
			input.vertex.xyz = p0;
            input.normal.xyz = normal;
		}
		
		struct Input {
			float2 uv_DiffuseTex;
            float3 viewDir;
		};
		
		sampler2D   _DiffuseTex;
        float4      _Color;
        float4      _RimColor;
		float       _RimPower;
        
		void surf(Input IN, inout SurfaceOutput OUT) {
			OUT.Albedo = _Color * tex2D(_DiffuseTex, IN.uv_DiffuseTex).rgb;
            
            #if ENABLE_RIM_LIGHTING
                half rim = 1.0 - saturate(dot(normalize(IN.viewDir), OUT.Normal));
                OUT.Emission = _RimColor.rgb * pow(rim, _RimPower);
            #endif
		}
		
		ENDCG
	}
	Fallback "Unlit/Color "
}
