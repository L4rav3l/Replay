using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using TiledSharp;

namespace Replay;

public class Indoor : IScene
{
    private GraphicsDevice _graphics;
    private SceneManager _sceneManager;
    private ContentManager _content;

    private Texture2D _tileset;
    private Texture2D _playerTexture;
    private Texture2D _stick;
    private SpriteFont _pixelfont;

    private TmxMap _map;
    private List<Rectangle> _collision;
    private List<Rectangle> _cuttable;

    private double _break;
    private bool _hand = false;
    private bool _wood = false;

    private Player _player;
    private Camera2D _camera;

    public List<Rectangle> LoadCollisionObjects(string mapFilePath)
    {
        var map = new TmxMap(mapFilePath);
        var solidTiles = new List<Rectangle>();

        foreach(var objectGroup in map.ObjectGroups)
        {
            if(objectGroup.Name == "Collision")
            {
                foreach(var obj in objectGroup.Objects)
                {
                     if(obj.Width > 0 && obj.Height > 0)
                     {
                        var rect = new Rectangle(
                            (int)obj.X,
                            (int)obj.Y,
                            (int)obj.Width,
                            (int)obj.Height
                        );

                        solidTiles.Add(rect);
                     }
                }
            }
        }

        return solidTiles;
    }

    public List<Rectangle> LoadCuttableObjects(string mapFilePath)
    {
        var map = new TmxMap(mapFilePath);
        var cuttableTiles = new List<Rectangle>();

        foreach(var objectGroup in map.ObjectGroups)
        {
            if(objectGroup.Name == "Tree")
            {
                foreach(var obj in objectGroup.Objects)
                {
                    var rect = new Rectangle(
                        (int)obj.X,
                        (int)obj.Y,
                        (int)obj.Width,
                        (int)obj.Height
                    );

                    cuttableTiles.Add(rect);
                }
            }
        }

        return cuttableTiles;
    }

    public Indoor(GraphicsDevice _graphics, SceneManager _sceneManager, ContentManager _content)
    {
        this._graphics = _graphics;
        this._sceneManager = _sceneManager;
        this._content = _content;
    }

    public void LoadContent()
    {
        _player = new Player(new Vector2(784, 560));
        _camera = new Camera2D(_graphics.Viewport);

        _map = new TmxMap("Content/indoor.tmx");

        _tileset = _content.Load<Texture2D>("tilemap");
        _playerTexture = _content.Load<Texture2D>("player");
        _stick = _content.Load<Texture2D>("stick");
        _pixelfont = _content.Load<SpriteFont>("pixelfont");

        _collision = LoadCollisionObjects("Content/indoor.tmx");
        _cuttable = LoadCuttableObjects("Content/indoor.tmx");
    }

    public void Update(GameTime gameTime)
    {
        _player.Update(gameTime, _collision, _camera);
        _camera.Follow(_player.Position, new Vector2(_map.Width * 16, _map.Height * 16));

        bool cutting = false;

        double elapsed = gameTime.ElapsedGameTime.TotalSeconds * 1000;

        if(_break > 0)
        {
            _break -= elapsed;
        }

        KeyboardState state = Keyboard.GetState();

        foreach(Rectangle tree in _cuttable)
        {
            if(_player.Hitbox.Intersects(tree) && state.IsKeyDown(Keys.E))
            {
                Console.WriteLine(_break);
                cutting = true;

                if(_break <= 0)
                {
                    _hand = true;
                    _wood = true;
                }
            }
        }

        if(!cutting)
        {
            _break = 5000;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _graphics.Clear(new Color(132, 198, 105));

        int mapWidth = _map.Width;
        int mapHeight = _map.Height;

        int Width = _graphics.Viewport.Width;
        int Height = _graphics.Viewport.Height;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                int i = y * mapWidth + x;
                if (i >= _map.Layers[0].Tiles.Count) continue;

                var tile = _map.Layers[0].Tiles[i];
                if (tile.Gid == 0) continue;

                int tilesPerRow = _tileset.Width / _map.TileWidth;
                int tileIndex = tile.Gid - 1;

                int tileIndexX = tileIndex % tilesPerRow;
                int tileIndexY = tileIndex / tilesPerRow;

                Rectangle source = new Rectangle(
                    tileIndexX * _map.TileWidth,
                    tileIndexY * _map.TileHeight,
                    _map.TileWidth,
                    _map.TileHeight
                );

                Vector2 worldPosition = new Vector2(x * _map.TileWidth, y * _map.TileHeight);
                Vector2 screenPosition = _camera.WorldToScreen(worldPosition);

                spriteBatch.Draw(_tileset, screenPosition, source, Color.White, 0f, Vector2.Zero, 5f, SpriteEffects.None, 0.1f);
            }
        }
                _player.Draw(spriteBatch, _playerTexture, _camera);

        if(_hand)
        {
            if(_wood)
            {
                spriteBatch.Draw(_stick, new Vector2(_player.screenPos.X + 40, _player.screenPos.Y + 10), null, Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.2f);
            }
        }
        
        Vector2 ObjectM = _pixelfont.MeasureString("WOOD HERE 3/0");
        Vector2 Object = _camera.WorldToScreen(new Vector2(984, 935)) - new Vector2(ObjectM.X / 2, ObjectM.Y / 2);

        spriteBatch.DrawString(_pixelfont, "WOOD HERE 3/0", Object, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.2f);
    }
}