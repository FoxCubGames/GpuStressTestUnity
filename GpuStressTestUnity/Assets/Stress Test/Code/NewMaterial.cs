using UnityEngine;
using System.Collections;

public class NewMaterial : MonoBehaviour {

	public Renderer _renderer;

	void Start() {
		var mat = new Material(_renderer.material);
		mat.color = new Color(Random.value, Random.value, Random.value);
		_renderer.material = mat;
	}
}
