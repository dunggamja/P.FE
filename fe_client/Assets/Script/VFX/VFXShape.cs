using UnityEngine;



public class VFXShape : VFXObject
{
    public new class Param : VFXObject.Param
    {
        public Color StartColor { get; set; } = Color.white;
        public Color EndColor   { get; set; } = Color.white;

        public override void Reset()
        {
            base.Reset();
            StartColor = Color.white;
            EndColor   = Color.white;
        }

        public Param FillColor(Color _color_start, Color _color_end)
        {
            StartColor = _color_start;
            EndColor   = _color_end;
            return this;
        }

        public override void Apply(VFXObject _vfx_object)
        {
            base.Apply(_vfx_object);

            _vfx_object.FillColor(StartColor, EndColor);
        }
    }
    
    public Shapes.Rectangle[] m_rectangles;


    public override void FillColor(Color _color_start, Color _color_end)
    {
        if (m_rectangles != null)
        {
            foreach (var e in m_rectangles)
            {
                if (e == null)
                    continue;

                var fill = e.Fill;

                fill.colorStart = _color_start;
                fill.colorEnd   = _color_end;

                e.Fill = fill;
            }
        }
    }
}
