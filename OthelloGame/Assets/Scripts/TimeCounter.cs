using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimeCounter : MonoBehaviour {

	private float time = 0;
	private Board board;			// ボードにアクセスするための情報格納庫.

	void Start()
	{
		board = GameObject.Find ("Board").GetComponent<Board> ();
		GetComponent<Text> ().text = ((int)time).ToString ();
	}

	void Update()
	{
		// 決着までの時間をカウント.
		if (board.SetAvailableManager) 
		{
			time += Time.deltaTime;
			GetComponent<Text> ().text = ((int)time).ToString ();
		}

	}
}
