// M2HCullingAuto: Unity Occlusion System by M2H ( http://m2h.nl/unity/ )
// If you use this for a commercial project I ask you to donate an amount
// between 5 - 100 Euros to support@M2H.nl (Paypal). After this donation 
// you're free to use this system in any of your commercial projects.
// Lastly you are not allowed to sell this script or remove this license note.
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic; //List<>


[CustomEditor(typeof(CullingArea_Auto))]
public class CullingAreaEditor_Auto : Editor
{

    CullingAreaSettings_Auto[] currentSettingsForAllAreas;

	//SETUP
	void Awake()
	{
        //Have Unity notify us if we're switching play mode
		EditorApplication.playmodeStateChanged = SwitchingToPlay;
	}
    

	void OnEnable()
	{
        //Load all groups
        CullingArea_Auto[] allAreas = (CullingArea_Auto[])FindObjectsOfType(typeof(CullingArea_Auto));
        currentSettingsForAllAreas = new CullingAreaSettings_Auto[allAreas.Length];
        int j = 0;
        foreach (CullingArea_Auto aGroup in allAreas)
        {
            CullingAreaSettings_Auto myGroup = new CullingAreaSettings_Auto();
            myGroup.script = aGroup;
            currentSettingsForAllAreas[j] = myGroup;
            j++;
        }


		ReloadAllAreas();
        if (showOnlyThisArea)
        {
            ShowOnlyThisArea();
        }
	}
    
	void OnDisable()
	{
        if (showObjectsOff != null || showOnlyThisArea)
		{
			EnableAllObjects();
		}
    }
    
	void SwitchingToPlay()
	{
        if (showObjectsOff != null || showOnlyThisArea)
		{
			EnableAllObjects();
		}
	}

    
	void ReloadAllAreas()
	{
        CullingArea_Auto theCullingArea = (CullingArea_Auto)target;

        //Setup default color
        if (theCullingArea.gizmoColor == new Color(0, 0, 0, 0))
        {
            theCullingArea.gizmoColor = new Color(1, 0, 0, 0.5f);
        }

        //Set up groups
        CullingArea_Auto[] groups = (CullingArea_Auto[])FindObjectsOfType(typeof(CullingArea_Auto));
        List<CullingAreaSettings_Auto> copyGroupList = theCullingArea.areaList; //Old copy    
        theCullingArea.areaList = new List<CullingAreaSettings_Auto>(); //New array to account for growth/shrink
        foreach (CullingArea_Auto aGroup in groups)
		{
			AddIfNotExists(aGroup, copyGroupList);
		}
        theCullingArea.areaList.Sort(new CullingAreaSettingsSorter_Auto());

        EditorUtility.SetDirty(target);
        EditorUtility.SetDirty(theCullingArea);
	}



	//OTHERS
    private CullingAreaSettings_Auto showObjectsOff = null;
    private static bool showOnlyThisArea = false;

    public override void OnInspectorGUI()
	{
        CullingArea_Auto theCullingArea = (CullingArea_Auto)target;

        theCullingArea.gizmoColor = EditorGUILayout.ColorField( theCullingArea.gizmoColor );

        if (showOnlyThisArea)
        {
            if (GUILayout.Button("Showing only this area"))
            {
                showOnlyThisArea = !showOnlyThisArea;
            }
        }
        else
        {
            if (GUILayout.Button("Showing everything"))
            {
                showOnlyThisArea = !showOnlyThisArea;
                ShowOnlyThisArea();
            }
        }

		EditorGUILayout.LabelField(" All culling areas: ", "");
        foreach (CullingAreaSettings_Auto entry in theCullingArea.areaList)
		{
			if (entry != null && entry.script != null)
			{
				EditorGUILayout.BeginHorizontal();

                GUI.SetNextControlName("Group" + entry.script.name);
                CullingOptions_Auto valueBefore = entry.cullingOptions;

                if (entry.script != theCullingArea)
                {
                    entry.cullingOptions = (CullingOptions_Auto)EditorGUILayout.EnumPopup("\t" + entry.script.name, (System.Enum)entry.cullingOptions);
                    if (entry.cullingOptions != valueBefore)
                    {
                        ChangedSetting(entry);
                    }
                }
                else
                {
                    GUILayout.Label("\t\t"+entry.script.name);
                }

				EditorGUILayout.EndHorizontal();

            
				EditorGUILayout.BeginHorizontal();

				bool debugShow = (showObjectsOff == entry);

                

				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				//EditorGUILayout.Space();
	
                bool debugShowAfter = (GUI.GetNameOfFocusedControl() == "Group" + entry.script.name);
                if (debugShowAfter != debugShow)
				{
					if (showObjectsOff == entry)
					{
						showObjectsOff = null;
                        if (showOnlyThisArea)
                        {
                            ShowOnlyThisArea();
                        }
                        else
                        {
                            EnableAllObjects();
                        }
					}
					else
					{
						showObjectsOff = entry;
                        ShowOnlyGroups(showObjectsOff);
					}

				}
                if (GUI.GetNameOfFocusedControl() == "" && showObjectsOff!=null)
                {
                    if (showOnlyThisArea)
                    {
                        ShowOnlyThisArea();
                    }
                    else
                    { 
                        EnableAllObjects();
                    }
                    showObjectsOff = null;                     
                }

                

				//EditorGUILayout.Space();
				EditorGUILayout.EndHorizontal();
			}
		}

        GUILayout.Label("Total objects in this area: " + theCullingArea.GetObjectCount());

		if (GUI.changed)
		{
            EditorUtility.SetDirty(target);
            EditorUtility.SetDirty(theCullingArea);
		}
	}

    
	void EnableAllObjects()
	{
        CullingArea_Auto cullArea = ((CullingArea_Auto)target);
        foreach (CullingAreaSettings_Auto entry in cullArea.areaList)
		{
			if (entry!=null)
			{
                CullingArea_Auto culGroup = entry.script;
                culGroup.SetupVars(true);
                cullArea.StartCoroutine(culGroup.DoShowRenderers());
			}
		}
	}

