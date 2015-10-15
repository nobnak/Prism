using UnityEngine;
using System.Collections;

namespace PrismSystem {
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class Prism : MonoBehaviour {
		public const float FULL_ROTATION = 360f;

		public int cameraCount = 4;
		public bool autoFit = false;
		public GameObject fab;
		public CameraParams overrideCameraParams;

		RenderTexture _outputTex;
		Camera _targetCamera;
		Camera[] _cameras;
		 
		void OnEnable() {
			_targetCamera = GetComponent<Camera> ();
			_targetCamera.clearFlags = CameraClearFlags.Nothing;
			_targetCamera.cullingMask = 0;

			CheckInit();
			UpdatePrism ();
			Activate (true);
		}
		void OnDisable() {
			Activate (false);
		}
		void OnDestroy() {
			Release ();
		}
		void Update() {
			CheckInit ();
			UpdatePrism ();
		}

		void CheckInit() {
			if (_cameras == null || _cameras.Length != cameraCount) {
				Release ();
				_cameras = new Camera[cameraCount];
				for (var i = 0; i < cameraCount; i++) {
					var name = string.Format ("{0} ({1})", _targetCamera.gameObject.name, i);
					var go = (fab == null ? new GameObject(name) : Instantiate(fab));
					go.name = name;
					go.hideFlags = HideFlags.DontSave;
					_cameras [i] = go.GetComponent<Camera>();
					if (_cameras[i] == null)
						_cameras[i] = go.AddComponent<Camera>();
				}
			}
		}

		void UpdatePrism () {
			var aspect = _targetCamera.aspect;
			var theta_h = _targetCamera.fieldOfView;
			var theta_w = FULL_ROTATION / cameraCount;
			var w = 1f / cameraCount;
			var a = w / (2f * Mathf.Tan (0.5f * theta_w * Mathf.Deg2Rad));
			var h = 2f * a * Mathf.Tan (0.5f * theta_h * Mathf.Deg2Rad) * aspect;
			if (autoFit) {
				theta_h = 2f * Mathf.Atan (1f / (2f * a * aspect)) * Mathf.Rad2Deg;
				_targetCamera.fieldOfView = theta_h;
				h = 1f;
			}
			var offsety = 0.5f * (1f - h);
			for (var i = 0; i < cameraCount; i++) {
				var c = _cameras [i];
				c.CopyFrom (_targetCamera);
				overrideCameraParams.Save (c);
				c.orthographic = false;
				c.rect = new Rect (w * i, offsety, w, h);
				c.transform.SetParent (transform, false);
				c.transform.localPosition = Vector3.zero;
				c.transform.localRotation = Quaternion.Euler (0f, i * theta_w, 0f);
			}
		}

		void Activate(bool active) {
			if (_cameras == null)
				return;
			foreach (var c in _cameras) {
				if (c == null)
					continue;
				c.gameObject.SetActive(active);
			}
		}
		void Release() {
			if (_cameras == null)
				return;

			foreach (var c in _cameras) {
				if (c != null) {
					var go = c.gameObject;
					DestroyModal(go);
				}
			}
			_cameras = null;
		}
		void DestroyModal(Object c) {
			if (c == null)
				return;
			if (Application.isPlaying)
				Destroy (c);
			else
				DestroyImmediate (c, true);
		}

		[System.Serializable]
		public class CameraParams {
			public CameraClearFlags clearFlags;
			public Color backgroundColor;
			public LayerMask cullingMasks;

			public void Load(Camera c) {
				clearFlags = c.clearFlags;
				backgroundColor = c.backgroundColor;
				cullingMasks = c.cullingMask;
			}
			public void Save(Camera c) {
				c.clearFlags = clearFlags;
				c.backgroundColor = backgroundColor;
				c.cullingMask = cullingMasks;
			}
		}
	}
}