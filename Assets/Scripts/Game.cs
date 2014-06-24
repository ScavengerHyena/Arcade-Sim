using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour, IArcadeService {

	#region Properties

	public string Title;
	private decimal earnings = 0.0m;
	public decimal Earnings {
		get { return earnings; }
	}

	[Range(1,10)]
	public int Difficulty = 1;

	public GameTypes GameType = GameTypes.Cabinet;

	public int MaxPlayers = 1;

	//[Range(1.0f, MaxPlayers)]
	//private int _currentPlayers = 0;
	public int CurrentPlayers {
		get {return Players.Count;}
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

	public decimal Expense = 1.0m;
	public int Payout = 0;
	public float Fun = 5.0f;
	public int Condition = 0;
	public float Age = 0;

	private List<Customer> Players = new List<Customer>();

	#endregion

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Play(){
		Debug.Log("playing a game, yo");
		if (Players.Count > 0){
			// Report money.
			earnings += price;
		}

	}

	public void Leave(Customer customer){
		// Remove players.
		if (Players.Contains(customer)){
			customer.Playing = false;
			Players.Remove(customer);
		}
	}

	public void Join(Customer customer){
		// Add Player
		if (Players.Count < MaxPlayers){
			Players.Add(customer);
			customer.Playing = true;
		}
	}

	public void ClearEarnings(){
		earnings = 0.0m;
	}

}
