using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Signpost : MonoBehaviour
{
	public GameObject messagePanel;
	public string message;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

	void OnTriggerEnter(Collider col){
		// Display the message to the player
		if(col.attachedRigidbody && col.attachedRigidbody.gameObject.name.Equals("Player") && col.attachedRigidbody.gameObject.tag.Equals("Character")){
			messagePanel.SetActive(true);
			GameObject.Find("Sign_Message").GetComponent<Text>().text = message;
		}
	}

	void OnTriggerExit(Collider col){
		// Hide message from the player
		if(col.attachedRigidbody && col.attachedRigidbody.gameObject.name.Equals("Player") && col.attachedRigidbody.gameObject.tag.Equals("Character")){
			messagePanel.SetActive(false);
		}
	}
}
