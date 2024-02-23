using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SceneManager = UnityEngine.SceneManagement.SceneManager;
using UnityEngine.SceneManagement;

public class LoadPlaceHolderScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
             SceneManager.LoadScene("placeholderScene", LoadSceneMode.Single);
        }
    }
}
