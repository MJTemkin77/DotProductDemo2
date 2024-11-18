using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDetect : MonoBehaviour
{
   
    [SerializeField]
    private float radius = .5f;
    [SerializeField]
    private LayerMask WallLayer;

   

    InputAction detectAction, moveAction;
    void Start()
    {
        detectAction = InputSystem.actions.FindAction("Click");
        moveAction = InputSystem.actions.FindAction("Move");
    }

   

    // Update is called once per frame
    void Update()
    {
        if (detectAction.IsPressed())
        {
            ClearLog();
            Collider[] colliders = FindCollisionsWithWalls(radius);
            ReportColliders(colliders);
            float[] dotProdCollisionResults = GetDotProductCollisionResults(colliders);
            

        }
        if (moveAction.IsPressed())
        {
            Vector2 movement = moveAction.ReadValue<Vector2>();
            //Debug.Log($"Movement is: {movement}");

            Vector3 nextMoveXZPlane = new Vector3(-movement.y, 0, movement.x);
            transform.Translate(nextMoveXZPlane * Time.deltaTime, Space.Self);
        }

    }

    /// <summary>
    /// Get the dot product for each collision with the player.
    /// </summary>
    /// <param name="colliders">The current colliders detected by the Physics.Overlap Sphere</param>
    /// <returns></returns>
    private float[] GetDotProductCollisionResults(Collider[] colliders)
    {
        float[] results = new float[colliders.Length];
        for (int i = 0; i < colliders.Length; i++)
        {
            // Get the position of each collider.
            // Makes it's y value equal the y value of the player so that direction is level on the y axis.
            Vector3 hitPosition = colliders[i].transform.position;
            hitPosition.y = transform.position.y;

            // Get the vector that describes how to get from the hitPosition to the players position.
            Vector3 direction = hitPosition - transform.position;

            // Use the Vector3.Dot method to get the dot product of the following Vectors
            // transform.forward - which is the positive z axis from the player.
            // See: https://docs.unity3d.com/ScriptReference/Transform-forward.html
            // direction.normalized is a vector with a magnitude of 1 in the same direction as the current vector.
            // See: https://docs.unity3d.com/ScriptReference/Vector3-normalized.html
            // What is magnitude? It is the length of the vector which is the square root of x^2 + y^2 + z^2.
            // If the vector is (2, -3, 1) then the magnitude would be the square root of (2^2 + -3^2 + 1^2),
            // which is reduced to the square root of 4 + 9 + 1 or which is the square root of 14 which equals 3.742 (rounded).
            // See: https://docs.unity3d.com/ScriptReference/Vector3-magnitude.html
            // See: https://www.cuemath.com/questions/how-to-find-the-magnitude-of-a-vector-with-3-components/
            // https://youtu.be/wgrKs6ItJUs
            // Example of the above. The value is not used in the results array.
            Vector3 test = new Vector3(2, -3, 1);
            float mag = test.magnitude;
            Vector3 normalizedProof = new Vector3(test.x/mag, test.y/mag, test.z/mag);
            Vector3 normalizedResult = test.normalized;

            Assert.AreEqual(normalizedProof, normalizedResult);
            float magnitude = normalizedResult.magnitude;

            results[i] = Vector3.Dot(transform.forward, direction.normalized);
        }

        return results;
    }

    /// <summary>
    /// TODO:
    /// Display to the Unity Console the name of the Colliders in the collision.
    /// </summary>
    /// <param name="colliders"></param>
    private void ReportColliders(Collider[] colliders)
    {
        for (int i = 0; i < colliders.Length; i++) {
            Debug.Log($"Collider {i +1} is {colliders[i].gameObject.name}");
        }
    }

    /// <summary>
    /// Get the collection of collisions with GameObjects that are in the Wall layer.
    /// NOTE: The term layer was used, not tag.
    /// </summary>
    /// <param name="r">The radius</param>
    private Collider[] FindCollisionsWithWalls(float r)
    {
        int layerMask = WallLayer.value;
        Collider[] colliders =
        Physics.OverlapSphere(transform.position, radius, layerMask);

        if (colliders.Length == 0)
        {
            Debug.Log($"FindHistPointsOnWalls: No collisions with radius {r}");
        }
        else
        {
            Debug.Log($"FindHistPointsOnWalls: Has {colliders.Length} collisions with radius {r}");
        }

        return colliders;
    }

    /// <summary>
    /// Use the DrawSphere or DrawWireSphere static methods of the Gizmos class.
    /// In Game View, make sure to toggle the Gizmos toggle.
    /// <see cref="https://docs.unity3d.com/6000.0/Documentation/Manual/GizmosMenu.html"/>
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }


    /// <summary>
    /// A utility function to clear the Unity Log Console.
    /// </summary>
    public void ClearLog()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
}
