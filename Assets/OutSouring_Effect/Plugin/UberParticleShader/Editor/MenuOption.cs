namespace UberParticleShader.Editor
{
    public class MenuOption
    {
        public string Name { get; }
        public bool showHide { get; }
        public System.Action Callback { get; }

        public MenuOption(string name, bool showHideVal, System.Action callback)
        {
            Name = name;
            showHide = showHideVal;
            Callback = callback;
        }
    }
}