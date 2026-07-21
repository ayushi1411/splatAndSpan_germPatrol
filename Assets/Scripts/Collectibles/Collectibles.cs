using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectibles : MonoBehaviour
{
    public int scoreValue;
    bool isBeingPulled = false;
    Transform targetPlayer;
    float pullSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isBeingPulled && targetPlayer != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPlayer.position, pullSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            PlayerWeapon playerWeapon = other.GetComponent<PlayerWeapon>();
            if (playerController != null)
            {
                OnCollect(playerController, playerWeapon);
            }
        }
    }

    public void StartSuction(Transform player, float speed) 
    { 
        targetPlayer = player;
        pullSpeed = speed;
        isBeingPulled = true;
    }

    public virtual bool CanBePulled(PlayerController player)
    {
        return true;
    }

    protected virtual void OnCollect(PlayerController playerController, PlayerWeapon playerWeapon)
    {
        Destroy(gameObject);
    }
}
