package player;

import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.Input;
import com.badlogic.gdx.graphics.Texture;
import com.badlogic.gdx.graphics.g2d.Sprite;
import com.badlogic.gdx.math.Vector2;

public class Player extends Sprite {

    int speed = 500;
    public Vector2 position;
//    public Player(String name, float x, float y /*x and y are the position of player*/){
    public Player(String name/*x and y are the position of player*/){
        super(new Texture(name)); //name would be the texture of player
        this.position = new Vector2(Gdx.graphics.getWidth()/2,Gdx.graphics.getHeight()/2);
        setPosition(position.x - getWidth()/2,position.y-getHeight()/2);
    }

    public void Update(float deltaTime){
        if(Gdx.input.isKeyPressed(Input.Keys.A)) {
            position.x -= deltaTime * speed;
            setPosition(position.x - getWidth()/2, position.y - getHeight()/2);
        }
        if(Gdx.input.isKeyPressed(Input.Keys.D)) {
            position.x += deltaTime * speed;
            setPosition(position.x - getWidth()/2, position.y - getHeight()/2);
        }

        if(Gdx.input.isKeyPressed(Input.Keys.S)) {
            position.y -= deltaTime * speed;
            setPosition(position.x - getWidth()/2, position.y - getHeight()/2);
        }

        if(Gdx.input.isKeyPressed(Input.Keys.W)) {
            position.y += deltaTime * speed;
            setPosition(position.x - getWidth()/2, position.y - getHeight()/2);
        }
    }

}
