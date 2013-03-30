// M2HCullingAuto: Unity Occlusion System by M2H ( http://m2h.nl/unity/ )
// If you use this for a commercial project I ask you to donate an amount
// between 5 - 100 Euros to support@M2H.nl (Paypal). After this donation 
// you're free to use this system in any of your commercial projects.
// Lastly you are not allowed to sell this script or remove this license note.
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public enum CullingCameraOption_OutsideAllAreas_Auto { PrintErrorHideAllGroups, PrintErrorShowAllGroups, HideAllGroups, ShowAllGroups }

public class CullingCamera_Auto : MonoBehaviour
{

    private CullingArea_Auto[] cullingAreas;
    private CullingAreaSettings_Auto[] currentCameraSettingsForAllAreas;
    private Transform thisTransform;


    public CullingCameraOption_OutsideAllAreas_Auto outsideAllAreas;
    public bool disableCullingForTest = false;

    void Awake()
    {
        Debug.Log("Started M2HCulling system: Automode");
        //Cache
        thisTransform = transform;

        if (disableCullingForTest)
        {
            Debug.LogWarning("Culling has been disabled!");
            return;
        }

        //Setup culling areas
        cullingAreas = (CullingArea_Auto[])FindObjectsOfType(typeof(CullingArea_Auto));

        //Setup groups
        CullingArea_Auto[] allAreas = (CullingArea_Auto[])FindObjectsOfType(typeof(CullingArea_Auto));
        currentCameraSettingsForAllAreas = new CullingAreaSettings_Auto[allAreas.Length];
        int j = 0;
        foreach (CullingArea_Auto aGroup in allAreas)
        {
            CullingAreaSettings_Auto myGroup = new CullingAreaSettings_Auto();
            myGroup.script = aGroup;
            currentCameraSettingsForAllAreas[j] = myGroup;
            j++;
        }

    }

    void OnApplicationQuit()
    {
        if (Application.isEditor)
        {
            //Disable culling on all areas
            foreach (CullingArea_Auto entry in cullingAreas)
            {
                entry.DoShowRenderers();
            }
        }
    }

    private ArrayList lastAreaList =new ArrayList();
    private Vector3 lastPos = Vector3.zero;

    bool SameAreaList(ArrayList oldList, ArrayList newList)
    {
        if (oldList.Count != newList.Count)
        {
            return false;
        }
        for (int i = 0; i < oldList.Count;i++ )
        {
            if (oldList[i] != newList[i])
            {
                return false;
            }
        }
        return true;
    }


    void Update()
    {
        if (disableCullingForTest || calculatingCulling)        
            return;
        
        if (Vector3.Distance(lastPos, thisTransform.position) < 0.05f)       
            return; //Skip calculations..we didn't even move
        
        lastPos = thisTransform.position;

        // Calculate the culling mask
        // Go through all areas and take the union of all visible layers

        //Per default, we cannot see any cullinggroup; hide all
        foreach (CullingAreaSettings_Auto aGroup in currentCameraSettingsForAllAreas)
        {
            aGroup.cullingOptions = CullingOptions_Auto._;
        }        
        //Now, check what areas we can enable: feed the master list
        Vector3 cameraPos = thisTransform.position;
        bool weAreInAtLeastOneArea = false;
        ArrayList thisAreaList = new ArrayList();
        foreach (CullingArea_Auto area in cullingAreas)
        {
            Transform areaTransform = area.transform;
            Vector3 relative = areaTransform.InverseTransformPoint(cameraPos);
            Bounds bounds = new Bounds(Vector3.zero, new Vector3(1,1,1));
            if (bounds.Contains(relative))
            {
                //Enable all areas that you're allowed to see here			
                thisAreaList.Add(area);              
            }
        }

        if (SameAreaList(lastAreaList, thisAreaList))
        {
            return; //Skip this frame, we're still in the same area
        }
        lastAreaList = thisAreaList;

        foreach (CullingArea_Auto area in thisAreaList)
        {
            weAreInAtLeastOneArea = true;
            EnableGroupsFromCollider(area);
        }

        StartCoroutine(RecalculateCulling(weAreInAtLeastOneArea));   
    }

