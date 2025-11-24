using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Replay;

public class Ending : IScene
{
    private GraphicsDevice _graphics;
    private SceneManager _sceneManager;
    private ContentManager _content;

    private SpriteFont _pixelfont;

    public Ending(GraphicsDevice _graphics, SceneManager _sceneManager, ContentManager _content)
    {
        this._graphics = _graphics;
        this._sceneManager = _sceneManager;
        this._content = _content;
    }
    
    public void LoadContent()
    {
        _pixelfont = _content.Load<SpriteFont>("pixelfont");
    }

    public void Update(GameTime gameTime)
    {
        KeyboardState state = Keyboard.GetState();

        if(state.IsKeyDown(Keys.E) && !GameData.previous.IsKeyDown(Keys.E))
        {
            _sceneManager.ChangeScene("menu");
        }

        GameData.previous = state;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        int Width = _graphics.Viewport.Width;
        int Height = _graphics.Viewport.Height;

        _graphics.Clear(Color.Black);

        Vector2 TextM = _pixelfont.MeasureString("You built the house, thus completing the trial.");
        Vector2 Text = new Vector2((Width / 2) - (TextM.X / 2), (Height / 2) - (TextM.Y / 2));

        spriteBatch.DrawString(_pixelfont, "You built the house, thus completing the trial.\nThe province of Hynn rewarded you with 3,000 gold coins.", Text, Color.White);
    }
}