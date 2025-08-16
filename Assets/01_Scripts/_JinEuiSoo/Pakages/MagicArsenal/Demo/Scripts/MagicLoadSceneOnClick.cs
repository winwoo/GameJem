using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace MagicArsenal
{

public class MagicLoadSceneOnClick : MonoBehaviour
{
	public bool GUIHide = false;
	public bool GUIHide2 = false;
	public bool GUIHide3 = false;
	
    public void LoadSceneProjectiles()
    {
    }
    public void LoadSceneSprays()
    {
    }
    public void LoadSceneAura()
    {
    }
    public void LoadSceneModular()
    {
    }
    public void LoadSceneShields2()
    {
    }
    public void LoadSceneShields()
    {
    }
    public void LoadSceneSphereBlast()
    {
    }
    public void LoadSceneEnchant()
    {
    }
    public void LoadSceneSlash()
    {
    }
    public void LoadSceneCharge()
    {
    }
    public void LoadSceneCleave()
    {
    }
    public void LoadSceneAura2()
    {
    }
    public void LoadSceneWalls()
    {
    }
	public void LoadSceneBeams()
    {
    }
	public void LoadSceneMeshGlow()
    {
    }
	public void LoadScenePillarBlast()
    {
    }
	public void LoadSceneAura3()
    {
    }
	public void LoadSceneAuraCast()
    {
    }
	public void LoadSceneRain()
    {
    }
	public void LoadSceneAOE()
    {
    }
	public void LoadSceneNova()
    {
    }
	public void LoadSceneFlame()
    {
    }
	public void LoadSceneAuraCast2()
    {
    }
	public void LoadSceneCurse()
    {
    }
	public void LoadSceneBeamBlast()
    {
    }
	public void LoadSceneOrbitSphere()
    {
    }
	public void LoadSceneDot()
    {
    }

	
	void Update ()
	 {
 
     if(Input.GetKeyDown(KeyCode.J))
	 {
         GUIHide = !GUIHide;
     
         if (GUIHide)
		 {
             GameObject.Find("CanvasSceneSelect").GetComponent<Canvas> ().enabled = false;
         }
		 else
		 {
             GameObject.Find("CanvasSceneSelect").GetComponent<Canvas> ().enabled = true;
         }
     }
	      if(Input.GetKeyDown(KeyCode.K))
	 {
         GUIHide2 = !GUIHide2;
     
         if (GUIHide2)
		 {
             GameObject.Find("Canvas").GetComponent<Canvas> ().enabled = false;
         }
		 else
		 {
             GameObject.Find("Canvas").GetComponent<Canvas> ().enabled = true;
         }
     }
		if(Input.GetKeyDown(KeyCode.L))
	 {
         GUIHide3 = !GUIHide3;
     
         if (GUIHide3)
		 {
             GameObject.Find("CanvasTips").GetComponent<Canvas> ().enabled = false;
         }
		 else
		 {
             GameObject.Find("CanvasTips").GetComponent<Canvas> ().enabled = true;
         }
     }
	 }
}
}