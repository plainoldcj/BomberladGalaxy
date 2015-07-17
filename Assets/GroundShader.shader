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

			// maps xy-coordinates from eye space to texture space (st-coordinates)			
			float2 MapST(float2 pos) {
				float f = 1.0 / (2.0 * _MappingDomain);
				return float2(
					clamp(f * (pos.x + _MappingDomain), 0.0, 1.0),
					1.0 - clamp(f * (pos.y + _MappingDomain), 0.0, 1.0));
			}
			
			// maps texture coordinates to parametric sphere coordinates (uv-coordinates)
			float2 MapUV(float2 texCoords) {
				float u = 2.0 * PI * texCoords.x + 0.5 * PI;
				if(u > 2.0 * PI) u -= 2.0 * PI;
				return float2(
					u,
					texCoords.y * 2.0 * PI - PI);
			}
			
			// returns point on parametric sphere
			float3 ParamSphere(float2 coords) {
				float3 v = float3(
					cos(coords.x) * cos(coords.y),
					sin(coords.x) * cos(coords.y),
					sin(coords.y));
				return float3(v.x, -v.z, v.y);
			}
			
			void vert(inout appdata_full input) {
				float zOff = input.vertex.z;
				float3 pos = mul(_LocalToWorld, input.vertex).xyz;
				pos = Warp(pos);
				float2 param = MapUV(MapST(pos.xy));
				if(param.y < -0.5 * PI || param.y > 0.5 * PI) {
					pos = float3(0.0f, 0.0f, 0.0f);
				} else {
					pos = (_SphereRadius + zOff) * ParamSphere(param);
				}
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
