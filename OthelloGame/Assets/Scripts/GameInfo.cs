using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameInfo : MonoBehaviour {

	private static bool m_BlackPlayer;	// true:プレイヤー	false:AI.
	private static bool m_WhitePlayer;	// true:プレイヤー	false:AI

	private static int m_Row;
	private static int m_Column;
	private static int m_TimeLimit;		// 一手ごとの制限時間/秒.

	private  int m_BoardNumButton;	// ボードの数設定ボタン.
	private int m_TimeLimitButton;		// タイムリミット設定ボタン.
	[SerializeField]Text m_TimeLimitText;

	// アクセサ.
	public bool BlackPlayer{get{return m_BlackPlayer;}set{m_BlackPlayer = value;}}
	public bool WhitePlayer{get{return m_WhitePlayer;}set{m_WhitePlayer = value;}}

	void Start()
	{
		m_TimeLimitText.text = "なし";
	}

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


	// 盤面のマスの数.
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


	// 制限時間の設定.
	public void OnTimeLimit()
	{
		m_TimeLimitButton++;
		if (m_TimeLimitButton > 4) 
		{
			m_TimeLimitButton = 0;
		}

		switch(m_TimeLimitButton)
		{
		case 0:
			m_TimeLimit = 0;
			m_TimeLimitText.text = "なし";
			break;

		case 1:
			m_TimeLimit = 3;
			m_TimeLimitText.text = ((int)m_TimeLimit).ToString() + "秒";
			break;
		
		case 2:
			m_TimeLimit = 5;
			m_TimeLimitText.text = ((int)m_TimeLimit).ToString() + "秒";
			break;
		
		case 3:
			m_TimeLimit = 10;
			m_TimeLimitText.text = ((int)m_TimeLimit).ToString() + "秒";
			break;

		case 4:
			m_TimeLimit = 30;
			m_TimeLimitText.text = ((int)m_TimeLimit).ToString() + "秒";
			break;
		}
	}

}
