 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandManager : MonoBehaviour {
    public enum WAND_STATE {
        IDEL,
        WARP,
        LAND,
        STATUS_MAX
    }

    public enum PLAYER_STATE {
        IDEL,
        WARP,
        DAMAGE
    }

    //Gemの数、種類が欲しい時にintではなくEnumに書き換える
    private int m_gem_num = 0;

    [SerializeField] private GameObject m_Wand;
    [SerializeField] private GemController m_GemController;

    public WAND_STATE WandState { get { return m_Wand.GetComponent<WandController>().WandState; } }
    public PLAYER_STATE PlayerState { get { return m_Wand.GetComponent<WandController>().PlayerState; } }
    public int GEM_NUM { get { return m_gem_num; } }

    private void Start() {
        //初期状態で動作しないものをセットをする
        m_GemController.SetGameStart(false);
        m_Wand.GetComponent<WandController>().SetBehaviorActive(false);
    }

    // Update is called once per frame
    void Update ( ) {
        MainManager.GameState main_state = MainManager.CurrentState;
        switch (main_state) {
            case MainManager.GameState.GAME_START:
                //ゲームスタート時に動作するものをセットする
                m_Wand.GetComponent<WandController>().SetBehaviorActive(true);
                m_GemController.SetGameStart(true);
                break;
            case MainManager.GameState.GAME_PLAYING:
                //ゲーム中のアップデート
                if (m_GemController.IsGetGem) {
                    GetGemAction();
                }
                break;
            case MainManager.GameState.GAME_FINISH:
                //ゲーム終了時に消すものはここで消す。
                m_Wand.GetComponent<WandController>().SetBehaviorActive(false);
                m_GemController.SetGameStart(false);
                break;
            case MainManager.GameState.GAME_RESULT:
                //リザルト時に消すものはここで消す。
                break;
        }

        DebugCode();
    }

    private void DebugCode() {
        if (Input.GetKeyDown(KeyCode.S)) {
            m_Wand.GetComponent<WandController>().SetBehaviorActive(true);
            m_GemController.SetGameStart(true);
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            m_Wand.GetComponent<WandController>().SetBehaviorActive(false);
            m_GemController.SetGameStart(false);
        }
        if (m_GemController.IsGetGem) {
            GetGemAction();
        }
    }

    //GemをGetしたときに呼ばれる関数
    private void GetGemAction ( ) {
        m_gem_num++;
        m_GemController.ResetHitState( );
        m_GemController.SetGemNum( m_gem_num );
      
    }
}
