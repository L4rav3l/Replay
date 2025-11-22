using System.Collections.Generic;

namespace Replay;

public class SceneManager
{
    private Dictionary<string, IScene> scenes;
    private IScene CurrentScene;

    public SceneManager()
    {
        scenes = new ();
    }

    public void AddScene(IScene iscene, string name)
    {
        iscene.LoadContent();

        scenes[name] = iscene;
    }

    public void ChangeScene(string name)
    {
        CurrentScene = scenes[name];
    }

    public IScene GetCurrentScene()
    {
        return CurrentScene;
    }
}    