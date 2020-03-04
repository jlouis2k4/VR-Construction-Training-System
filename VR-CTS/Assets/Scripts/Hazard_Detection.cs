using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Hazard_Detection : MonoBehaviour
{

    public Text myText;

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
        if(other.gameObject.tag == "low")
        { 
            print(other.gameObject.name);
            print("\n)");
            if(other.gameObject.name == "SafteyGlasses")
            {
                myText.text = other.gameObject.name.ToString() + " Picked Up!";
                Destroy(other.gameObject);

            }
        }
    }
}
