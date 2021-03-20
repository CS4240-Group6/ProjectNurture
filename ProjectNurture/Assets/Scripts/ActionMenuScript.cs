using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionMenuScript : MonoBehaviour
{
    private GameObject ActionMenu;

    // Name of Action Menu game object in the scene
    public string actionMenuName = "ActionMenu";

    // Start is called before the first frame update
    void Start()
    {
        ActionMenu = GameObject.Find(actionMenuName);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void showUI()
    {
        ActionMenu.SetActive(true);
    }

    public void removeUI()
    {
        ActionMenu.SetActive(false);
    }

}

