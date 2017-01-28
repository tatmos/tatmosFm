using UnityEngine;
using System.Collections;
using System;


public class ADSR: ICloneable
{
    public String stateStr = "";

    public float lastValue = 0;
    public float rrLevel = 1;

    public float Gen(float phase, float a,float al, float d, float s, float r, float gate, float duration, float fs)
    {
        float outValue = 0.0f;

        a = a * fs;
        d = d * fs;
        r = r * fs;
        gate = gate * fs;
        duration = duration * fs;

        phase = (float)(phase);    

        if (a > gate)
        {
            a = 0;  //  attackをスキップ
        }
        if (a + d > gate)
        {
            d = 0;  //  decayをスキップ
        }

        if (phase <= a)   //  attack 
        {
            if (a == 0)
            {
                outValue = 0;
            } else
            {
                //outValue = (1.0f - Mathf.Exp(-5.0f * (phase / (a))))*al;

                outValue =  al*Mathf.Exp(-5f*(1f-(phase/a)));
            }
            if ((int)(phase) % 4410 == 0)
                stateStr = string.Format("A {0:F2} {1:F2}", outValue, phase / fs);

            rrLevel = outValue;
        } else if (phase <= a + d)        //  decay
        {
            if (d == 0)
            {
                outValue = s;
            } else
            {
                outValue = s + ((al - s) * Mathf.Exp(-5.0f * (phase - a) / d));
            }

            if ((int)(phase) % 4410 == 0)
                stateStr = string.Format("D {0:F2} {1:F2}", outValue, phase / fs);

            rrLevel = outValue;
        } else if (phase < gate)    //sustain
        {
            outValue = s;
            if ((int)(phase) % 4410 == 0)
                stateStr = string.Format("S {0:F2} {1:F2}", outValue, phase / fs);

            rrLevel = outValue;

        } else if (phase <= gate + r || phase <= a + d + r)
        {
            if (gate > a + d)
            {
                if (r == 0)
                {
                    outValue = s;
                } else
                {
                    outValue = (rrLevel * Mathf.Exp(-5.0f * (phase - gate + 1.0f) / r));//  gateを基準に
                }
            } else
            {
                if (r == 0)
                {
                    outValue = s;
                } else
                {
                    outValue = (s * Mathf.Exp(-5.0f * (phase - (a + d) + 1.0f) / r));//  a+dを基準に
                }
            }

            if ((int)(phase) % 4410 == 0)
                stateStr = string.Format("R {0:F2} {1:F2}", outValue, phase / fs);
        } 

        lastValue = outValue;
        return outValue;
    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}

public class FmOparetor : ICloneable
{
    public int opId = 0;

    //
    public float lastSample = 0f;

    //  周波数
    public float freq = 0f;
    //  振幅
    public float amp = 1.0f;
    //  周波数比
    public float ratio = 1.0f;
    public bool fixedFlag = false;
    public bool carrier = true;
    //  モジュレータOp
    public int ModOpId = 4; //  1つめの接続
    public int Mod2OpId = 4;//  2つめの接続

    public int wave = 0;

    public float a = 0.1f;
    public float al = 1f;
    public float d = 0.1f;
    public float s = 1.0f;
    public float r = 0.1f;

    public float fb = 0.0f;

    public ADSR adsr = new ADSR();

    public object Clone()
    {
        return MemberwiseClone();
    }
}


public class FmParam : ICloneable
{

    public float phase = 0;

    public float fs = 44100f;
    //  標準化周波数

    public FmOparetor[] op = new FmOparetor [5];

    public FmParam()
    {
        for (int i = 0; i < 5; i++)
        {
            op[i] = new FmOparetor();
            op [i].opId = i;
        }

        op[0].ModOpId = 1;
        op[1].ModOpId = 4;  //  4は入力なし

        op[2].ModOpId = 3;
        op[3].ModOpId = 4;  //  4は入力なし
    }

    //  キャリア
    //  モジュレーター


    public float gain = 1.0f;
    //ゲイン

    public float gate = 0f;
    //  keyOnの時間

    public float duration = 4f;
    //  リリースまで含めた時間

    public float basefreq = 1f;
    //  KeyNoteNoの周波数

    public object Clone()
    {
        return MemberwiseClone();
    }
}
