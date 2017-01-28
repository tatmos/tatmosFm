using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;

public class FmAudio : MonoBehaviour
{
    public List<Text> textADSRState = new List<Text>();

    public enum MIDIEventType
    {
        Begin,
        ProgramChange,
        BankSelect,
        PBSens,
        PitchBend,
        Dynamics,
        Breathiness,
        Brightness,
        Clearness,
        Opening,
        Gender,
        VibratoDepth,
        VibratoRate,
        NoteOff,
        NoteOn,
        Ratio,
        AM,
        End
    }

    private AudioSource source = null;
    private Single[] renderData = new Single[2048];
    public void CreateFmAudio()
    {
        AudioClip clip = AudioClip.Create("FM" + gameObject.GetInstanceID(), 44100, 1, 44100, true);

        if(source == null)
        {
            this.gameObject.AddComponent<AudioSource>();
        }

        source = gameObject.GetComponent<AudioSource>();
        source.clip = clip;
        source.Play();
    }

    //  生成したサンプル数(応答速度にも依存）
    UInt32 FMRealtimeGetAudioNumData()
    {
        return 2048;    //  最大で2048
    }

    static public float Mtof(float note)
    {
        return (440f * Mathf.Exp(.057762265f * (note - 69f)));
    }

    System.Random cRandom;
    void Start()
    {
        cRandom = new System.Random();

        FmA.gain = 1.0f;    //   全体音量をすこし小さめ

        FmA.gate = 0f;

        FmA.op [0].freq = 1;//キャリア周波数　（あとでベースのノート周波数に置き換える）
        FmA.op [0].amp = 1.0f;  //  振幅
        FmA.op [0].ratio = 1f;//  ベースの周波数＊モジュレータの比率

        FmA.op [0].a = 0f;
        FmA.op [0].d = 1;
        FmA.op [0].s = 0.5f;
        FmA.op [0].r = 1;

        FmA.op [1].a = 1f;
        FmA.op [1].d = 1;
        FmA.op [1].s = 0.5f;
        FmA.op [1].r = 1;

        FmA.op [1].freq = 1;//モジュレーター周波数  （あとでキャリアと掛け合わせる）
        FmA.op [1].amp = 1.0f;  //  振幅
        FmA.op [1].ratio = 3.5f;//  ベースの周波数＊モジュレータの比率

        FmA.op [2].freq = 1;//キャリア周波数　（あとでベースのノート周波数に置き換える）
        FmA.op [2].amp = 1.0f;  //  振幅
        FmA.op [2].ratio = 3f;//  ベースの周波数＊モジュレータの比率

        FmA.op [2].a = 0f;
        FmA.op [2].d = 1;
        FmA.op [2].s = 0.5f;
        FmA.op [2].r = 1;

        FmA.op [3].a = 1f;
        FmA.op [3].d = 1;
        FmA.op [3].s = 0.5f;
        FmA.op [3].r = 1;

        FmA.op [3].freq = 1;//モジュレーター周波数  （あとでキャリアと掛け合わせる）
        FmA.op [3].amp = 1.0f;  //  振幅
        FmA.op [3].ratio = 3.5f;//  ベースの周波数＊モジュレータの比率

//        ParamUpdate();



    }

    void Update()
    {
    }


    Int32 phase_global = 0;
    float lastValue_global = 0; //  発音が来た時のフェード用最後のサンプル値
    public FmParam FmA = new FmParam();


    float WaveGen(int waveId,float input)
    {
        if(waveId == 1)
        {
            return (cRandom.Next(-1024, 1024)/1024.0f); //  noise
        }

        return Mathf.Sin(input);
    }



    //  生成したFM音源        FmA.op[1].freq = FmA.basefreq * FmA.op[1].ratio;
    void FMRealtimePopAudio(ref Single[] renderData, Int32 numOutSamples)
    {
        for (int i = 0; i < 4; i++)
        {
            FmA.op [i].freq = FmA.op [i].fixedFlag ? FmA.op [i].ratio : FmA.basefreq * FmA.op [i].ratio;    //  ベースの周波数＊モジュレータの比率 // fixedかどうか
        }

        //FM音源
        {
            for (int n = 0; n < numOutSamples; n++)
            {
                //  全オペレータ演算
                for (int i = 3; i >= 0; i--)
                {

                    FmA.op [i].lastSample = (float)(FmA.op [i].amp
                    * FmA.op [i].adsr.Gen(FmA.phase, FmA.op [i].a, FmA.op [i].al, FmA.op [i].d, FmA.op [i].s, FmA.op [i].r, FmA.gate, FmA.duration, FmA.fs)//ADSR
                        * WaveGen(FmA.op[i].wave,
                            (2.0f * Mathf.PI * FmA.op [i].freq * (float)(phase_global) / FmA.fs)//  opのwavecycle
                            + FmA.op [FmA.op [i].ModOpId].lastSample *1f// モジュレータからの入力
                            + FmA.op [FmA.op [i].Mod2OpId].lastSample*1f// モジュレータからの入力
                            + FmA.op [i].lastSample * FmA.op [i].fb*2f) //フィードバック
                    );
                }
                    
                float tmpBiasA = 1.0f / (float)((FmA.op [0].carrier ? 1 : 0) + (FmA.op [1].carrier ? 1 : 0) + (FmA.op [2].carrier ? 1 : 0) + (FmA.op [3].carrier ? 1 : 0)); //キャリアの数に応じて音量調整

                renderData [n] = 
                (float)(
                        FmA.op [0].lastSample * (FmA.op [0].carrier ? tmpBiasA : 0)
                    +
                        FmA.op [1].lastSample * (FmA.op [1].carrier ? tmpBiasA : 0)
                    +
                        FmA.op [2].lastSample * (FmA.op [2].carrier ? tmpBiasA : 0)
                    +
                        FmA.op [3].lastSample * (FmA.op [3].carrier ? tmpBiasA : 0) 
                    
                    ) * FmA.gain;// * 0.5f * ((float)n / (float)numOutSamples);
                

                //  トリガー時おノイズ対策用fade処理
                float noteOnFaderValue = 0;
                if(phase_global < 4410 && lastValue_global != 0){
                    noteOnFaderValue = (lastValue_global * Mathf.Exp(-5.0f *  ((float)phase_global + 1.0f) / 4410.0f));
                    renderData [n] += noteOnFaderValue;
                } else {
                    lastValue_global = renderData [n];
                }

                FmA.phase += 1.0f;
                phase_global++;

            }

        }
       
    }

