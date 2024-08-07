Shader "Custom/Scanlines" {

	Properties{
		_MainTex("_Color", 2D) = "white" {}
		_LineWidth("Line Width", Float) = 0.4
		_ColumnWidth("Line Width", Float) = 0.2
		_Hardness("Hardness", Float) = 0.9
		_Speed("Displacement Speed", Range(0,1)) = 0.1
	}

		SubShader{

			Tags {
				"IgnoreProjector" = "True"
				"Queue" = "Overlay"}

			Pass {
			   ZTest Always
			   Cull Off
			   ZWrite Off

			   Fog{ Mode off }

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			#pragma target 3.0

			struct v2f {
				float4 pos      : POSITION;
				float2 uv       : TEXCOORD0;
				float4 scr_pos : TEXCOORD1;
			};

			uniform sampler2D _MainTex;
			uniform float _LineWidth;
			uniform float _ColumnWidth;
			uniform float _Hardness;
			uniform float _Speed;

			v2f vert(appdata_img v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				o.scr_pos = ComputeScreenPos(o.pos);

				return o;
			}

			half4 frag(v2f i) : COLOR {
				half4 color = tex2D(_MainTex, i.uv);
				fixed lineSizeY = _ScreenParams.y * 0.0035f;
				fixed lineSizeX = _ScreenParams.x * 0.0060f;
				float displacement = ((_Time.y * 1000) * _Speed) % _ScreenParams.y;
				float psY = displacement + (i.scr_pos.y * _ScreenParams.y / i.scr_pos.w);
				float psX = displacement + (i.scr_pos.x * _ScreenParams.x / i.scr_pos.w);
				float xHardness = ((1 - _Hardness) / 1.07) + _Hardness;
				if (xHardness > 1)
					xHardness = 1;

				if ((uint)(psY / floor(_LineWidth * lineSizeY)) % (uint)4 == 0)
					return color * float4(_Hardness, _Hardness, _Hardness, 1);
				else if ((uint)(psX / floor(_ColumnWidth * lineSizeY)) % (uint)4 == 0)
					return color * float4(xHardness, xHardness, xHardness, 1);
				else
					return color;
			}

			ENDCG
			}
		}
			FallBack "Diffuse"
}