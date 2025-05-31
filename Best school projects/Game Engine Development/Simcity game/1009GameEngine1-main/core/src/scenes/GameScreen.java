package scenes;

import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.Input;
import com.badlogic.gdx.Screen;
import com.badlogic.gdx.graphics.GL20;
import com.badlogic.gdx.graphics.Texture;
import com.badlogic.gdx.graphics.g2d.Sprite;
import com.badlogic.gdx.graphics.g2d.TextureRegion;
import com.badlogic.gdx.scenes.scene2d.InputEvent;
import com.badlogic.gdx.scenes.scene2d.InputListener;
import com.badlogic.gdx.scenes.scene2d.Stage;
import com.badlogic.gdx.scenes.scene2d.ui.ImageButton;
import com.badlogic.gdx.scenes.scene2d.utils.TextureRegionDrawable;
import com.badlogic.gdx.utils.viewport.ScreenViewport;
import com.mygdx.game.GameEngine1;
import player.Player;

public class GameScreen implements Screen {
    private GameEngine1 game;
    private Texture bg; //this is background

    private Player player;

    private Stage stage;
    private Texture resumeTexture;
    private TextureRegion resumeTextureRegion;
    private TextureRegionDrawable resumeTexRegionDrawable;
    private ImageButton resumeButton;

    private Texture MainMenuTexture;
    private TextureRegion MainMenuTextureRegion;
    private TextureRegionDrawable MainMenuTexRegionDrawable;
    private ImageButton MainMenuButton;
    private int btnWidth = 200, btnHeight = 100;


    boolean isPaused = false;

    public GameScreen(GameEngine1 game){
        this.game = game;
        bg = new Texture("grasspatch.jpg");

        player = new Player("player1.png");

    }
    @Override
    public void show() {
        resumeTexture = new Texture(Gdx.files.internal("buttons/Resume Button.png"));
        resumeTextureRegion = new TextureRegion(resumeTexture);
        resumeTexRegionDrawable = new TextureRegionDrawable(resumeTextureRegion);
        resumeButton = new ImageButton(resumeTexRegionDrawable); //Set the button up
        resumeButton.setSize(btnWidth,btnHeight);
        resumeButton.setPosition(Gdx.graphics.getWidth()/2 - resumeButton.getWidth()/2,
                Gdx.graphics.getHeight()/2 );
        resumeButton.addListener(new InputListener(){

            @Override
            public boolean touchDown(InputEvent event, float x, float y, int pointer, int button) {
                isPaused = false;
                return true;
            }

            @Override
            public void touchUp(InputEvent event, float x, float y, int pointer, int button) {
                super.touchUp(event, x, y, pointer, button);
            }
        });

        MainMenuTexture = new Texture(Gdx.files.internal("buttons/Menu Button.png"));
        MainMenuTextureRegion = new TextureRegion(MainMenuTexture);
        MainMenuTexRegionDrawable = new TextureRegionDrawable(MainMenuTextureRegion);
        MainMenuButton = new ImageButton(MainMenuTexRegionDrawable); //Set the button up
        MainMenuButton.setSize(btnWidth,btnHeight);
        MainMenuButton.setPosition(Gdx.graphics.getWidth()/2 - MainMenuButton.getWidth()/2,
                Gdx.graphics.getHeight()/2 - resumeButton.getHeight());
        MainMenuButton.addListener(new InputListener(){
            @Override
            public boolean touchDown(InputEvent event, float x, float y, int pointer, int button) {
                game.switchMainScreen();
                return true;
            }

            @Override
            public void touchUp(InputEvent event, float x, float y, int pointer, int button) {
                super.touchUp(event, x, y, pointer, button);
            }
        });

        stage = new Stage(new ScreenViewport()); //Set up a stage for the ui
        stage.addActor(resumeButton);
        stage.addActor(MainMenuButton);
        Gdx.input.setInputProcessor(stage); //Start taking input from the ui
    }


    @Override
    public void render(float delta) {
        Gdx.gl.glClearColor(1,0,0,1);
        Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);


        pauseBtn();

        if(isPaused==false){
            player.Update(Gdx.graphics.getDeltaTime());
            game.getBatch().begin(); //everytime before we draw ANYTHING we need to call this.
            game.getBatch().draw(bg,0,0);
            game.getBatch().draw(player,player.getX(),player.getY());
            game.getBatch().end(); //after drawing, we need to END IT. *** VERY IMPORTANT ***

        }else{
            game.getBatch().begin(); //everytime before we draw ANYTHING we need to call this.
            game.getBatch().draw(bg,0,0);
            game.getBatch().draw(player,player.getX(),player.getY());
            game.getBatch().end(); //after drawing, we need to END IT. *** VERY IMPORTANT ***
            stage.draw();
        }


    }

    @Override
    public void resize(int width, int height) {
    }

    @Override
    public void pause() {

    }

    @Override
    public void resume() {

    }

    @Override
    public void hide() {

    }

    @Override
    public void dispose() {

    }

    public void pauseBtn(){
        if(Gdx.input.isKeyPressed(Input.Keys.ESCAPE)){
            isPaused = true;
        }
    }
}
