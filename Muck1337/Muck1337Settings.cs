namespace Muck1337
{
    public class Muck1337Settings
    {
        public Keybinds KeybindsInstance { get; } = new Keybinds();
        public Toggles TogglesInstance { get; } = new Toggles();
        
        public class Keybinds
        {
            public string KeyGUI { get; set; } = "RightShift";
            public string KeyFly { get; set; } = "F";
        }
        
        public class Toggles
        {
            public bool Advancements { get; set; } = false;
            public bool Invincibility { get; set; } = true;
        }
    }
}