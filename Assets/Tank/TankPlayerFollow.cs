using UnityEngine;

public class TankPlayerFollow : MonoBehaviour
{

    [SerializeField] private Transform player; // Reference to the player's transform

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z - 5); // Follow the player's x and z position while keeping the camera's y position

    }





}
