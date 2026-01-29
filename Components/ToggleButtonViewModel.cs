using regmock.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace regmock.Components
{
    internal class ToggleButtonViewModel : ViewModelBase
    {
        #region Classes
        public class ToggleCanvas : IDrawable
        {
            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                //canvas.StrokeColor = Colors.Red;
                //canvas.StrokeSize = 4;
                //canvas.DrawLine(0, 0, 100, 100);

                canvas.StrokeColor = Colors.YellowGreen;
                canvas.StrokeSize = 2;
                canvas.DrawEllipse(0, 0, 50, 50);

                //canvas.FillColor = Colors.BlueViolet;
                //canvas.FillEllipse(10, 60, 150, 50);

                //canvas.FillColor = Colors.DarkBlue;
                //canvas.FillRectangle(0, 0, 20, 40);

                //canvas.StrokeColor = Colors.Green;
                //canvas.StrokeSize = 4;
                //canvas.DrawRoundedRectangle(60, 1, 60, 60, 4);

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
