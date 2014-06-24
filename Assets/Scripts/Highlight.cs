using UnityEngine;
using System.Collections;

public class Highlight : MonoBehaviour {

	Color startColor;

	void Start(){
		//startColor = renderer.material.color;
	}

	void OnCollisionEnter(Collision collision){
		//renderer.material.color = Color.red;
		Debug.Log("Collide");
	}

	void OnCollisionExit(Collision collision){
		//renderer.material.color = startColor;
		Debug.Log("Uncollide");
	}
}