    //Show only the objects in the area we've passed!
    void ShowOnlyGroups(CullingAreaSettings_Auto showOnlyGroup)
	{
        //Disable all
        CullingArea_Auto cullArea = ((CullingArea_Auto)target);
        foreach (CullingAreaSettings_Auto entry in cullArea.areaList)
        {
            if (entry != null)
            {
                entry.script.SetupVars(false);
                cullArea.StartCoroutine(entry.script.DoHideRenderers());                
            }
        }

        cullArea.StartCoroutine(showOnlyGroup.script.DoShowRenderers());

	}


    void ShowOnlyThisArea()
    {
        if (!showOnlyThisArea) { return; }
        CullingArea_Auto cullArea = ((CullingArea_Auto)target);

         //Disable ALL
        foreach (CullingAreaSettings_Auto entry in cullArea.areaList)
        {
            cullArea.StartCoroutine(entry.script.DoHideRenderers());
        }

        //Enable a the selected areas...
        foreach (CullingAreaSettings_Auto entry in cullArea.areaList)
        {
            if (entry != null)
            {
                entry.script.SetupVars(true);
                if (entry.cullingOptions == CullingOptions_Auto.Show)
                {
                    //Enabled
                    cullArea.StartCoroutine(entry.script.DoShowRenderers());
                }
            }
        }

        //Disable all NEVER-SHOW areas
        foreach (CullingAreaSettings_Auto entry in cullArea.areaList)
        {
            if (entry != null)
            {
                entry.script.SetupVars(true);
                if (entry.cullingOptions == CullingOptions_Auto.AlwaysHide)
                {
                    //Enabled
                    cullArea.StartCoroutine(entry.script.DoHideRenderers());
                }
            }
        }

    }


    void ChangedSetting(CullingAreaSettings_Auto mainGroup)
	{
        if (showOnlyThisArea)
        {
            ShowOnlyThisArea();
        }
	}


    void AddIfNotExists(CullingArea_Auto anAreaScript, List<CullingAreaSettings_Auto> oldList)
	{
        CullingArea_Auto cullArea = ((CullingArea_Auto)target);
        //Check if it is not there yet
        foreach (CullingAreaSettings_Auto entry in cullArea.areaList)
		{
			if (entry!=null && entry.script == anAreaScript)
			{
				//This item was already added
				return;
			}
		}


		//Check if we know values of the OLD list
        bool foundOldValue = false;
        if (oldList != null && oldList.Count>0)
        {
            foreach (CullingAreaSettings_Auto entry in oldList)
            {                
                if (entry != null && entry.script == anAreaScript)
                {
                    cullArea.areaList.Add( entry );                    
                    if (anAreaScript == cullArea)
                    {
                        entry.cullingOptions = CullingOptions_Auto.Show;//Always show own group
                    }
                    foundOldValue = true;
                    break;
                }
            }
        }

        if (!foundOldValue)
        {
            CullingAreaSettings_Auto newCullgroup = new CullingAreaSettings_Auto();
            newCullgroup.script = anAreaScript;
            cullArea.areaList.Add(newCullgroup);

            if (anAreaScript == cullArea)
            {
                newCullgroup.cullingOptions = CullingOptions_Auto.Show;//Always show own group
            }
        }

	}
}

public class CullingAreaSettingsSorter_Auto : IComparer<CullingAreaSettings_Auto>
{
    public int Compare(CullingAreaSettings_Auto x, CullingAreaSettings_Auto y)
    {
        return (x as CullingAreaSettings_Auto).script.name.CompareTo((y as CullingAreaSettings_Auto).script.name);
    }
}