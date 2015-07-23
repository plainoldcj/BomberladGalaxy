Shader "Custom/GroundShader" {
	Properties {
		_MappingDomain ("mapping domain", Float) = 10.0
		_SphereRadius ("sphere radius", Float) = 5.0
		_DiffuseTex ("diffuse texture", 2D) = "white" {}
        _RimColor ("Rim Color", Color) = (0.26, 0.19, 0.16, 0.0)
        _RimPower ("Rim Power", Range(0.5, 8.0)) = 3.0
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }	
		CGPROGRAM
		
		#pragma surface surf Lambert vertex:vert addshadow
        #pragma multi_compile ___ COMPUTE_FLAT_NORMALS
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
        
        float3 MapOnSphere(float3 v) {
            float3 pos = mul(_LocalToWorld, float4(v, 1.0)).xyz;
            float zOff = pos.z;
            pos = Warp(pos);
            
            float2 pos2 = (1.0 / _MappingDomain) * pos.xy;
            
            float r = min(length(pos2), 1.0);
            
            float phi = atan2(pos2.y, pos2.x);
            float theta = PI * r;
            
            return (_SphereRadius + zOff) * float3(
                sin(theta) * cos(phi), sin(theta) * sin(phi), -cos(theta));
        }
		
		void vert(inout appdata_full input) {
			float3 pos = MapOnSphere(input.vertex.xyz);
                
            float3 normal;
            #if COMPUTE_FLAT_NORMALS
                float3 v0 = input.tangent.xyz;
                float3 v1 = float3(input.tangent.w, input.texcoord1.xy);
                
                normal = normalize(cross(
                    MapOnSphere(v1),
                    MapOnSphere(v0)));
            #else
                normal = normalize(pos);
            #endif
            
			input.vertex.xyz = pos;
            input.normal.xyz = normal;
		}
		
		struct Input {
			float2 uv_DiffuseTex;
            float3 viewDir;
		};
		
		sampler2D   _DiffuseTex;
        float4      _RimColor;
		float       _RimPower;
        
		void surf(Input IN, inout SurfaceOutput OUT) {
			OUT.Albedo = tex2D(_DiffuseTex, IN.uv_DiffuseTex).rgb;
            
            #if ENABLE_RIM_LIGHTING
                half rim = 1.0 - saturate(dot(normalize(IN.viewDir), OUT.Normal));
                OUT.Emission = _RimColor.rgb * pow(rim, _RimPower);
            #endif
		}
		
		ENDCG
	}
	Fallback "Unlit/Color "
}
