using UnityEngine;
using System.Collections.Generic;

public static class TransformExtensions {

	public static string GetFullHierarchyPath(this Transform transform) {
		List<string> parts = new List<string>();
		var t = transform;
		while (t != null) {
			parts.Add(t.gameObject.name);
			t = t.parent;
		}
		parts.Reverse();
		return string.Join(" - ", parts.ToArray());
	}

}
