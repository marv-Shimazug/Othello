using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AiInput : MonoBehaviour {

	private bool m_AiColor;	// プレイヤー石の色	true:白	false:黒.
	public Board board;			// ボードにアクセスするための情報格納庫.
	int m_Wait;
	private GameInfo gameInfo;
	private Ui ui;
	// アクセサ.
	public bool AiColor{get{return m_AiColor;}private set{m_AiColor = value;}}

	// Use this for initialization
	void Start () {
		m_AiColor = true;
		board = GameObject.Find ("Board").GetComponent<Board> ();
		gameInfo = GameObject.Find ("GameInfo").GetComponent<GameInfo>();
		ui = GameObject.Find ("Text").GetComponent<Ui>();
	}
	
	// Update is called once per frame
	void Update () {
		// 自ターン.
		if (board.TurnManager == m_AiColor && !board.TurnWait && board.SetAvailableManager) {
			board.SetPass(m_AiColor);
			if(gameInfo.WhitePlayer){
				PlayerMove();
			}else{
				m_Wait++;
				if(m_Wait > 3){
				//	AI(m_AiColor);
					RandomAI(m_AiColor);
				}
			}
		}
	}

	private void PlayerMove(){
		// 石配置.
		if (Input.GetMouseButtonDown (0)) {
			
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);	
			// タイルに衝突.
			if (Physics.Raycast (ray, out hit) && hit.collider.gameObject.tag == "tile") {
				
				Tile tile = hit.collider.gameObject.GetComponent<Tile> ();
				// 石設置.
				bool ok = board.SetReverse(tile, m_AiColor);
				// ターンを変更.
				if(ok){
					board.TurnCount++;
					board.TurnManager = !m_AiColor;
					board.TurnWait = true;
					board.RefreshmentBoard();
					board.SetAvailable(!m_AiColor);
					// 待った用にログ.
					board.BoardLog(false);

					ui.SendMessage("SetText");
				}
			}
		}
	}

	private void AI(bool p_MyColor){
		for (int i = 0; i < 64; i++) {
			if (!board.TileArray [i].GetComponent<Tile> ().Stone) {
				if(board.TurnAvailable(i, p_MyColor).Count > 0){
					// 石設置.
					board.SetReverse(board.TileArray[i].GetComponent<Tile>(), p_MyColor);
					break;
				}
			}
		}
		board.TurnManager = !m_AiColor;
		board.RefreshmentBoard();
		board.SetAvailable(!m_AiColor);
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
		board.TurnManager = !m_AiColor;
		board.RefreshmentBoard();
		board.SetAvailable(!m_AiColor);
		board.TurnCount++;
		m_Wait = 0;
		// 待った用にログ.
		board.BoardLog(false);

		ui.SendMessage("SetText");
	}
}
