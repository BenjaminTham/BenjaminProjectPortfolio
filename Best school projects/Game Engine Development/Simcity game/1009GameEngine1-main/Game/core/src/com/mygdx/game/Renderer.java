package com.mygdx.game;

import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.Input;
import com.badlogic.gdx.graphics.Texture;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;

import java.util.Random;

public class Renderer {

    public static final int TILE_WIDTH = 34;
    public static final int TILE_HEIGHT = 34;

    private int[][] map;
    private final Texture grass;
    private final Texture tree;
    private final Texture road;
    private final Texture roadLeft;
    private final Texture roadRight;
    private final Texture building;
    private final Texture building2;
    private final Texture building3;
    private final Texture building4;
    private final Texture building5;
    private final Texture building6;
    private final Texture sky;
    public Renderer() {
        grass = new Texture(Gdx.files.internal("grass.png"));
        tree = new Texture(Gdx.files.internal("Tree.png"));
        road = new Texture(Gdx.files.internal("Road.png"));
        roadLeft = new Texture(Gdx.files.internal("RoadLeft.png"));
        roadRight = new Texture(Gdx.files.internal("RoadRight.png"));
        building = new Texture(Gdx.files.internal("Building.png"));
        building2 = new Texture(Gdx.files.internal("Building2.png"));
        building3 = new Texture(Gdx.files.internal("Building3.png"));
        building4 = new Texture(Gdx.files.internal("Building4.png"));
        building5 = new Texture(Gdx.files.internal("Building5.jpg"));
        building6 = new Texture(Gdx.files.internal("Building6.png"));
        sky = new Texture(Gdx.files.internal("Sky.jpg"));
        map = randomGenerator();
    }

    public void draw(SpriteBatch batch) {
        batch.draw(sky, map.length * -20,map.length * -5, map.length * 40, map.length * 30);
        for (int row = map.length - 1; row >= 0; row--) {
            for (int col = map.length - 1; col >= 0; col--) {
                float x = (col - row) * (TILE_WIDTH / 2f - 2);
                float y = (col + row) * (TILE_HEIGHT / 4f);
                batch.draw(grass, x, y, TILE_WIDTH, TILE_HEIGHT);
                if (col % 3 == 0 && row % 3 != 0) {
                    batch.draw(roadLeft, x + 2,y + 14.5f, TILE_WIDTH - 5, TILE_HEIGHT - 11);
                } else if (row % 3 == 0 && col % 3 != 0) {
                    batch.draw(roadRight, x + 2,y + 15.5f, TILE_WIDTH - 5, TILE_HEIGHT - 12);
                } else if (col % 3 == 0) {
                    batch.draw(road, x + 2,y + 15.5f, TILE_WIDTH - 5, TILE_HEIGHT - 17);
                }
                if (map[row][col] == 6 && row % 3 > 0 && col % 3 > 0) {
                    batch.draw(tree, x, y + (TILE_HEIGHT/2.5f) + 1, TILE_WIDTH - 2, TILE_HEIGHT - 2);
                } else if (map[row][col] == 0 && row % 3 > 0 && col % 3 > 0) {
                    batch.draw(building, x - (TILE_WIDTH/2f) - 0.5f, y + (TILE_HEIGHT/3f) + 2, 2 * TILE_WIDTH, 2 * TILE_HEIGHT - 5);
                } else if (map[row][col] == 1 && row % 3 > 0 && col % 3 > 0) {
                    batch.draw(building2, x + 1, y + (TILE_HEIGHT/2.2f) + 1, TILE_WIDTH - 4, TILE_HEIGHT);
                } else if (map[row][col] == 2 && row % 3 > 0 && col % 3 > 0) {
                    batch.draw(building3, x, y + (TILE_HEIGHT/2f) + 1.5f, TILE_WIDTH, TILE_HEIGHT);
                } else if (map[row][col] == 3 && row % 3 > 0 && col % 3 > 0) {
                    batch.draw(building4, x + 2.5f, y + (TILE_HEIGHT/2.2f) + 2, TILE_WIDTH - 6, TILE_HEIGHT + 1);
                } else if (map[row][col] == 4 && row % 3 > 0 && col % 3 > 0) {
                    batch.draw(building5, x + 1, y + (TILE_HEIGHT/2.2f), TILE_WIDTH - 3, TILE_HEIGHT - 5);
                } else if (map[row][col] == 5 && row % 3 > 0 && col % 3 > 0) {
                    batch.draw(building6, x + 4, y + (TILE_HEIGHT/2.2f), TILE_WIDTH - 3, TILE_HEIGHT);
                }
            }
        }
        if (Gdx.input.isKeyJustPressed(Input.Keys.G)) {
            map = randomGenerator();
        }
    }

    public int[][] randomGenerator() {
        Random r = new Random();
        int Size = r.nextInt(25,100);
        int[][] map = new int[Size][Size];

        for(int row = 0; row < map.length; row++) {
            for(int col = 0; col < map.length; col++) {
                int Number = r.nextInt(10);
                if(Number == 0) {
                    map[row][col] = 0;
                } else if (Number == 1){
                    map[row][col] = 1;
                } else if (Number == 2){
                    map[row][col] = 2;
                } else if (Number == 3){
                    map[row][col] = 3;
                } else if (Number == 4) {
                    map[row][col] = 4;
                } else if (Number == 5) {
                    map[row][col] = 5;
                } else if (Number < 8) {
                    map[row][col] = 6;
                } else {
                    map[row][col] = 7;
                }
            }
        }
        return map;
    }
}
