using Godot;

static class Util
{
    public static float MyLuminance(this Color c)
    {
        return 0.299f * c.R + 0.587f * c.G + 0.114f * c.B;
    }
}