using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

/* Description:
 * Script accesses the sphere collider array of p1 protein&p2 protein
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

public class SpheresVDWScore : MonoBehaviour
{
    [SerializeField] // can be seen in Inspector but not changed
    private double vdw_score, low_score, vdw_score2loops, low_score2loops;
    public double max_dist, well_depth; 

    public Color green, clear, red;
    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

    // for p1 protein
    public GameObject p1ParentObject; // ENTERED BY USER
    public GameObject p1ChildObject; // ENTERED BY USER
    public SphereCollider[] p1CollidersArray;
    int p1MaxAtoms;

    // for p2 protein
    public GameObject p2ParentObject; // ENTERED BY USER
    public GameObject p2ChildObject; // ENTERED BY USER
    public SphereCollider[] p2CollidersArray;
    int p2MaxAtoms;

    // p2 layer mask makes sure that you only do
    // p1 collider-p2 collider pairwise calculations
    public string p2LayerName; // ENTERED BY USER
    LayerMask p2Mask;

    // used to fulfill max_dist constraint
    public Collider[] colliderHits;
    int maxCollidersHit;

    void Awake()
    {
        _propBlock = new MaterialPropertyBlock();

        low_score = 0;
        low_score2loops = 0;
        max_dist = 8;
        well_depth = 0.276;

        // for p1 protein
        p1CollidersArray = p1ChildObject.GetComponentsInChildren<SphereCollider>();
        p1MaxAtoms = p1CollidersArray.Length;

        // for p2 protein
        p2CollidersArray = p2ChildObject.GetComponentsInChildren<SphereCollider>();
        p2MaxAtoms = p2CollidersArray.Length;

        // for constraints
        p2Mask = LayerMask.GetMask(p2LayerName);
        colliderHits = new Collider[p2MaxAtoms];
    } // end of Start()

    // accessible to score-related response functions
    public double CurrentVDW { get { return vdw_score; } }
    public double LowestVDW { get { return low_score; } }
    public double CurrentVDW2 { get { return vdw_score2loops; } }
    public double LowestVDW2 { get { return vdw_score2loops; } }

    // Uses SphereOverlapNonAlloc for efficiency
    void VDWCalc()
    {
        vdw_score = 0;
        double center_dist, vdw_dist, A, B, pair_calc;
        SphereCollider p1Collider, p2Collider;
        // CHANGE CODE/CALCULATION ON THIS TO CHECK
        // SphereCollider[] p1CollidersArray;
        // SphereCollider[] p2CollidersArray;
        // p1CollidersArray = p1ChildObject.getComponents<SphereCollider>();
        // p2CollidersArray = p2ChildObject.getComponents<SphereCollider>();
        // CHANGE CODE/CALCULATION ON THIS TO CHECK
        for (int i = 0; i < p1MaxAtoms; i++)
        {
            p1Collider = p1CollidersArray[i];
            // CHANGE CODE/CALCULATION ON THIS TO CHECK
            maxCollidersHit = Physics.OverlapSphereNonAlloc(p1Collider.center + p1ParentObject.transform.position, (float)max_dist, colliderHits, p2Mask);
            // CHANGE CODE/CALCULATION ON THIS TO CHECK

            for (int j = 0; j < maxCollidersHit; j++)
            {
                p2Collider = (SphereCollider)colliderHits[j];
                // CHANGE CODE/CALCULATION ON THIS TO CHECK
                center_dist = Vector3.Distance(p1Collider.center + p1ParentObject.transform.position,
                                               p2Collider.center + p2ParentObject.transform.position);
                // CHANGE CODE/CALCULATION ON THIS TO CHECK

                if (center_dist < max_dist)
                {
                    vdw_dist = p1Collider.radius + p2Collider.radius;
                    A = Math.Pow(vdw_dist / center_dist, 12);
                    B = Math.Pow(vdw_dist / center_dist, 6);
                    pair_calc = 4 * well_depth * (A - B);

                    // call color change function
                    ChangeColor(p1Collider.gameObject, pair_calc);

                    vdw_score += pair_calc;
                } // end of if(p2-collider is close enough) { do pairwise calculation b/w p1-collider and p2-collider}
            } // end of for loop thru array of close enough p2-colliders
        } // end of for loop thru array of all p1-colliders
    } // end of VDWCalc()

    // Uses 2 for loops to check
    void VDWCalc2Loops()
    {
        vdw_score2loops = 0;
        double center_dist, vdw_dist, A, B, pair_calc;
        SphereCollider p1Collider, p2Collider;
        // CHANGE CODE/CALCULATION ON THIS TO CHECK
        // SphereCollider[] p1CollidersArray;
        // SphereCollider[] p2CollidersArray;
        // p1CollidersArray = p1ChildObject.getComponents<SphereCollider>();
        // p2CollidersArray = p2ChildObject.getComponents<SphereCollider>();
        // CHANGE CODE/CALCULATION ON THIS TO CHECK
        for (int i = 0; i < p1MaxAtoms; i++)
        {
            p1Collider = p1CollidersArray[i];
            for (int j = 0; j < p2MaxAtoms; j++)
            {
                p2Collider = p2CollidersArray[j];
                // CHANGE CODE/CALCULATION ON THIS TO CHECK
                center_dist = Vector3.Distance(p1Collider.gameObject.transform.position,
                                               p2Collider.gameObject.transform.position);
                // CHANGE CODE/CALCULATION ON THIS TO CHECK
                if (center_dist < max_dist)
                {
                    vdw_dist = p1Collider.radius + p2Collider.radius;
                    A = Math.Pow(vdw_dist / center_dist, 12);
                    B = Math.Pow(vdw_dist / center_dist, 6);
                    pair_calc = 4 * well_depth * (A - B);

                    

                    vdw_score2loops += pair_calc;

                    // call color change function
                    //ChangeColor(p1Collider.gameObject, pair_calc);
                }
            }
        }
    } // end of VDWCalc2Loops()
    
    public void ChangeColor(GameObject sphere, double pair_calc){
        _renderer = sphere.GetComponent<Renderer>();

        // Get the current value of the material properties in the renderer.
        _renderer.GetPropertyBlock(_propBlock);

        if (pair_calc == 0) { _propBlock.SetColor("_Color", clear); }
        else if (pair_calc < 0) { _propBlock.SetColor("_Color", green); }
        else { _propBlock.SetColor("_Color", red); }

        // Apply the edited values to the renderer.
        _renderer.SetPropertyBlock(_propBlock);
    }

    void Update()
    {
        //VDWCalc();
        VDWCalc2Loops();

        if (vdw_score < low_score)
        {
            low_score = vdw_score;
        }

        if (vdw_score2loops < low_score2loops)
        {
            low_score2loops = vdw_score2loops;
        }
    } // end of Update()
} // end of public class SpheresVDWScore