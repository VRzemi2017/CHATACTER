using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandController : MonoBehaviour {
    [SerializeField] GameObject m_WandModel;

    [SerializeField] LineRendererController m_Line;

    private WandManager.WAND_STATE m_wand_state;
    private WandManager.PLAYER_STATE m_player_state;

    private bool m_is_hit_enemy;
    public WandManager.WAND_STATE WandState { get { return m_wand_state; } }
    public WandManager.PLAYER_STATE PlayerState { get { return m_player_state; } }

    // Update is called once per frame
    void Update ( ) {
        //毎フレーム最初にステートを変える
        ChageWandState( );
        ChagePlayerState( );
    }

    private void OnTriggerEnter(Collider collision) {
        //ヒットした物の種類を取得するを取得する
        switch (collision.gameObject.tag) {
            case "Enemy":
                m_is_hit_enemy = true;
                break;
            default:
                break;
        }
    }
    private void ChageWandState ( ) {
        m_wand_state = WandManager.WAND_STATE.IDEL;
        if ( m_Line.IsWarpInput ) {
            m_wand_state = WandManager.WAND_STATE.WARP;
        }
    }

    private void ChagePlayerState() {
        m_player_state = WandManager.PLAYER_STATE.IDEL;
        if (m_Line.IsWarpInput) {
            m_player_state = WandManager.PLAYER_STATE.WARP;
        }
        if (m_is_hit_enemy) {
            m_player_state = WandManager.PLAYER_STATE.DAMAGE;
            m_is_hit_enemy = false;
        }
    }
    public void SetBehaviorActive(bool active) {
        if (!active) {
            m_Line.DeleteLine();
            m_Line.DeleteTarget();
        }
        m_Line.enabled = active;
    }
}
