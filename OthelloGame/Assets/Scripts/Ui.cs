using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Ui : MonoBehaviour {

	[SerializeField]Text btnText;
	public Board board;			// ボードにアクセスするための情報格納庫.
	private GameInfo gameInfo;
	
	void Start () {
		board = GameObject.Find ("Board").GetComponent<Board> ();
		gameInfo = GameObject.Find ("GameInfo").GetComponent<GameInfo>();
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
				+"黒の数:" + (board.StoneCount(false) + gameInfo.GetMultiplicationBlack) + "\n"
				+"白の数:" + (board.StoneCount(true) + gameInfo.GetMultiplicationWhite) + "\n";
	}
}
