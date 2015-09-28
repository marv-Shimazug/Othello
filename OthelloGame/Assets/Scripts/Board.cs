using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Board : MonoBehaviour {

	public const int ROW = 8;			//	行数.
	public const int COLUMN = 8;		//	列数.
	public const float MARGE = 0.1f;	// マスとマスの間.

	// BoardInfo
	private GameObject[] m_TileArray = new GameObject[ROW * COLUMN];	//タイルを格納.
	private List<GameObject> m_StoneList = new List<GameObject>();	// 石を格納.
	private GameObject m_Tile;	// タイルリソース取得.
	private GameObject m_Stone;	// 石リソース取得.
	private Log m_Log;			// ログの保管庫取得インスタンス.
	private bool m_TurnManager;	// ターンの判定	true:白	false:黒.
	private bool m_TurnWait;	// ターンを変えるのに1フレーム待つ.
	private int m_TurnCount;	// ターンのカウント.
	private int m_Count;		// 相手の駒数.
	private List<int> m_Available = new List<int> ();	// 置ける位置格納.
	private bool m_SetAvailableManager;	// まだ石が置けるか.
	private int m_PassCount;			// 2回以上でゲーム終了.
	private VictoryOrDefeatText m_VictoryOrDefeatText;	// 勝敗.
	private List<GameObject> m_GameSet = new List<GameObject>();
	private int m_ReCount;
	private Ui ui;

	private List<Tile.data[]> BoardHistory = new List<Tile.data[]>();	//	ログ用.
//	private List<GameObject[]> m_BoardHistory = new List<GameObject[]>();	//	ログ用.
	

	// 勝敗の判定.
	public enum VictoryOrDefeat{

		Victory = 0,
		Defeat = 1,
		Draw = 2,
	}
	private VictoryOrDefeat m_VictoryOrDefeat;

	// アクセサ.	
	public GameObject[] TileArray{get{return m_TileArray;}set{m_TileArray = value;}}
	public List<GameObject> StoneList{get{return m_StoneList;} set{m_StoneList = value;}}
	public Log LogInstance{get{return m_Log;}set{m_Log = value;}}
	public bool TurnManager {get{return m_TurnManager;} set{m_TurnManager = value;}}
	public bool TurnWait{get{return m_TurnWait;} set{m_TurnWait = value;}}
	public int TurnCount{get{return m_TurnCount;} set{m_TurnCount = value;}}
	public List<int> Available{get{return m_Available;}}
	public bool SetAvailableManager{get{return m_SetAvailableManager;}set{m_SetAvailableManager = value;}}
	public VictoryOrDefeatText VOD{get{return m_VictoryOrDefeatText;}set{m_VictoryOrDefeatText = value;}}
	public List<GameObject> GameSet{get{return m_GameSet;}set{m_GameSet = value;}}

	// タイルオブジェクトの作成.
	// @pos	:	配置する座標.
	public GameObject SetTile(Vector3 pos)
	{
		return (GameObject)GameObject.Instantiate (m_Tile, pos, Quaternion.identity);
	}
	
	// 石オブジェクトの作成.
	// @pos	:	作成する座標.
	// @color:	作成する色	true:白	false:黒.
	public GameObject CreateStone(Vector3 pos, bool color)
	{
		pos = new Vector3 (pos.x, pos.y, -0.2f);
		GameObject stone = (GameObject)GameObject.Instantiate (m_Stone, pos, m_Stone.transform.localRotation);
		if (color) {
			stone.GetComponent<MeshRenderer>().material.color = Color.white;
			stone.GetComponent<Stone>().StoneColor = true;
		}
		else{
			stone.GetComponent<MeshRenderer>().material.color = Color.black;
			stone.GetComponent<Stone>().StoneColor = false;
		}
		return stone;
	}

	public void DeleateStone(GameObject p_Stone, Tile p_Tile)
	{
		p_Tile.Stone = false;
		Destroy (p_Stone);
	}

	// 石オブジェクトの配置.
	// @p_Tile	:	作成する位置のタイル
	// @color:	作成する色	true:白	false:黒.
	public bool SetStone(Tile p_Tile, bool color)
	{
		// 石が置いてあるか.
		if(!p_Tile.Stone){
			// 置ける場所を探す.
			foreach(var i in m_Available){
				if(p_Tile.Num == i){
					p_Tile.Stone = true;
					p_Tile.StoneColor = color;
					StoneList.Add(CreateStone(MatrixPos(p_Tile.Row, p_Tile.Column), color));
					m_StoneList[m_StoneList.Count -1].GetComponent<Stone>().StoneColor = color;
					p_Tile.StoneNum = StoneList.Count - 1;
					return true;
				}
			}
		}
		return false;
	}

	// 行列から配列番号の計算.
	// @ p_Row		:	行数.
	// @ p_Column	:	列数.
	public int MatrixBox(int p_Row, int p_Column)
	{
		return (p_Row * COLUMN) + p_Column;
	}

	// 行列から座標を計算する.
	// @ p_Row		:	行数.
	// @ p_Column	:	列数.
	public Vector3 MatrixPos(int p_Row, int p_Column)
	{
		Vector3 retpos;
		retpos = new Vector3((m_Tile.transform.localScale.x + MARGE) * p_Row,
		                     (m_Tile.transform.localScale.y + MARGE)* p_Column,
		                      0);
		return retpos;
	}	

	// 裏返せる場所を検索.
	// @p_Num	:	空のマスの座標.
	// @p_MyColor:	置く側の色.
	// 返り値	:	裏返せる場所のタイルの配列番号.
	public List<int> TurnAvailable(int p_Num, bool p_MyColor)
	{
		List<int> retNum = new List<int>();

		// 8方向検索.
		int[] eightDirection = new int[]{-7, 1, 9, -8, 8, -9, -1, 7};
		for(int dir = 0; dir < 8; dir++)
		{
			// 現在調べている場所.
			int nowPos = p_Num;
			List<int> keepNum = new List<int> ();
 			while(true)
			{
				if(nowPos < 8 && (eightDirection[dir] == -7 || eightDirection[dir] == -8 || eightDirection[dir] == -9)){// 左端.
					break;
				}if(nowPos >= 56 && (eightDirection[dir] == 9 || eightDirection[dir] == 8 || eightDirection[dir] == 7)){// 右端. 
					break;
				}if(nowPos % 8 == 7 && (eightDirection[dir] == -7 || eightDirection[dir] == 1 || eightDirection[dir] == 9)){// 上端.
					break;
				}if(nowPos % 8 == 0 && (eightDirection[dir] == -9 || eightDirection[dir] == -1 || eightDirection[dir] == 7)){// 下端.
					break;
				}

				// 検索開始.
				nowPos += eightDirection[dir];
				// 石が存在しているか.
				if(m_TileArray [nowPos].GetComponent<Tile> ().Stone)
				{
					// 色が自分の色と異なる.
					if(m_TileArray [nowPos].GetComponent<Tile> ().StoneColor != p_MyColor){
						if(nowPos >= 0 && nowPos <= 63){
							keepNum.Add(nowPos);	// 裏返せる場所の格納.
							continue;
						}
					}else{
						retNum.AddRange(keepNum);
						break;
					}
				}else{
					break;
				}
			}
		}
		return retNum;
	}

	// 置ける場所の表示.
	// @p_MyColor	:	置く側の色.
	public void SetAvailable(bool p_MyColor)
	{
		m_Available.Clear ();
//		List<int> count = new List<int>();
//		Debug.Log("SetAvailable:"+p_MyColor);
		for (int i = 0; i < ROW * COLUMN; i++) {
			// そのマスに石が存在しない.
			if (!m_TileArray [i].GetComponent<Tile> ().Stone) {
	//			count.AddRange(TurnAvailable(i, p_MyColor));
				if(TurnAvailable(i, p_MyColor).Count > 0){
//					Debug.Log("TurnAvailable(i, p_MyColor).Count > 0");
					m_Available.Add(i);	// 石を置ける場所をプレイヤー用に格納.
					m_TileArray[i].GetComponent<MeshRenderer>().material.color = Color.red;
				}
			}
		}
	}

	public void SetPass(bool p_MyColor)
	{
		m_Available.Clear ();
		List<int> count = new List<int>();
		for (int i = 0; i < ROW * COLUMN; i++) {
			// そのマスに石が存在しない.
			if (!m_TileArray [i].GetComponent<Tile> ().Stone) {
				count.AddRange(TurnAvailable(i, p_MyColor));
				if(TurnAvailable(i, p_MyColor).Count > 0){
					m_Available.Add(i);	// 石を置ける場所をプレイヤー用に格納;
				}
			}
		}
		// 石が置けるマスの数が.
		if (m_Available.Count <= 0 && m_StoneList.Count < ROW * COLUMN) {
			// 0以下だと,もう石はおけない(パス).
			m_Log.StoneColor.Insert (0, p_MyColor);
			m_Log.Row.Insert (0,-1);
			m_Log.Column.Insert (0, -1);
			BoardLog(true);
			m_TurnManager = !p_MyColor;
			m_TurnCount++;
			m_PassCount++;
			SetAvailable(m_TurnManager);
			if (m_PassCount >= 2) {
				// 2回以上パスでゲーム終了.
				m_SetAvailableManager = false;
				CheckVictoryOrDefeat(p_MyColor);
				foreach(var x in m_GameSet){
					x.SendMessage("SetUI");
					x.SendMessage("SetText");
				}
			}
		} else {
			m_PassCount = 0;
		}
		
		if (m_StoneList.Count == ROW * COLUMN) {
			m_SetAvailableManager = false;
			foreach(var x in m_GameSet){
				x.SendMessage("SetUI");
				x.SendMessage("SetText");
			}
			CheckVictoryOrDefeat(p_MyColor);
		}
	}

	// 裏返す.
	// @i			;	次のターンのステージ情報.
	// @p_MyColor	:	自分の色(裏返す色).
	public void AvailableReverse(List<int> i, bool p_MyColor)
	{
		Color reverseColor = new Color();
		if (p_MyColor) {
			reverseColor = Color.white;
		} else {
			reverseColor = Color.black;
		}

		foreach (var num in i) {
			// タイル情報更新.
			m_TileArray[num].GetComponent<Tile>().StoneColor = p_MyColor;
//			int numj = m_TileArray[num].GetComponent<Tile>().StoneNum;

			StoneList[m_TileArray[num].GetComponent<Tile>().StoneNum].GetComponent<MeshRenderer>().material.color = reverseColor;
			StoneList[m_TileArray[num].GetComponent<Tile>().StoneNum].GetComponent<Stone>().StoneColor = p_MyColor;
		}
	}

	// 石を裏返す.
	// @tile	:	調べたいタイルのインスタンス.
	// @p_MyColor:	裏返した後の色.
	// 返り値:石が置けたか
	public bool SetReverse(Tile tile, bool p_MyColor)
	{
		// 石設置.
		bool ok = SetStone (tile, p_MyColor);
		if (ok) {
			// ログ挿入.
			m_Log.StoneColor.Insert (0, p_MyColor);
			m_Log.Row.Insert (0, tile.Row);
			m_Log.Column.Insert (0, tile.Column);
		
			// 石を裏返す.
			List <int> num = TurnAvailable (tile.Num, p_MyColor);

			AvailableReverse (num, p_MyColor);
//		m_TurnCount++;
		}
		return ok;
	}

	// ボードの色の初期化.
	public void RefreshmentBoard()
	{
		for (int i = 0; i < ROW * COLUMN; i++) {
			m_TileArray[i].GetComponent<MeshRenderer>().material.color = Color.green;

		}
	}

	// 現在どちらが勝っているか.
	// @p_MyColor	:	自手の色.
	public VictoryOrDefeat CheckVictoryOrDefeat(bool p_MyColor)
	{
		// まだ石が置けるか.
		// 石の数を比較.
		int countWhite = 0;
		int countBlack = 0;
		for (int i = 0; i < ROW * COLUMN; i++) {
			if (m_TileArray [i].GetComponent<Tile> ().StoneColor) {
				countWhite++;
			} else {
				countBlack++;
			}
		}
		Debug.Log ("whiteNum:" + countWhite);
		Debug.Log ("blackNum:" + countBlack);
		// 勝敗を返す.
		if (countWhite < countBlack) {
			Debug.Log ("vod1");
			m_VictoryOrDefeatText.SendMessage ("VictoryBlack");
			return VictoryOrDefeat.Victory;
		} else if (countWhite > countBlack) {
			Debug.Log ("vod3");
			m_VictoryOrDefeatText.SendMessage ("VictoryWhite");
			return VictoryOrDefeat.Victory;
		} else if (countWhite == countBlack) {
			Debug.Log ("vod5");
			m_VictoryOrDefeatText.SendMessage ("VictoryDraw");
			return VictoryOrDefeat.Draw;
		}
		return VictoryOrDefeat.Draw;
		
	}

	// 石の数を調べる.
	// @p_Color	:	調べたい色.
	public int StoneCount(bool p_Color)
	{
		int count = 0;
		for (int i = 0; i < m_StoneList.Count; i++) {
			if(m_StoneList[i] != null)
			{
				if(m_StoneList[i].GetComponent<Stone>().StoneColor == p_Color)
				{
					count++;
				}
			}
		}
		return count;
	}

	// 呼び出したターンの盤面情報を格納.
	public void BoardLog(bool p_Pass)
	{

		Tile.data[] array = new Tile.data[ROW * COLUMN];
		for (int i = 0; i < ROW * COLUMN; i++) {
			if (!p_Pass) {
				array [i] = new Tile.data (m_TileArray [i].GetComponent<Tile> ().Stone, m_TileArray [i].GetComponent<Tile> ().StoneColor, m_TileArray [i].GetComponent<Tile> ().StoneNum);
			} else {
				array [i] = new Tile.data (m_TileArray [i].GetComponent<Tile> ().Stone, m_TileArray [i].GetComponent<Tile> ().StoneColor, m_TileArray [i].GetComponent<Tile> ().StoneNum);
			} 
		}
		// リストにそのターンの盤面情報格納.
		BoardHistory.Insert(0, array);
	}


	// 呼び出したターンの盤面の状態に戻る.
	// @p_Turn	:	戻りたいターン数.(何ターン前か).
	// @p_MyColor:	戻りたいターンの色.
	public void BoardReStartTheWorld(int p_Turn, bool p_MyColor)
	{
		m_ReCount++;
		// 現在のターンが2ターン以上経過しているなら.
		if ((p_Turn >= 2 && m_TurnCount > 2) || (p_Turn == 1 && m_TurnCount >= 2)) {
			// ターン数の情報を更新.
			m_TurnCount -= p_Turn;

			// 盤面に合わせて石を設置.
			// 戻りたいターンまでの石を削除.
			// 元のリスト削除.
			for (int i = 0; i < p_Turn; i++) {
				if (m_Log.Row [m_ReCount-1] != -1){
					DestroyImmediate (StoneList [StoneList.Count - 1]);
					StoneList.RemoveAt (StoneList.Count - 1);
				}
				m_TurnManager = !m_TurnManager;
				BoardHistory.RemoveAt (0);
			}

			// 盤面の状態を更新.
			if (m_Log.Row [m_ReCount-1] == -1 && m_TurnCount >= 2) {
				// パス.
				BoardReStartTheWorld (p_Turn, p_MyColor);
			} else {
				for (int i = 0; i < ROW * COLUMN; i++) {
					m_TileArray [i].GetComponent<Tile> ().SetInfo (BoardHistory [0] [i].s_Stone, BoardHistory [0] [i].s_StoneColor, BoardHistory [0] [i].s_StoneNum);
				}
			}

			// 石の色を元に戻す.
			for (int i = 0; i < ROW * COLUMN; i++) {
				if (m_TileArray [i].GetComponent<Tile> ().Stone) {
					StoneList [m_TileArray [i].GetComponent<Tile> ().StoneNum].GetComponent<Stone> ().StoneColor = m_TileArray [i].GetComponent<Tile> ().StoneColor;
					if (m_TileArray [i].GetComponent<Tile> ().StoneColor) {
						StoneList [m_TileArray [i].GetComponent<Tile> ().StoneNum].GetComponent<MeshRenderer> ().material.color = Color.white;
					} else {
						StoneList [m_TileArray [i].GetComponent<Tile> ().StoneNum].GetComponent<MeshRenderer> ().material.color = Color.black;
					}
				}
			}
			Debug.Log (p_MyColor + "のターン");
			m_ReCount = 0;
		}
	}


	void Awake()
	{
		// 配置するプレハブの読み込み. 
		m_Tile = (GameObject) Resources.Load("Prefabs/tile");

		// 配置するプレハブの読み込み.
		m_Stone = (GameObject) Resources.Load("Prefabs/stone");
	}


	void Start()
	{
		// 初期化.
		m_VictoryOrDefeat = VictoryOrDefeat.Draw;
		m_SetAvailableManager = true;
		m_PassCount = 0;
		m_TurnCount = 1;
		m_Log = GameObject.Find("Log").GetComponent<Log>();
		m_VictoryOrDefeatText = GameObject.Find("VictoryOrDefeat").GetComponent<VictoryOrDefeatText>();
		m_GameSet.Add (GameObject.Find("PanelFront")as GameObject);
		m_GameSet.Add (GameObject.Find("PanelBG") as GameObject);
		m_GameSet.Add (GameObject.Find("Result") as GameObject);
		ui = GameObject.Find ("Text").GetComponent<Ui>();
		m_TurnWait = false;

		// タイル配置.
		int count = 0;
		Vector3 tilePos = Vector3.zero;
		for (int row = 0; row < ROW; row++) 
		{
			m_Tile.GetComponent<Tile>().Row = row;
			for(int column = 0; column < COLUMN; column++)
			{
				tilePos = new Vector3((m_Tile.transform.localScale.x + MARGE) * row,
				                      (m_Tile.transform.localScale.y + MARGE)* column,
				                      0);
				
				if(m_Tile != null)
				{
					m_TileArray[count] = (GameObject)GameObject.Instantiate(m_Tile, tilePos, Quaternion.identity);
					m_TileArray[count].name = "" + (row+1) + (column+1);
					m_TileArray[count].GetComponent<Tile>().Num = count;
					m_TileArray[count].GetComponent<Tile>().Column = column;

				}
				count++;
			}
		}

		// 石配置.

		m_StoneList.Add (CreateStone(MatrixPos(ROW/2 -1, COLUMN/2 - 1),false));	m_StoneList [0].name = "stone0";	m_TileArray[MatrixBox(ROW/2 -1, COLUMN/2 - 1)].GetComponent<Tile>().Stone = true;	m_TileArray [MatrixBox (ROW/2 -1, COLUMN/2 - 1)].GetComponent<Tile> ().StoneColor = false; m_TileArray [MatrixBox (ROW/2 -1, COLUMN/2 - 1)].GetComponent<Tile> ().StoneNum = m_StoneList.Count-1;	m_StoneList [0].GetComponent<Stone> ().StoneColor = false;
		m_StoneList.Add (CreateStone(MatrixPos(ROW/2 -1, COLUMN/2),true));	m_StoneList [1].name = "stone1";	m_TileArray[MatrixBox(ROW/2 -1, COLUMN/2)].GetComponent<Tile>().Stone = true;	m_TileArray [MatrixBox (ROW/2 -1, COLUMN/2)].GetComponent<Tile> ().StoneColor = true;	 m_TileArray [MatrixBox (ROW/2 -1, COLUMN/2)].GetComponent<Tile> ().StoneNum = m_StoneList.Count-1;	m_StoneList [1].GetComponent<Stone> ().StoneColor = true;
		m_StoneList.Add (CreateStone(MatrixPos(ROW/2, COLUMN/2 - 1),true));	m_StoneList [2].name = "stone2";	m_TileArray[MatrixBox(ROW/2, COLUMN/2 - 1)].GetComponent<Tile>().Stone = true;	m_TileArray [MatrixBox (ROW/2, COLUMN/2 - 1)].GetComponent<Tile> ().StoneColor = true;	 m_TileArray [MatrixBox (ROW/2, COLUMN/2 - 1)].GetComponent<Tile> ().StoneNum = m_StoneList.Count-1;	m_StoneList [2].GetComponent<Stone> ().StoneColor = true;
		m_StoneList.Add (CreateStone(MatrixPos(ROW/2, COLUMN/2),false));	m_StoneList [3].name = "stone3";	m_TileArray[MatrixBox(ROW/2, COLUMN/2)].GetComponent<Tile>().Stone = true;	m_TileArray [MatrixBox (ROW/2, COLUMN/2)].GetComponent<Tile> ().StoneColor = false; m_TileArray [MatrixBox (ROW/2, COLUMN/2)].GetComponent<Tile> ().StoneNum = m_StoneList.Count-1;	m_StoneList [3].GetComponent<Stone> ().StoneColor = false;

/*
		m_StoneList.Add (CreateStone(MatrixPos(3, 3),true));	m_StoneList [0].name = "stone0";	m_TileArray[MatrixBox(3,3)].GetComponent<Tile>().Stone = true;	m_TileArray [MatrixBox (3, 3)].GetComponent<Tile> ().StoneColor = true; m_TileArray [MatrixBox (3, 3)].GetComponent<Tile> ().StoneNum = m_StoneList.Count-1;	m_StoneList [0].GetComponent<Stone> ().StoneColor = true;
		m_StoneList.Add (CreateStone(MatrixPos(2, 2),false));	m_StoneList [1].name = "stone1";	m_TileArray[MatrixBox(2,2)].GetComponent<Tile>().Stone = true;	m_TileArray [MatrixBox (2, 2)].GetComponent<Tile> ().StoneColor = false; m_TileArray [MatrixBox (2, 2)].GetComponent<Tile> ().StoneNum = m_StoneList.Count-1;	m_StoneList [1].GetComponent<Stone> ().StoneColor = false;
		m_StoneList.Add (CreateStone(MatrixPos(2, 3),false));	m_StoneList [2].name = "stone2";	m_TileArray[MatrixBox(2,3)].GetComponent<Tile>().Stone = true;	m_TileArray [MatrixBox (2, 3)].GetComponent<Tile> ().StoneColor = false; m_TileArray [MatrixBox (2, 3)].GetComponent<Tile> ().StoneNum = m_StoneList.Count-1;	m_StoneList [2].GetComponent<Stone> ().StoneColor = false;
		m_StoneList.Add (CreateStone(MatrixPos(2, 4),false));	m_StoneList [3].name = "stone3";	m_TileArray[MatrixBox(2,4)].GetComponent<Tile>().Stone = true;	m_TileArray [MatrixBox (2, 4)].GetComponent<Tile> ().StoneColor = false; m_TileArray [MatrixBox (2, 4)].GetComponent<Tile> ().StoneNum = m_StoneList.Count-1;	m_StoneList [3].GetComponent<Stone> ().StoneColor = false;
		m_StoneList.Add (CreateStone(MatrixPos(3, 2),false));	m_StoneList [4].name = "stone4";	m_TileArray[MatrixBox(3,2)].GetComponent<Tile>().Stone = true;	m_TileArray [MatrixBox (3, 2)].GetComponent<Tile> ().StoneColor = false; m_TileArray [MatrixBox (3, 2)].GetComponent<Tile> ().StoneNum = m_StoneList.Count-1;	m_StoneList [4].GetComponent<Stone> ().StoneColor = false;
		m_StoneList.Add (CreateStone(MatrixPos(3, 4),false));	m_StoneList [5].name = "stone5";	m_TileArray[MatrixBox(3,4)].GetComponent<Tile>().Stone = true;	m_TileArray [MatrixBox (3, 4)].GetComponent<Tile> ().StoneColor = false; m_TileArray [MatrixBox (3, 4)].GetComponent<Tile> ().StoneNum = m_StoneList.Count-1;	m_StoneList [5].GetComponent<Stone> ().StoneColor = false;
		m_StoneList.Add (CreateStone(MatrixPos(4, 2),false));	m_StoneList [6].name = "stone6";	m_TileArray[MatrixBox(4,2)].GetComponent<Tile>().Stone = true;	m_TileArray [MatrixBox (4, 2)].GetComponent<Tile> ().StoneColor = false; m_TileArray [MatrixBox (4, 2)].GetComponent<Tile> ().StoneNum = m_StoneList.Count-1;	m_StoneList [6].GetComponent<Stone> ().StoneColor = false;
		m_StoneList.Add (CreateStone(MatrixPos(4, 3),false));	m_StoneList [7].name = "stone7";	m_TileArray[MatrixBox(4,3)].GetComponent<Tile>().Stone = true;	m_TileArray [MatrixBox (4, 3)].GetComponent<Tile> ().StoneColor = false; m_TileArray [MatrixBox (4, 3)].GetComponent<Tile> ().StoneNum = m_StoneList.Count-1;	m_StoneList [7].GetComponent<Stone> ().StoneColor = false;
		m_StoneList.Add (CreateStone(MatrixPos(4, 4),false));	m_StoneList [8].name = "stone8";	m_TileArray[MatrixBox(4,4)].GetComponent<Tile>().Stone = true;	m_TileArray [MatrixBox (4, 4)].GetComponent<Tile> ().StoneColor = false; m_TileArray [MatrixBox (4, 4)].GetComponent<Tile> ().StoneNum = m_StoneList.Count-1;	m_StoneList [8].GetComponent<Stone> ().StoneColor = false;

*/
/*
		m_StoneList.Add (CreateStone(MatrixPos(3, 3),true));	m_StoneList [0].name = "stone0";	m_TileArray[MatrixBox(3,3)].GetComponent<Tile>().Stone = true;	m_TileArray [MatrixBox (3, 3)].GetComponent<Tile> ().StoneColor = true; m_TileArray [MatrixBox (3, 3)].GetComponent<Tile> ().StoneNum = m_StoneList.Count-1;	m_StoneList [0].GetComponent<Stone> ().StoneColor = true;
		m_StoneList.Add (CreateStone(MatrixPos(2, 2),true));	m_StoneList [1].name = "stone1";	m_TileArray[MatrixBox(2,2)].GetComponent<Tile>().Stone = true;	m_TileArray [MatrixBox (2, 2)].GetComponent<Tile> ().StoneColor = true; m_TileArray [MatrixBox (2, 2)].GetComponent<Tile> ().StoneNum = m_StoneList.Count-1;	m_StoneList [1].GetComponent<Stone> ().StoneColor = false;
//		m_StoneList.Add (CreateStone(MatrixPos(2, 3),false));	m_StoneList [2].name = "stone2";	m_TileArray[MatrixBox(2,3)].GetComponent<Tile>().Stone = true;	m_TileArray [MatrixBox (2, 3)].GetComponent<Tile> ().StoneColor = false; m_TileArray [MatrixBox (2, 3)].GetComponent<Tile> ().StoneNum = m_StoneList.Count-1;	m_StoneList [2].GetComponent<Stone> ().StoneColor = false;
		m_StoneList.Add (CreateStone(MatrixPos(2, 4),true));	m_StoneList [2].name = "stone3";	m_TileArray[MatrixBox(2,4)].GetComponent<Tile>().Stone = true;	m_TileArray [MatrixBox (2, 4)].GetComponent<Tile> ().StoneColor = true; m_TileArray [MatrixBox (2, 4)].GetComponent<Tile> ().StoneNum = m_StoneList.Count-1;	m_StoneList [2].GetComponent<Stone> ().StoneColor = false;
//		m_StoneList.Add (CreateStone(MatrixPos(3, 2),false));	m_StoneList [4].name = "stone4";	m_TileArray[MatrixBox(3,2)].GetComponent<Tile>().Stone = true;	m_TileArray [MatrixBox (3, 2)].GetComponent<Tile> ().StoneColor = false; m_TileArray [MatrixBox (3, 2)].GetComponent<Tile> ().StoneNum = m_StoneList.Count-1;	m_StoneList [4].GetComponent<Stone> ().StoneColor = false;
		m_StoneList.Add (CreateStone(MatrixPos(3, 4),false));	m_StoneList [3].name = "stone5";	m_TileArray[MatrixBox(3,4)].GetComponent<Tile>().Stone = true;	m_TileArray [MatrixBox (3, 4)].GetComponent<Tile> ().StoneColor = false; m_TileArray [MatrixBox (3, 4)].GetComponent<Tile> ().StoneNum = m_StoneList.Count-1;	m_StoneList [3].GetComponent<Stone> ().StoneColor = false;
		m_StoneList.Add (CreateStone(MatrixPos(4, 2),true));	m_StoneList [4].name = "stone6";	m_TileArray[MatrixBox(4,2)].GetComponent<Tile>().Stone = true;	m_TileArray [MatrixBox (4, 2)].GetComponent<Tile> ().StoneColor = true; m_TileArray [MatrixBox (4, 2)].GetComponent<Tile> ().StoneNum = m_StoneList.Count-1;	m_StoneList [4].GetComponent<Stone> ().StoneColor = false;
//		m_StoneList.Add (CreateStone(MatrixPos(4, 3),false));	m_StoneList [7].name = "stone7";	m_TileArray[MatrixBox(4,3)].GetComponent<Tile>().Stone = true;	m_TileArray [MatrixBox (4, 3)].GetComponent<Tile> ().StoneColor = false; m_TileArray [MatrixBox (4, 3)].GetComponent<Tile> ().StoneNum = m_StoneList.Count-1;	m_StoneList [7].GetComponent<Stone> ().StoneColor = false;
		m_StoneList.Add (CreateStone(MatrixPos(4, 4),true));	m_StoneList [5].name = "stone8";	m_TileArray[MatrixBox(4,4)].GetComponent<Tile>().Stone = true;	m_TileArray [MatrixBox (4, 4)].GetComponent<Tile> ().StoneColor = true; m_TileArray [MatrixBox (4, 4)].GetComponent<Tile> ().StoneNum = m_StoneList.Count-1;	m_StoneList [5].GetComponent<Stone> ().StoneColor = false;
*/


		SetAvailable (false);
		// ターンの設定	初期は黒.
		m_TurnManager = false;

//		Debug.Log ("boardHistory" + BoardHistory.Count);
		BoardLog (false);

		ui.SendMessage("SetText");
	}

	void Update(){
		m_TurnWait = false;
	}
	
}
