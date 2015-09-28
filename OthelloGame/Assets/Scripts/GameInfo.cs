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

	private static int MultiplicationBlack = 0;
	private static int MultiplicationWhite = 0;
	[SerializeField]Text MultiplicationBlackText;
	[SerializeField]Text MultiplicationWhiteText;

	private bool CameraMode;	// true 2D false 3D
	[SerializeField]Text CameraModeText;
	[SerializeField]GameObject[] MatrixText;

	// アクセサ.
	public bool BlackPlayer{get{return m_BlackPlayer;}set{m_BlackPlayer = value;}}
	public bool WhitePlayer{get{return m_WhitePlayer;}set{m_WhitePlayer = value;}}
	public int GetTimeLimit{get{return m_TimeLimit;}}
	public int GetMultiplicationBlack{get{return MultiplicationBlack;}}
	public int GetMultiplicationWhite{get{return MultiplicationWhite;}}

	void Start()
	{
		CameraMode = true;

		if ("title" == Application.loadedLevelName)
		{
			m_TimeLimitText.text = "なし";
			MultiplicationBlackText.text = "x" + MultiplicationBlack.ToString() + "  ハンデ数";
			MultiplicationWhiteText.text = "x" + MultiplicationWhite.ToString() + "  ハンデ数";
		}
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

	public void OnCameraMode()
	{
		if (true == CameraMode) 
		{
			CameraMode = false;
			CameraModeText.text = "3Dカメラ";
			Camera.main.transform.position = new Vector3(4.0f, -4.8f, -8.0f);
			Camera.main.transform.rotation = Quaternion.Euler(320.0f,0.0f,0.0f);
			for(int i = 0; i < MatrixText.Length; i++)
			{
				MatrixText[i].SetActive(false);
			}
		}
		else
		{
			CameraMode = true;
			CameraModeText.text = "2Dカメラ";
			Camera.main.transform.position = new Vector3(3.6f, 3.6f, -10.0f);
			Camera.main.transform.rotation = Quaternion.Euler(0.0f,0.0f,0.0f);
			for(int i = 0; i < MatrixText.Length; i++)
			{
				MatrixText[i].SetActive(true);
			}
		}
	}

	public void OnBlackAdd()
	{
		MultiplicationBlack++;
		MultiplicationBlackText.text = "x" + MultiplicationBlack.ToString() + "  ハンデ数";
	}

	public void OnBlackSub()
	{
		MultiplicationBlack--;
		if (MultiplicationBlack < 0) 
		{
			MultiplicationBlack = 0;
		}
		MultiplicationBlackText.text = "x" + MultiplicationBlack.ToString () + "  ハンデ数";
	}


	public void OnWhiteAdd()
	{
		MultiplicationWhite++;
		MultiplicationWhiteText.text = "x" + MultiplicationWhite.ToString() + "  ハンデ数";
	}
	
	public void OnWhiteSub()
	{
		MultiplicationWhite--;
		if (MultiplicationWhite < 0) 
		{
			MultiplicationWhite = 0;
		}
		MultiplicationWhiteText.text = "x" + MultiplicationWhite.ToString () + "  ハンデ数";
	}
}
