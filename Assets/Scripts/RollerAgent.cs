using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using JetBrains.Annotations;
using UnityEngine.Rendering;
using Unity.VisualScripting;

public class RollerAgent : Agent
{
    Rigidbody rBody;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        platformID = platformObject.GetInstanceID();
    }
    private void FixedUpdate()
    {
        Target.localEulerAngles = Vector3.zero;
    }
    public Transform Target;
    public Transform Platform;
    public Rigidbody PlatformRBody;
    private float PreviousDistance;
    public GameObject platformObject;
    private int platformID;
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;

        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
    public override void OnEpisodeBegin()
    {
        //If the agent fell, zero its momentum
        if (this.transform.localPosition.y < Platform.localPosition.y - 5)
        {
            if(!this.GetComponent<Collider>().enabled)
            {
                this.GetComponent<Collider>().enabled = true;
            }
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            PlatformRBody.velocity = Vector3.zero;
            PlatformRBody.angularVelocity = Vector3.zero;
            PlatformRBody.rotation = Quaternion.identity;
            PlatformRBody.position = Vector3.zero;
            this.transform.localPosition = new Vector3(Platform.localPosition.x, 0.55f, Platform.localPosition.z);
        }

        Target.localEulerAngles = Vector3.zero;
        Target.localPosition = new Vector3(Platform.localPosition.x + Random.value * 8 - 4,
                                            0.25f,
                                            Platform.localPosition.z +  Random.value * 8 - 4);
        PreviousDistance = Vector3.Distance(this.transform.localPosition, Target.localPosition);


    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(PlatformRBody.rotation);
        sensor.AddObservation(PlatformRBody.angularVelocity);
        sensor.AddObservation(PlatformRBody.velocity);
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }

    public float forceMultiplier = 10;
    private bool collidedWithTarget = false;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];

        rBody.AddForce(controlSignal * forceMultiplier);

        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        if (distanceToTarget < 1.42f || collidedWithTarget)
        {
            AddReward(1.0f);
            collidedWithTarget = false;
            EndEpisode();

        }
        else if (this.transform.localPosition.y < -5)
        {
            AddReward(-0.05f);
            EndEpisode();
        }
        else if (distanceToTarget < PreviousDistance)
        {
            AddReward(0.01f);
            PreviousDistance = distanceToTarget;
        }
       
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetInstanceID() != platformID)
            this.GetComponent<Collider>().enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Target")
        {
            collidedWithTarget = true;
        }
    }
}
