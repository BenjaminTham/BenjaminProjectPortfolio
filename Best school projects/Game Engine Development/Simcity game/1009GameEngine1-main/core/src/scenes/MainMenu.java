package scenes;

import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.Screen;
import com.badlogic.gdx.graphics.GL20;
import com.badlogic.gdx.graphics.Texture;
import com.badlogic.gdx.graphics.g2d.BitmapFont;
import com.badlogic.gdx.graphics.g2d.Sprite;
import com.badlogic.gdx.graphics.g2d.TextureAtlas;
import com.badlogic.gdx.graphics.g2d.TextureRegion;
import com.badlogic.gdx.scenes.scene2d.InputEvent;
import com.badlogic.gdx.scenes.scene2d.InputListener;
import com.badlogic.gdx.scenes.scene2d.Stage;
import com.badlogic.gdx.scenes.scene2d.ui.Button;
import com.badlogic.gdx.scenes.scene2d.ui.ImageButton;
import com.badlogic.gdx.scenes.scene2d.ui.Skin;
import com.badlogic.gdx.scenes.scene2d.ui.TextButton;
import com.badlogic.gdx.scenes.scene2d.utils.TextureRegionDrawable;
import com.badlogic.gdx.utils.Timer;
import com.badlogic.gdx.utils.viewport.ScreenViewport;
import com.mygdx.game.GameEngine1;

import java.awt.*;

public class MainMenu implements Screen { //main menu
    private GameEngine1 game; //reference to our main class because we are gonna pass it to our main menu and use its spritebatch
    private Texture bg; //this is background
    private Sprite player;
    private int btnWidth = 300, btnHeight = 100;

    private Stage stage;
    private Texture startTexture;
    private TextureRegion startTextureRegion;
    private TextureRegionDrawable startTexRegionDrawable;
    private ImageButton startButton;

    private Texture optionTexture;
    private TextureRegion optionTextureRegion;
    private TextureRegionDrawable optionTexRegionDrawable;
    private ImageButton optionButton;

    private Texture mainMenuLogoTexture;
    private TextureRegion mainMenuLogoTextureRegion;
    private TextureRegionDrawable mainMenuLogoTexRegionDrawable;
    private ImageButton mainMenuLogoButton;


    public MainMenu(final GameEngine1 game){
        this.game = game;
        //this.game is refering to THIS CLASS (MainMenu) and game is referring to the GameEngine1 game variable we are passing (on top, in the parenthesis)
        //now we can use the spritebatch in our game
        bg = new Texture("citybg.jpg");
    }

    @Override
    public void show() {
        //same as public void create. will be the first method that's gonna be called.

        mainMenuLogoTexture = new Texture(Gdx.files.internal("logo.png"));
        mainMenuLogoTextureRegion = new TextureRegion(mainMenuLogoTexture);
        mainMenuLogoTexRegionDrawable = new TextureRegionDrawable(mainMenuLogoTextureRegion);
        mainMenuLogoButton = new ImageButton(mainMenuLogoTexRegionDrawable); //Set the button up
        mainMenuLogoButton.setSize(400,150);
        mainMenuLogoButton.setPosition(Gdx.graphics.getWidth()/2 - mainMenuLogoButton.getWidth()/2,
                Gdx.graphics.getHeight()/2+100 );

        startTexture = new Texture(Gdx.files.internal("buttons/Start Button.png"));
        startTextureRegion = new TextureRegion(startTexture);
        startTexRegionDrawable = new TextureRegionDrawable(startTextureRegion);
        startButton = new ImageButton(startTexRegionDrawable); //Set the button up
        startButton.setSize(200,100);
        startButton.setPosition(Gdx.graphics.getWidth()/2 - startButton.getWidth()/2,
                Gdx.graphics.getHeight()/2 - startButton.getHeight()/2);
        startButton.addListener(new InputListener(){
            @Override
            public boolean touchDown(InputEvent event, float x, float y, int pointer, int button) {
                game.switchGameScreen();
                return true;
            }
            @Override
            public void touchUp(InputEvent event, float x, float y, int pointer, int button) {
                super.touchUp(event, x, y, pointer, button);
            }
        });

        //one of the button i used to switch scene using addlistener
        optionTexture = new Texture(Gdx.files.internal("buttons/Options Button.png"));
        optionTextureRegion = new TextureRegion(optionTexture);
        optionTexRegionDrawable = new TextureRegionDrawable(optionTextureRegion);
        optionButton = new ImageButton(optionTexRegionDrawable); //Set the button up
        optionButton.setSize(200,100);
        optionButton.setPosition(Gdx.graphics.getWidth()/2 - optionButton.getWidth()/2,
                (Gdx.graphics.getHeight()/2 - optionButton.getHeight()/2)-(Gdx.graphics.getHeight()/2 - startButton.getHeight()/2 + 10)/2);
        optionButton.addListener(new InputListener(){
            @Override
            public boolean touchDown(InputEvent event, float x, float y, int pointer, int button) {
                game.switchSettingScreen();
                return true;
            }
            @Override
            public void touchUp(InputEvent event, float x, float y, int pointer, int button) {
                super.touchUp(event, x, y, pointer, button);
            }
        });

        stage = new Stage(new ScreenViewport()); //Set up a stage for the ui

        stage.addActor(startButton); //Add the button to the stage to perform rendering and take input.
        stage.addActor(mainMenuLogoButton); //Add the button to the stage to perform rendering and take input.
        stage.addActor(optionButton); //Add the button to the stage to perform rendering and take input.
        Gdx.input.setInputProcessor(stage); //Start taking input from the ui
    }

    @Override
    public void render(float delta) {
    //same as normal render. renders everything that we see on screen
    //called every single frame. so if 60 fps, this will render 60 times per second.

    //by calling all these two lines, they will clear the screen and all our gamebatch below and redraw our stuff. Its just good practice.
        Gdx.gl.glClearColor(1,0,0,1);
        Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);

        stage.act(Gdx.graphics.getDeltaTime()); //Perform ui logic
        game.getBatch().begin(); //everytime before we draw ANYTHING we need to call this.
        game.getBatch().draw(bg,0,0,Gdx.graphics.getWidth(), Gdx.graphics.getHeight());
        game.getBatch().end(); //after drawing, we need to END IT. *** VERY IMPORTANT ***

        stage.draw(); //Draw the ui
    }

    @Override
    public void resize(int width, int height) {
    //ensure our screen will always be our width and height
    }

    @Override
    public void pause() {
    //called right away when we hit pause
    //stop rendering sprite, stop counting scores etc
    }

    @Override
    public void resume() {
    //opposite of pause, resume everything
    }

    @Override
    public void hide() {
    //when application goes into background
    //know that our application will not be visible anymore
    }

    @Override
    public void dispose() {
    //upon termination. heres where we dispose all textures WHICH IS VERY IMPORTANT. Dispose all resources that takes up our memory
    }

}
