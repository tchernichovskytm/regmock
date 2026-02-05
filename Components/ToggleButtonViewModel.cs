using Microsoft.Maui.Graphics;
using regmock.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace regmock.Components
{
    public class ToggleButtonViewModel : ViewModelBase
    {
        #region Properties
        private bool isToggled { get; set; }

        public bool IsToggled
        {
            get
            {
                return isToggled;
            }
            set
            {
                isToggled = value;
                OnPropertyChanged(nameof(IsToggled));

                MainCanvas.IsToggled = isToggled;
            }
        }

        private ToggleCanvas mainCanvas { get; set; }

        public ToggleCanvas MainCanvas
        {
            get
            {
                return mainCanvas;
            }
            set
            {
                mainCanvas = value;
                OnPropertyChanged(nameof(MainCanvas));
            }
        }
        #endregion

        #region Classes
        public class ToggleCanvas : IDrawable
        {
            public bool IsToggled { get; set; }

            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                //canvas.StrokeColor = Colors.Green;
                //canvas.StrokeSize = 2;
                //canvas.FillColor = Colors.Green;
                //canvas.DrawRoundedRectangle(
                //    0, dirtyRect.Height / 4,
                //    dirtyRect.Width, dirtyRect.Height / 2,
                //    32
                //);

                SolidPaint solidPaint = new SolidPaint(Colors.Green);
                if (IsToggled == false) solidPaint = new SolidPaint(Colors.Red);

                RectF solidRectangle = new RectF(
                    0, dirtyRect.Height / 4,
                    dirtyRect.Width, dirtyRect.Height / 2
                );
                canvas.SetFillPaint(solidPaint, solidRectangle);
                //canvas.SetShadow(new SizeF(0, 0), 10, Colors.Grey);
                canvas.FillRoundedRectangle(solidRectangle, dirtyRect.Height / 4);

                canvas.SetFillPaint(new SolidPaint(Colors.Aqua), RectF.Zero);
                canvas.FillCircle(dirtyRect.Width / 2, dirtyRect.Height / 2, (float)(dirtyRect.Height / 4 / 1.5));
                //canvas.DrawCircle(dirtyRect.Width / 2, dirtyRect.Height / 2, (float)(dirtyRect.Height / 4 / 1.5));

                //PathF path = new PathF();
                //path.MoveTo(40, 10);
                //path.LineTo(70, 80);
                //path.LineTo(10, 50);
                //path.Close();
                //canvas.StrokeColor = Colors.Green;
                //canvas.StrokeSize = 1;
                //canvas.DrawPath(path);

                // Read more here: https://learn.microsoft.com/en-us/dotnet/maui/user-interface/graphics/draw?view=net-maui-10.0
            }
        }
        #endregion

        #region Commands
        #endregion

        #region Constructor
        public ToggleButtonViewModel()
        {
            MainCanvas = new ToggleCanvas();
        }
        #endregion

        #region Functions
        #endregion
    }
}
