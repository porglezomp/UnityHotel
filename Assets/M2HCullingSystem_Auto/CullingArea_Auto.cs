// M2HCullingAuto: Unity Occlusion System by M2H ( http://m2h.nl/unity/ )
// If you use this for a commercial project I ask you to donate an amount
// between 5 - 100 Euros to support@M2H.nl (Paypal). After this donation 
// you're free to use this system in any of your commercial projects.
// Lastly you are not allowed to sell this script or remove this license note.
using UnityEngine;
using System.Collections;
using System.Collections.Generic;//List<>

[System.Serializable]
public enum CullingOptions_Auto { _, Show, AlwaysHide, }

[System.Serializable]
public class CullingAreaSettings_Auto
{
    public CullingArea_Auto script;
    public CullingOptions_Auto cullingOptions;
}


public class CullingArea_Auto : MonoBehaviour
{
    //EDITOR SPECIFIC
    [SerializeField]
    public List<CullingAreaSettings_Auto> areaList;
    public Color gizmoColor;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;  
        Gizmos.DrawCube(transform.position, transform.localScale);
    }

    public int GetObjectCount()
    {
        if (myRenderers==null)
        {
            return -1;
        }
        return myRenderers.Count;
    }

    //RUNTIME SPECIFIC
    //private bool isVisible = true;
    private ArrayList myRenderers;

    IEnumerator Start()
    {
        yield return 0; //Wait for CombineChildren to take effect
        SetupVars(true);
    }



    private int lastTotalRenderers;
    public void SetupVars(bool forceRecalculate)
    {
        GameObject cullingObjects = GameObject.Find("CullingObjects");
        if (!cullingObjects)
        {
            Debug.LogError("There's no GameObject called -CullingObjects-. Couldn't setup culling!");
            return;
        }
        Component[] allRenderers = cullingObjects.GetComponentsInChildren<Renderer>();
        if (forceRecalculate || myRenderers == null || allRenderers.Length != lastTotalRenderers)
        {
            lastTotalRenderers = allRenderers.Length;
            myRenderers = new ArrayList();

            foreach (Renderer aRend in allRenderers)
            {
                Bounds b1 = new Bounds(transform.position, transform.localScale);
                Bounds b2 = aRend.bounds;
                if (BoundsHitSomeWhere(b1, b2))
                {
                    myRenderers.Add(aRend);
                }                
                
            }
        }
    }


    //test two bounds
    public static bool BoundsHitSomeWhere(Bounds b1, Bounds b2)
    {
        //B1 is all LEFT of B2  OR b1 is RIGHT of B2
        if ((b1.center.x + b1.extents.x) < (b2.center.x - b2.extents.x) || (b1.center.x - b1.extents.x) > (b2.center.x + b2.extents.x))
        {
            return false;
        }//B1 is HIGHER than b2 OR b1 is LOWER than B2
        else if ((b1.center.y + b1.extents.y) < (b2.center.y - b2.extents.y) || (b1.center.y - b1.extents.y) > (b2.center.y + b2.extents.y))
        {
            return false;
        }//B1 is BEFORE/AFTER b2
        else if ((b1.center.z + b1.extents.z) < (b2.center.z - b2.extents.z) || (b1.center.z - b1.extents.z) > (b2.center.z + b2.extents.z))
        {
            return false;
        }
        else
        {
            //OVERLAPPING bounds  OR INSIDE BOUNDS!
            return true;
        }
    }

    //Hide the objects
    public IEnumerator GetHideRenderers(MyCustomIENumeratorOutput output)
    {
        if (myRenderers == null)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                SetupVars(false);
            }
            yield return 0;
        }
        output.SetOutput((object)myRenderers);
    }

    //Hide the objects
    public IEnumerator DoHideRenderers()
    {
        if (myRenderers == null)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                SetupVars(false);
            }
            yield return 0;
        }
        foreach (Renderer myRenderer in myRenderers)
        {
           myRenderer.enabled = false;
        }
    }


    //Show the objects
    public IEnumerator GetShowRenderers(MyCustomIENumeratorOutput output)
    {
        if (myRenderers == null)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                SetupVars(false);
            }
            yield return 0;
        }
        output.SetOutput((object)myRenderers);        
    }

    public IEnumerator DoShowRenderers()
    {
        if (myRenderers == null)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                SetupVars(false);
            }
            yield return 0;
        }
        foreach (Renderer myRenderer in myRenderers)
        {
            myRenderer.enabled = true;
        }
    }



}