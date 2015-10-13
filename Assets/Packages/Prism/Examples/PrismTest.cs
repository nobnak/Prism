using UnityEngine;
using System.Collections;

public class PrismTest : MonoBehaviour {
	public int count;
	public float radius;
	public Vector3 angularSpeed;
	public GameObject particlefab;

	void Start () {
		for (var i = 0; i < count; i++) {
			var pos = radius * Random.insideUnitSphere;
			var go = (GameObject)Instantiate(particlefab, pos, Random.rotationUniform);
			go.transform.SetParent(transform, false);
		}
	}

	void Update() {
		transform.localRotation *= Quaternion.Euler (Time.deltaTime * angularSpeed);
	}
}
