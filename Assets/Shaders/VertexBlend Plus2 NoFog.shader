Shader "Digit/Vertex Blend+2 NoFog"
{
	properties {
		_MainTex ("Color (RGB) Alpha (A)", 2D) = "white"{}
		_Alpha("Alpha", Range (0, 1)) = 1.0
	}
	
	subshader {	
		Fog {Mode Off}
		Blend SrcAlpha OneMinusSrcAlpha
		ZTest LEqual
		ZWrite Off
		Tags { "Queue"="Transparent-8" "RenderType"="Transparent" }
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert	
		
		sampler2D _MainTex;
		float _Alpha;
		
		struct Input {
        	float2 uv_MainTex;
        	float4 vertexColor;
	    };
	    
	    void vert (inout appdata_full v, out Input o) {
	        o.vertexColor = v.color;
	        o.uv_MainTex = v.texcoord;
	    }
			
		void surf (Input IN, inout SurfaceOutput o) {
			float4 albedo = tex2D(_MainTex, IN.uv_MainTex) * IN.vertexColor;
			o.Albedo = 0;
			o.Alpha = albedo.a * _Alpha;
			o.Specular = 0;
			o.Gloss = 0;
			o.Emission = albedo.rgb;
		}
		ENDCG
	}
	Fallback "Diffuse"
}