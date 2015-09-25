using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Ui : MonoBehaviour {

	[SerializeField]Text btnText;
	public Board board;			// ボードにアクセスするための情報格納庫.

	
	void Start () {
		board = GameObject.Find ("Board").GetComponent<Board> ();
	}

	void Update () {
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

	private void SetText()
	{
		btnText.text = 	"ターン:" + board.TurnCount + "\n"
				+"現在:" + TurnText(board.TurnManager) + "\n"
				+"黒の数:" + board.StoneCount(false) + "\n"
				+"白の数:" + board.StoneCount(true) + "\n";
	}
}
