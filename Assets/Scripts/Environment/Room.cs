using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Add different types of rooms here
public enum ROOM_TYPE{None = 0, Monster = 1, Puzzle = 2, Platform = 4};

public class Room : MonoBehaviour
{
	public ROOM_TYPE roomType = ROOM_TYPE.None;

    // Start is called before the first frame update
    void Start()
    {
        // Get the room's size
		Transform wallsCollection = transform.Find("Walls").gameObject.transform;
		float minX = 9999f, maxX = -9999f, minZ = 9999f, maxZ = -9999f;
		// Iterate through the 4 walls
		for(int i = 0; i < 4; i++){
			GameObject wall = wallsCollection.GetChild(i).gameObject;
			Vector3 wallPos = wall.transform.position;
			if(wallPos.x > maxX) maxX = wallPos.x;
			if(wallPos.x < minX) minX = wallPos.x;
			if(wallPos.z > maxZ) maxZ = wallPos.z;
			if(wallPos.z < minZ) minZ = wallPos.z;
		}
		// Scale the room's bounding box accordingly
		GetComponent<BoxCollider>().size = new Vector3(1f + maxX - minX, 10f, 1f + maxZ - minZ);
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