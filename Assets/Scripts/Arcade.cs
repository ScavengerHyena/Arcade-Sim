using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Arcade:MonoBehaviour {

	public bool ArcadeInitialized = false;
	public List<GameObject> GamesToAdd = new List<GameObject>(); // Debug variable, remove later.

	/* Handle generating customers, handle 
	 * */
	private float _popularity = 1.0f;
	public float Popularity {
		get { return _popularity; }
	}

	public Hashtable Games = new Hashtable();
	public Dictionary<GameTypes, List<Game>> GamesByType = new Dictionary<GameTypes, List<Game>>();
	public List<Concession> Concessions = new List<Concession>();

	void Start(){

		if (GamesByType.Count == 0){
			InitializeDictionaries();
		}

		foreach(GameObject go in GamesToAdd){
			Game gameToAdd = go.GetComponent<Game>();
			AddMachine(gameToAdd);
		}

		GamesToAdd.Clear();
		GamesToAdd = null;

		ArcadeInitialized = true;
	}

    public Game GetIdealGame(string Title, GameTypes typePref) {
		if(Games.Count < 1) return null;

		Game returnGame = null;
		if (Title != "none"){
			returnGame = ReturnGame(Title);
		}

		if (returnGame){ return returnGame; }

		returnGame = ReturnRandomGameByType(typePref);

		if (returnGame){ return returnGame; }

		return ReturnRandomGame();
    }

	Game ReturnGame(string title){
		Game returnGame;
		if (Games.ContainsKey(title))
		{
			if (Games[title] is Game){
				Game game = Games[title] as Game;
				if(OpenToPlay(game)){
					returnGame = game;
					return returnGame;
				}
			}
			else if (Games[title] is IList){
				List<Game> gameList = Games[title] as List<Game>;
				returnGame = GameFromList(gameList);
				return returnGame;
			}
		}
		
		return null;
	}

	Game ReturnRandomGame(){

		Game returnGame = null;
		List<string> gamesList = Games.Keys as List<string>;

		bool searchComplete = false;

		while(!searchComplete){
			int randomIndex = 0;
			
			randomIndex = Random.Range(0, gamesList.Count);
			string randomKey = gamesList[randomIndex];

			returnGame = ReturnGame(randomKey);
			if (!returnGame){
				gamesList.Remove(randomKey);
				if (gamesList.Count < 1){
					searchComplete = true;
				}
			}
			else{
				searchComplete = true;
			}
		}

		return returnGame;
	}

	Game GameFromList(List<Game> gameList){
		foreach(Game gameCheckPlayers in gameList){
			if(OpenToPlay(gameCheckPlayers))
				return gameCheckPlayers;
		}

		return null;
	}

	Game ReturnRandomGameByType(GameTypes selectedType){
		int randomIndex = 0;
		bool searchComplete = false;
		List<Game> potentialGames = GamesByType[selectedType];
		Game returnGame = null;

		if (potentialGames.Count > 0){

			while (!searchComplete){
				randomIndex = Random.Range(0, potentialGames.Count);
				returnGame = potentialGames[randomIndex] as Game;

				if(OpenToPlay(returnGame)){
					searchComplete = true;
				}
				else {
					potentialGames.Remove(returnGame);
					returnGame = null;

					if (potentialGames.Count < 1){
						searchComplete = true;
					} // 

				} // if/else Open to play

			} // While search isn't complete

		} // If potentialGames.count > 0

		return returnGame;
	}

	bool OpenToPlay(Game game){
		if (game.CurrentPlayers < game.MaxPlayers){
			return true;
		}
		else {
			return false;
		}
	}

	public Concession GetConcessions(float fatigue){
		Concession returnConcession = Concessions[0];

		// Use conWeight system to determine best concession stand on basis of 
		// line length and fatigue value. May factor in cost or adjust with coefficients later.
		var conWeight = Mathf.Abs(fatigue - returnConcession.FatigueValue) + returnConcession.LineLength;

		foreach (Concession testCon in Concessions) {
			if (Mathf.Abs(fatigue - testCon.FatigueValue) + returnConcession.LineLength < conWeight){
				returnConcession = testCon;
			}
		}

		return returnConcession;
	}

	//public decimal Money = 100m;

	public void GenerateCustomers() {
		// Create number of customers based on popularity
		// Assign them to entrances, etc.
	}

	public void AddMachine(Game game) {
		Debug.Log("Adding game to hash table.");
		if (Games.ContainsKey(game.Title)){
			if (Games[game.Title] is Game){
				// Create a placeholder to keep track of the already present game.
				Game placeHolder = Games[game.Title] as Game;
				// Create a collection to hold all games of the same  name.
				List<Game> similarGameList = new List<Game>();
				// Add the original, then the duplicate machines.
				similarGameList.Add(placeHolder);
				similarGameList.Add(game);
				// Reassign the dictionary entry so the key points towards the collection
				Games[game.Title] = similarGameList;
			}
			else if (Games[game.Title] is IList<Game>) {
				List<Game> gameList = Games[game.Title] as List<Game>;
				gameList.Add(game);
			}
		}
		else {
			Games.Add(game.Title, game);
		}

		Debug.Log("Game added: " + game.Title);

		if (GamesByType.Count == 0){
			InitializeDictionaries();
		}

		List<Game> typeOfGame = GamesByType[game.GameType] as List<Game>;
		typeOfGame.Add(game);
	}

	public void RemoveMachine(Game game) {

		if (Games.ContainsKey(game.Title)){
			if (Games[game.Title] is Game){
				Games.Remove(game.Title);
			}
			else if (Games[game.Title] is IList<Game>){
				List<Game> gameList = Games[game.Title] as List<Game>;

				if (gameList.Contains(game)){
					gameList.Remove(game);
					if (gameList.Count == 0){
						Games.Remove(game.Title);
					}
				}

		    }
		}

		if (GamesByType[game.GameType].Contains(game)){
			GamesByType[game.GameType].Remove(game);
		}

	}

	public void AddConcession(Concession concession){
		if (!Concessions.Contains(concession)){
			Concessions.Add(concession);
		}
	}

	public void RemoveConcession(Concession concession){
		if (Concessions.Contains(concession)){
			Concessions.Remove(concession);
		}
	}

	public decimal Earnings(){
		// Go through machines, update money earned
		// Go through machines, update operating costs
		decimal earnings = 0.0m;
		decimal expenses = 0.0m;

		foreach (Game g in Games){
			earnings += g.Earnings;
			expenses -= g.Expense;

			g.ClearEarnings();
		}

		// Go through concessions, update money earned

		// Go through concessesions, update operating costs

		// Go through employees, update payment

		// Go through prizes, update expenses

		earnings += expenses;
		return earnings;
	}

	void InitializeDictionaries(){
		List<Game> CabinetType = new List<Game>();
		GamesByType[GameTypes.Cabinet] = CabinetType;
		
		List<Game> CarnivalType = new List<Game>();
		GamesByType[GameTypes.Carnival] = CarnivalType;
		
		List<Game> LightGunType = new List<Game>();
		GamesByType[GameTypes.LightGun] = LightGunType;
		
		List<Game> PinballType = new List<Game>();
		GamesByType[GameTypes.Pinball] = PinballType;
		
		List<Game> NoveltyType = new List<Game>();
		GamesByType[GameTypes.Novelty] = NoveltyType;
		
		List<Game> RacingType = new List<Game>();
		GamesByType[GameTypes.Racing] = RacingType;

		//ArcadeInitialized = true;
	}
}