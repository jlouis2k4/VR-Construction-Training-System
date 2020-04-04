using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Hazard_Detection : MonoBehaviour
{

    public Text hazard_Panel;
    public GameObject hazard_Menu_UI;
    public GameObject death_Menu_UI;
    public Transform player_Camera;
    private GameObject safety_item;
    private bool pause_Game = false;



    // Start is called before the first frame update
    void Start()
    {
        death_Menu_UI.SetActive(false);
        hazard_Menu_UI.SetActive(false);
       
    }

    // Update is called once per frame


    void Update()
    {

    }
    //checks to see if player collided with object of hazard, or 
    //of death
    /**    SCORE AND COMPLETENESS NEED TO BE IMPLEMENTED      **/
    private void OnTriggerEnter(Collider other)
    {
        //calls function death if player collides with death object
        if (other.gameObject.tag == "deathZone")
        {
            game_Over();
        }

        if (other.gameObject.tag == "low")
        {
            if(other.gameObject.name == "Hard Hat")
            {
                other.gameObject.tag = "Untagged";
            }
            safety_Gear_Interact(other.gameObject);   
        }
    }

    private void safety_Gear_Interact(GameObject other)
    {
            safety_item = other;
           // print(other.name);
            hazard_Panel.text = " Pick Up " + other.name.ToString() + "?!";
            //safety_item.gameObject.tag = "untagged";
            hazard_Menu_UI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            toggle_Time();
      

    }

    //collison bug (needs fixings)
    public void equip()
    {

        toggle_Time();
        hazard_Menu_UI.SetActive(false);
        Hazard hazard = safety_item.GetComponent<Hazard>();
        if (hazard != null)
        {
            hazard.Completed = true;
        }
        if(safety_item.name == "Hard Hat")
        {
            //print(safety_item.name);
            safety_item.transform.SetParent(player_Camera.transform);
            safety_item.transform.SetPositionAndRotation(player_Camera.transform.position, player_Camera.transform.rotation);
            safety_item.transform.position += new Vector3(0, 0.7f, 0);
        }
        else
        {
            Destroy(safety_item);

        }
        //destroy object and implement attaching to player;
    }

    public void dont_Equip()
    {
        toggle_Time();
        hazard_Menu_UI.SetActive(false);
        
    }


    public void game_Over()
    {
        //open death panel
        death_Menu_UI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //stop time....
        toggle_Time();
    }


    private void toggle_Time()
    {
        pause_Game = !pause_Game;

        if (pause_Game)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void Retry()
    {

        toggle_Time();
        death_Menu_UI.SetActive(false);

    }
}
