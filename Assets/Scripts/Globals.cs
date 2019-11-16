using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Globals : MonoBehaviour
{
	public GameObject panelForSignposts;
	public GameObject defaultExplosion;
	public string sceneOnExitTutorial = "MainGameScene";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Go to next scene?
		if(Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.Alpha9)){
			if(SceneManager.GetActiveScene().name.Equals("TutorialScene")){
				SceneManager.LoadScene(sceneOnExitTutorial);
			}
		}
    }

	void OnTriggerEnter(Collider col){
		// Check if there is a rigidbody attached
		if(col.attachedRigidbody == null) return;
		// Check if it's the player
		GameObject obj = col.attachedRigidbody.gameObject;
		if(!obj.name.Equals("Player")) return;
		// Report this as the most recent valid tile the player stepped on
		PlayerCharacter pc = obj.GetComponent<PlayerCharacter>() as PlayerCharacter;
		pc.ReturnToLastValidTile();
	}

	public static GameObject getSignpostPanel(){
		GameObject panel = GameObject.Find("BottomOfMap").GetComponent<Globals>().panelForSignposts;
		return panel;
	}

	public static void MakeExplosion(Vector3 position){
		GameObject explosion = Instantiate(GameObject.Find("BottomOfMap").GetComponent<Globals>().defaultExplosion, position + Vector3.up, Quaternion.identity) as GameObject;
	}
}