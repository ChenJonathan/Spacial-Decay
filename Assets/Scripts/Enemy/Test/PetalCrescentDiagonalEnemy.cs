using UnityEngine;
using System.Collections;
using DanmakU;
using DanmakU.Modifiers;

public class PetalCrescentDiagonalEnemy : Enemy
{
	/*
	HOW QUICKLY THE TIDE TURNS
	Paranoid
	Selfish
	Irrational
	Fearful
	Hopeless
	Abusive
	Masochistic
	*/
	public DanmakuPrefab petalPrefab;
	public DanmakuPrefab crescentPrefab;

	private FireBuilder fireDataPetal;
	private FireBuilder fireDataCrescent;
	private Rigidbody2D rigidbody2d;

	//moves in a circle
	private float radius = 5.0f;
	private float period = 2.0f; //seconds


	public override void Start()
	{
		rigidbody2d = GetComponent<Rigidbody2D>();

		fireDataPetal = new FireBuilder(petalPrefab, Field);
		fireDataPetal.From(transform);
		fireDataPetal.Towards(Player.transform);
		fireDataPetal.WithSpeed(6);
		fireDataPetal.WithModifier(new CircularBurstModifier(45, 1, 0, 0));

		fireDataCrescent = new FireBuilder (crescentPrefab, Field);
		fireDataCrescent.From(transform);
		fireDataCrescent.Towards(Player.transform);
		fireDataCrescent.WithSpeed(6);
		fireDataCrescent.WithModifier(new CircularBurstModifier(45, 5, 0, 0));
		base.Start();
	}

	protected override IEnumerator Run() {
		//I propose circular movement
		Vector3 center = transform.position - new Vector3(0, radius, 0);
		float timePassed = 0;
		float radians = Mathf.PI / 2.0f; //init at top of circle
		float time = 0;
		StartCoroutine (Attack());
		for (int cycles = 0; cycles < 100; cycles++) {
			timePassed = 0;
			while (timePassed < period) {
				time = Time.deltaTime;
				radians += time * (2 * Mathf.PI) / period;
				transform.position = center + new Vector3 (radius * Mathf.Cos(radians), radius*Mathf.Sin(radians), 0);
				//center -= new Vector3 (0, 5 * time, 0);
				yield return new WaitForSeconds(0.01f);
				timePassed += time;
			}
		}
		rigidbody2d.velocity = new Vector2(-10, 0);
		yield return new WaitForSeconds (2);
		Die();
	}

	private IEnumerator Attack()
	{
		while (true) {
			fireDataPetal.Fire ();
			yield return new WaitForSeconds (0.1f);
			fireDataCrescent.Fire();
			yield return new WaitForSeconds (2.0f);
		}
	}
}

