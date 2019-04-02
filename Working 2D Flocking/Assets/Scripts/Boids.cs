using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boids : MonoBehaviour
{
    public GameObject boidPrefab; //Link boid prefab to this
    public GameObject[] boids; //Array of bvoid gameobjects
    public int boidCount = 15; //Number of boids
    public Vector3 range = new Vector3(5, 5, 5); //Size of the area around the boid manager - boids spawn within this area
    [Range(0, 200)] //Slider in inspector allows you to set value of following variable
    public int neighborDistance = 50; //Determines how close a boid has to be to another boid to consider it a neighbor
    [Range(0, 2)]
    public float maximumForce = 0.5f;
    [Range(0, 5)]
    public float maximumVelocity = 2.0f;

    public bool moveToTarget = true;
    public bool followFlockRules = true;
    public bool ignoreFlockRules = false;

    private void OnDrawGizmosSelected() //This helps to see boid movement
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(this.transform.position, range * 2);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, 0.2f);
    }

    // Use this for initialization
    void Start ()
    {
        boids = new GameObject[boidCount]; //boids array is set to boidCount
        for(int i = 0; i < boidCount; i++) //For each boid in the boidCount
        {
            Vector3 boidPosition = new Vector3(Random.Range(-range.x, range.x), Random.Range(-range.y, range.y), Random.Range(0, 0)); //Calculates random boid spawn position within range stated above
            boids[i] = Instantiate(boidPrefab, this.transform.position + boidPosition, Quaternion.identity) as GameObject; //Creates boid gameobject at position based on boidPosition calculation
            boids[i].GetComponent<Boid>().boidManager = this.gameObject; //Sets the boid manager for each boid to this attached Gameobject (This attached gameObject is the boid manager)
        }
	}

}
