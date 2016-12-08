using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FpsDisplay : MonoBehaviour {

	public Text _fpsText;

	public static FpsDisplay _instance; 

	int _frames;
	float _time;

	/// <summary>
	/// the frame that we last updated the fps on
	/// </summary>
	int _updateFrame = 0;

	float _fps;
	/// <summary>
	/// current frame rate. averaged over 30 frames.
	/// </summary>
	public float _Fps { get { return _fps; } }

	float _ms;
	/// <summary>
	/// number of milliseconds to render 1 frame. averaged over 30 frames.
	/// </summary>
	public float _Ms { get { return _ms; } }

	void Awake() {
		_instance = this;
	}

	void Update() {
		_frames++;
		_time += Time.deltaTime;

		if (_frames >= 30) {
			_updateFrame = Time.frameCount;
			_ms = (_time / (float)_frames) * 1000f;
			_fps = (float)_frames / _time;
			UpdateText();
			_frames = 0;
			_time = 0f;
		}
	}

	void UpdateText() {
		_fpsText.text = string.Format("FPS: {0:0.0} ms ({1:0.} fps)", _ms, _fps);
	}

	/// <summary>
	/// lets you wait for the next update. will not return on the same frame.
	/// </summary>
	public IEnumerator WaitForUpdate() {
		_updateFrame = 0;
		while (Time.frameCount != _updateFrame) {
			yield return null;
		}
	}

}
