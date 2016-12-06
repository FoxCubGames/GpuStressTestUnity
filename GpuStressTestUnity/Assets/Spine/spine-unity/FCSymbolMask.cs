#if !UNITY_EDITOR || FCLOG
using Debug = FC.Debug;
#else
using Debug = UnityEngine.Debug;
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class FCSymbolMask : MonoBehaviour {

	public const string SHADER_UNIQUE_ID = "_UniqueId";

	public float _width;
	public float _height;
	public bool _reset;

	public FCSymbolMask[] _childMasks;

	/// <summary>
	/// all masks with the same ID will affect each other. different ID = no effect.
	/// </summary>
	public int _uniqueId = -1;

	/// <summary>
	/// if a child particle system needs masking, it should be linked in here. at runtime, its material will be duped and set to the right mask id.
	/// </summary>
	public List<ParticleSystem> _maskedParticleSystems;

	/// <summary>
	/// if a particle system needs masking, its material will get cloned and stored here
	/// </summary>
	private List<Material> _particleMaterials;

	///// <summary>
	///// if using unique ID, you need to setup all the mask targets of this symbol mask so we can give them the same ID.
	///// </summary>
	//public FCSymbolMaskTarget[] _targets;

	static Material[] _mat = new Material[129];
	Mesh _m;
	public MeshRenderer _mr;

	public Mesh _customMaskShape;

	public const int NUM_REELS = 5;
	static int[] _lastMaskIdPerReel = new int[NUM_REELS];


	void Awake() {
		CreateMask(_width, _height);
	}

#if UNITY_EDITOR
	void Update() {
		if (Application.isPlaying == false) {
			if (_reset) {
				OnDestroy();
			}

			CreateMask(_width, _height);

			if (_reset) {
				_reset = false;
				var masks = GameObject.FindObjectsOfType<FCSymbolMask>();
				foreach (var mask in masks) {
					if (mask != this) {
						mask.CreateMask(mask._width, mask._height);
					}
				}
			}
		}
	}
#endif

	public void CreateMask(float width, float height) {
#if UNITY_EDITOR
		if (_m != null) {
#if UNITY_EDITOR
			if (Application.isPlaying) {
				Destroy(_m);
			} else {
				DestroyImmediate(_m);
			}
#else
			Destroy(_m);
#endif
			_m = null;
		}
#endif

		if (_customMaskShape != null) {
			_m = new Mesh();
			_m.hideFlags = HideFlags.None;
			_m.name = "mask mesh";
			GetComponent<MeshFilter>().sharedMesh = _m;
			_m.vertices = _customMaskShape.vertices;
			_m.triangles = _customMaskShape.triangles;

			GetComponent<MeshFilter>().sharedMesh = _customMaskShape;
		} else if (_m == null) {
			_m = new Mesh();
			_m.hideFlags = HideFlags.None;
			_m.name = "mask mesh";
			GetComponent<MeshFilter>().sharedMesh = _m;
			_m.vertices = new Vector3[] {
				new Vector3(width/2f, height/2f, 0f),
				new Vector3(width/2f, -height/2f, 0f),
				new Vector3(-width/2f, -height/2f, 0f),
				new Vector3(-width/2f, height/2f, 0f)
			};
			_m.triangles = new int[] {
				1, 2, 0, 0, 2, 3
			};
		}

		if (_mr == null) {
			_mr = GetComponent<MeshRenderer>();
		}

		if (_mat[_uniqueId] == null) {
			var baseMat = Resources.Load<Material>("rect mask mat");
			var mat = new Material(baseMat);
			_mat[_uniqueId] = mat;
			mat.SetFloat(SHADER_UNIQUE_ID, _uniqueId);
#if UNITY_EDITOR
			mat.name = baseMat.name + " " + _uniqueId;
#endif
			mat.hideFlags = HideFlags.HideAndDontSave;
			_mr.sharedMaterial = mat;
		} else {
			_mr.sharedMaterial = _mat[_uniqueId];
		}

#if UNITY_EDITOR
		if (Application.isPlaying) {
#endif
			if (_maskedParticleSystems != null) {
				if (_particleMaterials == null) {
					_particleMaterials = new List<Material>();
				}
				for (int i = 0, n = _maskedParticleSystems.Count; i < n; ++i) {
					if (_maskedParticleSystems[i] != null) {
						var rend = _maskedParticleSystems[i].GetComponent<Renderer>();
#if UNITY_EDITOR
						if (rend.sharedMaterial == null) {
							Debug.LogErrorFormat("FCSymbolMask: error setting up masked particle system because the material is null: " + _maskedParticleSystems[i].transform.GetFullHierarchyPath());
						}
#endif
						if (rend.sharedMaterial != null) {
							var mat = new Material(rend.sharedMaterial);
							mat.hideFlags = HideFlags.HideAndDontSave;
							rend.material = mat;
							mat.SetFloat(SHADER_UNIQUE_ID, _uniqueId);
							_particleMaterials.Add(mat);
						}
					}
				}
				_maskedParticleSystems = null;
			}
#if UNITY_EDITOR
		}
#endif
	}

	public void SetUniqueId(int id) {
		if (id != _uniqueId) {
			_uniqueId = id;
			CreateMask(_width, _height);

			if (_childMasks != null) {
				for (int i = 0, n = _childMasks.Length; i < n; ++i) {
					_childMasks[i].SetUniqueId(id);
				}
			}

			if (_particleMaterials != null) {
				for (int i = 0, n = _particleMaterials.Count; i < n; ++i) {
					_particleMaterials[i].SetFloat(FCSymbolMask.SHADER_UNIQUE_ID, _uniqueId);
				}
			}

			//for (int i = 0, n = _targets.Length; i < n; ++i) {
			//	var target = _targets[i];
			//}
		}
	}

	/// <summary>
	/// Cycles between bits 1-4 for even reels and 5-8 for odd reels.
	/// </summary>
	public static int GetMaskIdForReel(int reelId) {
		int next = _lastMaskIdPerReel[reelId] % 4;
		_lastMaskIdPerReel[reelId]++;
		next += (reelId % 2) * 4;
		return (int)Mathf.Pow(2, next);
	}

	void OnDestroy() {
		if (_m != null) {
#if UNITY_EDITOR
			if (Application.isPlaying) {
				Destroy(_m);
			} else {
				DestroyImmediate(_m);
			}
#else
			Destroy(_m);
#endif
			_m = null;

			// destroy the cloned particle materials
			if (_particleMaterials != null) {
				for (int i = 0, n = _particleMaterials.Count; i < n; ++i) {
					Destroy(_particleMaterials[i]);
				}
			}

			// dont need to destroy the material anymore
//			if (_mat[_uniqueId] != null) {
//#if UNITY_EDITOR
//				if (Application.isPlaying) {
//					Destroy(_mat[_uniqueId]);
//				} else {
//					DestroyImmediate(_mat[_uniqueId]);
//				}
//#else
//				if (_mat[_uniqueId] != null) {
//					Destroy(_mat[_uniqueId]);
//					_mat[_uniqueId] = null;
//				}
//#endif
//				_mat[_uniqueId] = null;
//			}
		}
	}

	private static int _offscreenCameraId = 0;

	/// <summary>
	/// This is used by the spine animations, because we need to change their mask id right before the camera renders them.
	/// Each offscreen camera has a unique id, which we use as our unique mask id.
	/// </summary>
	public void OnRenderMaskedObject() {
#if UNITY_EDITOR
		if (Application.isPlaying) {
#endif
			var id = -1;
#if !SANDBOX
			// background: mask id is the stencil buffer value (8 bits). so, you can only have 8 unique mask ids.
			// the way fox cub does this, is to have a unique id per reel, and unique id per symbol on that reel, and then combine the 2 to get the final mask id per symbol.
			// it stores this value on the camera that is rendering that symbol, since we render each symbol with a different camera.
			//var id = OffScreenCameras.instance.GetOffscreenCameraId(Camera.current);

			// since we dont have the off screen camera system here, i am doing something else that should work 90% of the time. it will probably work 100% of the time in themes that dont have a lot of symbols that use masking.
			// for testing purposes, the theme that has the most masked symbols is Oz.
			// if you don't implement off screen cameras, you should probably make this off screen camera id be based on the reel, so adjacent reels never share the same mask id.
			// you could have odd reels use values 0-3 and even reels use values 4-7, for example.

			// for this dumb way, we also only want to set it once, per symbol mask. if we dont just set it once, then every spine object will run this and you will have a mis-match where the mask only matches up with 1 of the dependant spine animations.
			if (_uniqueId == -1) {
				id = (int) Mathf.Pow(2, (_offscreenCameraId%8));
				++_offscreenCameraId;
			}
#endif
			if (id == -1) {
				return;
			}
			Debug.Log("OnRenderMaskedObject: " + transform.parent.GetFullHierarchyPath() + " = " + id);
			SetUniqueId(id);
			
#if UNITY_EDITOR
			OnDrawGizmos();
		}
#endif
	}

	//public static void Init(SlotMachine slotMachine) {
	//	// now our slot machine is created so we can hook into the slotmachine load & unload events
	//	slotMachine.OnLoad += OnSlotMachineLoad;
	//	slotMachine.OnBeforeDisable += OnSlotMachineUnload;
	//}

	//private static void OnSlotMachineUnload() {
	//	SkeletonRenderer.runOnRenderMaskedObjectCallback = false;
	//}

	//private static void OnSlotMachineLoad() {
	//	SkeletonRenderer.runOnRenderMaskedObjectCallback = true;
	//}

#if UNITY_EDITOR
	void OnDrawGizmos() {
		if (_uniqueId != 0) {
			var s = new GUIStyle {fontSize = 18, fontStyle = FontStyle.BoldAndItalic, richText = true};
			UnityEditor.Handles.Label(transform.position, string.Format("<color=#BB3333>'{0}'</color>", _uniqueId), s);
		}
	}
#endif

}
