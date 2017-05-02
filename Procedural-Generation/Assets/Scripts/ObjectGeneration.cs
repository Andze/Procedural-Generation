using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGeneration : MonoBehaviour {

	 public void Start()
	 {
	     StartCoroutine(GenerateTree());
	 }
	 IEnumerator GenerateTree()
	 {
	     yield return new WaitForSeconds(.2f);
	     GameObject tree1 = GameObject.FindGameObjectWithTag("Tree");
	     Mesh mesh = GetComponent<MeshFilter>().mesh; //Get the mesh filter of the gameobject we are connected to (Should be the terrain.)
	     Vector3[] vertices = mesh.vertices; // Create an array and reference it to all of the vertices in the terrain. (basically create an array that lists all of the vertices)
	     for (int j = 0; j < vertices.Length; j++) // For every single vertex in array (and by extension the terrain)
	     {
	         int treerange = Random.Range(0, 45);
	         Vector3 position = transform.TransformPoint(vertices[j]); // Get the position of the vertex we're on.
	             if (position.y > 2 && position.y < 10)
	             {
	                     Instantiate(tree1, position, Quaternion.identity);
	             }
	         }
	         yield break;
	 }
}
