using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;



public class FmAudioManager : MonoBehaviour
{
    public TextMesh textFmParam;

    int presetNo = 0;

    void Update()
    {
        Uodate_KeyMidiPlay();

        if(Input.GetKeyDown(KeyCode.Q))
        {
            this.OnDoremi();
        }
        if(Input.GetKeyDown(KeyCode.X))
        {
            SetPreset(presetNo++);
        }
        if(Input.GetKeyDown(KeyCode.Z))
        {
            if(presetNo == 0)return;
            SetPreset(presetNo--);
        }

        textFmParam.text = fmParameterText;
    }

    void Uodate_KeyMidiPlay()
    {
        //  PCキーでキーボード
        if(Input.GetKeyDown(KeyCode.A))
        {
            OnNoteOnDirect(60);
        }
        if(Input.GetKeyDown(KeyCode.W))
        {
            OnNoteOnDirect(61);
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            OnNoteOnDirect(62);
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            OnNoteOnDirect(63);
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            OnNoteOnDirect(64);
        }
        if(Input.GetKeyDown(KeyCode.F))
        {
            OnNoteOnDirect(65);
        }
        if(Input.GetKeyDown(KeyCode.T))
        {
            OnNoteOnDirect(66);
        }
        if(Input.GetKeyDown(KeyCode.G))
        {
            OnNoteOnDirect(67);
        }
        if(Input.GetKeyDown(KeyCode.Y))
        {
            OnNoteOnDirect(68);
        }
        if(Input.GetKeyDown(KeyCode.H))
        {
            OnNoteOnDirect(69);
        }
        if(Input.GetKeyDown(KeyCode.U))
        {
            OnNoteOnDirect(70);
        }
        if(Input.GetKeyDown(KeyCode.J))
        {
            OnNoteOnDirect(71);
        }
        if(Input.GetKeyDown(KeyCode.K))
        {
            OnNoteOnDirect(72);
        }
        if(Input.GetKeyDown(KeyCode.O))
        {
            OnNoteOnDirect(73);
        }
        if(Input.GetKeyDown(KeyCode.L))
        {
            OnNoteOnDirect(74);
        }
        if(Input.GetKeyDown(KeyCode.P))
        {
            OnNoteOnDirect(75);
        }
        if(Input.GetKeyDown(KeyCode.Colon))
        {
            OnNoteOnDirect(76);
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            OnNoteOffDirect();
        }


    }

    public float octave
    { set; get; }

    public float seqDuration
    { set; get; }

    public List<FmAudio> fmAudio = new List<FmAudio>();

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void OnApplicationQuit()
    {
        Shutdown();
    }

    void OnDestroy()
    {
    }

    void Start()
    {
        initPreset();

        seqDuration = 1f;

        this.Startup();
    }

    int[] noteList = new int[]
    { 60, 62, 64, 65, 67, 69, 71, 72, 74, 76, 74, 72, 71, 69, 67, 65, 64, 62,  
        60, 64, 67, 72, 76,
        62, 65, 69, 74, 77,
        64, 67, 71, 76, 79,
        62, 65, 69, 74, 77,
        60, 64, 67, 72, 76
    };
    //  ドレミのノート番号リスト
    int noteCount = 0;
    int fmAudioCurrent = 0;

    public void OnDoremi()
    {
        StopCoroutine("RandomNote");
        StopCoroutine("PlayScale");
        StartCoroutine("PlayScale", seqDuration);

    }

    IEnumerator PlayScale(float duration)
    {
        noteCount = 0;
        while (true)
        {
            OnNoteOnDirect(noteList [noteCount]);
            yield return new WaitForSeconds(seqDuration * 4f / 5f);
            OnNoteOffDirect();
            yield return new WaitForSeconds(seqDuration * 1f / 5f);
            noteCount++;

            if (noteCount == noteList.Length - 1)
            {
                noteCount = 0;
            }
        }
    }

    public void OnStopSequence()
    {
        StopCoroutine("RandomNote");
        StopCoroutine("PlayScale");
        OnNoteOffDirect();
    }

    public void OnRandomNote()
    {
        StopCoroutine("RandomNote");
        StopCoroutine("PlayScale");
        StartCoroutine("RandomNote", seqDuration);
    }

    IEnumerator RandomNote(float duration)
    {
        noteCount = 0;
        while (true)
        {
            OnNoteOnDirect(60 + UnityEngine.Random.Range(-24, 24));
            yield return new WaitForSeconds(seqDuration / 2);
            OnNoteOffDirect();
            yield return new WaitForSeconds(seqDuration / 2);
            noteCount++;

        }
    }
        
    public bool Startup()
    {
        {
            for (int i = 0; i < 4; i++) //  発音数分作成
            {
                // Audioの生成.
                {
                    GameObject fmObj = new GameObject("fmObj");
                    fmObj.AddComponent<FmAudio>();
                    fmObj.transform.parent = this.transform;
                    fmAudio.Add(fmObj.GetComponent<FmAudio>());
                }
                fmAudio [i].CreateFmAudio();

            }

            this.SetPreset(0);

            return true;
        }
        return false;
    }

    /**
		 * @brief	エンジンを停止
		 */
    private void Shutdown()
    {

    }
        

