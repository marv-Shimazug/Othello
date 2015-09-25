using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VictoryOrDefeatText : MonoBehaviour {

	[SerializeField]Text btnText;
	private Board board;			// ボードにアクセスするための情報格納庫.

	public enum Vod{
		black,
		white,
		draw,
	}

	void Start () {
		board = GameObject.Find ("Board").GetComponent<Board> ();
		btnText.text = "";
	}

	// 勝ち負け表示.
	// p_VoD	:	勝った色.
	private string VictoryText(Vod p_VoD)
	{
		string retText = "";
		if (p_VoD == Vod.white) {
			retText = "白の勝ち\n\n";
		} else if (p_VoD == Vod.black) {
			retText = "黒の勝ち\n\n";
		} else {
			retText = "引き分け\n\n";
		}

		retText += "黒の数:" + board.StoneCount (false) + "\n";
		retText += "白の数:" + board.StoneCount (true) + "\n";

		return retText;
	}

	public void VictoryBlack()
	{
		btnText.text = VictoryText (Vod.black);

	}

	public void VictoryWhite()
	{
		btnText.text = VictoryText (Vod.white);
	}

	public void VictoryDraw()
	{
		btnText.text = VictoryText (Vod.draw);
	}

	public string SetVictory(int p_White, int p_Black){
		string retText = "";
		if (p_White > p_Black) {
			retText = "白の勝ち";
		} else if (p_White < p_Black) {
			retText = "黒の勝ち";
		} else {
			retText = "引き分け";
		}
		btnText.text = retText;
		return retText;
	}

	public void SetPos(Vector3 pos)
	{
		btnText.rectTransform.localPosition = pos;
	}
}
