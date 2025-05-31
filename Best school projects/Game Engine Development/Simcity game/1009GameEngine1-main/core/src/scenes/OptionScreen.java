package scenes;

import com.badlogic.gdx.Gdx;
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

public class OptionScreen implements Screen {

    private Stage stage;
    private Texture backTexture;
    private TextureRegion backTextureRegion;
    private TextureRegionDrawable backTexRegionDrawable;
    private ImageButton backButton;
    private int btnWidth = 300, btnHeight = 100;

    private GameEngine1 game;
    private Texture bg; //this is background
    private Sprite player;

    public OptionScreen(GameEngine1 game){
        this.game = game;
        bg = new Texture("stone.jpg");

    }
    @Override
    public void show() {

        backTexture = new Texture(Gdx.files.internal("buttons/Back Square Button.png"));
        backTextureRegion = new TextureRegion(backTexture);
        backTexRegionDrawable = new TextureRegionDrawable(backTextureRegion);
        backButton = new ImageButton(backTexRegionDrawable); //Set the button up
        backButton.setSize(60,60);
        backButton.setPosition(backButton.getWidth() - backButton.getWidth()+10,
                Gdx.graphics.getHeight() - backButton.getHeight()-10);
        backButton.addListener(new InputListener(){
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

        stage.addActor(backButton); //Add the button to the stage to perform rendering and take input.
        Gdx.input.setInputProcessor(stage); //Start taking input from the ui
    }

    @Override
    public void render(float delta) {
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
}
