using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Add different types of rooms here
public enum ROOM_TYPE{None = 0, Monster = 1, Puzzle = 2, Platform = 4};

public class Room : MonoBehaviour
{
	public ROOM_TYPE roomType = ROOM_TYPE.None;
	public bool blockFront = false, blockRight = false, blockBack = false, blockLeft = false;
	public string entryMessage = "";
	public bool repeatMessage = false;
	private int messageDisplayTicks = -1;
	private bool displayedMessage = false;

    // Start is called before the first frame update
    void Start()
    {
        // Get the room's size
		Transform wallsCollection = transform.Find("Walls").gameObject.transform;
		float minX = 9999f, maxX = -9999f, minZ = 9999f, maxZ = -9999f;
		// The blocking tiles, in case we do need to block
		GameObject frontDoor = null, rightDoor = null, backDoor = null, leftDoor = null;
		// Iterate through the 4 walls
		for(int i = 0; i < 4; i++){
			GameObject wall = wallsCollection.GetChild(i).gameObject;
			Vector3 wallPos = wall.transform.position;
			if(wallPos.x > maxX) maxX = wallPos.x;
			if(wallPos.x < minX) minX = wallPos.x;
			if(wallPos.z > maxZ) maxZ = wallPos.z;
			if(wallPos.z < minZ) minZ = wallPos.z;
			// Find each of the blocking doors
			Transform tf = wall.transform.Find("frontDoor");
			if(tf != null){
				frontDoor = tf.gameObject;
				continue;
			}
			// Not front
			tf = wall.transform.Find("rightDoor");
			if(tf != null){
				rightDoor = tf.gameObject;
				continue;
			}
			// Not right
			tf = wall.transform.Find("backDoor");
			if(tf != null){
				backDoor = tf.gameObject;
				continue;
			}
			// Not left
			tf = wall.transform.Find("leftDoor");
			if(tf != null){
				leftDoor = tf.gameObject;
			}
		}
		// Scale the room's bounding box accordingly
		GetComponent<BoxCollider>().size = new Vector3(1f + maxX - minX, 10f, 1f + maxZ - minZ);
		// Determine if we should seal the room or not
		if(blockFront && frontDoor != null)
			frontDoor.SetActive(true);
		if(blockRight && rightDoor != null)
			rightDoor.SetActive(true);
		if(blockBack && backDoor != null)
			backDoor.SetActive(true);
		if(blockLeft && leftDoor != null)
			leftDoor.SetActive(true);
    }

    void FixedUpdate()
    {
		if(messageDisplayTicks > 0)
			--messageDisplayTicks;
		if(messageDisplayTicks == 0){
			GameObject.Find("SystemMessage").GetComponent<Text>().text = "";
			messageDisplayTicks = -1;
		}
    }

	void OnTriggerEnter(Collider col){
		if(col.attachedRigidbody && col.attachedRigidbody.gameObject.name.Equals("Player")){
			if(displayedMessage && !repeatMessage) return; // No need to show any system message again if we choose not to
			GameObject.Find("SystemMessage").GetComponent<Text>().text = entryMessage;
			messageDisplayTicks = 350; // Show message for 7 seconds
			displayedMessage = true;
		}
	}

	void OnTriggerExit(Collider col){
		if(col.attachedRigidbody && col.attachedRigidbody.gameObject.name.Equals("Player")){
			messageDisplayTicks = 0; // Hide system message if there was any
			// If there are any living enemies, have them go back to patrolling
			Transform characters = transform.Find("Characters");
			int numCharacters = characters.childCount;
			for(int i = 0; i < numCharacters; i++){
				Transform tf = characters.GetChild(i);
				if(tf == null) continue;
				GameObject obj = tf.gameObject;
				if(obj != null && obj.activeSelf){
					Transform fn = tf.Find("Functional");
					if(fn == null) continue;
					BasicEnemyCharacter en = fn.gameObject.GetComponent<BasicEnemyCharacter>();
					if(en == null) continue;
					en.ForceReturnToPatrol();
				}
			}
		}
	}

	public ROOM_TYPE GetRoomType(){ return roomType; }
}