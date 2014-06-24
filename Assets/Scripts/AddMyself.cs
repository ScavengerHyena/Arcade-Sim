using UnityEngine;
using System.Collections;

public class AddMyself : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Arcade arcade = Camera.main.GetComponent<Arcade>();
		Game game = GetComponent<Game>();

		arcade.AddMachine(game);
	}
}