    //  MIDIメッセージ
    public void RealtimeAddMidi(FmAudio.MIDIEventType midiType, int value)
    {
        switch (midiType)
        {
            case MIDIEventType.NoteOn:
                
                FmA.gate = 44100 * 100f;
                FmA.phase = 0;
                phase_global = 0;
                FmA.basefreq = Mtof(value);
                break;
            case MIDIEventType.NoteOff:
                FmA.gate = FmA.phase / FmA.fs;
                FmA.op [0].adsr.rrLevel = FmA.op [0].adsr.lastValue;
                FmA.op [1].adsr.rrLevel = FmA.op [1].adsr.lastValue;

//                FmA.phase = (FmA.op[0].a + FmA.op[0].d + FmA.op[0].s) * FmA.fs;    //  リリースへ
                break;
            case MIDIEventType.Ratio:
                FmA.op [1].ratio = (float)value / (127.0f);
                break;
            case MIDIEventType.AM:
                FmA.op [1].amp = (float)value / (127.0f);
                break;
        }

    }

    //  パラメータ即時反映
    public void RealtimeCommitMidi()
    {

    }

    void OnAudioFilterRead(Single[] data, Int32 channels)
    {
        UInt32 numBufferdSamples = FMRealtimeGetAudioNumData();

        Int32 numOutSamples = data.Length / channels;   //  長さ／チャンネル数   (必要なチャンネル数で割った分だけ生成）
        if (numBufferdSamples < numOutSamples)          //  バッファ＜サンプル数
        {
            numOutSamples = (Int32)numBufferdSamples;
        }
				
        // リアルタイム合成の結果をYVFから受け取る.
        if (renderData.Length < numOutSamples)
        {
            renderData = new Single[numOutSamples];  //  配列用意
        }

        FMRealtimePopAudio(ref renderData, numOutSamples);
       

        // 合成結果をAudioClipに書き込む.
        for (int i = 0; i < numOutSamples; ++i)
        {
            Single value = renderData [i];	
            int index = i * channels;
            for (int j = index; j < index + channels; ++j)
            {
                data [j] = value;
            }
        }

        // 不足した場合はゼロで埋める.
        for (int i = numOutSamples * channels; i < data.Length; ++i)
        {
            data [i] = 0;
            Debug.Log("不足");
        }
    }

    /**
		 * @brief	再生を停止し,ゲームオブジェクトを破棄する.
		 */
    public void Delete()
    {
        if (source != null)
        {
            source.Stop();
        }
        Destroy(gameObject);
    }





    #region Op1

    public String Op1Amp
    {
        set { FmA.op [0].amp = float.Parse(value); }
        get { return FmA.op [0].amp.ToString(); }
    }

    public String Op1A
    {
        set { FmA.op [0].a = float.Parse(value); }
        get { return FmA.op [0].a.ToString(); }
    }

    public String Op1D
    {
        set { FmA.op [0].d = float.Parse(value); }
        get { return FmA.op [0].d.ToString(); }
    }

    public String Op1S
    {
        set { FmA.op [0].s = float.Parse(value); }
        get { return FmA.op [0].s.ToString(); }
    }

    public String Op1R
    {
        set { FmA.op [0].r = float.Parse(value); }
        get { return FmA.op [0].r.ToString(); }
    }

    public String Op1Ratio
    {
        set { FmA.op [0].ratio = float.Parse(value); }
        get { return FmA.op [0].ratio.ToString(); }
    }

    public float Op1AmpFloat
    {
        set { FmA.op [0].amp = value; }
        get { return FmA.op [0].amp; }
    }

    public float Op1AFloat
    {
        set { FmA.op [0].a = value; }
        get { return FmA.op [0].a; }
    }

    public float Op1DFloat
    {
        set { FmA.op [0].d = value; }
        get { return FmA.op [0].d; }
    }

    public float Op1SFloat
    {
        set { FmA.op [0].s = value; }
        get { return FmA.op [0].s; }
    }

    public float Op1RFloat
    {
        set { FmA.op [0].r = value; }
        get { return FmA.op [0].r; }
    }

    public float Op1RatioFloat
    {
        set { FmA.op [0].ratio = value; }
        get { return FmA.op [0].ratio; }
    }

    #endregion

  


}
