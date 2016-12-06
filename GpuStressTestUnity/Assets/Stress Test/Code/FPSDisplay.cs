using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour {

	public Text _fpsText;

	int _frames;
	float _time;

	void Update() {
		_frames++;
		_time += Time.deltaTime;

		if (_frames >= 30) {
			UpdateText();
			_frames = 0;
			_time = 0f;
		}
	}

	void UpdateText() {
		_fpsText.text = string.Format("FPS: {0:0.0} ms ({1:0.} fps)", (_time / (float)_frames) * 1000f, (float)_frames / _time);
	}
}
