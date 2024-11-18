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
    private GameObject Barriers;
    [SerializeField]
    private float radius = .5f;
    [SerializeField]
    private LayerMask WallLayer;

    List<GameObject> Walls = new List<GameObject>();

    InputAction detectAction, moveAction;
    void Start()
    {
        int cnt = GetWallsInBarrier();
        Debug.Log($"Child count = {cnt}");
        detectAction = InputSystem.actions.FindAction("Click");
        moveAction = InputSystem.actions.FindAction("Move");
    }

    /// <summary>
    /// TODO: Add each Wall in the Barriers to a List of GameObject named Walls.
    /// Walls is a class variable.
    /// The method will return the number of GameObjects in the Walls List.

    /// DO NOT USE Constants. Do NOT Assume that the return value is 4.
    /// The Test program can use data that will add or subtract a wall from 
    /// the barrier.
    /// </summary>
    private int GetWallsInBarrier()
    {
        GameObject[] tagWalls =
        GameObject.FindGameObjectsWithTag("Walls");

        foreach (GameObject tagWall in tagWalls)
        {
            Walls.Add(tagWall);
        }

/*
        for (int i = 0; i < Barriers.transform.childCount; i++)
        {

            if (Barriers.transform.GetChild(i) != null)
            {
                Transform t = Barriers.transform.GetChild(i);
                if (t.childCount > 0)
                {
                    for (int j = 0; j < t.childCount; j++)
                        Walls.Add(t.GetChild(j).gameObject);
                }
                else
                {
                    Walls.Add(t.gameObject);
                }

            }
        }
*/
        return Walls.Count;
    }

    // Update is called once per frame
    void Update()
    {
        if (detectAction.IsPressed())
        {
            ClearLog();
            Collider[] colliders = FindCollisionsWithWalls(Walls, radius);
            ReportColliders(colliders);


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
    /// <param name="walls">The list of GameObjects that are walls.</param>
    /// <param name="r">The radius</param>
    private Collider[] FindCollisionsWithWalls(List<GameObject> walls, float r)
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