    public void OnNoteOnWithMouseDown(int noteNo)
    {
        if (Input.GetMouseButton(0))
        {
            OnNoteOnDirect(noteNo);
        }
    }



    public void OnNoteOnDirect(int noteNo)
    {
        // ノートオンを送信する.
        fmAudio [fmAudioCurrent % (fmAudio.Count - 1)].RealtimeAddMidi(FmAudio.MIDIEventType.NoteOn, noteNo + (int)(octave * 12));
    }




    public void OnNoteOffDirect()
    {
        // ノートオフを送信する.
        fmAudio [fmAudioCurrent % (fmAudio.Count - 1)].RealtimeAddMidi(FmAudio.MIDIEventType.NoteOff, 0);
        fmAudioCurrent++;
    }


    Dictionary<int,string> presetDict = new Dictionary<int, string>();

    // tatmosFmの特徴
    // 入力が２つある ModOpId,Mod2OpId
    // 各オペレータにFb(フィードバック）がある
    // Waveが選べる(サイン波形とノイズ波形)
    //  DXFi（http://www.taktech.org/takm/DXi/DXi_for_iPhone.html）をすごく参考にしています。

    // name, 
    //     Amp,wave(0:sin,1:noise),Ratio, fixed(t/f), carrier(t/f), ModOpId,ModOpId2, A,TL,D,S,R, Fb
    // op1
    // op2
    // op3
    // op4

