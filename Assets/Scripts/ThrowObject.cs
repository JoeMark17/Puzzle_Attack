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
    public bool beingCarried = false;
    public AudioClip[] soundToPlay;
    //private AudioSource audio;
    public int dmg;
    public bool catchBool = false;
    private bool touched = false;
    private InputManager inputManager;

    private void Start()
    {
        inputManager = InputManager.Instance;
    }

    public void Update()
    {
        float dist = Vector3.Distance(gameObject.transform.position, player.position);
        int keyCount = playerCam.transform.childCount;
        Debug.Log(keyCount);

        if (dist <= 4f)
        {
            hasPlayer = true;
        }
        else
        {
            hasPlayer = false;
        }
              
        if (hasPlayer == true && beingCarried == false && keyCount == 0 && inputManager.PlayerGrabedThisFrame())
        {
            PickUpKey();
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
                    ThrowKey();
                }
            }
        }

   public void OnTriggerEnter(Collider collision)
    {
        if (beingCarried)
        {
            touched = true;
        }

        FindObjectOfType<EnemyAI>();
        collision.gameObject.GetComponent<EnemyAI>().TakeDmg(dmg);
    }


    public void ThrowKey()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        transform.parent = null;
        beingCarried = false;
        GetComponent<Rigidbody>().AddForce(playerCam.forward * throwForce);

        catchBool = true;
        Debug.Log("Thrown");
        Invoke("dontCatch", 2f);
    }

    public void PickUpKey ()
    {
        transform.parent = playerCam.transform;
        transform.localPosition = puloc;

        GetComponent<Rigidbody>().isKinematic = true;
        Transform puzzleObject = GetComponent<Rigidbody>().transform;
        puzzleObject.localPosition = puloc + new Vector3 (0f, 0f, 4f);

        beingCarried = true;
    }

    public void EnemyPickUp (Transform enemy)
    {
        transform.parent = enemy.transform;
        transform.localPosition = puloc;

        GetComponent<Rigidbody>().isKinematic = true;
        Transform puzzleObject =  GetComponent<Rigidbody>().transform;
        puzzleObject.localPosition = puloc + new Vector3 (0f, 0f, 1.5f);
        
        beingCarried = true;
    }

    public void EnemyThrowKey(Transform enemy)
    {
        GetComponent<Rigidbody>().isKinematic = false;
        transform.parent = null;
        GetComponent<Rigidbody>().AddForce(enemy.forward * throwForce);
        beingCarried = false;
    }

    public void dontCatch()
    {
        Debug.Log("Ready for pickup");
        catchBool = false;
    }

}