using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimeLimit : MonoBehaviour {

	private float time = 3;
	private Board board;			// ボードにアクセスするための情報格納庫.
	private GameInfo gameInfo;
	private PlayerInput playerInput;
	private bool UseTimeLimit;

	void Start()
	{
		gameInfo = GameObject.Find ("GameInfo").GetComponent<GameInfo>();
		time = gameInfo.GetTimeLimit;
		UseTimeLimit = true;
		board = GameObject.Find ("Board").GetComponent<Board> ();
		GetComponent<Text> ().text = ((int)time).ToString ();
		playerInput = GameObject.Find("PlayerInput").GetComponent<PlayerInput>();
		if (0 == time) 
		{
			UseTimeLimit = false;
			GetComponent<Text> ().text = "";
		}
	}

	void Update()
	{
		// 決着までの時間をカウント.
		if (board.SetAvailableManager && true == UseTimeLimit) 
		{
			time -= Time.deltaTime;
			GetComponent<Text> ().text = "制限時間：" + ((int)time).ToString ();

			// 制限時間.
			if(0 >= time)
			{
				board.SetAvailableManager = false;
				if (true == playerInput.PlayerColor)
				{
					board.VOD.SendMessage ("VictoryBlack");
					foreach(var x in board.GameSet)
					{
						x.SendMessage("SetUI");
						x.SendMessage("SetText");
					}
				} 
				else if (false == playerInput.PlayerColor) 
				{
					board.VOD.SendMessage ("VictoryWhite");
					foreach(var x in board.GameSet)
					{
						x.SendMessage("SetUI");
						x.SendMessage("SetText");
					}
				}
			}
		}
	}

	void ResetTimeLimit()
	{
		time = time = gameInfo.GetTimeLimit;
	}

}
