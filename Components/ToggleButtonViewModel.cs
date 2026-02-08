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

                // TODO: this is a very stupid hack, but it works
                //MainCanvas.IsToggled = isToggled;
                MainCanvas = new ToggleCanvas { IsToggled = isToggled };
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
                // Colors:
                //      Inside Color
                //      Border Color
                //      Circle Color
                Color insideColor = IsToggled ? Colors.Green : Colors.Red;
                Color borderColor = !IsToggled ? Colors.Green : Colors.Red;
                Color circleColor = Colors.White;

                float rectRadius = dirtyRect.Height / 4;
                canvas.FillColor = insideColor;
                canvas.FillRoundedRectangle(
                    0, dirtyRect.Height / 4,
                    dirtyRect.Width, dirtyRect.Height / 2,
                    rectRadius
                );

                float borderSize = 16;
                canvas.StrokeColor = borderColor;
                canvas.StrokeSize = borderSize;
                canvas.DrawRoundedRectangle(
                    0, dirtyRect.Height / 4,
                    dirtyRect.Width + borderSize / 2, dirtyRect.Height / 2 + borderSize / 2,
                    rectRadius
                );

                float circleRadius = dirtyRect.Height / 4 / 1.5f;
                float circleX = IsToggled ? dirtyRect.Width - (circleRadius + rectRadius / 2) : (circleRadius + rectRadius / 2);
                canvas.FillColor = circleColor;
                canvas.FillCircle(circleX, dirtyRect.Height / 2, circleRadius);

                // Read more here: https://learn.microsoft.com/en-us/dotnet/maui/user-interface/graphics/draw?view=net-maui-10.0
            }
        }
        #endregion

        #region Commands
        public Command ToggleCmd { get; set; }
        #endregion

        #region Constructor
        public ToggleButtonViewModel(Command ToggleCmd)
        {
            MainCanvas = new ToggleCanvas();

            this.ToggleCmd = ToggleCmd;
        }
        #endregion

        #region Functions

        #endregion
    }
}