    public void initPreset()
    {
        presetDict.Clear();

        int presetCount = 0;

        presetDict.Add(presetCount++, "TEST1,\n" +
        "0,0, 2.00,f,t, 1,3, 0.0,1.00, 0.0,1.0, 1.0, 0, \n" +
        "0,0, 0.50,f,f, 4,4, 0.0,1.00, 0.0,1.0, 1.0, 0, \n" +
        "1,0, 2.00,f,t, 3,4, 0.0,1.00, 0.0,1.0, 1.0, 0, \n" +
        "1,0, 0.50,f,f, 4,4, 0.0,1.00, 0.0,1.0, 1.0, 0.00,"); 
        presetDict.Add(presetCount++, "TEST2,\n" +
        "0,0, 2.00,f,t, 1,3, 0.0,1.00, 0.0,0.0, 0.1, 0, \n" +
        "0,0, 0.50,f,f, 2,4, 0.0,1.00, 0.0,0.0, 0.1, 0, \n" +
        "1,0, 2.00,f,t, 3,4, 0.0,1.00, 0.2,0.0, 0.1, 0, \n" +
        "1,0, 0.50,f,f, 4,4, 0.0,1.00, 0.2,0.0, 0.1, 0.00,"); 
        presetDict.Add(presetCount++, "TEST3,\n" +
        "0,0, 1.00,f,t, 4,4, 0.1,1.00, 0.2,1.0, 0.1, 0, \n" +
        "0,0, 1.00,f,t, 4,4, 0.1,1.00, 0.2,1.0, 0.1, 0, \n" +
        "0,0, 1.00,f,t, 4,4, 0.1,1.00, 0.2,1.0, 0.1, 0, \n" +
        "1,0, 1.00,f,t, 4,4, 0.1,1.00, 0.2,1.0, 0.1, 0.00,"); 
        
        presetDict.Add(presetCount++, "BELL,\n" +
        "1,0, 1.00,f,t, 1,4, 0.0,1.00, 1.0,0.10, 1.0, 0, \n" +
        "1,0, 3.50,f,f, 4,4, 0.0,0.50, 1.0,0.50, 1.0, 0, \n" +
        "1,0, 1.00,f,t, 3,4, 0.0,0.81, 1.0,0.50, 1.0, 0, \n" +
        "1,0, 3.50,f,f, 4,4, 0.0,0.60, 1.0,0.10, 1.0, 0.01,"); 
        presetDict.Add(presetCount++, "STRINGS,\n" +
        "1,0, 2.00,f,t, 1,4, 1.8,0.80, 0.0,0.80, 1, 0, \n" +
        "1,0, 2.00,f,f, 4,4, 1.3,0.80, 0.0,0.80, 1, 0, \n" +
        "1,0, 1.98,f,t, 3,4, 1.2,0.75, 0.0,0.75, 1, 0, \n" +
        "1,0, 8.00,f,f, 4,4, 0.0,0.50, 2.9,0.30, 1, 0.5"); 
        presetDict.Add(presetCount++, "PIANO,\n" +
        "1,0, 1.00,f,t, 1,4, 0.0,1.00, 1.0,0.30, 1, 0, \n" +
        "1,0, 1.00,f,f, 4,4, 0.0,0.80, 0.3,0.01, 1, 0, \n" +
        "1,0, 1.00,f,t, 3,4, 0.1,1.00, 1.0,0.30, 1, 0, \n" +
        "1,0, 2.00,f,f, 4,4, 0.0,0.60, 1.0,0.50, 1, 0.15"); 

        presetDict.Add(presetCount++, "PIPE ORGAN,\n" +
        "1,0, 0.50,f,t, 4,4, 0.1,1.00, 0.2,1.00, 1, 0, \n" +
        "1,0, 2.00,f,t, 4,4, 0.2,1.00, 0.2,1.00, 1, 0, \n" +
        "1,0, 1.00,f,t, 4,4, 0.3,0.90, 0.2,1.00, 1, 0, \n" +
        "1,0, 4.00,f,t, 4,4, 0.4,0.80, 0.2,1.00, 1, 0.10"); 


        presetDict.Add(presetCount++, "5th PAD,\n" +
        "1,0, 1.00,f,t, 1,4, 0.1,0.90, 0.2,1.00, 1, 0, \n" +
        "1,0, 1.00,f,f, 4,4, 0.1,0.20, 0.0,0.20, 1, 0, \n" +
        "1,0, 2.14,t,t, 3,4, 0.1,0.50, 0.1,0.60, 1, 0, \n" +
        "1,0, 1.50,f,f, 4,4, 0.1,0.30, 0.1,0.50, 1, 1.00"); 

        presetDict.Add(presetCount++, "GUITAR,\n" +
        "1,0, 1.00,f,t, 1,4, 0.0,1.00, 1.0,0.00, 1, 0, \n" +
        "1,0, 3.00,f,f, 4,4, 0.0,0.30, 3.0,0.20, 1, 0, \n" +
        "1,0, 7.00,f,f, 3,4, 0.0,0.50, 0.1,0.20, 1, 0, \n" +
        "1,0, 10.0,f,f, 4,4, 0.0,0.45, 0.1,0.20, 1, 0.77"); 

        presetDict.Add(presetCount++, "SynthBrass,\n" +
        "1,0, 1.00,f,t, 1,4, 0.1,1.00, 1.0,0.70, 1, 0, \n" +
        "1,0, 1.00,f,f, 2,4, 0.7,0.30, 1.2,0.75, 1, 0, \n" +
        "1,0, 0.50,f,f, 3,4, 0.4,0.50, 2.0,0.50, 1, 0, \n" +
        "1,0, 3.00,f,f, 4,4, 0.0,0.45, 1.0,0.00, 1, 0.28"); 

        presetDict.Add(presetCount++, "EG LOOPER,\n" +
        "1,0, 3.00,f,t, 1,4, 0.2,0.60, 0.2,0.00, 2, 0, \n" +
        "1,0, 6.00,f,f, 4,4, 0.1,0.30, 0.1,0.20, 2, 0, \n" +
        "1,0, 4.14,f,t, 3,4, 0.3,0.90, 0.3,0.00, 2, 0, \n" +
        "1,0, 2.50,f,f, 4,4, 0.4,0.70, 0.4,0.00, 2, 0.29"); 
        presetDict.Add(presetCount++, "TOY BELL,\n" +
        "1,0, 8.00,f,t, 1,4, 0.0,1.00, 3.00,0.20, 1, 0, \n" +
        "1,0, 6.50,f,f, 4,4, 0.0,0.60, 0.05,0.15, 1, 0, \n" +
        "1,0, 3.14,f,t, 3,4, 0.0,1.00, 3.00,0.20, 1, 0, \n" +
        "1,0, 6.50,f,f, 4,4, 0.0,0.10, 0.05,0.15, 1, 0.00"); 
        presetDict.Add(presetCount++, "KICK,\n" +
        "1,0, 0.50,f,t, 1,4, 0.0,0.95, 0.5,0.00, 0, 0, \n" +
        "1,0, 1.00,f,f, 4,4, 0.0,0.90, 0.1,0.00, 0, 0, \n" +
        "1,0, 32.49,t,t, 3,4, 0.0,0.60, 0.1,0.00, 0, 0, \n" +
        "1,0, 2.76,t, f, 4,4, 0.0,0.60, 0.5,0.00, 0, 0.49"); 

        presetDict.Add(presetCount++, "SNARE,\n" +
        "1,1, 1.00,f,t, 1,4, 0.0,0.75, 0.2,0.00, 0, 0, \n" +
        "1,0, 0.50,f,t, 4,4, 0.0,0.90, 0.1,0.00, 0, 0, \n" +
        "1,0, 0.50,t,t, 3,4, 0.0,0.60, 0.1,0.00, 0, 0, \n" +
        "1,0, 0.73,t,f, 4,4, 0.0,0.60, 0.5,0.00, 0, 1.00"); 

        presetDict.Add(presetCount++, "BeachGlass SA,\n" +
        "1,0,1.00,f,t, 1,4, 0.2,0.60, 0.2,0.00, 2, 0, \n" +
        "1,0,7.00,f,f, 4,4, 0.1,0.30, 0.1,0.20, 2, 0, \n" +
        "1,0,8.00,f,t, 3,4, 0.3,0.90, 0.3,0.00, 2, 0, \n" +
        "1,0,9.00,f,f, 4,4, 0.4,0.70, 0.4,0.00, 2, 1.00"); 
        presetDict.Add(presetCount++, "Electric Tine SA,\n" +
        "1,0, 1.00,f,t, 1,4, 0.2,0.60, 0.2,0.00, 2, 0, \n" +
        "1,0, 0.50,f,f, 4,4, 0.1,0.30, 0.1,0.20, 2, 0, \n" +
        "1,0, 1.29,f,t, 3,4, 0.3,0.90, 0.3,0.00, 2, 0, \n" +
        "1,0, 1.00,f,f, 4,4, 0.4,0.70, 0.4,0.00, 2, 0.00");
        presetDict.Add(presetCount++, "Beverly Hils SA,\n" +
        "1,0, 1.00,f,t, 1,4, 0.2,0.60, 0.2,0.00, 2, 0, \n" +
        "1,0, 4.00,f,f, 4,4, 0.1,0.30, 0.1,0.20, 2, 0, \n" +
        "1,0, 0.50,f,t, 3,4, 0.3,0.90, 0.3,0.00, 2, 0, \n" +
        "1,0, 0.50,f,f, 4,4, 0.4,0.70, 0.4,0.00, 2, 0.71"); 
        presetDict.Add(presetCount++, "SquareMod SA,\n" +
        "1,0, 1.00,f,t, 1,4, 0.2,0.60, 0.2,0.00, 2, 0, \n" +
        "1,0, 0.50,f,f, 4,4, 0.1,0.30, 0.1,0.20, 2, 0, \n" +
        "1,0, 0.50,f,t, 3,4, 0.3,0.90, 0.3,0.00, 2, 0, \n" +
        "1,0, 1.00,f,f, 4,4, 0.4,0.70, 0.4,0.00, 2, 0.27");     

        presetDict.Add(presetCount++, "SpaceShip,\n" +
        "1,0, 1.00,f,t, 1,3, 0.1,1.00, 0.5,0.40, 1, 0, \n" +
        "1,0, 4.00,f,f, 2,4, 0.5,0.20, 0.1,0.60, 1, 0, \n" +
        "1,0, 6.96,t,f, 4,4, 0.9,0.50, 2.5,0.85, 1, 0, \n" +
        "1,0, 2.14,t,f, 4,4, 0.4,0.30, 2.5,0.90, 1, 0.21"); 


        presetDict.Add(presetCount++, "Overflow,\n" +
        "1,0, 1.00,f,t, 1,2, 0.5,0.70, 0.5,0.80, 1, 0, \n" +
        "1,0, 2.71,f,f, 2,4, 0.6,0.20, 0.6,0.90, 1, 0, \n" +
        "1,0, 1.61,f,f, 3,4, 0.9,0.50, 1.5,0.20, 1, 0, \n" +
        "1,0, 57.89,t,f, 4,4, 0.4,1.00, 1.5,0.40, 1, 0.25"); 

        presetDict.Add(presetCount++, "Overflow2,\n" +
        "1,0, 1.00,f,t, 1, 2, 0.50, 0.85,  0.50, 0.80,  4.00, 0.00,\n" +
        "1,0, 2.71,f,f, 2, 4, 0.14, 0.38,  0.60, 0.90,  1.00, 0.00,\n" +
        "1,0, 1.61,f,f, 3, 4, 0.90, 0.97,  1.50, 0.20,  1.00, 0.00,\n" +
        "1.0, 57.89,t,f, 4, 4, 0.40, 1.00,  1.50, 0.40,  1.00, 0.53,"); 



        presetDict.Add(presetCount++, "More Bass,\n" +
        "1,0, 0.50,f,t, 1,4, 0.0,0.95, 3.0,0.30, 0.2, 0, \n" +
        "1,0, 0.50,f,f, 2,3, 0.0,0.95, 4.0,0.30, 0.2, 0, \n" +
        "1,0, 1.01,f,f, 4,4, 0.0,0.20, 1.5,0.00, 1, 0, \n" +
        "1,0, 1.00,f,f, 4,4, 0.0,0.25, 1.5,0.15, 1, 1.00"); 

        presetDict.Add(presetCount++, "SYN BASS,\n" +
        "1,0, 0.50,f,t, 1,3, 0.0,1.00, 1.0,0.9, 0.2, 0, \n" +
        "1,0, 0.50,f,f, 2,4, 0.0,0.80, 0.5,0.2, 0.2, 0, \n" +
        "1,0, 2.00,f,f, 4,4, 0.5,0.20, 0.8,0.6, 1.0, 0, \n" +
        "1,0, 0.50,f,f, 4,4, 0.0,1.00, 0.2,0.2, 1.0, 0.57,"); 

        presetDict.Add(presetCount++, "Mallet SA,\n" +
        "1,0, 1.00,f,t, 1,4, 0.0,1.00, 1.0,0.25, 1, 0, \n" +
        "1,0, 6.00,f,f, 2,3, 0.0,0.95, 0.1,0.00, 1, 0, \n" +
        "1,0, 0.50,f,f, 4,4, 0.0,0.75, 0.1,0.00, 1, 0, \n" +
        "1,0, 6.77,f,f, 4,4, 0.0,0.30, 3.5,0.00, 1, 0.00"); 


        presetDict.Add(presetCount++, "Bomb,\n" +
        "1,0, 1.50,t,t, 1,3, 0.0,1.00, 8.0,0.10, 2.0, 0, \n" +
        "1,0, 0.50,f,f, 2,4, 0.0,0.50, 8.0,0.50, 3.0, 0, \n" +
        "1,0, 25.00,t,f, 3,4, 0.0,0.81, 6.0,0.50, 2.0, 0, \n" +
        "1,0, 15.00,t,f, 4,4, 0.0,0.60, 4.0,0.10, 1.0, 1.50,"); 

        presetDict.Add(presetCount++, "ItemGet1,\n" +
        "1,0, 1.50,t,t, 1,3, 0.0,1.00, 3.0,0.0, 2.0, 0, \n" +
        "1,0, 0.01,t,f, 2,4, 0.0,0.50, 4.0,0.50, 3.0, 0, \n" +
        "1,0, 25.00,f,f, 3,4, 0.0,0.81, 3.0,0.50, 2.0, 0, \n" +
        "1,0, 15.00,t,f, 4,4, 0.0,0.60, 4.0,0.10, 1.0, 1.00,"); 

        presetDict.Add(presetCount++, "NoisePIANO,\n" +
        "1,0, 1.00,f,t, 1,4, 0.0,1.00, 1.0,0.30, 1, 0, \n" +
        "1,0, 1.00,f,f, 4,4, 0.0,0.80, 0.3,0.01, 1, 0, \n" +
        "1,0, 1.00,f,t, 3,4, 0.1,1.00, 1.0,0.30, 1, 0, \n" +
        "1,0, 0.10,f,f, 4,4, 0.0,0.60, 0.75,0.00, 1, 31.00,"); 

        presetDict.Add(presetCount++, "NoisePIANO2,\n" +
        "1,0, 1.01,t,t, 1,4, 0.0,0.90, 1.0,0.30, 1, 0, \n" +
        "1,0, 2.04,f,f, 4,4, 0.0,0.80, 0.3,0.01, 1, 0, \n" +
        "1,0, 1.00,f,t, 3,4, 0.1,1.00, 1.0,0.30, 1, 0, \n" +
        "1,0, 4.00,f,f, 4,4, 0.0,0.90, 0.15,0.00, 1, 2.00,"); 

        presetDict.Add(presetCount++, "Laser,\n" +
        "1,0, 3.655,False,True, 1, 4, 0.488, 0.62,  0.855, 0.84,  0.31, 0.16,\n" +
        "1,0, 3.53,False,False, 0, 0, 0.154, 0.72,  0.202, 0.58,  0.32, 0.32,\n" +
        "1,0, 5.63,False,True, 4, 3, 0.871, 0.8,  0.132, 0.43,  0.97, 0.04,\n" +
        "1,0, 6.255,True,False, 4, 3, 0.727, 0.42,  0.998, 0.43,  0.5, 0.05,"); 

        presetDict.Add(presetCount++, "Select,\n" +
        "1,0, 6.71,False,False, 4, 2, 0.862, 0.82,  0.171, 0.07,  0.02, 0.36,\n" +
        "1,0, 2.245,False,True, 3, 4, 0.353, 0.01,  0.703, 0.05,  0.57, 0.33,\n" +
        "1,0, 2.045,False,False, 2, 3, 0.748, 0.59,  0.503, 0.09,  0.25, 0.38,\n" +
        "1,0, 6.545,False,True, 3, 3, 0.043, 0.81,  0.977, 0.05,  0.67, 0.58,"); 

        presetDict.Add(presetCount++, "Sword,\n" +
        "0.4,0, 6.71,t,False, 4, 2, 0.862, 0.82,  0.171, 0.07,  0.02, 0.36,\n" +
        "0.4,0, 2.245,t,True, 3, 4, 0.353, 0.01,  0.703, 0.05,  0.57, 0.33,\n" +
        "0.4,0, 2.045,t,False, 2, 3, 0.748, 0.59,  0.503, 0.09,  0.25, 0.38,\n" +
        "1.0,0, 64.545,f,True, 3, 3, 0.043, 0.81,  0.977, 0.05,  0.67, 0.58,"); 
        
        presetDict.Add(presetCount++, "Harmonica,\n" +
        "1,0, 1.00,f,t, 1,4, 0.1,1.00, 0.2,1.0, 0.1, 0.4, \n" +
        "1,0, 4.01,f,t, 4,4, 0.1,1.00, 0.2,1.0, 0.1, 0.20, \n" +
        "1,0, 1.00,f,t, 3,4, 0.1,1.00, 0.2,1.0, 0.1, 0.4, \n" +
        "1,0, 4.00,f,f, 4,4, 0.1,1.00, 0.2,1.0, 0.1, 0.20,"); 

        presetDict.Add(presetCount++, "Horn?,\n" +
        "1,0, 1.72,True,True, 1, 1, 0.2, 0.5,  0.2, 0.5,  4.4, 0.0,\n" +
        "1,0, 1,False,False,   4, 4, 0.2, 0.5,  0.2, 0.7,  0.4, 0.0,\n" +
        "1,0, 4,False,False,   4, 4, 0.2, 0.7,  0.2, 1,  0.4, 0.0,\n" +
        "1,0, 5.64,False,False, 2, 4, 0.2, 0.7,  0.2, 1,  0.4, 0.0,"); 

    }

