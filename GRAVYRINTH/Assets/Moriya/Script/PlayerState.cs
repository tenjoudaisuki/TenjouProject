
//プレイヤーの状態列挙
public enum PlayerState
{
    NONE = 0,
    NORMAL,     //通常時
    //NOT_MOVE,   //移動しないで重力に従う
    //JUMP,     //ジャンプ中、通常時にジャンプ時の処理を書いて、この状態は使わない方針
    ROLL,       //丸まり中
    IRON_BAR_DANGLE,    //鉄棒ぶら下がり
    IRON_BAR_CLIMB,     //鉄棒よじ登り
    CANNON_BLOCK,       //最終ステージの大砲のブロック
    STAGE_CLEAR,        //ステージクリア
    STAGE_FINAL_CLEAR,        //最終ステージクリア
    MAX
}
