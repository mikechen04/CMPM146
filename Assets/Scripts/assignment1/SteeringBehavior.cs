using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class SteeringBehavior : MonoBehaviour
{
    public Vector3 target;
    public KinematicBehavior kinematic;
    public List<Vector3> path;
    // you can use this label to show debug information,
    // like the distance to the (next) target
    public TextMeshProUGUI label;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        kinematic = GetComponent<KinematicBehavior>();
        target = transform.position;
        path = null;
        EventBus.OnSetMap += SetMap;
    }

    // Update is called once per frame
    void Update()
    {
        // Assignment 1: If a single target was set, move to that target
        //                If a path was set, follow that path ("tightly")

        // you can use kinematic.SetDesiredSpeed(...) and kinematic.SetDesiredRotationalVelocity(...)
        //    to "request" acceleration/decceleration to a target speed/rotational velocity

        /* 
        Part 1 of the assignment goes here:
        Seek to a single target, calls SetTarget

        Determine forward and rotational velocities (kinematic.SetDesiredSpeed and kinematic.SetDesiredRotationalVelocity)
        Forward postive goes forward, forward negative goes backward 
        Rotational velocity positive goes right, Rotational velocity negative goes left (scaled by delta between frames)

        How far do we turn, we set it by using: SetDesiredRotationalVelocity
        Increase it to turn more, decrease it to turn less (i think delta is something we dont use here)

        Problems with car circling instead of stopping, use arrival radius to stop the car down
        */ 

        // Find the direction of the car and target
        Vector3 directionToTarget = (target - transform.position).normalized;
        // car pointing to the target
        // normalize so we can have more consistent calculations regardless of how far the target is

        float targetAngle = Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up);
        // function returns the angle in degrees between two directions, one direction is the direction the car is facing and the other direction is the target
        // Vector3.up is the axis we are moving around in, i set it to the horizontal plane

        // Forward speed
        float speed = 5.0f; // The speed of the car
        float arrivalRadius = 1.0f; // Stop the car when its close
        float slowingRadius = 2.0f; // Slow the car

        float distanceToTarget = Vector3.Distance(transform.position, target); // Distance between car and target
        float desiredSpeed = speed;

        // Slow down section
        // If we are not looking at the target, slow down so we can turn later
        if (Mathf.Abs(targetAngle) > 45) {
            desiredSpeed *= 0.5f;
        }

        // Do not reverse the if statements
        if (distanceToTarget < arrivalRadius) { // If we are within the target, stop
            desiredSpeed = 0;
        } else if (distanceToTarget < slowingRadius) { // If we get closer, slow down
            desiredSpeed *= distanceToTarget / slowingRadius;
        }

        kinematic.SetDesiredSpeed(desiredSpeed); // apply the speed

        // Rotation speed
        float rotationSpeed = 2.0f;
        float desiredRotation = 0;
        
        // If the target is to the right, turn right
        if (targetAngle > 0) {
            desiredRotation = Mathf.Min(targetAngle * 0.1f, rotationSpeed);
        } else { // else, target is to the left, turn left
            desiredRotation = Mathf.Max(targetAngle * 0.1f, -rotationSpeed);
        }

        kinematic.SetDesiredRotationalVelocity(desiredRotation); // apply rotation
        
    }

    public void SetTarget(Vector3 target)
    {
        this.target = target;
        EventBus.ShowTarget(target);
    }

    public void SetPath(List<Vector3> path)
    {
        this.path = path;
    }

    public void SetMap(List<Wall> outline)
    {
        this.path = null;
        this.target = transform.position;
    }
}
