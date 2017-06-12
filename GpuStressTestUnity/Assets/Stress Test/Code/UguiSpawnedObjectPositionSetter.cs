using UnityEngine;

public sealed class UguiSpawnedObjectPositionSetter : SpawnedObjectPositionSetter {

	public RectTransform _rectTransform;

	public override void SetPosition(Vector3 worldPosition) {
		_rectTransform.anchoredPosition = worldPosition;
	}

}