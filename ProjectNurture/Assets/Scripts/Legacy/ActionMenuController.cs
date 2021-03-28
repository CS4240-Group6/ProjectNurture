using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMenuController : MonoBehaviour
{
    /**
     * Script for toggling action menu on right hand
     */

    public string showActionMenuButtonName = "ActionMenu";  
    public GameObject actionMenu;

    // Start is called before the first frame update
    void Start()
    {
        actionMenu = GameObject.Find("ActionMenu");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis(showActionMenuButtonName) == 1)
        {
            actionMenu.GetComponent<ActionMenuScript>().showUI();
        }
        else
        {
            actionMenu.GetComponent<ActionMenuScript>().removeUI();
        }
    }
}
