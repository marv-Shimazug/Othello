using UnityEngine;
using System.Collections;

public class Stone : MonoBehaviour {

	private bool m_Color;		// 自分の色	true:白	false:黒.

	// アクセサ.
	public bool StoneColor{get{return m_Color;}set{m_Color = value;}}
}
