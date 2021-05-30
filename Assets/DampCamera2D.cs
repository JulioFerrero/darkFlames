using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class DampCamera2D : MonoBehaviour
     {
         public Transform target;
         public float smoothTime = 0.3F;
         private Vector3 velocity = Vector3.zero;
         private Vector3 targetPosition;

         void Start()
         {
             targetPosition = target.TransformPoint(new Vector3(0.8f, 0.85f, -1));
         }

         void Update()
         {
             // Define a target position above and behind the target transform
             
             if (Input.GetKey("left"))
             {
                 targetPosition = target.TransformPoint(new Vector3(-0.8f, 0.85f, -1));
             }
             else if (Input.GetKey("right"))
             {
                 targetPosition = target.TransformPoint(new Vector3(0.8f, 0.85f, -1));
             }

             // Smoothly move the camera towards that target position
             transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
         }
     }
