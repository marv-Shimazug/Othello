using UnityEngine;
using System.Collections;

public class GameInfo : MonoBehaviour {

	private static bool m_BlackPlayer;	// true:プレイヤー	false:AI.
	private static bool m_WhitePlayer;	// true:プレイヤー	false:AI
	private  int m_BoardNumButton;		// ボードの数.
	private static int m_Row;
	private static int m_Column;

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


	public void OnClickBoardNum()
	{
		switch (m_BoardNumButton) 
		{
		case 0:
			m_Row = 8;
			m_Column = 8;
			break;

		case 1:
			m_Row = 4;
			m_Column = 4;
			break;

		case 2:
			m_Row = 6;
			m_Column = 6;
			break;

		case 3:
			m_Row = 10;
			m_Column = 10;
			break;
		}
	}

}
