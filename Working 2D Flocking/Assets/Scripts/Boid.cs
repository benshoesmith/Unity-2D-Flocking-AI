using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour {

    public GameObject boidManager;                  //Boid manager GameObject
    public Vector2 location = Vector2.zero;         //Zeroed position
    public Vector2 velocity;

    private Vector2 targetPosition = Vector2.zero;  //Position of boid target (zeroed)
    private Vector2 currentForce;

	// Use this for initialization
	void Start ()
    {
        velocity = new Vector2(Random.Range(0.01f, 0.1f), Random.Range(0.01f, 0.1f));                       //Assignes random velocity (no huge range)
        location = new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y); //Sets location to this gameobjects position
	}

    Vector2 MoveToTarget(Vector2 target)     //Returns vector2f by taking the location vector from the target vector
    {
        return (target - location);
    }
	
    void ApplyForce(Vector2 f)
    {
        Vector3 force = new Vector3(f.x, f.y, 0);                                                                           //Takes force and makes it a Vector3
        if (force.magnitude > boidManager.GetComponent<Boids>().maximumForce)                                               //If magnitude of the force is greater than the max force
        {
            force = force.normalized;                                                                                       //Normalize force and multiply by max force, making it the 
            force *= boidManager.GetComponent<Boids>().maximumForce;                                                        //length of max force (caps to max force while keeping direction)
        }
        this.GetComponent<Rigidbody2D>().AddForce(force);                                                                   //Adds force to boid rigidbody

        if(this.GetComponent<Rigidbody2D>().velocity.magnitude > boidManager.GetComponent<Boids>().maximumVelocity)         //If velocity is greater than maximum velocity
        {
            this.GetComponent<Rigidbody2D>().velocity = this.GetComponent<Rigidbody2D>().velocity.normalized;               //Normalize velocity and set to max velocity (caps to max velocity)
            this.GetComponent<Rigidbody2D>().velocity *= boidManager.GetComponent<Boids>().maximumVelocity;
        }
        Debug.DrawRay(this.transform.position, force, Color.white);                                             //Draws a line form the current potition of the particle using the foce as its direction
    }

    void Flocking()
    {
        location = this.transform.position;
        velocity = this.GetComponent<Rigidbody2D>().velocity;

        if (boidManager.GetComponent<Boids>().followFlockRules && Random.Range(0, 50) <= 1) //If boids are following flocking rules AND random is less than 1 (Not as heavy on computer capabilities (Every boid doing this every update is too intense)
        {
            //Reset allignment, cohesion and target
            Vector2 allignVector = Allign();
            Vector2 cohesionVector = Cohesion();
            Vector2 tl;
            if(boidManager.GetComponent<Boids>().moveToTarget) //If boids are set to move to target
            {
                tl = MoveToTarget(targetPosition);
                currentForce = tl + allignVector + cohesionVector; //Add target location, allignment vector and cohesion vector to make new force
            }
            else
            {
                currentForce = allignVector + cohesionVector;
            }
            currentForce = currentForce.normalized;
        }

        if(boidManager.GetComponent<Boids>().ignoreFlockRules && Random.Range(0, 50) <= 1) //IF boids are ignoring flocking rules AND rnage is less than or equal to 1
        {
            if(Random.Range(0, 50) < 1) //Select new random direction to move in
            {
                currentForce = new Vector2(Random.Range(0.01f, 0.1f), Random.Range(0.01f, 0.1f));
            }
        }

        ApplyForce(currentForce);

    }

    Vector2 Allign()
    {
        float neighborDistance = boidManager.GetComponent<Boids>().neighborDistance; //Gets neighbor distance value form Boids script
        Vector2 sum = Vector2.zero; //Zero out sum and count variables
        int count = 0;
        foreach(GameObject go in boidManager.GetComponent<Boids>().boids) //For each boid in the boids array
        {
            if(go == this.gameObject) //Ignore self
            {
                continue;
            }
            float dis = Vector2.Distance(location, go.GetComponent<Boid>().location); //Distance between this boid and the other boid in the array
            
            if(dis < neighborDistance) //If found boid is within the nieghbor distance
            {
                sum += go.GetComponent<Boid>().velocity; //Add that velocity to our sum and increment count
                count++;
            }
        }
        if(count > 0) //If count is greater than zero
        {
            sum /= count; //Divide sum by its count (calc average)
            Vector2 steer = sum - velocity; //Steer value is the avergae heading of the boids minus the velocity
            return steer;
        }
        return Vector2.zero; //If there are no neighbors, return zero vector (null)
    }

    Vector2 Cohesion()
    {
        float neighborDistance = boidManager.GetComponent<Boids>().neighborDistance; //Gets neighbor distance value from Boids script
        Vector2 sum = Vector2.zero; //Zero out sum and count variables
        int count = 0;
        foreach(GameObject go in boidManager.GetComponent<Boids>().boids) //For each boid in the boids array
        {
            if(go == this.gameObject) //ignore self
            {
                continue;
            }
            float dis = Vector2.Distance(location, go.GetComponent<Boid>().location); //Diustance between this boid and the other boid in the array

            if(dis < neighborDistance) //If found boid is within neighbor distance
            {
                sum += go.GetComponent<Boid>().location; //Add that velocity to sum and increment count
                count++;
            }
        }

        if(count > 0) //If count is greater than zero
        {
            sum /= count; //Find average
            return MoveToTarget(sum); //Get vector of sum (sets target location to the average of the group)
        }

        return Vector2.zero; //If there are no neighbors return zero (null)
    }

	// Update is called once per frame
	void Update ()
    {
        Flocking();
        targetPosition = boidManager.transform.position; //Keeps setting target position to the manager position
	}
}
