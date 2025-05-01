using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerCharacter playerCharacter;
    [SerializeField] private PlayerCamera playerCamera;
    
    // Start is called before the first frame update
    public TextMeshProUGUI countText;

    // UI object to display winning text.
    public GameObject winTextObject;
    public int count;

    // Update is called once per frame
    private void Start()
    {
        SetCountText();
        playerCharacter.Initialize();
        //playerCamera.Initialize(playerCharacter.getCameraTarget());
    }

    void Update()
    {
        
    }
    
    public void SetCountText() 
    {
        // Update the count text with the current count.
        countText.text = "Enemies killed: " + count + "/5";

        // Check if the count has reached or exceeded the win condition.
        if (count >= 5)
        {
            // Display the win text.
            winTextObject.SetActive(true);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            winTextObject.gameObject.SetActive(true);
            winTextObject.GetComponent<TextMeshProUGUI>().text = "You Lose!";
        }
    }
}
