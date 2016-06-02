using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace cmdr.WpfControls.Adorners
{
    class InsertAdorner : Adorner
    {
        private static Brush BRUSH = new SolidColorBrush(Color.FromRgb(0xFF,0xB8,0xB8));
        private static double THICKNESS = 2;

        private static Pen PEN = new Pen(BRUSH, THICKNESS);

        private bool _above;


        public InsertAdorner(UIElement adornedElement, bool above)
            : base(adornedElement)
        {
            _above = above;

            if (!PEN.IsFrozen)
                PEN.Freeze();

            IsHitTestVisible = false; // no mouse interaction with adorner
        }


        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            Rect adornedElementRect = new Rect(AdornedElement.RenderSize);
            drawingContext.DrawLine(PEN, _above ? adornedElementRect.TopLeft : adornedElementRect.BottomLeft, _above ? adornedElementRect.TopRight : adornedElementRect.BottomRight);
        }
    }
}
