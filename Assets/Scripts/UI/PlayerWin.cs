using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWin : MonoBehaviour
{
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody.gameObject.name.Equals("Player"))
            GameManager.instance.Win();
    }
}
