
//プレイヤーの状態列挙
public enum PlayerState
{
    NONE = 0,
    NORMAL,     //通常時
    //JUMP,     //ジャンプ中、通常時にジャンプ時の処理を書いて、この状態は使わない方針
    ROLL,       //丸まり中
    IRON_BAR_DANGLE,    //鉄棒ぶら下がり
    IRON_BAR_CLIMB,     //鉄棒よじ登り
    STAGE_CLEAR,        //ステージクリア
    MAX
}
