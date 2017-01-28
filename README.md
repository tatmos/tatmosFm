# tatmosFm
UnityでFM音源  
  
UnityのAudioCallbackでFM音源（4op）  
  
tatmosFmの特徴  
各オペレータに入力が２つある ModOpId,Mod2OpId  
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
  
