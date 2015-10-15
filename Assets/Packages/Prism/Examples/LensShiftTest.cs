using UnityEngine;
using System.Collections;

namespace PrismSystem {
	public class LensShiftTest : MonoBehaviour {
		public GameObject fab;
		public int count;
		public float radius;

		void Start () {
			for (var i = 0; i < count; i++) {
				var pos = radius * Random.insideUnitSphere;
				var go = (GameObject)Instantiate(fab, pos, Random.rotationUniform);
				go.transform.SetParent(transform, false);
			}
		}
	}
}