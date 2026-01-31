Shader "Hidden/MaskCombine"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "HDRenderPipeline" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma editor_sync_compilation

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _RTexture, _GTexture, _BTexture, _ATexture;
            int _RChannel, _GChannel, _BChannel, _AChannel;
            int _RInverse, _GInverse, _BInverse, _AInverse;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float PlotSourcetoChanel(int channel, sampler2D sourceTexture, v2f i, int inverse)
            {
                float col;
                col = tex2D(sourceTexture, i.uv)[channel];
                col = lerp(col, 1 - col, inverse);
                return col;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                
                col.r = PlotSourcetoChanel(_RChannel, _RTexture, i, _RInverse);
                col.g = PlotSourcetoChanel(_GChannel, _GTexture, i, _GInverse);
                col.b = PlotSourcetoChanel(_BChannel, _BTexture, i, _BInverse);
                col.a = PlotSourcetoChanel(_AChannel, _ATexture, i, _AInverse);
                //线性空间会对A通道做Gamma矫正，要自己校正
                //col.a = pow(col.a, 0.45f);
                return col;
            }
            ENDCG
        }
    }
}