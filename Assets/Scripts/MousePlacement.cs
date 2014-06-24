using UnityEngine;
using System.Collections;

public class MousePlacement : MonoBehaviour {

	public int gridsize = 1;
	float gridOffset = 0.5f;

	public GameObject redBox;

	GameObject go;

	// Use this for initialization
	void Start () {
		go = Instantiate(redBox) as GameObject;
		go.layer = 2;
	}
	
	// Update is called once per frame
	void Update () {
		Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		go.GetComponent<Rigidbody>().AddForce(new Vector3(0.1f, 0.1f, 0.1f));

		if (Input.GetAxis("Mouse ScrollWheel") > 0) {
			go.transform.Rotate(Vector3.up * 90);
		}
		else if (Input.GetAxis("Mouse ScrollWheel") < 0){
			go.transform.Rotate(Vector3.up * -90);
		}

		if (Physics.Raycast(r, out hit)){

			Vector3 pos = new Vector3(Mathf.Round(hit.point.x / gridsize) * gridsize + gridOffset,
			                          gridOffset,
			                          Mathf.Round(hit.point.z / gridsize) * gridsize + gridOffset);

			go.transform.position = pos;

			if(Input.GetMouseButtonUp(0)){
				Instantiate(redBox, pos, go.transform.rotation);
			}
		}
		else {
			Vector3 pos = r.GetPoint(100);
			pos.y = 0.5f;
			go.transform.position = pos;
		}
	}
}
