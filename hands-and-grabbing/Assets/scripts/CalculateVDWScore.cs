using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

/* Description:
 * Script accesses the sphere collider array of
 * 1) p1 protein: the script is attached to p1's parent
 * 2) p2 protein: the script is NOT attached to p2 or its parent
 * to retrieve a pair of sphere colliders
 * in order to complete a Van der Waals (VDW) potential energy calculation,
 * AKA a pairwise potential energy calculation between 2 non-bonded atoms (atoms of separate proteins).
 * 
 * Script loops through pairs (ignoring some pairs due to constraints),
 * calculates each pair's VDW score, and sums all of these pairwise VDW scores
 * in order to produce the VDW score for the entire protein interaction.
 * 
 * VDW potential energy calculation: a more negative score is better
 * center_dist = distance between the centers of 2 atoms
 * max_dist = ignore pair if center_dist >= 8 Angstroms
 * vdw_dist = sum of the vdw radii of 2 atoms
 * well_depth = measure of how strongly two particles attract e/o
 *              (currently using C-C well depth of 0.276 for all atom pairs)
 * pair_calc = 4*well_depth*(A-B) where
 *              A = (vdw_dist/center_dist)^12
 *              B = (vdw_dist/center_dist)^6
 * vdw_score = sum of all pair_calc
 */

public class CalculateVDWScore : MonoBehaviour {
    // see Description above
    double center_dist, max_dist,
        vdw_dist, well_depth,
        A, B, pair_calc;
    public double vdw_score, low_score; // accessible to score-related response functions

    // the colliders used in 1 pairwise calculation between 2 nonbonded atoms
    SphereCollider p1Collider, p2Collider;

    // for p1 protein
    // remember: current gameObject script is attached to = parent of p1
    public GameObject p1ChildObject; // ENTERED BY USER
    MapCollidersToAtoms p1MapScriptObject; // need this to access array in mapping script
    SphereCollider[] p1CollidersArray;
    int p1MaxAtoms;

    // for p2 protein
    public GameObject p2ChildObject; // ENTERED BY USER
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

    void Start(){
        low_score = 0;
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

        // for constraints
        p2Mask = LayerMask.GetMask(p2LayerName);
        colliderHits = new Collider[p2MaxAtoms];
    }

    void SumVDWPairCalc(){
        vdw_score = 0;

        for (int i = 0; i < p1MaxAtoms; i++)
        {
            p1Collider = p1CollidersArray[i];
            maxCollidersHit = Physics.OverlapSphereNonAlloc(p1Collider.center, (float)max_dist, colliderHits, p2Mask);

            for (int j = 0; j < maxCollidersHit; j++)
            {
                p2Collider = (SphereCollider)colliderHits[j];
                center_dist = Vector3.Distance(p1Collider.center, p2Collider.center);

                if (center_dist < max_dist)
                {
                    vdw_dist = p1Collider.radius + p2Collider.radius;
                    A = Math.Pow(vdw_dist / center_dist, 12);
                    B = Math.Pow(vdw_dist / center_dist, 6);
                    pair_calc = 4 * well_depth * (A - B);
                    vdw_score += pair_calc;
                }
            }
        }
    }

    void Update() {
        SumVDWPairCalc();
    
        if (vdw_score < low_score){
            low_score = vdw_score;
        }
    }
}