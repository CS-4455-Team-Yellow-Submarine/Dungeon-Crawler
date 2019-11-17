using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballShooter : MonoBehaviour
{
	public GameObject projectile;
	public int attackDamage = 18;
	public float shootInterval = 2.5f;
	public float firstShotDelay = 0f;
	private float timeSinceLastShot;

    // Start is called before the first frame update
    void Start()
    {
		timeSinceLastShot = shootInterval - firstShotDelay;
    }

    // Update is called once per frame
    void Update()
    {
		timeSinceLastShot += Time.deltaTime;
		if(timeSinceLastShot > shootInterval){
			timeSinceLastShot = 0f;
			ShootFireball();
		}
    }

	// Shoots a fireball
	private void ShootFireball(){
		GameObject obj = Object.Instantiate(projectile, transform.position, Quaternion.identity) as GameObject;
		Projectile pr = obj.GetComponent<Projectile>();
		// Associate definitions with the projectile
		pr.Define(GetComponent<Rigidbody>(), transform.position, null, 3f, 15f);
		// Override the direction of this projectile
		pr.SetMoveDirection(transform.rotation * Vector3.forward, 3f, 15f);
		pr.damage = attackDamage;
	}
}
