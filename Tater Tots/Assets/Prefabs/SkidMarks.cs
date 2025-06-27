using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkidMarks : MonoBehaviour
{
    public TrailRenderer[] Tyremarks;

    void Update()
    {
        // Check if steering left or right
        bool isTurning = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow)
        || Input.GetKey(KeyCode.RightArrow);

        foreach (TrailRenderer trail in Tyremarks)
        {
            trail.emitting = isTurning;
        }
    }
}
