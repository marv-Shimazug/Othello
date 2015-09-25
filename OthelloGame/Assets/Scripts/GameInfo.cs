using UnityEngine;
using System.Collections;

public class GameInfo : MonoBehaviour {

	private static bool m_BlackPlayer;	// true:プレイヤー	false:AI.
	private static bool m_WhitePlayer;	// true:プレイヤー	false:AI.

	// アクセサ.
	public bool BlackPlayer{get{return m_BlackPlayer;}set{m_BlackPlayer = value;}}
	public bool WhitePlayer{get{return m_WhitePlayer;}set{m_WhitePlayer = value;}}

	// プレイヤーが何人存在するかを返却.
	public int PlayerNum(){
		int num = 0;
		if (m_BlackPlayer && m_WhitePlayer) {
			num = 2;
		} else if (!m_BlackPlayer && m_WhitePlayer) {
			num = 1;
		} else if (m_BlackPlayer && !m_WhitePlayer) {
			num = 1;
		} else if (!m_BlackPlayer && !m_WhitePlayer) {
			num = 0;
		}
		return num;
	}

}
