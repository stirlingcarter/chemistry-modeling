using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

/* Description:
 * Script reads .xyz file (which has been converted to .txt file) and put the values
 * in the file in a matrix. Adds a sphere collider to every point (x,y,z) from the file.
 * The sphere colliders are stored in a separate array.
 */

[ExecuteInEditMode]
public class mapCollidersToAtoms : MonoBehaviour {
    public string xyz_file; // have user type in path to .xyz/.txt file
    int total_atoms; // in line #1 of txt file
    string protein_name; // in line # 2 of txt file
    int data_cols;
    string[,] xyz_matrix;
    SphereCollider[] colliders_array;

    // Use this for initialization
    void Awake () {
        data_cols = 4; // refers to elem name, x-coor, y-coor, z-coor
        string line;
        string[] split = new string[data_cols];
        int row = 0;
        float x_coor;
        float y_coor;
        float z_coor;
        SphereCollider sphrCollider;

        using (StreamReader sr = File.OpenText(xyz_file)){
            total_atoms = Int32.Parse(sr.ReadLine());
            protein_name = sr.ReadLine();
            xyz_matrix = new string[total_atoms, data_cols];
            colliders_array = new SphereCollider[total_atoms];

            // for each line in file
            while ((line = sr.ReadLine()) != null){
                split = line.Split(' ');

                for (int col = 0; col<data_cols; ++col){
                    xyz_matrix[row, col] = split[col];
                }

                // convert string to float
                // more precision using ToDouble
                x_coor = (float)Convert.ToDouble(xyz_matrix[row, 1]);
                y_coor = (float)Convert.ToDouble(xyz_matrix[row, 2]);
                z_coor = (float)Convert.ToDouble(xyz_matrix[row, 3]);

                // add and store collider component
                sphrCollider = gameObject.AddComponent<SphereCollider>();
                sphrCollider.center = new Vector3(x_coor, y_coor, z_coor);
                colliders_array[row] = sphrCollider;

                ++row;
            } // end of while loop
        } // end of using
    } // end of Start()
}