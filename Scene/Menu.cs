using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Replay;

public class Menu : IScene
{
    private GraphicsDevice _graphics;
    private SceneManager _sceneManager;
    private ContentManager _content;

    private SpriteFont _pixelfont;
    
    private int _selected = 0;
    private bool _blinking = false;

    private double _blinkingCountdown;

    public Menu(GraphicsDevice _graphics, SceneManager _sceneManager, ContentManager _content)
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

        double elapsed = gameTime.ElapsedGameTime.TotalSeconds * 1000;

        if(_blinkingCountdown <= 0)
        {
            _blinkingCountdown = 300;
            _blinking = !_blinking;
        } else {
            _blinkingCountdown -= elapsed;
        }

        if((state.IsKeyDown(Keys.Up) && !GameData.previous.IsKeyDown(Keys.Up)) || (state.IsKeyDown(Keys.Down) && !GameData.previous.IsKeyDown(Keys.Down)))
        {
            if(_selected == 0)
            {
                _selected = 1;
            } else {
                _selected = 0;
            }
        }

        if(state.IsKeyDown(Keys.Enter) && !GameData.previous.IsKeyDown(Keys.Enter))
        {
            if(_selected == 0)
            {

            } else {
                GameData.Exit = true;
            }
        }

        GameData.previous = state;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _graphics.Clear(Color.OliveDrab);

        int Width = _graphics.Viewport.Width;
        int Height = _graphics.Viewport.Height;

        string playText = "";
        string quitText = "";

        if(_selected == 0)
        {
            if(_blinking == true)
            {
                playText = "> Play <";;
            } else {
                playText = "Play";
            }

            quitText = "Quit";
        } else {

            if(_blinking == true)
            {
                quitText = "> Quit <";;
            } else {
                quitText = "Quit";
            }

            playText = "Play"; 
        }
        
        Vector2 TitleM = _pixelfont.MeasureString("Replay");
        Vector2 Title = new Vector2((Width / 2) - (TitleM.X / 2), (Height / 4) - (TitleM.Y / 2));

        spriteBatch.DrawString(_pixelfont, "Replay", Title, Color.White);

        Vector2 PlayM = _pixelfont.MeasureString(playText);
        Vector2 Play = new Vector2((Width / 2) - (PlayM.X / 2), (Height / 4) - (PlayM.Y / 2) + 100);

        spriteBatch.DrawString(_pixelfont, playText, Play, Color.White);

        Vector2 QuitM = _pixelfont.MeasureString(quitText);
        Vector2 Quit = new Vector2((Width / 2) - (QuitM.X / 2), (Height / 4) - (QuitM.Y / 2) + 150);

        spriteBatch.DrawString(_pixelfont, quitText, Quit, Color.White);
    }
}