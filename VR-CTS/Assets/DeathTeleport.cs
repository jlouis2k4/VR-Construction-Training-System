using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DeathTeleport : MonoBehaviour
{
    public GameObject deathPanel;
    private bool pauseGame = false;

    public Transform respawnPoint = null;
 
    // Start is called before the first frame update
    void Start()
    {
        deathPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        print("Touching box collider!\n");
        if (other.gameObject.tag == "deathZone")
        {
            print("Let's die!!\n");
            this.transform.position = new Vector3(respawnPoint.position.x, respawnPoint.position.y, respawnPoint.position.z) ;
        }
    }


    public void gameOver()
    {
        //open death panel
        deathPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //stop time....
        toggleTime();
    }


    private void toggleTime()
    {
        pauseGame = !pauseGame;

        if (pauseGame)
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

        toggleTime();
        deathPanel.SetActive(false);

    }
}
