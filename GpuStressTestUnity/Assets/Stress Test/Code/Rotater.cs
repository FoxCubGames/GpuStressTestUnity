using UnityEngine;
using System.Collections;

public class Rotater : MonoBehaviour {

	public Vector3 _rot;
	public bool _random;

	Transform _t;

	void Awake() {
		_t = transform;
		if (_random) {
			_rot = new Vector3(_rot.x * Random.value, _rot.y * Random.value, _rot.z * Random.value);
		}
	}

	void Update() {
		_t.Rotate(_rot * Time.deltaTime, Space.Self);
	}

}
