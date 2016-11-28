using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour {

	public GameObject _prefab;
	public Text _countText;
	public Vector3 _size;
	public Vector3 _padding;
	public Vector3 _offset;

	Stack<GameObject> _gos = new Stack<GameObject>();

	public void UpdateObjects(float num) {
		while (_gos.Count > num) {
			Destroy(_gos.Pop());
		}

		while (_gos.Count < num) {
			var go = Instantiate(_prefab);
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
			go.transform.position = _offset + new Vector3(x * _padding.x, y * _padding.y, z * _padding.z);
			_gos.Push(go);
		}
		_countText.text = _gos.Count.ToString();
	}
}
