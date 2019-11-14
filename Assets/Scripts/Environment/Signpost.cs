using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Signpost : MonoBehaviour
{
	public string[] messageLines;

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
			Globals.getSignpostPanel().SetActive(true);
			string displayMessage = "";
			foreach(string s in messageLines){
				displayMessage = displayMessage + s + "\n";
			}
			Globals.getSignpostPanel().transform.GetChild(0).GetComponent<Text>().text = displayMessage;
		}
	}

	void OnTriggerExit(Collider col){
		// Hide message from the player
		if(col.attachedRigidbody && col.attachedRigidbody.gameObject.name.Equals("Player") && col.attachedRigidbody.gameObject.tag.Equals("Character")){
			Globals.getSignpostPanel().SetActive(false);
		}
	}
}
