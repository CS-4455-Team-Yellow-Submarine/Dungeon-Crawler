using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Add different types of rooms here
public enum ROOM_TYPE{None = 0, Monster = 1, Puzzle = 2, Platform = 4};

public class Room : MonoBehaviour
{
	public ROOM_TYPE roomType = ROOM_TYPE.None;
	public bool blockFront = false, blockRight = false, blockBack = false, blockLeft = false;

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
        
    }

	void OnTriggerEnter(Collider col){
		if(col.attachedRigidbody && col.attachedRigidbody.gameObject.name.Equals("Player")){
			//Debug.Log("Player entered room!");
		}
	}

	public ROOM_TYPE GetRoomType(){ return roomType; }
}