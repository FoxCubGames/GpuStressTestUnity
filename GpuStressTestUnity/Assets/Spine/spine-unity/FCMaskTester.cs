#if UNITY_EDITOR

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class FCMaskTester : MonoBehaviour {

	public int _maskId;

	void Update() {
		var anim = GetComponent<SkeletonAnimation>();
		if (anim != null) {
			anim.SetMaskId(_maskId);
		}
		var mask = GetComponent<FCSymbolMask>();
		if (mask != null) {
			mask.SetUniqueId(_maskId);
		}
	}

}

#endif