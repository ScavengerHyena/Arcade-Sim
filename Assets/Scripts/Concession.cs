using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Concession : MonoBehaviour {

	#region Properties

	public string Title;

	private decimal earnings;
	public decimal Earnings {
		get { return earnings; }
	}

	private float fatigueValue = 1.0f;
	public float FatigueValue {
		get { return fatigueValue; }
	}

	private decimal price = 0.25m;
	public decimal Price{
		set {
			if (value < 0.0m)
				price = 0.0m;
			else
				price = value;
		}
		get { return price; }
	}
	
	public Vector3 ServicePosition() {
		return transform.position;
	}

	private List<Customer> Seated;
	private List<Customer> LineToServe = new List<Customer>();

	public int LineLength {
		get { return LineToServe.Count; }
	}

	public float WaitTime = 1.0f;
	public float EatingTime = 2.0f;

	public int CounterSpaces = 1;

	public int SeatingSpaces = 0;

	#endregion

	void Start(){
		if (SeatingSpaces > 0){
			Seated = new List<Customer>();
		}
	}


	#region Utility

	public void Play(){
		
	}
	
	public void Leave(Customer customer){
		// Remove players.

	}
	
	public void Join(Customer customer){

		// If we're adding our first customer, we need to start the coroutine
		// otherwise it's running and we don't want to start it twice.
		bool startCoroutine = LineToServe.Count < 1;
		LineToServe.Add(customer);
		if (startCoroutine){
			StartCoroutine("Serve_Line");
		}
	}

	IEnumerator Serve_Line(){
		// Wait for a few seconds before sending people off to dining area or whatever.
		yield return new WaitForSeconds(WaitTime);
		// Remove first customer in line for number of people that can be served.
		for (int i = 0; i < CounterSpaces; i++){
			if (LineToServe.Count > 0){

				if (SeatingSpaces > 0){
					assignSeating(LineToServe[0]);
				}
				else {
					// Need to do something with rep, or lodging complaints
					// if more people than can be seated want food. Or perhaps
					// people just eat standing and the arcade gets dirtier.
				}

				LineToServe.RemoveAt(0);
			}
		}


	}

	void assignSeating(Customer customer){
		// Just like joining, if it's our first customer, we need to start the coroutine.
		bool startCoroutine = Seated.Count < 1;
		if (Seated.Count < SeatingSpaces){
			Seated.Add(customer);
		}
		else {
			// Need to do something with rep, or lodging complaints
			// if more people than can be seated want food. Or perhaps
			// people just eat standing and the arcade gets dirtier.
		}

	}

	public void Close(){
		// Get rid of all customers.
	}
	
	public void ClearEarnings(){
		earnings = 0.0m;
	}

	#endregion

}
