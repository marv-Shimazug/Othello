using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SelectPlayerButton : MonoBehaviour {

	[SerializeField]Text btnText;
	GameInfo gameInfo;

	void Start()
	{
		gameInfo = GameObject.Find ("GameInfo").GetComponent<GameInfo>();
		OnClick ();
	}

	public void OnClick()
	{
		if (this.gameObject.name == "blackPlayer") {
			// 黒プレイヤーのボタン.
			gameInfo.BlackPlayer = SelectPlayerInfo(gameInfo.BlackPlayer);
		}
		else if(this.gameObject.name == "whitePlayer"){
			// 白プレイヤーのボタン.
			gameInfo.WhitePlayer = SelectPlayerInfo(gameInfo.WhitePlayer);
		}
	}

	public bool SelectPlayerInfo(bool p_Player){
		if(p_Player){
			// プレイヤーを表示している状態でおされたらAIに変える.
			btnText.text = "AI";
			return false;
		}else{
			btnText.text = "Player";
			return true;
		}
	}
}
