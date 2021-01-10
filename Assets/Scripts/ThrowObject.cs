using UnityEngine;
using System.Collections;

public class ThrowObject : MonoBehaviour
{
    public Transform player;
    public Transform playerCam;
    public Vector3 puloc;
    public Transform vcam;
    public float throwForce = 10;
    bool hasPlayer = false;
    float limitObjects = 1f;
    bool beingCarried = false;
    public AudioClip[] soundToPlay;
    //private AudioSource audio;
    public int dmg;
    private bool touched = false;
    private InputManager inputManager;

    private void Start()
    {
        inputManager = InputManager.Instance;
    }
    

    // void Start()
    // {
    //     audio = GetComponent<AudioSource>();
    // }

    void Update()
    {
        float dist = Vector3.Distance(gameObject.transform.position, player.position);
        Debug.Log(limitObjects);

        if (dist <= 4f)
        {
            hasPlayer = true;
        }
        else
        {
            hasPlayer = false;
        }

                   
        if (hasPlayer == true && beingCarried == false && inputManager.PlayerGrabedThisFrame())
        {
            if (limitObjects == 1f)
            {
                transform.parent = playerCam.transform;
                transform.localPosition = puloc;

                GetComponent<Rigidbody>().isKinematic = true;
                Transform puzzleObject =  GetComponent<Rigidbody>().transform;

                // puzzleObject.position = playerCam.position + new Vector3 (0.98f,-0.28f,3.25f);
                puzzleObject.localPosition = puloc + new Vector3 (0f, 0f, 4f);

                beingCarried = true;
                Debug.Log("Looping");
                
            }
            limitObjects++;                
            Debug.Log(limitObjects);
            
        }
    
        if (beingCarried)
        {
            if (touched)
            {
                GetComponent<Rigidbody>().isKinematic = false;
                transform.parent = null;
                beingCarried = false;
                touched = false;
            }
            if (inputManager.PlayerThrewThisFrame())
                {
                    GetComponent<Rigidbody>().isKinematic = false;
                    transform.parent = null;
                    beingCarried = false;
                    GetComponent<Rigidbody>().AddForce(playerCam.forward * throwForce);

                    limitObjects--;
                    Debug.Log(limitObjects);
                    //RandomAudio();
                }
                // else if (Input.GetMouseButtonDown(1))
                // {
                //     GetComponent<Rigidbody>().isKinematic = false;
                //     transform.parent = null;
                //     beingCarried = false;
                // }
            }
        }
    // void RandomAudio()
    // {
    //     if (audio.isPlaying){
    //         return;
    //             }
    //     audio.clip = soundToPlay[Random.Range(0, soundToPlay.Length)];
    //     audio.Play();

    // }
   void OnTriggerEnter()
    {
        if (beingCarried)
        {
            touched = true;
        }
    }
    }