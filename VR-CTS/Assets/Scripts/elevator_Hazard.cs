using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//script for elevator hazard interatction
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

    //when an object with tag barricade collides with this object
    //sets off a true command telling the game hazard has been completed
    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "barricade")
        {
            print(other.gameObject.name);
            hazard.Completed = true; //communicates and updates score.

        }
      
    }

    //turns hazard completion to false, if barricade is removed
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "barricade")
        {
            print(other.gameObject.name);
            hazard.Completed = false;
            
        }
    }
}
