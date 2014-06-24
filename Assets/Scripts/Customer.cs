using UnityEngine;
using System.Collections;
using System;

public class Customer : MonoBehaviour {

	public GameTypes TypePref = GameTypes.Cabinet;
	public int MultiplayerPref = 1;
	public string GamePref = "Centipede";

	public bool Playing = false;

	public float Excitement = 1.0f;
	public float Interest = 0.0f;
	public float Fatigue = 0.0f;
	float gameplayTime = 1.0f;

	public int Skill = 1;

	public Game TargetGame;
	public Game LastGame;
	public Concession TargetConcession;
    public AstarAI customerAI;
	public Arcade arcade;

	public CustomerStates currentState = CustomerStates.Seeking;

	// Use this for initialization
	void Start () {
		Debug.Log("Customer start");
        customerAI = GetComponent<AstarAI>();
		arcade = Camera.main.GetComponent<Arcade>();

		EnterSeekingState();
	}

	
	// Update is called once per frame
	void Update () {
		/*switch (currentState){
		case CustomerStates.Seeking:
            Seek_Update();
			break;
		case CustomerStates.Playing:
            Play_Update();
			break;
		case CustomerStates.Tired:
            Tired_Update();
			break;
		case CustomerStates.Watching:
			break;
		case CustomerStates.Leaving:
			break;
        default:
            DoNothing();
            break;
		}*/
		//DoNothing();
	}

	#region Seeking

    void EnterSeekingState() {
		Debug.Log("Entering seeking state.");
		currentState = CustomerStates.Seeking;
        if(GetTarget()) {
			StartCoroutine("Seek_Update");
		}
		else {
			Debug.Log("Entering leaving state.");
			// Leave.
		}
    }

    IEnumerator Seek_Update() {
        // Update possible distractions occurring.
		while(currentState == CustomerStates.Seeking){
	        if (customerAI.ReachedDestination){
				Debug.Log("Seek_Update.");
	            //EnterPlayingState();
				ExitSeekingState();
	        }
			yield return 0;
		}
    }

	void ExitSeekingState(){
		Debug.Log("Exiting seeking state.");
		StopCoroutine("Seek_Update");

		EnterPlayingState();
	}

	#endregion

	#region Playing

    void EnterPlayingState() {
		Debug.Log("Entering playing state.");
		if (TargetGame != null)
	        TargetGame.Join(this);
        currentState = CustomerStates.Playing;

		gameplayTime = Mathf.Floor(5 * (Skill / TargetGame.Difficulty));
		Debug.Log("Gameplaytime: " + gameplayTime);
		StartCoroutine("Play_Update");
    }

    IEnumerator Play_Update() {
		// Start animation. Wait for it to finish.
		while (currentState == CustomerStates.Playing){
			Debug.Log("Play update. Interest: " + Interest);
			if (Interest <= 0){
				TargetGame.Leave(this);
				ExitPlayingState();
			}
			else if (Interest > 0 && TargetGame != null) {
		        TargetGame.Play();
			}

			yield return new WaitForSeconds(gameplayTime);

			// Do interest calculation.
			Interest -= TargetGame.Difficulty / Skill;
			if (TargetGame.Condition == 10){
				Interest = 0;
			}
			else {
				Interest -= 2 * (TargetGame.Condition / 10);
			}
			// Do fatigue calculation.
			if (TargetGame.GameType == GameTypes.Novelty){
				Fatigue += 2;
			}
			else {
				Fatigue += 1;
			}

			// Do animation.
		}

	}

    void ExitPlayingState() {
		Debug.Log("Exiting playing state.");
		StopCoroutine("Play_Update");
		LastGame = TargetGame;

		if (Fatigue > 0){
        	EnterTiredState();
		}
		else {
			Debug.Log("I should reenter the seeking state");
		}
    }

	#endregion

	#region Tired region

    void EnterTiredState() {
        currentState = CustomerStates.Tired;
		Debug.Log("Entering tired state.");
        // Get concessions service from Arcade
		TargetConcession = arcade.GetConcessions(Fatigue);

        if (customerAI && TargetConcession) {
            customerAI.SetDestination(TargetConcession.ServicePosition());
			StartCoroutine("Tired_Update");
        }
    }

    IEnumerator Tired_Update() {
		while (currentState == CustomerStates.Tired) {
	        if (customerAI.ReachedDestination) {
	            // Decrease fatigue, play animation.

				TargetConcession.Join(this);

	            // At end of yield, exit tired state.
	            //ExitTiredState();
	        }
			yield return 0;
		}
    }

    void ExitTiredState() {

		StopCoroutine("Tired_Update");
        
        EnterSeekingState();
    }

	#endregion

	#region Leaving

	void EnterLeavingState(){
		currentState = CustomerStates.Leaving;

		// Get Target destination to leave from.
		if (customerAI) {
			customerAI.SetDestination(new Vector3(-4.5f, 1.0f, 4.5f));
			StartCoroutine("Leaving_Update");
		}
	}

	IEnumerator Leaving_Update(){
		while (currentState == CustomerStates.Leaving){
			// Enter logic where if Fatigue isn't too low, they might stay if 
			// something catches their interest.

			if (customerAI.ReachedDestination){
				ExitLeavingState();
			}

			yield return 0;
		}
	}

	void ExitLeavingState(){
		StopAllCoroutines();

		// Remove from Arcade once the arcade is keeping track of people it creates.

		Destroy(gameObject);
	}

	#endregion

	#region Utility.

	bool GetTarget() { 
		// Get target from Arcade.
		// TODO: Make sure LastGame won't be picked twice in a row.
		if (!LastGame){
			TargetGame = arcade.GetIdealGame(GamePref, TypePref);
		}
		else if (LastGame && LastGame.Title != GamePref){
			TargetGame = arcade.GetIdealGame(GamePref, TypePref);
		}
		else {
			TargetGame = arcade.GetIdealGame("none", TypePref);
		}
		Debug.Log("Retrieved target game: " + TargetGame.Title);

		if (!TargetGame){
			// Enter leaving state.
			Debug.Log("Man, I really need a leaving state because I certainly need to leave");
			return false;
		}

		// Determine interest.
		// Interest is literally how many times a person will replay a game.
		// Interest is built up by how fun a game is, if it's a person's preferred type,
		// and brought down by the condition or the age
		// and modified by the price and the difficulty
		Interest = TargetGame.Fun;
		if(TargetGame.Title == GamePref){
			Interest *= 1.0f;
		}
		else if (TargetGame.GameType == TypePref){
			Interest *= 1.0f;
		}

		// Set Target position to that.
		if (customerAI) {
			customerAI.SetDestination(TargetGame.ServicePosition());
		}

		return true;
	}

	void GetConcessions(){

	}

	void DoNothing() {

	}

	#endregion
}

public enum CustomerStates {
	Seeking,
	Playing,
	Tired,
	Leaving,
	Watching
}