    public string presetName;
    public string fmParameterText;

    public void SetPreset(int no)
    {   
        SetFmParam(presetDict [no % (presetDict.Count)]);

        ParamUpdate();
    }
    //  全部文字列で指定
    public void SetFmParam(string inStr)
    {
        fmParameterText = inStr;

        inStr = inStr.Replace("\n", "").Replace("t,", "True,").Replace("f,", "False,");

        string[] tmpArray = inStr.Split(',');

        foreach (FmAudio tmpFmAudio in fmAudio)
        {
            int readNo = 0;

            presetName = tmpArray [readNo++];
            for (int opNo = 0; opNo < 4; opNo++)
            {
                tmpFmAudio.FmA.op [opNo].amp = float.Parse(tmpArray [readNo++]);
                tmpFmAudio.FmA.op [opNo].wave = int.Parse(tmpArray [readNo++]);
                tmpFmAudio.FmA.op [opNo].ratio = float.Parse(tmpArray [readNo++]);
                tmpFmAudio.FmA.op [opNo].fixedFlag = bool.Parse(tmpArray [readNo++]);
                tmpFmAudio.FmA.op [opNo].carrier = bool.Parse(tmpArray [readNo++]);

                tmpFmAudio.FmA.op [opNo].ModOpId = int.Parse(tmpArray [readNo++]);
                tmpFmAudio.FmA.op [opNo].Mod2OpId = int.Parse(tmpArray [readNo++]);

                tmpFmAudio.FmA.op [opNo].a = float.Parse(tmpArray [readNo++]);
                tmpFmAudio.FmA.op [opNo].al = float.Parse(tmpArray [readNo++]);
                tmpFmAudio.FmA.op [opNo].d = float.Parse(tmpArray [readNo++]);
                tmpFmAudio.FmA.op [opNo].s = float.Parse(tmpArray [readNo++]);
                tmpFmAudio.FmA.op [opNo].r = float.Parse(tmpArray [readNo++]);

                tmpFmAudio.FmA.op [opNo].fb = float.Parse(tmpArray [readNo++]);
            }
        }

    }

