Shader "Custom/StraightLineShader"
{
	Properties
	{
		_Color1("Main Color", Color) = (1, 0, 1, 1)
		_Color2("Secondary Color", Color) = (1, 0, 1, 1)
		_Alpha ("Alpha", Float) = 1
		_LineLength( "Line Length", Float ) = 1
		_DashLength1("Dash Length 1", Float) = 1
		_DashLength2("Dash Length 2", Float) = 1
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
	
			uniform float4 _Color1; 
			uniform float4 _Color2;
			float _Alpha;
			float _LineLength;
			float _DashLength1;
			float _DashLength2;

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
				float d = _LineLength * i.uv.x;
				float dashD = d % (_DashLength1 + _DashLength2);
				fixed4 texColor = (dashD < _DashLength1) ?( _Color1) :( _Color2);

	            texColor.a = _Alpha * texColor.a;
	            
				return texColor;
			}
			
			ENDCG
		}
	}
	
	FallBack "Diffuse"
}
