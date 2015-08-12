Shader "Custom/GroundShader" {
	Properties {
		_MappingDomain ("mapping domain", Float) = 20.0
		_SphereRadius ("sphere radius", Float) = 5.0
	}
	SubShader {
		Pass {		
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			float 		_MappingDomain;
			float		_SphereRadius;
			float4x4	_LocalToWorld;
			
			// general idea: sphere is "texture-mapped with vertices"
			
			// assumption: view matrix does only translate on z-axis
			
			static const float PI = 3.14159265;

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
					texCoords.y * PI - 0.5 * PI);
			}
			
			// returns point on parametric sphere
			float3 ParamSphere(float2 coords) {
				float3 v = float3(
					cos(coords.x) * cos(coords.y),
					sin(coords.x) * cos(coords.y),
					sin(coords.y));
				return float3(v.x, -v.z, v.y);
			}
			
			struct VertexInput {
				float4	pos 		: POSITION;
				float2	texCoord0	: TEXCOORD0;
			};
			
			struct VertexOutput {
				float4 	pos 		: SV_POSITION;
				float2	texCoord0	: TEXCOORD0;
			};
			
			VertexOutput vert(VertexInput input) {
				VertexOutput output;
				float3 pos = mul(_LocalToWorld, input.pos).xyz;
				pos = _SphereRadius * ParamSphere(MapUV(MapST(pos.xy)));
				output.pos = mul(UNITY_MATRIX_MVP, float4(pos, 1.0));
				output.texCoord0 = input.texCoord0;
				return output;
			}
			
			fixed4 frag(VertexOutput input) : SV_Target {
				return fixed4(input.texCoord0.x, input.texCoord0.y, 0.0, 1.0);
			}
			
			ENDCG
		}
	}
	Fallback "Unlit/Color "
}
