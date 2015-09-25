using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class GameSet : MonoBehaviour {

	private Board board;			// ボードにアクセスするための情報格納庫.
	private Color m_Color;
	[SerializeField]Text btnText;

	// Use this for initialization
	void Start () {
		board = GameObject.Find ("Board").GetComponent<Board> ();
		if (this.GetComponent<Image> () != null) {
			m_Color = this.GetComponent<Image> ().color;
			this.GetComponent<Image> ().color = new Color (m_Color.r, m_Color.g, m_Color.b, 0.0f);
		}
		if(btnText != null)
		TransmissionText ();
	}
	
	void SetUI()
	{
		this.GetComponent<Image>().color = new Color(m_Color.r, m_Color.g, m_Color.b, m_Color.a);
	}

	void TransmissionUI()
	{
		this.GetComponent<Image>().color = new Color(m_Color.r, m_Color.g, m_Color.b, 0.0f);
	}

	void TransmissionText(){
		btnText.text = "";
	}

	void SetText(){
		if(btnText != null)
		btnText.text = "Result";
	}

	public void OnClick()
	{
		// ゲームが終了している.
		if (!board.SetAvailableManager) {
			// 見やすいようにゲームセット時の表示消す.
			foreach (var x in board.GameSet) {
				x.SendMessage ("TransmissionUI");
			}

			if (btnText != null) {
				btnText.text = "";
			}
			board.VOD.SetVictory (board.StoneCount (true), board.StoneCount (false));
			board.VOD.SetPos(new Vector3(320.0f, 120.0f, 0.0f));
		}
	}
}
