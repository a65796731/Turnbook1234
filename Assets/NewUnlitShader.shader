Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _FrontTex("FrontTex", 2D) = "white" {}
        _BackTex("BackTex", 2D) = "white" {}
        _Angle("Angle",Range(0,180))=0
        _Warp("Warp",Range(0,2))=0
        _WarpX("WarpX",Range(0,2))=0
        _Amount("Amount",Range(0,2))=0
        _vec("_vec",Range(0,1))=0
        
          _clip("clip",Range(0,1))=0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        CGINCLUDE

            #include "UnityCG.cginc"

            CBUFFER_START(ZZW)
            sampler2D _FrontTex;
            sampler2D _BackTex;
            float4 _FrontTex_ST;
            float4 _BackTex_ST;
            float _Angle;
            float _Warp;
            float _WarpX;
            float _Amount;
            float _vec;
            float _clip;
            CBUFFER_END

             struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                 float  dis : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };
           v2f vert (appdata v)
            {
                v2f o;
              
               float s;
               float c;
               sincos(radians(_Angle),s,c);
             float4x4 rotate={  
             c,s,0,0,
            -s,c,0,0,
             0,0,1,0,
              0,0,0,1};
                 o.dis= (v.vertex.x+5-0)/ (10-0);
               
                v.vertex -= float4(5,0,0,0);
                float x=(v.vertex .x+10-10);
                 
                  if(_vec>o.dis)
                  {
                float factor=saturate(1.0-abs(90-_Angle)/90);
             
                  v.vertex.y+=sin(v.vertex.x*0.4-v.vertex.x*_Warp)*-_Amount*factor;
                  v.vertex.x-=_WarpX*factor*  v.vertex.x;
                  v.vertex=mul(rotate,v.vertex);
                v.vertex += float4(5,0,0,0); 
                  o.vertex = UnityObjectToClipPos(v.vertex);
                  o.uv =v.uv;
                  o.dis=1;
                  }
                  
                   
                return o;
            }
            ENDCG


        Pass
        {
            cull back
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
          
           

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_FrontTex, i.uv);
                // apply fog
              
                return fixed4( i.dis, i.dis, i.dis,1);
            }
            ENDCG
        }


          Pass
        {
         cull front
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_BackTex, i.uv);
                // apply fog
              
                return col;
            }
            ENDCG
        }
    }
}
