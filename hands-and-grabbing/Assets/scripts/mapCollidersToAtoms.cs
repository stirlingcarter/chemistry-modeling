using UnityEngine;
using System.IO;
using System;
using UnityEditor;

/* Description:
 * Script reads .xyz file (which has been converted to .txt file)
 * and puts the values (e/a row = elem name, x-coor, y-coor, z-coor, vdw radius)
 * from the file in a matrix. Adds a sphere collider to every point (x-coor, y-coor, z-coor) from the file.
 * The sphere colliders are stored in a separate array.
 */

public class MapCollidersToAtoms : MonoBehaviour {
    // the .txt file
    public string xyz_file; // have user type in path to .txt file
    int total_atoms; // in line #1 of txt file
    string protein_name; // in line # 2 of txt file

    // the .txt file matrix
    string[,] xyz_matrix;
    int data_cols;

    // the collider array
    public SphereCollider[] colliders_array;

    // Use this for initialization
    public void Map () {// each row of .txt file => row of matrix => sphere collider
            int row = 0;
            data_cols = 5; // refers to elem name, x-coor, y-coor, z-coor, vdw radius
            float x_coor, y_coor, z_coor, vdw_rad;
            string line;
            string[] split = new string[data_cols];
            SphereCollider sphrCollider;

            using (StreamReader sr = File.OpenText(xyz_file))
            {
                total_atoms = Int32.Parse(sr.ReadLine());
                protein_name = sr.ReadLine();
                xyz_matrix = new string[total_atoms, data_cols];
                colliders_array = new SphereCollider[total_atoms];

                // for each line in file
                while ((line = sr.ReadLine()) != null)
                {
                    split = line.Split(' ');

                    for (int col = 0; col < data_cols; ++col)
                    {
                        xyz_matrix[row, col] = split[col];
                    }

                    // convert string to float
                    x_coor = float.Parse(xyz_matrix[row, 1]);
                    y_coor = float.Parse(xyz_matrix[row, 2]);
                    z_coor = float.Parse(xyz_matrix[row, 3]);
                    vdw_rad = float.Parse(xyz_matrix[row, 4]);


                    // add and store collider component
                    sphrCollider = gameObject.AddComponent<SphereCollider>();
                    sphrCollider.center = new Vector3(-x_coor - 5, y_coor + 2, z_coor + 3);
                    sphrCollider.isTrigger = true;
                    sphrCollider.radius = vdw_rad;
                    colliders_array[row] = sphrCollider;

                    ++row;
                } // end of while loop
            } // end of using
    } // end of Map()
} // end of public class MapCollidersToAtoms

#if UNITY_EDITOR
[CustomEditor(typeof(MapCollidersToAtoms))]
public class MapCollidersToAtomsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var script = (MapCollidersToAtoms)target;

        if (GUILayout.Button("Build") && script.gameObject.GetComponent<SphereCollider>() == null)
        {
            script.Map();
        }
    }
}
#endif