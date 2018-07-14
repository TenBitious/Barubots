Shader "Sprite/ChrageEffect"
{
	Properties {
		_MainTex("Texture Image", 2D) = "white" {}
		_Color("Color", Color) = (0,0,0,0)
		_MinCutofValue("MinCutofValue", Float) = 0.1
		_AlphaValue("Alpha value", Float) = 0
		_ScaleX("Scale X", Float) = 1.0
		_ScaleY("Scale Y", Float) = 1.0
	}
	SubShader {
		Pass {
		CGPROGRAM

		#pragma vertex vert  
		#pragma fragment frag

		// User-specified uniforms            
		uniform sampler2D _MainTex;
		uniform float4 _Color;
		uniform float _AlphaValue; 
		uniform float _MinCutofValue;
		uniform float _ScaleX;
		uniform float _ScaleY;

		struct vertexInput {
			float4 vertex : POSITION;
			float4 tex : TEXCOORD0;
		};
		struct vertexOutput {
			float4 pos : SV_POSITION;
			float4 tex : TEXCOORD0;
		};

		vertexOutput vert(vertexInput input)
		{
			vertexOutput output;
			output.pos = UnityObjectToClipPos(input.vertex); ;
			/*
			output.pos = mul(UNITY_MATRIX_P,
				mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0))
				+ float4(input.vertex.x, input.vertex.y, 0.0, 0.0)
				* float4(_ScaleX, _ScaleY, 1.0, 1.0));
			*/
			output.tex = input.tex;

			return output;
		}

		float4 frag(vertexOutput input) : COLOR
		{
			float4 texAlpha = tex2D(_MainTex, float2(input.tex.xy));
			if (texAlpha.r < clamp(_AlphaValue, _MinCutofValue, 1)) discard;
			return _Color;
		}

		ENDCG
		}
	}
}