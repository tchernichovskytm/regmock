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
                MainCanvas.IsToggled = isToggled;
                //MainCanvas = new ToggleCanvas { IsToggled = isToggled };
            }
        }

        private float rectRadius;

        public float RectRadius
        {
            get
            {
                return rectRadius;
            }
            set
            {
                rectRadius = value;
                OnPropertyChanged(nameof(RectRadius));

                MainCanvas.RectRadius = rectRadius;
            }
        }

        private Paint onBackground;

        public Paint OnBackground
        {
            get
            {
                return onBackground;
            }
            set
            {
                onBackground = value;
                OnPropertyChanged(nameof(OnBackground));

                MainCanvas.OnBackground = onBackground;
            }
        }

        private Paint offBackground;

        public Paint OffBackground
        {
            get
            {
                return offBackground;
            }
            set
            {
                offBackground = value;
                OnPropertyChanged(nameof(OffBackground));

                MainCanvas.OffBackground = offBackground;
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
            public float RectRadius { get; set; }
            public Paint OnBackground { get; set; }
            public Paint OffBackground { get; set; }

            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                // Colors:
                //      Inside Color
                //      Border Color
                //      Circle Color
                //Color insideColor = IsToggled ? Colors.Green : Colors.Red;
                LinearGradientPaint insideGradient = new LinearGradientPaint()
                {
                    StartColor = Colors.Green,
                    EndColor = Colors.Aqua,
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(0, 1),
                };
                //insideGradient.AddOffset(0.0f, Colors.Green);
                //insideGradient.AddOffset(1.0f, Colors.Aqua);
                Color borderColor = !IsToggled ? Colors.Green : Colors.Red;
                Color circleColor = Colors.White;

                float x = 0;
                float y = 0;
                float width = dirtyRect.Width;
                float height = dirtyRect.Height;
                float borderSize = 0;
                float rectRadius = RectRadius;

                // Calculations
                {
                    x += borderSize;
                    y += borderSize;
                    width -= borderSize * 2;
                    height -= borderSize * 2;
                }

                float circleRadius = height / 2 * 0.666f;
                float circleX = x + circleRadius + rectRadius / 2;

                //canvas.FillColor = insideColor;
                canvas.SetFillPaint(IsToggled ? OnBackground : OffBackground, dirtyRect);
                canvas.FillRoundedRectangle(
                    x, y,
                    width, height,
                    rectRadius
                );

                //canvas.StrokeColor = borderColor;
                //canvas.StrokeSize = borderSize;
                //canvas.DrawRoundedRectangle(
                //    x - borderSize / 2, y - borderSize / 2,
                //    width + borderSize, height + borderSize,
                //    rectRadius
                //);

                if (IsToggled) circleX = width + borderSize * 2 - circleX;

                canvas.FillColor = circleColor;
                canvas.FillCircle(circleX, y + height / 2, circleRadius);

                // Read more here: https://learn.microsoft.com/en-us/dotnet/maui/user-interface/graphics/draw?view=net-maui-10.0
            }
        }
        #endregion

        #region Commands
        public ICommand ToggleCmd { get; set; }
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