    float Mulitiple(int n)
    {
        float ret = n;
        switch (n)
        {
            case 0:
                ret = 0.5f;
                break;
        }
        return ret;
    }

    float Detune(int n)
    {
        float ret = 1;
        switch (n)
        {
            case 0:
                ret = 1f;
                break;
            case 1:
                ret = 1.41f;
                break;
            case 2:
                ret = 1.57f;
                break;
            case 3:
                ret = 1.73f;
                break;
        }
        return ret;
    }

    //  YM OPP Algorithm
    void Algorithm(int n, ref FmParam fm)
    {
        switch (n)
        {
            case 0:
                fm.op [0].carrier = true;
                fm.op [0].ModOpId = 1;
                fm.op [0].Mod2OpId = 4;
                fm.op [1].carrier = false;
                fm.op [1].ModOpId = 2;
                fm.op [1].Mod2OpId = 4;
                fm.op [2].carrier = false;
                fm.op [2].ModOpId = 3;
                fm.op [2].Mod2OpId = 4;
                fm.op [3].carrier = false;
                fm.op [3].ModOpId = 4;
                fm.op [3].Mod2OpId = 4;
                ;
                break;
            case 1:
                fm.op [0].carrier = true;
                fm.op [0].ModOpId = 1;
                fm.op [0].Mod2OpId = 4;
                fm.op [1].carrier = false;
                fm.op [1].ModOpId = 2;
                fm.op [1].Mod2OpId = 3;
                fm.op [2].carrier = false;
                fm.op [2].ModOpId = 4;
                fm.op [2].Mod2OpId = 4;
                fm.op [3].carrier = false;
                fm.op [3].ModOpId = 4;
                fm.op [3].Mod2OpId = 4;
                ;
                break;
            case 2:
                fm.op [0].carrier = true;
                fm.op [0].ModOpId = 1;
                fm.op [0].Mod2OpId = 3;
                fm.op [1].carrier = true;
                fm.op [1].ModOpId = 2;
                fm.op [1].Mod2OpId = 4;
                fm.op [2].carrier = true;
                fm.op [2].ModOpId = 4;
                fm.op [2].Mod2OpId = 4;
                fm.op [3].carrier = true;
                fm.op [3].ModOpId = 4;
                fm.op [3].Mod2OpId = 4;
                ;
                break;
            case 3:
                fm.op [0].carrier = true;
                fm.op [0].ModOpId = 1;
                fm.op [0].Mod2OpId = 2;
                fm.op [1].carrier = false;
                fm.op [1].ModOpId = 4;
                fm.op [1].Mod2OpId = 4;
                fm.op [2].carrier = false;
                fm.op [2].ModOpId = 3;
                fm.op [2].Mod2OpId = 4;
                fm.op [3].carrier = false;
                fm.op [3].ModOpId = 4;
                fm.op [3].Mod2OpId = 4;
                ;
                break;
            case 4:
                fm.op [0].carrier = true;
                fm.op [0].ModOpId = 1;
                fm.op [0].Mod2OpId = 4;
                fm.op [1].carrier = false;
                fm.op [1].ModOpId = 4;
                fm.op [1].Mod2OpId = 4;
                fm.op [2].carrier = false;
                fm.op [2].ModOpId = 3;
                fm.op [2].Mod2OpId = 4;
                fm.op [3].carrier = false;
                fm.op [3].ModOpId = 4;
                fm.op [3].Mod2OpId = 4;
                ;
                break;
            case 5:
                fm.op [0].carrier = true;
                fm.op [0].ModOpId = 3;
                fm.op [0].Mod2OpId = 4;
                fm.op [1].carrier = false;
                fm.op [1].ModOpId = 3;
                fm.op [1].Mod2OpId = 4;
                fm.op [2].carrier = true;
                fm.op [2].ModOpId = 3;
                fm.op [2].Mod2OpId = 4;
                fm.op [3].carrier = false;
                fm.op [3].ModOpId = 4;
                fm.op [3].Mod2OpId = 4;
                ;
                break;
            case 6:
                fm.op [0].carrier = true;
                fm.op [0].ModOpId = 4;
                fm.op [0].Mod2OpId = 4;
                fm.op [1].carrier = true;
                fm.op [1].ModOpId = 4;
                fm.op [1].Mod2OpId = 4;
                fm.op [2].carrier = true;
                fm.op [2].ModOpId = 3;
                fm.op [2].Mod2OpId = 4;
                fm.op [3].carrier = false;
                fm.op [3].ModOpId = 4;
                fm.op [3].Mod2OpId = 4;
                ;
                break;
            case 7:
                fm.op [0].carrier = true;
                fm.op [0].ModOpId = 4;
                fm.op [0].Mod2OpId = 4;
                fm.op [1].carrier = true;
                fm.op [1].ModOpId = 4;
                fm.op [1].Mod2OpId = 4;
                fm.op [2].carrier = true;
                fm.op [2].ModOpId = 4;
                fm.op [2].Mod2OpId = 4;
                fm.op [3].carrier = true;
                fm.op [3].ModOpId = 4;
                fm.op [3].Mod2OpId = 4;
                ;
                break;
        }
    }

