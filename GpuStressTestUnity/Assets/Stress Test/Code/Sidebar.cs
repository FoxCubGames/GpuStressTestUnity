using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Sidebar : MonoBehaviour {

	public Text _alphaText;
	public CanvasGroup _canvasGroup;

	public void UpdateTransparency(float alpha) {
		_canvasGroup.alpha = alpha;
		_alphaText.text = alpha.ToString();
	}

}
