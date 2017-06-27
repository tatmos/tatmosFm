# tatmosFm
UnityでFM音源  
  
UnityのAudioCallbackでFM音源（4op）  
  
tatmosFmの特徴  
各オペレータに入力が２つある ModOpId,Mod2OpId  (入力元のOp番号を指定する0~3 4:OFF)
各オペレータにFb(フィードバック）がある  
Waveが選べる(サイン波形とノイズ波形)  
キャリアかどうかのフラグがある（キャリアの場合基本の音程との掛け合わせが行われる）  
DXFi（ http://www.taktech.org/takm/DXi/DXi_for_iPhone.html ）  
をすごく参考にしています。  

プリセット  
name,   
//    Amp,wave(0:sin,1:noise),Ratio, fixed(t/f), carrier(t/f), ModOpId,ModOpId2, A,TL,D,S,R, Fb  
op1  
op2  
op3  
op4  
  
動画：https://youtu.be/kYpW1z12pf4  

# Class

## FmAudio.cs
- WaveGen ノイズかSinを返す
- FMRealtimePopAudio 実際のFMの計算

## FmAudioManager.cs
- FmAudioManager シーケンス再生やプリセットなど

## FmClass.cs
- ADSR エンベロープ処理
- FmOparetor　オペレータの設定
- FmParam　4つのオペレータと再生処理のための設定

# ToDo
- Total LFO
- Total Sweep
