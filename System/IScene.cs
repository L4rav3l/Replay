using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Replay;

public interface IScene
{
    public void LoadContent();

    public void Update(GameTime gameTime);

    public void Draw(SpriteBatch spriteBatch);
}