    //  ratioのみランダム
    public void RandomRatio()
    {
        for (int i = 0; i < 4; i++)
        {
            fmAudio [0].FmA.op [i].ratio = (float)(Mulitiple(UnityEngine.Random.Range(0, 31))) * Detune(UnityEngine.Random.Range(0, 3));
        }

        for (int fmAudioNo = 1; fmAudioNo < fmAudio.Count; fmAudioNo++)
        {
            for (int i = 0; i < 4; i++)
            {
                fmAudio [fmAudioNo].FmA.op [i] = fmAudio [0].FmA.op [i];    //  0番の設定をコピー
            }
        }
        ParamUpdate();
    }

    //  結線のみランダム
    public void RandomAlgorithm()
    {
        Algorithm(UnityEngine.Random.Range(0, 7), ref fmAudio [0].FmA);

        for (int fmAudioNo = 1; fmAudioNo < fmAudio.Count; fmAudioNo++)
        {
            for (int i = 0; i < 4; i++)
            {
                fmAudio [fmAudioNo].FmA.op [i] = fmAudio [0].FmA.op [i];    //  0番の設定をコピー
            }
        }
        ParamUpdate();
    }

    public float Mod1000(float inValue)
    {
        return Mathf.Round(inValue * 100) / 100f;
    }

