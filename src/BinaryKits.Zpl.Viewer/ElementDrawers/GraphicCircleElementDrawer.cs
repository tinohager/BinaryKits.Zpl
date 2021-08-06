﻿using BinaryKits.Zpl.Label.Elements;

namespace BinaryKits.Zpl.Viewer.ElementDrawers
{
    public class GraphicCircleElementDrawer : ElementDrawerBase
    {
        ///<inheritdoc/>
        public override bool CanDraw(ZplElementBase element)
        {
            return element is ZplGraphicCircle;
        }

        ///<inheritdoc/>
        public override void Draw(ZplElementBase element)
        {
            if (element is ZplGraphicCircle graphicCircle)
            {
                this._skPaint.StrokeWidth = graphicCircle.BorderThickness;

                var radius = graphicCircle.Diameter / 2;
                var offset = (graphicCircle.BorderThickness / 2) + radius;

                var x = graphicCircle.PositionX + this._padding + offset;
                var y = graphicCircle.PositionY + this._padding + offset;

                this._skCanvas.DrawCircle(x, y, radius, this._skPaint);
            }
        }
    }
}