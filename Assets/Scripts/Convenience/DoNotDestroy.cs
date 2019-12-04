using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNotDestroy : MonoBehaviour
{
	void Awake(){
		DontDestroyOnLoad(this.gameObject);
	}

	public void SetCheckpointLocation(Vector3 pos){
		transform.position = pos;
	}
}
