using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour {

	public Text _fpsText;
	float _deltaTime = 0.0f;

	void Update() {
		_deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
		UpdateText();
	}

	void UpdateText() {
		float msec = _deltaTime * 1000.0f;
		float fps = 1.0f / _deltaTime;
		_fpsText.text = string.Format("FPS: {0:0.0} ms ({1:0.} fps)", msec, fps);
	}
}
