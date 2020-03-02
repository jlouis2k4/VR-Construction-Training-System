using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sendPlayer : MonoBehaviour
{
    public Transform respawnPoint = null;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        print("player touched!\n");
        if (other.gameObject.tag == "Player")
        {
            print("player die!!\n");
            other.transform.position = new Vector3(respawnPoint.position.x, respawnPoint.position.y, respawnPoint.position.z);
        }
    }
}
