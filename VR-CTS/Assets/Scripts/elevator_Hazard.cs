using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elevator_Hazard : MonoBehaviour
{
    private Hazard hazard;
    
    // Start is called before the first frame update
    void Awake()
    {
        hazard = GetComponent<Hazard>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "barricade")
        {
            print(other.gameObject.name);
            hazard.Completed = true; //communicates and updates score.

        }
      
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "barricade")
        {
            print(other.gameObject.name);
            hazard.Completed = false;
            
        }
    }
}