    public void RandomADSR()
    {
        for (int i = 0; i < 4; i++)
        {
            fmAudio [0].FmA.op [i].a = Mod1000(UnityEngine.Random.Range(0, 31) / 31.0f);
            fmAudio [0].FmA.op [i].al = Mod1000(UnityEngine.Random.Range(0, 31) / 31.0f);
            fmAudio [0].FmA.op [i].d = Mod1000(UnityEngine.Random.Range(0, 31) / 31.0f);
            fmAudio [0].FmA.op [i].s = Mod1000(UnityEngine.Random.Range(0, 31) / 31.0f);
            fmAudio [0].FmA.op [i].r = Mod1000(UnityEngine.Random.Range(0, 31) / 31.0f);
        }

        for (int fmAudioNo = 1; fmAudioNo < fmAudio.Count; fmAudioNo++)
        {
            for (int i = 0; i < 4; i++)
            {
                fmAudio [fmAudioNo].FmA.op [i] = fmAudio [0].FmA.op [i];    //  0番の設定をコピー
            }
        }
        ParamUpdate();
    }

    //全パラメータランダム
    public void RandomAll()
    {   
        for (int i = 0; i < 4; i++)
        {
            fmAudio [0].FmA.op [i].a = Mod1000(UnityEngine.Random.Range(0, 31) / 31.0f);
            fmAudio [0].FmA.op [i].al = Mod1000(UnityEngine.Random.Range(0, 31) / 31.0f);
            fmAudio [0].FmA.op [i].d = Mod1000(UnityEngine.Random.Range(0, 31) / 31.0f);
            fmAudio [0].FmA.op [i].s = Mod1000(UnityEngine.Random.Range(0, 31) / 31.0f);
            fmAudio [0].FmA.op [i].r = Mod1000(UnityEngine.Random.Range(0, 31) / 31.0f);

            fmAudio [0].FmA.op [i].fb = (float)(UnityEngine.Random.Range(0, 100) / 100.0f);

            fmAudio [0].FmA.op [i].ratio = (float)(Mulitiple(UnityEngine.Random.Range(0, 31))) * Detune(UnityEngine.Random.Range(0, 3));
            fmAudio [0].FmA.op [i].carrier = (UnityEngine.Random.Range(0, 2) == 0);
            fmAudio [0].FmA.op [i].fixedFlag = (UnityEngine.Random.Range(0, 5) == 0);
            fmAudio [0].FmA.op [i].ModOpId = (UnityEngine.Random.Range(0, 5));
            fmAudio [0].FmA.op [i].Mod2OpId = (UnityEngine.Random.Range(0, 5));
        }

        for (int fmAudioNo = 1; fmAudioNo < fmAudio.Count; fmAudioNo++)
        {
            for (int i = 0; i < 4; i++)
            {
                fmAudio [fmAudioNo].FmA.op [i] = fmAudio [0].FmA.op [i];    //  0番の設定をコピー
            }
        }

        ParamUpdate();
    }

