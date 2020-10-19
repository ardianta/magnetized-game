using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerController : MonoBehaviour
{

    public float moveSpeed = 5f;
    private Rigidbody2D rb2D;

    public float pullForce = 100f;
    public float rotateSpeed = 360f;
    private GameObject closestTower;
    private GameObject hookedTower;
    private bool isPulled = false;

    private AudioSource myAudio;
    private bool isCrashed = false;

    public GameObject clearPanel;
    
    // Start is called before the first frame update
    void Start()
    {
        rb2D = this.gameObject.GetComponent<Rigidbody2D>();
        myAudio = this.gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        rb2D.velocity = -transform.up * moveSpeed;

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(0) && !isPulled)
        {
            if (closestTower != null && hookedTower == null)
            {
                hookedTower = closestTower;
            }
            if (hookedTower)
            {
                float distance = Vector2.Distance(transform.position, hookedTower.transform.position);

                // gravitation toward tower
                Vector3 pullDirection = (hookedTower.transform.position - transform.position).normalized;
                float newPullForce = Mathf.Clamp(pullForce / distance, 20, 50);
                rb2D.AddForce(pullDirection * newPullForce);

                // putar player dengan angular velocity
                rb2D.angularVelocity = -rotateSpeed / distance;

                isPulled = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.Z) || Input.GetMouseButtonUp(0))
        {
            isPulled = false;
            hookedTower = null;
            rb2D.angularVelocity = 0f;
        }

        if (isCrashed && !myAudio.isPlaying)
        {
            // restart scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    
    }

    public void OnCollisionEnter2D(Collision2D collections)
    {
        if (collections.gameObject.tag == "Wall" && !isCrashed)
        {
            // play SFX
            myAudio.Play();
            rb2D.velocity = new Vector3(0f, 0f, 0f);
            rb2D.angularVelocity = 0f;
            isCrashed = true;

            // hide this game object
            // this.gameObject.SetActive(false);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            Debug.Log("Levelcelar!");
            clearPanel.SetActive(true);
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Tower")
        {
            closestTower = collision.gameObject;

            // change tower color back to green as indicator
            collision.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (isPulled) return;

        if (collision.gameObject.tag == "Tower")
        {
            closestTower = null;
            hookedTower = null;

            // change tower color back to normal
            collision.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
