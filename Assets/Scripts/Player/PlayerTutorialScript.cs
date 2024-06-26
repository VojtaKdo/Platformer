using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTutorialScript : MonoBehaviour
{
    // Start is called before the first frame update

    [Header("Tutorial Images")]
    [SerializeField] PlayerDamageScript playerDamage;
    [SerializeField] PlayerMovementScript playerMovement;
    public GameObject leftrightTutorialImage;
    public GameObject dashTutorialImage;
    public GameObject jumpTutorialImage;
    public GameObject descendTutorialImage;
    public GameObject shootInstantlyImage;
    public GameObject shootChargedImage;
    public GameObject shootAirShotImage;
    public GameObject portalSpawnImage;
    public GameObject[] tutorialImages = new GameObject[9];
    [Header("Tutorial Triggers")]
    public GameObject leftrightTrigger;
    public GameObject dashTrigger;
    public GameObject jumpTrigger;
    public GameObject descendTrigger;
    public GameObject shootTrigger;
    public GameObject Portal;
    public static int tutorialProgress = 0;

    void Start()
    {
        if(playerDamage == null)
        {
            playerDamage = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerDamageScript>();
        }
        if (playerMovement == null) {
            playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerMovementScript>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (tutorialProgress == 1 && playerMovement.horizontal > 0)
        {
            DisableImages();
        }
        else if (tutorialProgress == 2 && Input.GetKeyDown(KeyCode.LeftShift))
        {
            DisableImages();
        }
        else if (tutorialProgress == 3 && Input.GetButtonDown("Jump"))
        {
            DisableImages();
        }
        else if (tutorialProgress == 4 && (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)))
        {
            DisableImages();
        }
        else if (tutorialProgress == 5 && Input.GetMouseButtonDown(0))
        {
            DisableImages();
            tutorialProgress += 1;
        }
        else if (tutorialProgress == 6 && Input.GetMouseButtonUp(0))
        {
            DisableImages();
            tutorialProgress += 1;
        }
        else if (tutorialProgress == 7 && Input.GetMouseButtonUp(0) && playerDamage.playSFXonce)
        {
            DisableImages();
            tutorialProgress += 1;
        }
        else if (tutorialProgress == 8 && playerDamage.airShotsAvailable == 0)
        {
            DisableImages();
            tutorialProgress += 1;
            Portal.SetActive(true);
            Destroy(shootTrigger);
        }
        else if (tutorialProgress == 9) {
            portalSpawnImage.SetActive(true);
        }

        switch (tutorialProgress)
        {
            case 5:
                {
                    descendTutorialImage.SetActive(false);
                    shootTrigger.transform.localScale = new Vector3(100, 100, 0);
                    if (tutorialProgress != 5)
                    {
                        for (int i = 0; i < tutorialImages.Length; i++)
                        {
                            tutorialImages[i].SetActive(false);
                        }
                    }
                    else {
                        tutorialImages[tutorialProgress-1].SetActive(true);
                    }
                }
                break;
            case 6:
                DisableImages();
                break;
            case 7:
                {
                    if (tutorialProgress != 7)
                    {
                        for (int i = 0; i < tutorialImages.Length; i++)
                        {
                            tutorialImages[i].SetActive(false);
                        }
                    }
                    else
                    {
                        tutorialImages[tutorialProgress - 1].SetActive(true);
                    }
                }
                break;
            case 8:
                {
                    if (tutorialProgress != 8)
                    {
                        for (int i = 0; i < tutorialImages.Length; i++)
                        {
                            tutorialImages[i].SetActive(false);
                        }
                    }
                    else
                    {
                        tutorialImages[tutorialProgress - 1].SetActive(true);
                    }
                }
                break;
            case 9:
                {
                    if (tutorialProgress != 9)
                    {
                        for (int i = 0; i < tutorialImages.Length; i++)
                        {
                            tutorialImages[i].SetActive(false);
                        }
                    }
                    else
                    {
                        tutorialImages[tutorialProgress - 1].SetActive(true);
                    }
                }
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            tutorialProgress += 1;
            Debug.Log("Player touched!");
            switch (tutorialProgress) {
                case 1:
                    {
                        if (tutorialProgress != 1)
                        {
                            for (int i = 0; i < tutorialImages.Length; i++)
                            {
                                tutorialImages[i].SetActive(false);
                            }
                        }
                        else
                        {
                            tutorialImages[tutorialProgress - 1].SetActive(true);
                        }
                    }
                    Destroy(leftrightTrigger);
                    break;
                case 2:
                    {
                        if (tutorialProgress != 2)
                        {
                            for (int i = 0; i < tutorialImages.Length; i++)
                            {
                                tutorialImages[i].SetActive(false);
                            }
                        }
                        else
                        {
                            tutorialImages[tutorialProgress - 1].SetActive(true);
                        }
                    }
                    Destroy(dashTrigger);
                    break;
                case 3:
                    {
                        if (tutorialProgress != 3)
                        {
                            for (int i = 0; i < tutorialImages.Length; i++)
                            {
                                tutorialImages[i].SetActive(false);
                            }
                        }
                        else
                        {
                            tutorialImages[tutorialProgress - 1].SetActive(true);
                        }
                    }
                    Destroy(jumpTrigger);
                    break;
                case 4:
                    {
                        if (tutorialProgress != 4)
                        {
                            for (int i = 0; i < tutorialImages.Length; i++)
                            {
                                tutorialImages[i].SetActive(false);
                            }
                        }
                        else
                        {
                            tutorialImages[tutorialProgress - 1].SetActive(true);
                        }
                    }
                    Destroy(descendTrigger);
                    break;
            }
        }
    }

    void DisableImages(){
        leftrightTutorialImage.SetActive(false);
        dashTutorialImage.SetActive(false);
        jumpTutorialImage.SetActive(false);
        descendTutorialImage.SetActive(false);
        shootInstantlyImage.SetActive(false);
        shootChargedImage.SetActive(false);
        shootAirShotImage.SetActive(false);
        portalSpawnImage.SetActive(false);
    }
}
