Shader "Custom/ParallelogramShader"
{
	Properties
	{
		_Color("Main Color", Color) = (1, 0, 1, 1)
      	_Alpha ("Alpha", Float) = 1
   	}
   	
	SubShader
	{
		Tags 
		{ 
			"RenderType" = "Transparent" 
		}
	
		Pass
		{
         	Blend SrcAlpha OneMinusSrcAlpha 
         	Cull Off
         
			CGPROGRAM
			
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
	
			uniform float4 _Color; 
			float _Alpha;
			
			struct v2f
			{
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
			};
	
			v2f vert (appdata_full v)
			{
				v2f o;
				
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord;
	            
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 texColor = _Color;				
	            texColor.a = _Alpha;
	            
				return texColor;
			}
			
			ENDCG
		}
	}
	
	FallBack "Diffuse"
}
