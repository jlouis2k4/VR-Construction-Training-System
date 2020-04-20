using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//anything that is commetted out probably has to do with respawning a player into the game again.
//wasn't working at all even though could should've preformed as intended. still being implemented.
public class Hazard_Detection : MonoBehaviour
{

    public Text hazard_Panel;
    public GameObject hazard_Menu_UI;
    public GameObject death_Menu_UI;
    public Transform player_Camera;
    public GameObject safety_item = null;
    private bool pause_Game = false;
   // public GameObject respawn_Location;

    public PlayerIcon player_Icon;
    public float collision_Timer;


    // Start is called before the first frame update
    void Start()
    {
        death_Menu_UI.SetActive(false);
        hazard_Menu_UI.SetActive(false);
        collision_Timer = 10f;
       // print(respawn_Location.transform.position.x);
       // print(respawn_Location.transform.position.y);
       // print(respawn_Location.transform.position.z);

    }

    // Update is called once per frame


    void Update()
    {
        collision_Timer += Time.deltaTime;
    }
    //checks to see if player collided with object of hazard, or 
    //of death
    private void OnTriggerEnter(Collider other)
    {
        if (collision_Timer >= 10)
        {
            safety_item = other.gameObject;
            safety_item.SetActive(false);

            collision_Timer = 0;
            //calls function death if player collides with death object
            if (other.gameObject.tag == "deathZone")
            {
                
                game_Over();
            }

            if (other.gameObject.tag == "low")
            {
               // Debug.Log(other.gameObject.name);
                //if (safety_item == null)
                //{
                    safety_Gear_Interact(other.gameObject);

                //}
            }
        }
    }

    //sends message to canvas panel about current equipment being picked up
    private void safety_Gear_Interact(GameObject other)
    {
        
       
        safety_item = other;
        hazard_Panel.text = " Pick Up " + other.name.ToString() + "?!";

        //aligns menu panels with players current headset direction
        Vector3 headsetRot = player_Camera.rotation.eulerAngles;
        hazard_Menu_UI.transform.rotation = Quaternion.Euler(headsetRot.x, headsetRot.y, 0);
        hazard_Menu_UI.transform.position = player_Camera.position + player_Camera.TransformDirection(0, 0, 2);
        hazard_Menu_UI.SetActive(true);
        Pointer.MenuIsActive(true);
       // toggle_Time();
    }

    //checks to see which safety item is getting picked up and acts accordingly
    public void equip()
    {
        //collision timer fixes bugs of having constant visuals blocked by assets and 
        //collision boxes
		collision_Timer = 10f;
        //toggle_Time();
        hazard_Menu_UI.SetActive(false);
        Pointer.MenuIsActive(false);
        Hazard hazard = safety_item.GetComponent<Hazard>();
        if (hazard != null)
        {
            hazard.Completed = true;
        }

        //attachs the helmet to player given it a more realistic feeling
        if(safety_item.name == "Hard_Hat")
        {
            safety_item.gameObject.tag = "Untagged";

            //print(safety_item.name);
            safety_item.transform.SetParent(player_Camera.transform);
            safety_item.transform.SetPositionAndRotation(player_Camera.transform.position, player_Camera.transform.rotation);

            player_Icon.ToggleHardHat(true);
            safety_item = null;
        }
        else if(safety_item.name == "Safety Vest")
        {
            player_Icon.ToggleVest(true);
            Destroy(safety_item);
            safety_item = null;

        }
        else if(safety_item.name == "Safety Glasses")
        {
            player_Icon.ToggleGoggles(true);
            Destroy(safety_item);
            safety_item = null;
        }
        //destroy object and implement attaching to player;
    }

    public void dont_Equip()
    {
        safety_item.SetActive(true);
        //toggle_Time();
        hazard_Menu_UI.SetActive(false);
        
    }

    //game over when a player performs a task the is dangerous
    public void game_Over()
    {
        //open death panel
        death_Menu_UI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //stop time....
        toggle_Time();
    }

    //not in use, doesn't work within VR mode
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

    //restarts player in same location to continue game
    public void Retry()
    {
        collision_Timer = 0;

        toggle_Time();
        safety_item.SetActive(true);
        death_Menu_UI.SetActive(false);
        

    }
}
