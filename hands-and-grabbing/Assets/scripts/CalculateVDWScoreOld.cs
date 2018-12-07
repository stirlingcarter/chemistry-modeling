using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class CalculateVDWScoreOld : MonoBehaviour {
    public double low_score;
    public Text vdwScore;

    public double max_dist, well_depth,
        center_dist, vdw_dist, A, B,
        pair_calc, total_vdw_calc;
    public SphereCollider collider1, collider2;
    public Collider[] colliderHits;
    int maxCollidersHit;

    // for protein this script is attached to
    MapCollidersToAtoms protein1GameObject;
    public SphereCollider[] protein1CollidersArray;
    int protein1MaxAtoms;

    // for protein this script is NOT attached to
    public GameObject p2GameObject;
    MapCollidersToAtoms protein2GameObject;
    public SphereCollider[] protein2CollidersArray;
    int protein2MaxAtoms;

    string protein2Name;
    LayerMask protein2Mask;

    void Start(){
        protein2Name = "target";
        low_score = 0.0;
        total_vdw_calc = 0.0;
        SetCountText();

        max_dist = 8;
        well_depth = 0.276;

        protein1GameObject = GetComponent<MapCollidersToAtoms>();
        protein1CollidersArray = protein1GameObject.colliders_array;
        protein1MaxAtoms = protein1CollidersArray.Length;

        protein2GameObject = p2GameObject.GetComponent<MapCollidersToAtoms>();
        protein2CollidersArray = protein2GameObject.colliders_array;
        protein2MaxAtoms = protein2CollidersArray.Length;
        colliderHits = new Collider[protein2MaxAtoms];

        protein2Mask = LayerMask.GetMask(protein2Name);
    }

    void SumVDWPairCalc(){
        total_vdw_calc = 0;

        for (int i = 0; i < protein1MaxAtoms; i++)
        {
            collider1 = protein1CollidersArray[i];
            maxCollidersHit = Physics.OverlapSphereNonAlloc
                                     (collider1.center, (float)max_dist, colliderHits, protein2Mask);

            for (int j = 0; j < maxCollidersHit; j++)
            {
                collider2 = (SphereCollider)colliderHits[j];
                center_dist = Vector3.Distance(collider1.center, collider2.center);

                if (center_dist < max_dist)
                {
                    vdw_dist = collider1.radius + collider2.radius;
                    A = Math.Pow(vdw_dist/center_dist, 12);
                    B = Math.Pow(vdw_dist/center_dist, 6);
                    pair_calc = 4 * well_depth * (A - B);
                    total_vdw_calc += pair_calc;
                }
            }
        }
    }

    void SetCountText(){
        string score = total_vdw_calc.ToString("N2");
        vdwScore.text = "Score = " + score;
    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.A)){
            SumVDWPairCalc();
            SetCountText();
        }

        if (total_vdw_calc < low_score){
            low_score = total_vdw_calc;
        }
    }
}