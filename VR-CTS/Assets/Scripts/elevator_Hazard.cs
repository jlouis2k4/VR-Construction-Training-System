using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class elevator_Hazard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /**    SCORE AND COMPLETENESS NEED TO BE IMPLEMENTED      **/

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "barricade")
        {
            print(other.gameObject.name);
            //call complete hazard function
            //call score function
        }
    }
}
