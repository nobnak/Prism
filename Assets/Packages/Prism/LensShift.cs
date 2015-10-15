using UnityEngine;
using System.Collections;

namespace PrismSystem {
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class LensShift : MonoBehaviour {
		public LensShiftData data;

		Camera _targetCamera;

		void OnEnable() {
			_targetCamera = GetComponent<Camera> ();
			UpdateProjectionMatrix ();
		}
		void OnDisable() {
			_targetCamera.ResetProjectionMatrix ();
		}
		void Update() {
			UpdateProjectionMatrix ();
		}

		void UpdateProjectionMatrix() {
			var n = _targetCamera.nearClipPlane;
			var f = _targetCamera.farClipPlane;
			var tan = Mathf.Tan (0.5f * _targetCamera.fieldOfView * Mathf.Deg2Rad);
			var t = n * tan;
			var b = -t;
			var r = t * _targetCamera.aspect;
			var l = -r;

			var offset_y = t * data.shift.y;
			var offset_x = r * data.shift.x;
			t += offset_y; b += offset_y;
			r += offset_x; l += offset_x;

			_targetCamera.projectionMatrix = PerspectiveOffCenter (l, r, b, t, n, f);
		}

		public static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far) {
			float x = 2.0F * near / (right - left);
			float y = 2.0F * near / (top - bottom);
			float a = (right + left) / (right - left);
			float b = (top + bottom) / (top - bottom);
			float c = -(far + near) / (far - near);
			float d = -(2.0F * far * near) / (far - near);
			float e = -1.0F;

			Matrix4x4 m = Matrix4x4.zero;
			m[ 0]=x;				m[ 8]=a;
						m[ 5]=y;	m[ 9]=b;
									m[10]=c;	m[14]=d;
									m[11]=e;

			return m;
		}
	}
}