    public void ParamUpdate()
    {
        for (int fmAudioNo = 1; fmAudioNo < fmAudio.Count; fmAudioNo++)
        {
            for (int i = 0; i < 4; i++)
            {
                fmAudio [fmAudioNo].FmA.op [i] = fmAudio [0].FmA.op [i];    //  0番の設定をコピー
            }
        }

        string tmpText = presetName + ",\n";

        for (int opNo = 0; opNo < 4; opNo++)
        {
            tmpText += fmAudio [0].FmA.op [opNo].amp.ToString() + ",";
            tmpText += fmAudio [0].FmA.op [opNo].wave.ToString() + ", ";
            tmpText += fmAudio [0].FmA.op [opNo].ratio.ToString("F2") + ",";
            tmpText += fmAudio [0].FmA.op [opNo].fixedFlag.ToString().Replace("True,", "t,").Replace("False,", "f,") + ",";
            tmpText += fmAudio [0].FmA.op [opNo].carrier.ToString().Replace("True,", "t,").Replace("False,", "f,") + ", ";

            tmpText += fmAudio [0].FmA.op [opNo].ModOpId.ToString() + ", ";
            tmpText += fmAudio [0].FmA.op [opNo].Mod2OpId.ToString() + ", ";

            tmpText += fmAudio [0].FmA.op [opNo].a.ToString("F2") + ", ";
            tmpText += fmAudio [0].FmA.op [opNo].al.ToString("F2") + ",  ";
            tmpText += fmAudio [0].FmA.op [opNo].d.ToString("F2") + ", ";
            tmpText += fmAudio [0].FmA.op [opNo].s.ToString("F2") + ",  ";
            tmpText += fmAudio [0].FmA.op [opNo].r.ToString("F2") + ", ";

            tmpText += fmAudio [0].FmA.op [opNo].fb.ToString("F2") + ",\n";
        }

        fmParameterText = tmpText.Replace("True,", "t,").Replace("False,", "f,");


    }


    #region Op1

    public int currentEditOp = 0;

    public String Op1Amp
    {
        set { fmAudio [0].FmA.op [currentEditOp].amp = float.Parse(value); }
        get { return fmAudio [0].FmA.op [currentEditOp].amp.ToString(); }
    }

    public String Op1A
    {
        set { fmAudio [0].FmA.op [currentEditOp].a = float.Parse(value); }
        get { return fmAudio [0].FmA.op [currentEditOp].a.ToString(); }
    }

    public String Op1D
    {
        set { fmAudio [0].FmA.op [currentEditOp].d = float.Parse(value); }
        get { return fmAudio [0].FmA.op [currentEditOp].d.ToString(); }
    }

    public String Op1S
    {
        set { fmAudio [0].FmA.op [currentEditOp].s = float.Parse(value); }
        get { return fmAudio [0].FmA.op [currentEditOp].s.ToString(); }
    }

    public String Op1R
    {
        set { fmAudio [0].FmA.op [currentEditOp].r = float.Parse(value); }
        get { return fmAudio [0].FmA.op [currentEditOp].r.ToString(); }
    }

    public String Op1Ratio
    {
        set { fmAudio [0].FmA.op [currentEditOp].ratio = float.Parse(value); }
        get { return fmAudio [0].FmA.op [currentEditOp].ratio.ToString(); }
    }

    public float Op1AmpFloat
    {
        set { fmAudio [0].FmA.op [currentEditOp].amp = value; }
        get { return fmAudio [0].FmA.op [currentEditOp].amp; }
    }

    public float Op1AFloat
    {
        set { fmAudio [0].FmA.op [currentEditOp].a = value; }
        get { return fmAudio [0].FmA.op [currentEditOp].a; }
    }

    public float Op1AlFloat
    {
        set { fmAudio [0].FmA.op [currentEditOp].al = value; }
        get { return fmAudio [0].FmA.op [currentEditOp].al; }
    }

    public float Op1DFloat
    {
        set { fmAudio [0].FmA.op [currentEditOp].d = value; }
        get { return fmAudio [0].FmA.op [currentEditOp].d; }
    }

    public float Op1SFloat
    {
        set { fmAudio [0].FmA.op [currentEditOp].s = value; }
        get { return fmAudio [0].FmA.op [currentEditOp].s; }
    }

    public float Op1RFloat
    {
        set { fmAudio [0].FmA.op [currentEditOp].r = value; }
        get { return fmAudio [0].FmA.op [currentEditOp].r; }
    }

    public float Op1FbFloat
    {
        set { fmAudio [0].FmA.op [currentEditOp].fb = value; }
        get { return fmAudio [0].FmA.op [currentEditOp].fb; }
    }

    public float Op1RatioFloat
    {
        set { fmAudio [0].FmA.op [currentEditOp].ratio = value; }
        get { return fmAudio [0].FmA.op [currentEditOp].ratio; }
    }

    #endregion
}
