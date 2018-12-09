using UnityEngine;
using System.IO;
using System;
using UnityEditor;

    /* Description:
     * CalculateVDWScore.cs has description of calculation.
     * This uses n^2 function to check values of more efficient function in CalculateVDWScore.cs.
     */

public class TestVDWCalc : MonoBehaviour
{   
    // TEST CALC VARIABLES //
    double center_dist, max_dist,
        vdw_dist, well_depth,
        A, B, pair_calc;
    public double vdw_score_2loops, vdw_score_final;

    // the colliders used in 1 pairwise calculation between 2 nonbonded atoms p1 and p2
    SphereCollider p1Collider, p2Collider;

    // for p1 protein
    public GameObject p1ChildObject; // ENTERED BY USER
    public GameObject p1ParentObject; // ENTERED BY USER
    MapCollidersToAtoms p1MapScriptObject; // need this to access array in mapping script
    SphereCollider[] p1CollidersArray;
    int p1MaxAtoms;

    // for p2 protein
    public GameObject p2ChildObject; // ENTERED BY USER
    public GameObject p2ParentObject; // ENTERED BY USER
    MapCollidersToAtoms p2MapScriptObject;
    SphereCollider[] p2CollidersArray;
    int p2MaxAtoms;

    // CONSTRAINTS //
    // p2 layer mask makes sure that you only do p1 collider-p2 collider pairwise calculations
    public string p2LayerName; // ENTERED BY USER
    LayerMask p2Mask;

    // used to fulfill max_dist constraint
    Collider[] colliderHits;
    int maxCollidersHit;

    // TEST CALC VARIABLES //

    // TEST CALC FUNCTIONS //
    public void VDWCalc2Loops()
    {
        max_dist = 8;
        well_depth = 0.276;

        // for p1 protein
        p1MapScriptObject = p1ChildObject.GetComponent<MapCollidersToAtoms>();
        p1CollidersArray = p1MapScriptObject.colliders_array;
        p1MaxAtoms = p1CollidersArray.Length;

        // for p2 protein
        p2MapScriptObject = p2ChildObject.GetComponent<MapCollidersToAtoms>();
        p2CollidersArray = p2MapScriptObject.colliders_array;
        p2MaxAtoms = p2CollidersArray.Length;

        vdw_score_2loops = 0;

        for (int i = 0; i < p1MaxAtoms; i++)
        {
            p1Collider = p1CollidersArray[i];

            for (int j = 0; j < p2MaxAtoms; j++)
            {
                p2Collider = p2CollidersArray[j];

                center_dist = Vector3.Distance(p1Collider.center+p1ParentObject.transform.position, p2Collider.center + p2ParentObject.transform.position);

                if (center_dist < max_dist)
                {
                    vdw_dist = p1Collider.radius + p2Collider.radius;
                    A = Math.Pow(vdw_dist / center_dist, 12);
                    B = Math.Pow(vdw_dist / center_dist, 6);
                    pair_calc = 4 * well_depth * (A - B);
                    vdw_score_2loops += pair_calc;
                } // end of if(p2-collider is close enough) { do pairwise calculation b/w p1-collider and p2-collider}
            } // end of for loop thru array of all p2-colliders
        } // end of for loop thru array of all p1-colliders
    } // end of VDWCalc2Loops()

    public void VDWCalcFinal()
    {
        max_dist = 8;
        well_depth = 0.276;

        // for p1 protein
        p1MapScriptObject = p1ChildObject.GetComponent<MapCollidersToAtoms>();
        p1CollidersArray = p1MapScriptObject.colliders_array;
        p1MaxAtoms = p1CollidersArray.Length;

        // for p2 protein
        p2MapScriptObject = p2ChildObject.GetComponent<MapCollidersToAtoms>();
        p2MaxAtoms = p2MapScriptObject.colliders_array.Length;

        // for constraints
        p2Mask = LayerMask.GetMask(p2LayerName);
        colliderHits = new Collider[p2MaxAtoms];

        vdw_score_final = 0;

        for (int i = 0; i < p1MaxAtoms; i++)
        {
            p1Collider = p1CollidersArray[i];
            maxCollidersHit = Physics.OverlapSphereNonAlloc(p1Collider.center + p1ParentObject.transform.position, (float)max_dist, colliderHits, p2Mask);

            for (int j = 0; j < maxCollidersHit; j++)
            {
                p2Collider = (SphereCollider)colliderHits[j];
                center_dist = Vector3.Distance(p1Collider.center + p1ParentObject.transform.position, p2Collider.center + p2ParentObject.transform.position);

                if (center_dist < max_dist)
                {
                    vdw_dist = p1Collider.radius + p2Collider.radius;
                    A = Math.Pow(vdw_dist / center_dist, 12);
                    B = Math.Pow(vdw_dist / center_dist, 6);
                    pair_calc = 4 * well_depth * (A - B);
                    vdw_score_final += pair_calc;
                } // end of if(p2-collider is close enough) { do pairwise calculation b/w p1-collider and p2-collider}
            } // end of for loop thru array of close enough p2-colliders
            Array.Clear(colliderHits, 0, p2MaxAtoms);
        } // end of for loop thru array of all p1-colliders
    } // end of VDWCalcFinal()

    // TEST CALC FUNCTIONS //
} // end of public class TestVDWCalc()

#if UNITY_EDITOR
[CustomEditor(typeof(TestVDWCalc))]
public class TestVDWCalcEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var script = (TestVDWCalc)target;

        if (GUILayout.Button("VDW Calc using 2 For Loops"))
        {
            script.VDWCalc2Loops();
        }

        if (GUILayout.Button("VDW Calc for Project Final"))
        {
            script.VDWCalcFinal();
        }
    }
}
#endif