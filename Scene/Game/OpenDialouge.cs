using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Replay;

public class OpenDialouge : IScene
{
    private GraphicsDevice _graphics;
    private SceneManager _sceneManager;
    private ContentManager _content;

    private SpriteFont _pixelfont;
    private Texture2D _dialouge;
    private Texture2D _wizard;

    private string _text = "In Hynn Province, every 18-year-old\nboy and girl must build a house\nwithout some of the framework.\nThis includes slower movement, slower\ncutting, and other challenges.\nGood luck, sir or madam!\nPRESS E KEY";
    private double _textCountdown;
    private int _textInt;

    public OpenDialouge(GraphicsDevice _graphics, SceneManager _sceneManager, ContentManager _content)
    {
        this._graphics = _graphics;
        this._sceneManager = _sceneManager;
        this._content = _content;
    }

    public void LoadContent()
    {
        _pixelfont = _content.Load<SpriteFont>("pixelfont");
        _wizard = _content.Load<Texture2D>("Wizard");

        _dialouge = new Texture2D(_graphics, 1, 1);
        _dialouge.SetData(new [] {Color.White});
    }

    public void Update(GameTime gameTime)
    {
        KeyboardState state = Keyboard.GetState();

        double elapsed = gameTime.ElapsedGameTime.TotalSeconds * 1000;

        if(_textCountdown <= 0)
        {
            _textCountdown = 100;
            _textInt++;
        } else {
            _textCountdown -= elapsed;
        }

        if(state.IsKeyDown(Keys.E) && !GameData.previous.IsKeyDown(Keys.E))
        {
            GameData.Move = true;
            _sceneManager.AddScene(new Indoor(_graphics, _sceneManager, _content), "indoor");
            
            _sceneManager.ChangeScene("indoor");
        }

        GameData.previous = state;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _graphics.Clear(Color.Black);

        int Width = _graphics.Viewport.Width;
        int Height = _graphics.Viewport.Height;

        Vector2 Wizard = new Vector2((Width / 2) - 35, (Height / 2) - 49);

        spriteBatch.Draw(_wizard, Wizard, Color.White);

        Vector2 Paper = new Vector2((Width / 2) - 350, ((Height / 4) * 3) - 200);

        spriteBatch.Draw(_dialouge, new Rectangle((int)Paper.X, (int)Paper.Y, 700, 400), null, Color.White);

        int length = Math.Min(_textInt, _text.Length);
        string text = _text.Substring(0, length);

        Vector2 Text = new Vector2((Width / 2) - 330, ((Height / 4) * 3) - 175);

        spriteBatch.DrawString(_pixelfont, text, Text, Color.Black, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0.2f);
    }
}