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
            private Thread interpolateThread;
            public ToggleCanvas()
            {
                interpolateThread = new Thread(UpdateLoop);
                interpolateThread.Start();

                positionX = isToggled ? 1.0f : 0.0f;
            }

            public event Action? RequestCanvasRedraw;

            private void Redraw()
            {
                RequestCanvasRedraw?.Invoke();
            }

            private void UpdateLoop()
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                UInt64 last = (UInt64)stopwatch.ElapsedTicks;
                while (true)
                {
                    UInt64 cur = (UInt64)stopwatch.ElapsedTicks;
                    double deltaTime = (double)(cur - last) / Stopwatch.Frequency / 1000;
                    //double deltaTime = 0.05;
                    bool redraw = false;
                    if (positionX < targetX)
                    {
                        positionX = SmoothStep(positionX, deltaTime, StepMode.Forward);
                        redraw = true;
                    }
                    else if (positionX > targetX)
                    {
                        positionX = SmoothStep(positionX, deltaTime, StepMode.Backward);
                        redraw = true;
                    }
                    if (redraw) Redraw();
                    Thread.Sleep(1);
                }
            }

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
                }
            }
            public float RectRadius { get; set; }
            public Paint OnBackground { get; set; }
            public Paint OffBackground { get; set; }

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

                float circleRadius = height / 2 * 0.666f;
                float circleXStart = circleRadius + rectRadius / 2;
                float circleXEnd = width - circleRadius - rectRadius / 2;

                float circleX = circleXStart + (circleXEnd - circleXStart) * positionX;

                canvas.FillColor = circleColor;
                canvas.FillCircle(circleX, height / 2, circleRadius);

                // Read more here: https://learn.microsoft.com/en-us/dotnet/maui/user-interface/graphics/draw?view=net-maui-10.0
            }

            private float positionOfTime(float time)
            {
                // 0 <= time <= 1
                return (float)Math.Sin(time * Math.PI / 2.0f);
            }

            private float timeOfPosition(float x)
            {
                // 0 <= x <= 1
                return (float)((float)Math.Asin(x) * 2.0f / Math.PI);
            }

            enum StepMode
            {
                Forward,
                Backward,
            }

            private float SmoothStep(float x, double deltaTime, StepMode mode)
            {
                // 0 <= x <= 1
                // 0 <= deltaTime <= 1
                if (mode == StepMode.Backward)
                {
                    deltaTime = -deltaTime;
                }
                return positionOfTime((float)Math.Clamp(timeOfPosition(x) + deltaTime, 0, 1));
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