    private bool calculatingCulling = false;
    IEnumerator RecalculateCulling(bool weAreInAtLeastOneArea)
    {
        calculatingCulling = true;
        Dictionary<Renderer, bool> rendererSettings = new Dictionary<Renderer, bool>();

        //Disable all  disabled areas using the new master list
        //After which we enable the other areas to take the right union
        foreach (CullingAreaSettings_Auto liveCullGroup in currentCameraSettingsForAllAreas)
        {
            if (liveCullGroup.cullingOptions == CullingOptions_Auto._)
            {
                //Add all renderers to the list
                MyCustomIENumeratorOutput output = new MyCustomIENumeratorOutput();
                yield return StartCoroutine(liveCullGroup.script.GetHideRenderers(output));
                if (output.GetOutput() != null)
                {
                    SetRendererSetting((ArrayList)output.GetOutput(), rendererSettings, false);
                }
            }
        }
        //Enable all enabled areas
        foreach (CullingAreaSettings_Auto liveCullGroup in currentCameraSettingsForAllAreas)
        {
            if (liveCullGroup.cullingOptions == CullingOptions_Auto.Show || (!weAreInAtLeastOneArea && (outsideAllAreas == CullingCameraOption_OutsideAllAreas_Auto.PrintErrorShowAllGroups || outsideAllAreas == CullingCameraOption_OutsideAllAreas_Auto.ShowAllGroups)))
            {
                
                MyCustomIENumeratorOutput output = new MyCustomIENumeratorOutput();
                yield return StartCoroutine(liveCullGroup.script.GetShowRenderers(output));
                if (output.GetOutput() != null)
                {
                    SetRendererSetting((ArrayList)output.GetOutput(), rendererSettings, true);
                }
            }
        }
        //Final pass, remove the HIDDEN renderers
        foreach (CullingAreaSettings_Auto liveCullGroup in currentCameraSettingsForAllAreas)
        {
            if (liveCullGroup.cullingOptions == CullingOptions_Auto.AlwaysHide)
            {
                MyCustomIENumeratorOutput output = new MyCustomIENumeratorOutput();
                yield return StartCoroutine(liveCullGroup.script.GetHideRenderers(output));
                if (output.GetOutput() != null)
                {
                    SetRendererSetting((ArrayList)output.GetOutput(), rendererSettings, false);
                }
            }
        }

        //Apply actual culling
        foreach (KeyValuePair<Renderer, bool> kvp in rendererSettings)
        {
            kvp.Key.enabled = kvp.Value;
        }

        //We aren't in any area! Show error?
        if (!weAreInAtLeastOneArea && (outsideAllAreas == CullingCameraOption_OutsideAllAreas_Auto.PrintErrorHideAllGroups || outsideAllAreas == CullingCameraOption_OutsideAllAreas_Auto.PrintErrorShowAllGroups))
        {
            Debug.LogError("CullingError: OUTSIDE of all  CullingAreas at " + transform.position);
        }
        calculatingCulling = false;
    }

    void SetRendererSetting(ArrayList rens, Dictionary<Renderer, bool> dict, bool value)
    {
        foreach (Renderer ren in rens)
        {
            if (dict.ContainsKey(ren))            
                dict[ren] = value;            
            else            
                dict.Add(ren, value);            
        }     
    }




    void EnableGroupsFromCollider(CullingArea_Auto area)
    {
        if (disableCullingForTest)        
            return;
        

        foreach (CullingAreaSettings_Auto cullGroup in area.areaList)
        {
            if (cullGroup.cullingOptions == CullingOptions_Auto.Show)
            {
                foreach (CullingAreaSettings_Auto liveCullGroup in currentCameraSettingsForAllAreas)
                {
                    if (liveCullGroup.script == cullGroup.script)
                    {
                        if (!(liveCullGroup.cullingOptions == CullingOptions_Auto.AlwaysHide))
                        {
                            liveCullGroup.cullingOptions = CullingOptions_Auto.Show;
                        }
                        break;
                    }
                }
            }
            else if (cullGroup.cullingOptions == CullingOptions_Auto.AlwaysHide)
            {
                foreach (CullingAreaSettings_Auto liveCullGroup in currentCameraSettingsForAllAreas)
                {
                    if (liveCullGroup.script == cullGroup.script)
                    {
                        liveCullGroup.cullingOptions = CullingOptions_Auto.AlwaysHide;
                        break;
                    }
                }
            }
        }
    }



}

public class MyCustomIENumeratorOutput
{
    private object output;
    private bool isDone = false;
    private bool failed = false;

    public void SetOutput(object obj)
    {
        output = obj;
        isDone = true;
    }
    public object GetOutput()
    {
        if (!IsDone())
        {
            Debug.LogError("MyCustomIENumeratorOutput: GetOuput but is not done yet!");
        }
        return output;
    }
    public bool IsDone()
    {
        return isDone;
    }
    public void SetFailed()
    {
        failed = true;
    }
    public bool Failed()
    {
        return failed;
    }
}