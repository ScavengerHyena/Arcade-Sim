using UnityEngine;
using System.Collections;
using Pathfinding;

public class AstarAI : MonoBehaviour {

	public Vector3 targetPosition;
	public Path path;
	public float speed = 100.0f;
	public float nextWaypointDistance = 3.0f;
    public bool ReachedDestination = false;

	private Seeker seeker;
	private CharacterController characterController;
	private int currentWaypoint = 0;

	// Use this for initialization
	void Start () {
		seeker = GetComponent<Seeker>();
		characterController = GetComponent<CharacterController>();

		//seeker.StartPath(transform.position, targetPosition, OnPathComplete);
	}

	public void OnDisable(){
		seeker.pathCallback -= OnPathComplete;
	}

	public void OnPathComplete(Path p){
		Debug.Log("Got a path back. Did it have an error?" + p.error);
		if (!p.error){
			path = p;
			currentWaypoint = 0;
		}
	}

	Vector3 testCheck = new Vector3(0.0f, -2.0f, 0.0f);

	// Update is called once per frame
	public void FixedUpdate () {
		if (path == null)
			return;

		//if (currentWaypoint >= path.vectorPath.Length){
		if (ReachedDestination){
			//ReachedDestination = true;
			return;
		}

		Vector3 dir = (path.vectorPath[currentWaypoint] - this.transform.position).normalized;
		dir *= speed * Time.fixedDeltaTime;

		if (dir == testCheck)
			ReachedDestination = true;
		characterController.SimpleMove(dir);

		if (Vector3.Distance(this.transform.position, this.path.vectorPath[this.currentWaypoint]) < this.nextWaypointDistance){
			currentWaypoint++;
			if (currentWaypoint > this.path.vectorPath.Length - 1) {
				currentWaypoint = this.path.vectorPath.Length - 1;
			}
			return;
		}
	}

    public void SetDestination(Vector3 target) {
		Debug.Log("Setting destination.");
		path = null;
        currentWaypoint = 0;
        ReachedDestination = false;
        targetPosition = target;
        seeker.StartPath(transform.position, targetPosition, OnPathComplete);
    }
}
