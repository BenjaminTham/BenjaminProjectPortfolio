package com.mygdx.game;

import com.badlogic.gdx.Game;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import scenes.GameScreen;
import scenes.MainMenu;
import scenes.OptionScreen;

public class GameEngine1 extends Game {
	//this gameEngine1 will only serve as a reference to get spritebatch and change scenes
	private SpriteBatch batch;
	
	@Override
	public void create () {
		batch = new SpriteBatch();
		setScreen(new MainMenu(this/*this refers to this exact class GameEngine1*/));
	}

	@Override
	public void render () {
		super.render(); // pass the render method TO ALL OF THE SCREENS
	}

	public SpriteBatch getBatch() {
		//we set spritebatch as private to be safe.
		//but for MainMenu to use spritebatch, we need this public method and use it as a getter for MainMenu class
		//to call it and retrieve spritebatch. Otherwise, we wont be able to use spritebatch cus its privated.
		return batch;
	}

	@Override
	public void dispose () {
	}

	//my three scene switching codes
	public void switchGameScreen(){
		GameScreen gameScreen = new GameScreen(this);
		setScreen(gameScreen);
	}

	public void switchSettingScreen(){
		OptionScreen settingScreen = new OptionScreen(this);
		setScreen(settingScreen);

	}

	public void switchMainScreen(){
		MainMenu mainMenu = new MainMenu(this);
		setScreen(mainMenu);
	}
}
