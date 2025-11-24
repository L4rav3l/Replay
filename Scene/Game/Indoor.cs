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
    private Texture2D _piecemushroom;
    private Texture2D _honeyjar;
    private Texture2D _keyTexture;
    private SpriteFont _pixelfont;

    private TmxMap _map;
    private List<Rectangle> _collision;
    private List<Rectangle> _cuttable;
    private List<Rectangle> _pickable;
    private List<Rectangle> _beehive;
    private Rectangle _door;
    private Rectangle _item;
    private Rectangle _deleteCuttable;
    private Rectangle _deleteMushroom;
    private Rectangle _deleteHoney;

    private double _break;
    private int _woodSum;
    private int _mushroomSum;
    private int _honeySum;
    private int _keySum;
    private int _object;
    private bool _hand = false;
    private bool _wood = false;
    private bool _mushroom = false;
    private bool _honey = false;
    private bool _breakcountdown = false;
    private bool _key = false;

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

    public List<Rectangle> LoadMushroomObjects(string mapFilePath)
    {
        var map = new TmxMap(mapFilePath);
        var mushroomTiles = new List<Rectangle>();

        foreach(var objectGroup in map.ObjectGroups)
        {
            if(objectGroup.Name == "Mushroom")
            {
                foreach(var obj in objectGroup.Objects)
                {
                    var rect = new Rectangle(
                        (int)obj.X,
                        (int)obj.Y,
                        (int)obj.Width,
                        (int)obj.Height
                    );

                    mushroomTiles.Add(rect);
                }
            }
        }

        return mushroomTiles;
    }

    public List<Rectangle> LoadBeeHiveObjects(string mapFilePath)
    {
        var map = new TmxMap(mapFilePath);
        var BeehiveTiles = new List<Rectangle>();
        foreach(var objectGroup in map.ObjectGroups)
        {
            if(objectGroup.Name == "Honey")
            {
                foreach(var obj in objectGroup.Objects)
                {
                    var rect = new Rectangle(
                            (int)obj.X,
                            (int)obj.Y,
                            (int)obj.Width,
                            (int)obj.Height
                        );

                    BeehiveTiles.Add(rect);
                }
            }
        }

        return BeehiveTiles;
    }

    public Rectangle LoadDoor(string mapFilePath)
    {
        var map = new TmxMap(mapFilePath);
        var DoorTiles = new Rectangle();

        foreach(var objectGroup in map.ObjectGroups)
        {
            if(objectGroup.Name == "Door")
            {
                foreach(var obj in objectGroup.Objects)
                {
                    var rect = new Rectangle(
                        (int)obj.X,
                        (int)obj.Y,
                        (int)obj.Width,
                        (int)obj.Height
                    );

                    DoorTiles = rect;
                }
            }
        }

        return DoorTiles;
    }

    public Rectangle LoadItem(string mapFilePath)
    {
        var map = new TmxMap(mapFilePath);
        var item = new Rectangle();

        foreach(var objectGroup in map.ObjectGroups)
        {
            if(objectGroup.Name == "Item")
             {
                foreach(var obj in objectGroup.Objects)
                {
                    var rect = new Rectangle(
                        (int)obj.X,
                        (int)obj.Y,
                        (int)obj.Width,
                        (int)obj.Height
                    );

                    item = rect;
                }
             }
        }

        return item;
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
        _piecemushroom = _content.Load<Texture2D>("mushroom");
        _honeyjar = _content.Load<Texture2D>("honey");
        _keyTexture = _content.Load<Texture2D>("key");
        _pixelfont = _content.Load<SpriteFont>("pixelfont");

        _collision = LoadCollisionObjects("Content/indoor.tmx");
        _cuttable = LoadCuttableObjects("Content/indoor.tmx");
        _pickable = LoadMushroomObjects("Content/indoor.tmx");
        _beehive = LoadBeeHiveObjects("Content/indoor.tmx");
        _door = LoadDoor("Content/indoor.tmx");
        _item = LoadItem("Content/indoor.tmx");
    }

    public void Update(GameTime gameTime)
    {
        _player.Update(gameTime, _collision, _camera);
        _camera.Follow(_player.Position, new Vector2(_map.Width * 16, _map.Height * 16));

        _hand = GameData.Hand;
        _wood = GameData.Wood;
        _honey = GameData.Honey;
        _mushroom = GameData.Mushroom;
        _key = GameData.Key;

        _player.Speed = GameData.Speed;

        bool cutting = false;
        bool picking = false;
        bool stoling = false;

        double elapsed = gameTime.ElapsedGameTime.TotalSeconds * 1000;

        if(_break > 0)
        {
            _break -= elapsed;
        }

        KeyboardState state = Keyboard.GetState();

        foreach(Rectangle tree in _cuttable)
        {
            if(_player.Hitbox.Intersects(tree) && state.IsKeyDown(Keys.E) && _hand == false)
            {
                cutting = true;
                _breakcountdown = true;

                if(_break <= 0)
                {   
                    _hand = true;
                    _wood = true;
                    _deleteCuttable = tree;
                }
            }
        }

        foreach(Rectangle mushroom in _pickable)
        {
            if(_player.Hitbox.Intersects(mushroom) && state.IsKeyDown(Keys.E) && _hand == false)
            {
                picking = true;
                _breakcountdown = true;

                if(_break <= 0)
                {
                    _hand = true;
                    _mushroom = true;
                    _deleteMushroom = mushroom;
                }
            }
        }

        foreach(Rectangle honey in _beehive)
        {
            if(_player.Hitbox.Intersects(honey) && state.IsKeyDown(Keys.E) && _hand == false)
            {
                stoling = true;
                _breakcountdown = true;

                if(_break <= 0)
                {
                    _hand = true;
                    _honey = true;
                    _deleteHoney = honey;
                }
            }
        }

        _cuttable.Remove(_deleteCuttable);
        _pickable.Remove(_deleteMushroom);
        _beehive.Remove(_deleteHoney);

        if(!cutting && !picking && !stoling)
        {
            _break = GameData.Break;
            _breakcountdown = false;
        }

        if(_player.Hitbox.Intersects(_door) && state.IsKeyDown(Keys.E) && !GameData.previous.IsKeyDown(Keys.E))
        {
            _sceneManager.ChangeScene("house");
        }

        if(_player.Hitbox.Intersects(_item) && state.IsKeyDown(Keys.E) && _hand == true)
        {
            if(_wood)
            {
                if(_woodSum < 3)
                {
                    _woodSum++;
                    _wood = false;
                    _hand = false;
                    if(_woodSum == 3 && _object == 0)
                    {
                        _object++;
                    }
                } else {
                    _wood = false;
                    _hand = false;
                }
            }

            if(_mushroom)
            {
                if(_mushroomSum < 3 && _object == 1)
                {
                    _mushroomSum++;
                    _mushroom = false;
                    _hand = false;

                    if(_mushroomSum == 3 && _object == 1)
                    {
                        _object++;
                    }
                } else {
                    _mushroom = false;
                    _hand = false;
                }
            }

            if(_honey)
            {
                if(_honeySum < 3 && _object == 2)
                {
                    _honeySum++;
                    _honey = false;
                    _hand = false;

                    if(_honeySum == 3 && _object == 2)
                    {
                        _object++;
                    }
                } else {
                    _honey = false;
                    _hand = false;
                }
            }

            if(_key)
            {
                if(_keySum < 1 && _object == 3)
                {
                    _keySum++;
                    _key = false;
                    _hand = false;

                    if(_keySum == 1 && _object == 3)
                    {
                        _sceneManager.ChangeScene("ending");
                    }
                } else {
                    _key = false;
                    _hand = false;
                }
            }
        }

        GameData.Hand = _hand;
        GameData.Wood = _wood;
        GameData.Honey = _honey;
        GameData.Mushroom = _mushroom;
        GameData.Key = _key;

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

        if(_hand)
        {
            if(_wood)
            {
                spriteBatch.Draw(_stick, new Vector2(_player.screenPos.X + 40, _player.screenPos.Y + 10), null, Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.3f);
            }

            if(_mushroom)
            {
                spriteBatch.Draw(_piecemushroom, new Vector2(_player.screenPos.X + 40, _player.screenPos.Y + 10), null, Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.3f);
            }

            if(_honey)
            {
                spriteBatch.Draw(_honeyjar, new Vector2(_player.screenPos.X + 40, _player.screenPos.Y + 10), null, Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.3f);
            }

            if(_key)
            {
                spriteBatch.Draw(_keyTexture, new Vector2(_player.screenPos.X + 40, _player.screenPos.Y + 10), null, Color.White, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.3f);
            }
        }

        if(_breakcountdown == true)
        {

        Texture2D circle = CreateSegmentedCircle(_graphics, 15, GameData.Break, (int)_break, Color.Gray);
        spriteBatch.Draw(circle, new Vector2(_player.screenPos.X + 75, _player.screenPos.Y), null, Color.White, 0f, new Vector2(circle.Width / 2, circle.Height / 2), 1f, SpriteEffects.None, 0.3f);
        }
        
        Vector2 ObjectM = Vector2.Zero;

        if(_object == 0)
        {
            ObjectM = _pixelfont.MeasureString($"Bring Wood 3/{_woodSum}");
            Vector2 Object = _camera.WorldToScreen(new Vector2(984, 935)) - new Vector2(ObjectM.X / 2, ObjectM.Y / 2);
            
            spriteBatch.DrawString(_pixelfont, $"Bring Wood 3/{_woodSum}", Object, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.2f);
        }

        if(_object == 1)
        {
            ObjectM = _pixelfont.MeasureString($"Bring Mushroom 3/{_mushroomSum}");
            Vector2 Object = _camera.WorldToScreen(new Vector2(984, 935)) - new Vector2(ObjectM.X / 2, ObjectM.Y / 2);
            
            spriteBatch.DrawString(_pixelfont, $"Bring Mushroom 3/{_mushroomSum}", Object, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.2f);
        }

        if(_object == 2)
        {
            ObjectM = _pixelfont.MeasureString($"Stole Honey 3/{_honeySum}");
            Vector2 Object = _camera.WorldToScreen(new Vector2(984, 935)) - new Vector2(ObjectM.X / 2, ObjectM.Y / 2);
            
            spriteBatch.DrawString(_pixelfont, $"Stole Honey 3/{_honeySum}", Object, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.2f);
        }

        if(_object == 3)
        {
            ObjectM = _pixelfont.MeasureString($"Bring Key 1/{_keySum}");
            Vector2 Object = _camera.WorldToScreen(new Vector2(984, 935)) - new Vector2(ObjectM.X / 2, ObjectM.Y / 2);
            
            spriteBatch.DrawString(_pixelfont, $"Bring Key 1/{_keySum}", Object, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.2f);
        }

    }

    Texture2D CreateSegmentedCircle(GraphicsDevice _graphics, int radius, int totalSegments, int filledSegments, Color color)
    {
        int diameter = radius * 2;
        Texture2D texture = new Texture2D(_graphics, diameter, diameter);
        Color[] colorData = new Color[diameter * diameter];

        Vector2 center = new Vector2(radius, radius);

        for(int y = 0; y < diameter; y++)
        {
            for(int x = 0; x < diameter; x++)
            {
                Vector2 pos = new Vector2(x,y) - center;
                float distance = pos.Length();

                if(distance <= radius)
                {
                    float angle = (float)Math.Atan2(pos.Y, pos.X);

                    if(angle < 0)
                    {
                        angle += MathHelper.TwoPi;
                    }

                        int segment = (int)(angle / (MathHelper.TwoPi / totalSegments));
                        if(segment < filledSegments)
                        {
                            colorData[x + y * diameter] = color;
                        } else {
                            colorData[x + y * diameter] = Color.Transparent;
                        }
                    } else {
                        colorData[x + y * diameter] = Color.Transparent;
                }
            }
        }

        texture.SetData(colorData);
        return texture;
    }
}