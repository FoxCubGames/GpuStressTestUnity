using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour {

	public GameObject _prefab;

	/// <summary>
	/// this prefab will only get spawned once
	/// </summary>
	public GameObject _prefabStatic;

	public Text _countText;
	public Vector3 _size;
	public Vector3 _padding;
	public Vector3 _offset;

	Stack<GameObject> _gos = new Stack<GameObject>();
	GameObject _goStatic;

	int _maxSpawn;
	Coroutine _autoSpawnCor;
	Slider _slider;

	void Awake() {
		GetComponentInChildren<Toggle>().onValueChanged.AddListener(OnToggleAutoSpawnChanged);
		_slider = GetComponentInChildren<Slider>();
		_maxSpawn = (int)_slider.maxValue;
	}

	public void UpdateObjects(float num) {
		if (_prefabStatic != null) {
			if (num == 0 && _goStatic != null) {
				Destroy(_goStatic);
				_goStatic = null;
			}
			if (num > 0 && _goStatic == null) {
				_goStatic = Instantiate(_prefabStatic);
			}
		}

		while (_gos.Count > num) {
			Destroy(_gos.Pop());
		}

		while (_gos.Count < num) {
			var go = Instantiate(_prefab);
			if (_goStatic != null) {
				go.transform.SetParent(_goStatic.transform);
			}
			var count = 1;
			int x = 0, y = 0, z = 0;
			for (z = 0; z < _size.z; z++) {
				for (y = 0; y < _size.y; y++) {
					for (x = 0; x < _size.x; x++) {
						count++;
						if (count >= _gos.Count) break;
					}
					if (count >= _gos.Count) break;
				}
				if (count >= _gos.Count) break;
			}
			go.transform.localPosition = _offset + new Vector3(x * _padding.x, y * _padding.y, z * _padding.z);
			_gos.Push(go);
		}
		_countText.text = _gos.Count.ToString();
	}

	void OnToggleAutoSpawnChanged(bool val) {
		if (val) {
			Debug.Log("auto spawn: on");
			_autoSpawnCor = StartCoroutine(AutoSpawnCor());
		} else {
			if (_autoSpawnCor != null) {
				Debug.Log("auto spawn: off");
				StopCoroutine(_autoSpawnCor);
				_autoSpawnCor = null;
			}
		}
	}

	IEnumerator AutoSpawnCor() {
		int step = 0;
		float stepSize = .25f;
		var fps = FpsDisplay._instance;

		while (true) {
			int currentSpawn = (int)_slider.value;
			Debug.Log("auto spawn: step=" + step + ", stepSize=" + (stepSize * (float)_maxSpawn).ToString("N0") + ", spawned=" + currentSpawn + ", fps=" + fps._Fps.ToString("N2"));

			// did we find 30 fps?
			if (fps._Fps >= 30f && fps._Fps < 30.5f) {
				Debug.Log("auto spawn: found fps!");
				GetComponentInChildren<Toggle>().isOn = false;
			}

			// are we maxxed out and still above 30 fps?
			else if (fps._Fps > 30.5f && currentSpawn >= _maxSpawn) {
				// raise the max spawn
				_maxSpawn = Mathf.RoundToInt((float)_maxSpawn * 1.25f);
				_slider.maxValue = _maxSpawn;
				Debug.Log("auto spawn: maxxed out! raising max to: " + _maxSpawn);
			}

			// are we minned out and still below 30 fps?
			else if (fps._Fps < 30f && currentSpawn <= 0) {
				Debug.Log("auto spawn: minned out but already below 30 fps!");
				GetComponentInChildren<Toggle>().isOn = false;
			}

			// if we get here, we either need to spawn more or less
			// should we spawn more?
			bool spawnMore = (fps._Fps > 30.5f);
			// if we're still spawning more (and never went over 30 fps), we keep our step size at 0
			if (spawnMore && step == 0) {
				// dont change step or step size
				Debug.Log("auto spawn: still trying to find the breaking point...");
			} else {
				// reduce stepsize by half
				step++;
				stepSize = stepSize * .5f;
				Debug.Log("auto spawn: new step size: " + stepSize.ToString("N6") + ", spawnMore=" + spawnMore);
			}

			// now, spawn more or less
			_slider.value += (float)_maxSpawn * stepSize * (spawnMore ? 1f : -1f);

			// wait 1 second, then wait for the next fps update
			yield return new WaitForSeconds(1f);
			yield return StartCoroutine(fps.WaitForUpdate());
		}
	}

}
