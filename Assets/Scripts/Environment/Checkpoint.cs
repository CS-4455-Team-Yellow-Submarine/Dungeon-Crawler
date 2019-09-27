using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	public float moveTime; // Time it should take for movable platform to reach here from previous checkpoint

    // Start is called before the first frame update
    void Start()
    {
		// Just double check to make sure we don't have invalid movement times
		if(moveTime < 0) moveTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public int GetMoveTime(){
		// Convert to number of ticks and return the result
		return 1 + (int)(moveTime * 50f - 0.00001f);
	}
}
