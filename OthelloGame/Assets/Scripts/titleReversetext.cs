using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class titleReversetext : MonoBehaviour {

	[SerializeField]Text btnText;
//	Text.Color
	private float alpha;
	private bool apFlag;
	// Use this for initialization
	void Start () {
		alpha = 1.0f;
		apFlag = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (apFlag) {
			alpha += 0.005f;
			if (alpha >= 1.0f) {
				apFlag = false;
			}
		} else {
			alpha -= 0.005f;
			if(alpha <= 0.0f){
				apFlag = true;
			}
		}

		btnText.color = new Color (btnText.color.r, btnText.color.g, btnText.color.b, alpha);
	}
}
