using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DroneDirection {
    UNASSIGNED,
    LEFT,
    RIGHT,
    UP,
    DOWN
};

public class DronePath : MonoBehaviour
{
    public Vector3[] waypoints;

    int currentWaypoint, nextWaypoint;
    public float speed;

    Vector3 startPosition;
    Vector3 currentRelativePosition;
    DroneDirection currentDirection;
    
    // Start is called before the first frame update
    void Start() {
        startPosition = transform.position;
        currentWaypoint = 0;
    }

    // Update is called once per frame
    void FixedUpdate() {
        nextWaypoint = currentWaypoint + 1;

        //figure out drone's current direction
        if (waypoints[nextWaypoint].x < 0) {
            currentDirection = DroneDirection.LEFT;
        }
        else if (waypoints[nextWaypoint].x > 0) {
            currentDirection = DroneDirection.RIGHT;
        }
        else if (waypoints[nextWaypoint].y > 0) {
            currentDirection = DroneDirection.UP;
        }
        else if (waypoints[nextWaypoint].y < 0) {
            currentDirection = DroneDirection.DOWN;
        }

        //move the drone...
        transform.position = Vector3.Lerp
            (transform.position, transform.position + waypoints[nextWaypoint], speed * Time.deltaTime);

        if (currentDirection == DroneDirection.LEFT && 
            transform.position.x - startPosition.x < waypoints[nextWaypoint].x - currentRelativePosition.x) {
            NextWaypoint();
        }
        if (currentDirection == DroneDirection.RIGHT &&
            transform.position.x - startPosition.x > waypoints[nextWaypoint].x - currentRelativePosition.x) {
            NextWaypoint();
        }
        if (currentDirection == DroneDirection.UP &&
            transform.position.y - startPosition.y > waypoints[nextWaypoint].y - currentRelativePosition.y) {
            NextWaypoint();
        }
        if (currentDirection == DroneDirection.DOWN &&
            transform.position.y - startPosition.y < waypoints[nextWaypoint].y - currentRelativePosition.y) {
            NextWaypoint();
        }
    }

    void NextWaypoint() {
        currentDirection = DroneDirection.UNASSIGNED;

        if (currentWaypoint < waypoints.Length - 2) {
            currentWaypoint++;
            //update position variable relative to waypoints:
            currentRelativePosition = new Vector3(
                currentRelativePosition.x - waypoints[currentWaypoint].x,
                currentRelativePosition.y - waypoints[currentWaypoint].y,
                0);
        }
        else {
            currentWaypoint = 0; //cyclical loop
            //update position variable relative to waypoints:
            currentRelativePosition = new Vector3(0, 0, 0);
        }
    }
}
