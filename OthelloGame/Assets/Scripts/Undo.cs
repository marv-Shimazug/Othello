using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Undo : MonoBehaviour {

	private const int tileNum = 64;
	private Board board;			// ボードにアクセスするための情報格納庫.
	private PlayerInput player;
	private GameInfo gameInfo;

	// 毎ターンの情報を記憶.
	public List<int> m_Turn = new List<int>();		// 裏返した場所.
	public List<List<int>> m_TurnLog = new List<List<int>> ();	// 裏返した場所のログ.
	public List<int> m_SetPos = new List<int> ();	// 置いた場所.
	public List<bool> m_SetTurn = new List<bool>();	// 置くターンの色.
	private int m_Count;	// Undoした回数.

	// アクセサ.
	public int ResetCount{get{return m_Count;}set {m_Count = value;}}

	void Start()
	{
		m_Count = 0;
		board = GameObject.Find ("Board").GetComponent<Board> ();
		player = GameObject.Find ("PlayerInput").GetComponent<PlayerInput>();
		gameInfo = GameObject.Find ("GameInfo").GetComponent<GameInfo>();
	}

	public void OnClick()
	{
		if(board.TurnCount > RemoveTurn ()){
			// ゲームが終了してしまっている場合は再開する.
			foreach(var x in board.GameSet){
				x.SendMessage("TransmissionUI");
			}

			// プレイヤーの数によって戻すターン数変える.

			// 戻す.
			Debug.Log("戻すターン:"+RemoveTurn ());
			Debug.Log("現在のターン:"+board.TurnManager);
			board.BoardReStartTheWorld (RemoveTurn (), board.TurnManager);

			// 置ける場所を表示.
			board.RefreshmentBoard ();
			board.SetAvailable (board.TurnManager);

			// ログ情報変える.
			board.LogInstance.DeleteLog (RemoveTurn ());
		
		}
	}
	// プレイヤーの数によって戻すターン数変える.
	public int RemoveTurn(){
		int num = 0;
		switch (gameInfo.PlayerNum ()) {
		case 0:
			num = 2;
			break;
		case 1:
			num = 2;
			break;
		case 2:
			num = 1;
			break;
		}
		return num;
	}
}
