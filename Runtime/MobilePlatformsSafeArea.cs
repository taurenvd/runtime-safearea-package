using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

#if !UNITY_IOS && !UNITY_ANDROID && ODIN_INSPECTOR
[TypeInfoBox("Only for <b>IOS</b> or <b>Android</b>. Other platforms will be ignored")]
#endif

[RequireComponent(typeof(RectTransform))]
public class MobilePlatformsSafeArea : MonoBehaviour
{
	[Header("Components")]
#if ODIN_INSPECTOR
	[Required, ChildGameObjectsOnly]
#endif
	[SerializeField] RectTransform _rect;

#if UNITY_IOS || UNITY_ANDROID
	[Header("Runtime")]
#if ODIN_INSPECTOR
	[ShowInInspector, ReadOnly]
#endif
	Rect _lastRectChange;

	#region Mono Callbacks

	void Start()
	{
		CheckSafeArea();
	}

	#endregion

#if ODIN_INSPECTOR
	[Button, DisableInEditorMode, FoldoutGroup("Runtime Methods")]
#endif
	void CheckSafeArea()
	{
		var safeArea = Screen.safeArea;

		if ( _lastRectChange == safeArea) // Block execution after playmode exit
		{
			return;
		}
		
		_lastRectChange = safeArea;

		//Screen.height/width returns real pixel size(window size, if in Editor),
		//which doesnt work correct with Device Simulator
		//Use Screen.currentResolution instead
		var resolution = Screen.currentResolution;

#if UNITY_EDITOR
		if (Application.isEditor)
		{
			resolution = new Resolution() {height = Screen.height, width = Screen.width};
		}
#endif
		
		Debug.Log($"<b>MobilePlatformsSafeArea.CheckSafeArea {name}, safe_area: {safeArea}, screen: {Screen.width}x{Screen.height}, res: {resolution}</b>", gameObject);
		
		// Convert safe area rectangle from absolute pixels to normalised anchor coordinates
		var anchorMin = safeArea.position;
		var anchorMax = safeArea.position + safeArea.size;
		
		anchorMin.x /= resolution.width;
		anchorMin.y /= resolution.height;
		anchorMax.x /= resolution.width;
		anchorMax.y /= resolution.height;
		
		_rect.anchorMin = anchorMin;
		_rect.anchorMax = anchorMax;
	}

	void OnRectTransformDimensionsChange()
	{
		if (gameObject.activeSelf && enabled)
		{
			Debug.Log($"<b>MobilePlatformsSafeArea.OnRectTransformDimensionsChange</b> {name}", gameObject);

			CheckSafeArea();
		}
	}

#if ODIN_INSPECTOR
	[Button, FoldoutGroup("Runtime Methods")]
#endif
	public void SetTarget(RectTransform new_target, bool force_update = false)
	{
		_rect = new_target;

		if (force_update)
		{
			_lastRectChange = Rect.zero;
			
			CheckSafeArea();
		}
	}
#endif
}
