using System.Collections;

using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Video; 
using UnityEngine.Sprites;

using UnityEngine.UI;

public class StartStop : MonoBehaviour{
    private VideoPlayer player;

    public Button button;

    public Sprite startSprite; 
    public Sprite stopSprite;

// Start is called before the first frame update 
void Start(){
    player = GetComponent<VideoPlayer>();
    player.Pause(); // Pause the player by default
    button.image.sprite = startSprite; // Set the button sprite to startSprite
}

//

// Update is called once per frame 
void Update(){

}

public void ChangeStartStop()
{
if (player.isPlaying == false){
    player.Play();
    button.image.sprite = stopSprite;
}
else{
    player.Pause();
    button.image.sprite = startSprite;
}



}

}

