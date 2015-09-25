using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	// TileInfo.
	private int m_Num;			// 自分の番号.
	public int m_Row;			// 自分の行.
	public int m_Column;		// 自分の列.
	private bool m_Stone;		// 自分の上に石は置いてあるか.
	private bool m_StoneColor;	// 自分の上の石は何色か.	true:白	false:黒
	private int m_StoneNum;		// 自分の上の石の番号.

	public struct data
	{
		public bool s_Stone;
		public bool s_StoneColor;
		public int s_StoneNum;
		// コンストラクタ.
		public data(bool p_Stone, bool p_StoneColor, int p_StoneNum)
		{
			s_Stone = p_Stone; 
			s_StoneColor = p_StoneColor; 
			s_StoneNum = p_StoneNum;
		}
	}

	// アクセサ.
	public int Num{get{return m_Num;}set{m_Num = value;}}
	public int Row{get{return m_Row;}set{m_Row = value;}}
	public int Column{get{return m_Column;}set{m_Column = value;}}
	public bool Stone{get{return m_Stone;}set{m_Stone = value;}}
	public bool StoneColor{get{return m_StoneColor;}set{m_StoneColor = value;}}
	public int StoneNum{get{return m_StoneNum;}set{m_StoneNum = value;}}

	public void SetInfo(bool p_Stone, bool p_StoneColor, int p_StoneNum){
		m_Stone = p_Stone;
		m_StoneColor = p_StoneColor;
		m_StoneNum = p_StoneNum;
	}
}
