package com.mygdx.game;

import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.Input;
import com.badlogic.gdx.InputProcessor;
import com.badlogic.gdx.ScreenAdapter;
import com.badlogic.gdx.graphics.GL20;
import com.badlogic.gdx.graphics.OrthographicCamera;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;

public class GameScreen extends ScreenAdapter implements InputProcessor {

    public static final int WIDTH = 1000;
    public static final int HEIGHT = 800;
    public boolean run = true;
    public float offsetX, offsetY = 0;

    private final SpriteBatch batch;
    private OrthographicCamera camera;

    private Renderer renderer;
    public GameScreen(SpriteBatch batch) {
        this.batch = batch;
    }

    public void show() {
        camera = new OrthographicCamera(WIDTH, HEIGHT);
        camera.zoom = 0.3f;
        camera.position.set(WIDTH / 2f - 500, HEIGHT / 2f, 10);

        renderer = new Renderer();
        Gdx.input.setInputProcessor(this);
    }

    public void render(float delta) {
        Gdx.gl.glClearColor(0, 0, 0, 1);
        Gdx.gl.glClear(GL20.GL_COLOR_BUFFER_BIT);
        batch.setProjectionMatrix(camera.combined);

        camera.update();

        handleInput();

        batch.begin();
        renderer.draw(batch);
        batch.end();
    }

    public void handleInput() {
        if (Gdx.input.isKeyPressed(Input.Keys.EQUALS) && camera.zoom > 0.1) {
            camera.zoom -= 0.01f;
        }
        if (Gdx.input.isKeyPressed(Input.Keys.MINUS) && camera.zoom < 1.5) {
            camera.zoom += 0.01f;
        }

        if (Gdx.input.getX() < 70 && Gdx.input.getX() > 10 && run || Gdx.input.isKeyPressed(Input.Keys.LEFT)) {
            camera.position.x -= camera.zoom * 3;
        }
        if (Gdx.input.getX() > Gdx.graphics.getWidth() - 70 && Gdx.input.getX() < Gdx.graphics.getWidth() - 10 && run || Gdx.input.isKeyPressed(Input.Keys.RIGHT)) {
            camera.position.x += camera.zoom * 3;
        }
        if (Gdx.input.getY() < 70 && Gdx.input.getY() > 10 && run || Gdx.input.isKeyPressed(Input.Keys.UP)) {
            camera.position.y += camera.zoom * 3;
        }
        if (Gdx.input.getY() > Gdx.graphics.getHeight() - 70 && Gdx.input.getY() < Gdx.graphics.getHeight() - 10 && run || Gdx.input.isKeyPressed(Input.Keys.DOWN)) {
            camera.position.y -= camera.zoom * 3;
        }
    }

    public void dispose() {

    }

    @Override
    public boolean keyDown(int keycode) {
        return false;
    }

    @Override
    public boolean keyUp(int keycode) {
        return false;
    }

    @Override
    public boolean keyTyped(char character) {
        return false;
    }

    @Override
    public boolean touchDown(int screenX, int screenY, int pointer, int button) {
        return false;
    }

    @Override
    public boolean touchUp(int screenX, int screenY, int pointer, int button) {
        run = true;
        return false;
    }

    @Override
    public boolean touchDragged(int screenX, int screenY, int pointer) {
        screenX *= -camera.zoom * 1.5;
        screenY *= camera.zoom * 1.5;
        if (run) {
            offsetX = screenX - camera.position.x;
            offsetY = screenY - camera.position.y;
            run = false;
        }
        camera.position.x = screenX - offsetX;
        camera.position.y = screenY - offsetY;
        return false;
    }

    @Override
    public boolean mouseMoved(int screenX, int screenY) {
        return false;
    }

    @Override
    public boolean scrolled(float amountX, float amountY) {
        camera.zoom -= amountX * 0.05;
        camera.zoom += amountY * 0.05;
        if (camera.zoom < 0.1) {
            camera.zoom = 0.1f;
        }
        if (camera.zoom > 1.5) {
            camera.zoom = 1.5f;
        }
        return false;
    }
}
