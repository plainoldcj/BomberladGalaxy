Shader "Custom/GroundShader" {
	Properties {
		_MappingDomain ("mapping domain", Float) = 10.0
		_SphereRadius ("sphere radius", Float) = 5.0
		_DiffuseTex ("diffuse texture", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }	
			CGPROGRAM
			
			#pragma surface surf Lambert vertex:vert
			
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

			// maps xy-coordinates from [-mappingDomain, mappingDomain] to [-1.0, 1.0]			
			float2 MapST(float2 pos) {
				return (1.0 / _MappingDomain) * pos.xy;
			}
			
			void vert(inout appdata_full input) {
				float3 pos = mul(_LocalToWorld, input.vertex).xyz;
                float zOff = pos.z;
				pos = Warp(pos);
                
                float2 pos2 = MapST(pos.xy);
                
                float r = min(length(pos2), 1.0);
                
                float phi = atan2(pos2.y, pos2.x);
                float theta = PI * r;
                
                pos = (_SphereRadius + zOff) * float3(
                    sin(theta) * cos(phi), sin(theta) * sin(phi), -cos(theta));
                
				input.vertex.xyz = pos;
			}
			
			struct Input {
				float2 uv_DiffuseTex;
			};
			
			sampler2D _DiffuseTex;
			
			void surf(Input IN, inout SurfaceOutput OUT) {
				OUT.Albedo = tex2D(_DiffuseTex, IN.uv_DiffuseTex).rgb;
			}
			
			ENDCG
	}
	Fallback "Unlit/Color "
}
