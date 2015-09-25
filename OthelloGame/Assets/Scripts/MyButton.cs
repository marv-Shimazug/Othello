using UnityEngine;
using System.Collections;

public class MyButton : MonoBehaviour {

	public void OnClick()
	{
		if (Application.loadedLevelName == "title") {
			Application.LoadLevel ("main");
		} else if(Application.loadedLevelName == "main"){
			Application.LoadLevel("title");
		}
	}
}
