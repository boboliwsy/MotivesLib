namespace SimsMotivePrototype
{
    public struct Rect
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Height { get; set; }
        public float Width { get; set; }

        public Rect (float x, float y, float height, float width)
        {
            X = x;
            Y = y;
            Height = height;
            Width = width;
        }
    }
}
