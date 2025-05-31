using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitchScript : MonoBehaviour
{
    private bool isLandscape = false;
    public void SceneLoader(int SceneIndex)
    {
        SceneManager.LoadScene(SceneIndex);
    }
    public void playGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void LoadVideoLandscape(int SceneIndex)
    {
        // Ensure the orientation is set to landscape mode before transitioning
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        SceneManager.LoadScene(SceneIndex); // Change "Scene2" to the name of your Scene 2.
    }

    public void LoadPortraitMode(int SceneIndex)
        {
            if(SceneManager.GetActiveScene().name == "Customise Last Step"){
                Screen.orientation = ScreenOrientation.Portrait;
                Screen.orientation = ScreenOrientation.AutoRotation;
            }else{
            // Ensure the orientation is set to landscape mode before transitioning
            Screen.orientation = ScreenOrientation.Portrait;
            SceneManager.LoadScene(SceneIndex); // Change "Scene2" to the name of your Scene 2.
            }
           
        }
    // Add a method to set the orientation at the start of the scene to auto-rotate
    public void SetSceneOrientationToAutoRotate()
    {
        Screen.orientation = ScreenOrientation.AutoRotation;
    }
    void Awake()
    {
        // Check if this is the specific scene where you want auto-rotation
        if (SceneManager.GetActiveScene().name == "Confirm2 Scene")
        {
            // Set the orientation to auto-rotate when this scene starts
            SetSceneOrientationToAutoRotate();
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Confirm2 Scene")
        {
            if ((Screen.orientation == ScreenOrientation.LandscapeLeft && !isLandscape) || (Screen.orientation == ScreenOrientation.LandscapeRight && !isLandscape))
            {
                // Landscape mode detected, transition to the next scene
                isLandscape = true;
                LoadNextScene();
            }
            else if ((Screen.orientation != ScreenOrientation.LandscapeLeft) || (Screen.orientation != ScreenOrientation.LandscapeRight))
            {
                isLandscape = false;
            }
        }

        else if (SceneManager.GetActiveScene().name == "Customise Last Step")
        {
            if ((Screen.orientation == ScreenOrientation.LandscapeLeft && !isLandscape) || (Screen.orientation == ScreenOrientation.LandscapeRight && !isLandscape))
            {
                // Landscape mode detected, transition to the next scene
                isLandscape = true;
                LoadAR();
            }
            else if ((Screen.orientation != ScreenOrientation.LandscapeLeft) || (Screen.orientation != ScreenOrientation.LandscapeRight))
            {
                isLandscape = false;
            }
        }
    }
    private void LoadAR(){
        // Set the desired orientation based on the current orientation
        ScreenOrientation desiredOrientation = Screen.orientation;
        
        // Set the desired orientation before transitioning
        Screen.orientation = desiredOrientation;
        SceneManager.LoadScene(7);
    }
    private void LoadNextScene()
    {
        
       
        // Set the desired orientation based on the current orientation
        ScreenOrientation desiredOrientation = Screen.orientation;
        
        // Set the desired orientation before transitioning
        Screen.orientation = desiredOrientation;
        SceneManager.LoadScene(15);
    }
}
