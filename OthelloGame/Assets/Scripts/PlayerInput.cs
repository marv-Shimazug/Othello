using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInput : MonoBehaviour {

	private bool m_PlayerColor;	// プレイヤー石の色	true:白	false:黒.
	public Board board;			// ボードにアクセスするための情報格納庫.
	private Undo undo;			// 待った処理のための情報.

	private GameInfo gameInfo;
	private Ui ui;
	int m_Wait;
	// アクセサ.
	public bool PlayerColor{get{return m_PlayerColor;}private set{m_PlayerColor = value;}}

	void Start()
	{
		m_PlayerColor = false;
		board = GameObject.Find ("Board").GetComponent<Board> ();
		undo = GameObject.Find("Undo").GetComponent<Undo>();
		gameInfo = GameObject.Find ("GameInfo").GetComponent<GameInfo>();
		ui = GameObject.Find ("Text").GetComponent<Ui>();
	}

	void Update () {
		// 自ターン. まだ石が置ける.
		if (board.TurnManager == m_PlayerColor && !board.TurnWait && board.SetAvailableManager) {
			board.SetPass(m_PlayerColor);
			if(gameInfo.BlackPlayer){
				PlayerMove();
			}else{
				m_Wait++;
				if(m_Wait > 3){
//					AI(m_PlayerColor);
					RandomAI(m_PlayerColor);
				}
			}
		}
	}


	private void PlayerMove()
	{
		// 石配置.
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);	
			// タイルに衝突.
			if (Physics.Raycast (ray, out hit) && hit.collider.gameObject.tag == "tile") {
				
				Tile tile = hit.collider.gameObject.GetComponent<Tile> ();
				// 石設置.
				bool ok = board.SetReverse(tile, m_PlayerColor);
				// ターンを変更.
				if (ok) {
					board.TurnCount++;
					// ターンエンド.
					board.TurnManager = !m_PlayerColor;
					board.TurnWait = true;
					// ボードの色を初期化.
					board.RefreshmentBoard ();
					// ボードの置ける場所表示.
					board.SetAvailable (!m_PlayerColor);
					
					// 待った用にログ.
					board.BoardLog(false);
					undo.ResetCount = 0;

					ui.SendMessage("SetText");
				}
			}
		}
	}


	private void AI(bool p_MyColor)
	{
		for (int i = 0; i < 64; i++) {
			if (!board.TileArray [i].GetComponent<Tile> ().Stone) {
				if(board.TurnAvailable(i, p_MyColor).Count > 0){
					// 石設置.
					board.SetReverse(board.TileArray[i].GetComponent<Tile>(), p_MyColor);
					break;
				}
			}
		}
		board.TurnManager = !m_PlayerColor;
		board.RefreshmentBoard();
		board.SetAvailable(!m_PlayerColor);
		board.TurnCount++;
		m_Wait = 0;
		// 待った用にログ.
		board.BoardLog(false);

		ui.SendMessage("SetText");
	}

	private void RandomAI(bool p_MyColor){
		int num = 0;
		List<int> tiles = new List<int> ();
		for (int i = 0; i < 64; i++) {
			if (!board.TileArray [i].GetComponent<Tile> ().Stone) {
				if(board.TurnAvailable(i, p_MyColor).Count > 0){
					// 石設置.
					tiles.Insert(0, i);
				}
			}
		}
		num = Random.Range(0, tiles.Count);
		board.SetReverse(board.TileArray[tiles[num]].GetComponent<Tile>(), p_MyColor);
		board.TurnManager = !m_PlayerColor;
		board.RefreshmentBoard();
		board.SetAvailable(!m_PlayerColor);
		board.TurnCount++;
		m_Wait = 0;
		// 待った用にログ.
		board.BoardLog(false);

		ui.SendMessage("SetText");
	}
}
