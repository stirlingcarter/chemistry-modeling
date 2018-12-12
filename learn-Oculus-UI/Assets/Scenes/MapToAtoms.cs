using UnityEngine;
using System.IO;
using System;
using UnityEditor;

/* Description:
 * 
 * Script reads .xyz file (which has been converted to .txt file)
 * where, after line1 and line2, e/a line = "elem name, x-coor, y-coor, z-coor, vdw radius".
 * 
 * MapColliders(): Adds a sphere collider to every point (x-coor, y-coor, z-coor) from the file.
 * MapSpheres(): Adds a sphere to every point (x-coor, y-coor, z-coor) from the file.
 */

public class MapToAtoms : MonoBehaviour {
    public string xyz_file; // have user type in path to .txt file

    public void MapColliders (){
        SphereCollider colliderToAdd; // ref to sphere collider
        float x_coor, y_coor, z_coor, vdw_rad;
        string line;
        string[] split = new string[5];

        using (StreamReader sr = File.OpenText(xyz_file))
        {
            sr.ReadLine(); sr.ReadLine(); // skip line #1&2

            // for each line in file
            while ((line = sr.ReadLine()) != null)
            {
                split = line.Split(' ');

                // convert string to float
                x_coor = -float.Parse(split[1])-4.3F;
                y_coor = float.Parse(split[2])+2.8F;
                z_coor = float.Parse(split[3])+3.6F;
                vdw_rad = float.Parse(split[4]);

                // add collider component
                colliderToAdd = gameObject.AddComponent<SphereCollider>();
                colliderToAdd.center = new Vector3(x_coor, y_coor, z_coor);
                colliderToAdd.isTrigger = true;
                colliderToAdd.radius = vdw_rad;
            } // end of while loop
        } // end of using
    } // end of MapColliders()
    
    public void MapSpheres(){
        GameObject objToSpawn; // ref to game object
        SphereCollider collider; // ref to sphere collider
        int row = 0; // # atom for sphere name
        float x_coor, y_coor, z_coor, vdw_rad;
        Vector3 spherePos; // center of sphere (x,y,z)
        string line;
        string[] split = new string[5];

        using (StreamReader sr = File.OpenText(xyz_file))
        {
            sr.ReadLine(); sr.ReadLine(); // skip line #1&2

            // for each line in file
            while ((line = sr.ReadLine()) != null)
            {
                split = line.Split(' ');

                // convert string to float
                x_coor = -float.Parse(split[1]) - 4.3F;
                y_coor = float.Parse(split[2]) + 2.8F;
                z_coor = float.Parse(split[3]) + 3.6F;
                vdw_rad = float.Parse(split[4]);
                spherePos = new Vector3(x_coor, y_coor, z_coor);

                objToSpawn = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                objToSpawn.transform.position = spherePos;
                objToSpawn.name = ("Collider/Atom #" + row.ToString() + " " + spherePos.ToString());
                collider = objToSpawn.GetComponent<SphereCollider>();
                collider.isTrigger = true;
                collider.radius = vdw_rad;

                ++row;
            } // end of while loop
        } // end of using
    } // end of MapSpheres()


} // end of public class MapToAtoms

#if UNITY_EDITOR
[CustomEditor(typeof(MapToAtoms))]
public class MapToAtomsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var script = (MapToAtoms)target;

        if (GUILayout.Button("Build Colliders")
            && script.gameObject.GetComponent<SphereCollider>() == null)
        {
            script.MapColliders();
        }
        
        if (GUILayout.Button("Build Spheres"))
        {
            script.MapSpheres();
        }
    }
}
#endif