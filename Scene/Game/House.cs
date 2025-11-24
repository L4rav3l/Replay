using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using TiledSharp;
using System.Collections.Generic;
using System;


namespace Replay;

public class House : IScene
{
    private GraphicsDevice _graphics;
    private SceneManager _sceneManager;
    private ContentManager _content;

    private TmxMap _map;

    private List<Rectangle> _collision;
    private Rectangle _door;
    private Rectangle _npc;

    private Texture2D _tileset;
    private Texture2D _playerTexture;
    private Texture2D _dialougeTexture;
    private Texture2D _honeyjar;
    private Texture2D _piecemushroom;
    private Texture2D _keyTexture;
    private Texture2D _stick;

    private SpriteFont _pixelfont;

    private Player _player;
    private Camera2D _camera;

    private bool _dialouge;

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


    public Rectangle LoadInteractableObjects(string mapFilePath, string item)
    {
        var map = new TmxMap(mapFilePath);
        var interactable = new Rectangle();

        foreach(var objectGroup in map.ObjectGroups)
        {
            if(objectGroup.Name == item)
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

                        interactable = rect;
                     }
                }
            }
        }

        return interactable;
    }

    public House(GraphicsDevice _graphics, SceneManager _sceneManager, ContentManager _content)
    {
        this._graphics = _graphics;
        this._sceneManager = _sceneManager;
        this._content = _content;
    }

    public void LoadContent()
    {
        _player = new Player(new Vector2(791, 601));
        _camera = new Camera2D(_graphics.Viewport);

        _collision = LoadCollisionObjects("Content/house.tmx");
        _door = LoadInteractableObjects("Content/house.tmx", "Door");
        _npc = LoadInteractableObjects("Content/house.tmx", "npc");

        _tileset = _content.Load<Texture2D>("tilemap");
        _playerTexture = _content.Load<Texture2D>("player");
        _stick = _content.Load<Texture2D>("stick");
        _piecemushroom = _content.Load<Texture2D>("mushroom");
        _honeyjar = _content.Load<Texture2D>("honey");
        _keyTexture = _content.Load<Texture2D>("key");
        _pixelfont = _content.Load<SpriteFont>("pixelfont");

        _map = new TmxMap("Content/house.tmx");

        _dialougeTexture = new Texture2D(_graphics, 1, 1);
        _dialougeTexture.SetData(new [] {Color.White});
    }

    public void Update(GameTime gameTime)
    {
        KeyboardState state = Keyboard.GetState();

        _player.Speed = GameData.Speed;

        _player.Update(gameTime, _collision, _camera);
        _camera.Follow(_player.Position, new Vector2(_map.Width * 16, _map.Height * 16));

        if(_player.Hitbox.Intersects(_door) && state.IsKeyDown(Keys.E) && !GameData.previous.IsKeyDown(Keys.E))
        {
            _sceneManager.ChangeScene("indoor");
        }

        if(state.IsKeyDown(Keys.E) && !GameData.previous.IsKeyDown(Keys.E) && _dialouge)
        {
            GameData.Move = true;
            _dialouge = false;
        } else if(_player.Hitbox.Intersects(_npc) && state.IsKeyDown(Keys.E) && !GameData.previous.IsKeyDown(Keys.E) && !_dialouge)
        {
            GameData.Move = false;
            _dialouge = true;
        }

        if(state.IsKeyDown(Keys.E) && !GameData.previous.IsKeyDown(Keys.E) && GameData.Mushroom)
        {
            GameData.Mushroom = false;
            GameData.Key = true;
            _dialouge = false;
        }

        GameData.previous = state;
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

        if(_dialouge == true)
        {
            Vector2 Paper = new Vector2((Width / 2) - 350, ((Height / 4) * 3) - 200);

            spriteBatch.Draw(_dialougeTexture, new Rectangle((int)Paper.X, (int)Paper.Y, 700, 400), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.2f);

            Vector2 Text = new Vector2((Width / 2) - 330, ((Height / 4) * 3) - 175);

            spriteBatch.DrawString(_pixelfont, "Hi traveler, you can't imagine it.\nSomeone stole from me. I can give you\na key if you bring me a mushroom.\nPress E Key", Text, Color.Black, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.3f);
        }

        if(GameData.Hand)
        {
            if(GameData.Wood)
            {
                spriteBatch.Draw(_stick, new Vector2(_player.screenPos.X + 40, _player.screenPos.Y + 10), null, Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.3f);
            }

            if(GameData.Mushroom)
            {
                spriteBatch.Draw(_piecemushroom, new Vector2(_player.screenPos.X + 40, _player.screenPos.Y + 10), null, Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.3f);
            }

            if(GameData.Honey)
            {
                spriteBatch.Draw(_honeyjar, new Vector2(_player.screenPos.X + 40, _player.screenPos.Y + 10), null, Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.3f);
            }
            
            if(GameData.Key)
            {
                spriteBatch.Draw(_keyTexture, new Vector2(_player.screenPos.X + 40, _player.screenPos.Y + 10), null, Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.3f);
            }
        }
    }
}