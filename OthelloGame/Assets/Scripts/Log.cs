using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Log : MonoBehaviour {

	private List<int> m_Before = new List<int>();		// 何手前か.
	private List<bool> m_StoneColor = new List<bool>();	// 色.
	private List<int> m_Row = new List<int>();			// 座標.
	private List<int> m_Column = new List<int>();		// 座標.

	// アクセサ.
	public List<int> Before{ get { return m_Before; } set { m_Before = value; } }
	public List<bool> StoneColor{get{return m_StoneColor;}set{m_StoneColor = value;}}
	public List<int> Row{get{return m_Row;}set{m_Row = value;}}
	public List<int> Column{get{return m_Column;}set{m_Column = value;}}

	[SerializeField]Text btnText;

	void Update()
	{
		btnText.text = "Log\n";
		int count = 0;
		for (int i = 0; i < m_StoneColor.Count; i++) {
			count++;
			btnText.text += "" + count + "手前:" + TurnText(m_StoneColor[i]) + "  " + PosText(m_Row[i], m_Column[i]) + "\n";
		}
	}

	private string TurnText(bool p_Turn)
	{
		string text;
		if (p_Turn) {
			text = "白";
		} else {
			text = "黒";
		}
		return text;
	}

	// 座標.
	// @p_Row	:	行.
	// @p_Column:	列.
	private string PosText(int p_Row, int p_Column)
	{
		string text = "";

		switch (p_Column) {
		case -1:
			text += "パス";
			break;
		case 0:
			text += "8";
			break;
		case 1:
			text += "7";
			break;
		case 2:
			text += "6";
			break;
		case 3:
			text += "5";
			break;
		case 4:
			text += "4";
			break;
		case 5:
			text += "3";
			break;
		case 6:
			text += "2";
			break;
		case 7:
			text += "1";
			break;
			
		}

		switch (p_Row) {
		case -1:
			break;
		case 0:
			text += "a";
			break;
		case 1:
			text += "b";
			break;
		case 2:
			text += "c";
			break;
		case 3:
			text += "d";
			break;
		case 4:
			text += "e";
			break;
		case 5:
			text += "f";
			break;
		case 6:
			text += "g";
			break;
		case 7:
			text += "h";
			break;
		
		}

		return text;
	}

	public void DeleteLog(int p_Turn){
		for (int i = 0; i < p_Turn; i++) {
			m_StoneColor.RemoveAt (0);
			m_Row.RemoveAt (0);
			m_Column.RemoveAt (0);
		}
	}

}
