using Microsoft.Maui.Graphics;
using regmock.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private float circleRadius;
        public float CircleRadius
        {
            get
            {
                return circleRadius;
            }
            set
            {
                circleRadius = value;
                OnPropertyChanged(nameof(CircleRadius));

                MainCanvas.CircleRadius = circleRadius;
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
            private GraphicsView GFX { get; set; }

            private float positionX = 0.0f; // 0 to 1
            private float targetX = 0.0f; // 0 to 1

            private bool isToggled;
            public bool IsToggled
            {
                get
                {
                    return isToggled;
                }
                set
                {
                    isToggled = value;
                    targetX = value ? 1.0f : 0.0f;
                    if (GFX.IsLoaded)
                    {
                        AnimateTo(targetX);
                    }
                    else
                    {
                        positionX = targetX;
                    }
                }
            }
            public float RectRadius { get; set; }
            public float CircleRadius { get; set; }
            public Paint OnBackground { get; set; }
            public Paint OffBackground { get; set; }

            public ToggleCanvas(GraphicsView GFX)
            {
                this.GFX = GFX;

                positionX = isToggled ? 1.0f : 0.0f;
            }

            public void AnimateTo(float target)
            {
                float start = positionX;

                var animation = new Animation(
                    v =>
                    {
                        positionX = (float)v;
                        Redraw();
                    },
                    start,
                    target
                );

                animation.Commit(
                    owner: this.GFX,
                    name: "ToggleAnim",
                    length: 500, // milliseconds
                    easing: Easing.SinInOut
                );
            }

            public event Action? RequestCanvasRedraw;

            private void Redraw()
            {
                //RequestCanvasRedraw?.Invoke();
                this.GFX.Invalidate();
            }

            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                LinearGradientPaint insideGradient = new LinearGradientPaint()
                {
                    StartColor = Colors.Green,
                    EndColor = Colors.Aqua,
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(0, 1),
                };

                Color borderColor = !IsToggled ? Colors.Green : Colors.Red;
                Color circleColor = Colors.White;

                float width = dirtyRect.Width;
                float height = dirtyRect.Height;
                float rectRadius = RectRadius;

                canvas.SetFillPaint(IsToggled ? OnBackground : OffBackground, dirtyRect);
                canvas.FillRoundedRectangle(
                    0, 0,
                    width, height,
                    rectRadius
                );

                float circleRadius;
                if (CircleRadius == 0)
                {
                    circleRadius = height / 2 * 0.666f;
                }
                else
                {
                    circleRadius = CircleRadius;
                }
                float circleXStart = circleRadius + rectRadius / 2;
                float circleXEnd = width - circleRadius - rectRadius / 2;

                float circleX = circleXStart + (circleXEnd - circleXStart) * positionX;

                canvas.SetFillPaint(new SolidPaint(circleColor), dirtyRect);
                canvas.FillCircle(circleX, height / 2, circleRadius);

                // Read more here: https://learn.microsoft.com/en-us/dotnet/maui/user-interface/graphics/draw?view=net-maui-10.0
            }
        }
        #endregion

        #region Commands
        public ICommand ToggleCmd { get; set; }
        #endregion

        #region Constructor
        public ToggleButtonViewModel(Command ToggleCmd, GraphicsView GFX)
        {
            MainCanvas = new ToggleCanvas(GFX);

            this.ToggleCmd = ToggleCmd;
        }
        #endregion

        #region Functions

        #endregion
    }
}
