using UnityEngine;

public interface IArcadeService {
	void Join(Customer customer);
	void Leave(Customer customer);
	void Play();
	Vector3 ServicePosition();
}
