using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WinForms;
using SkiaSharp;

namespace LiveCharts2Demo
{
    public partial class Form1 : Form
    {
        private CartesianChart cartesianChart;
        private PieChart pieChart;
        private GeoMap geoMap;
        private Panel chartPanel;
        private Panel controlPanel;
        private readonly Random random = new();

        // Control elements
        private TrackBar animationSpeedTrackBar;
        private Label animationSpeedLabel;
        private ComboBox easingComboBox;
        private Button refreshDataButton;
        private Button clearChartButton;
        private CheckBox enableAnimationsCheckBox;
        private Label easingLabel;

        // Track current chart type
        private string currentChartType = "Line";

        public Form1()
        {
            SetupUI();
            ShowLineChart(); // Default chart

            Text = "LiveCharts2 WinForms Demo - .NET 9";
            Size = new Size(1200, 800);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(240, 240, 240);
        }

        private void SetupUI()
        {
            // Create main panels
            controlPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 250,
                BackColor = Color.FromArgb(250, 250, 250),
                Padding = new Padding(10)
            };

            chartPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            // Setup charts
            cartesianChart = new CartesianChart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            pieChart = new PieChart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Visible = false
            };

            geoMap = new GeoMap
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Visible = false
            };

            chartPanel.Controls.Add(cartesianChart);
            chartPanel.Controls.Add(pieChart);
            chartPanel.Controls.Add(geoMap);

            CreateControlElements();

            // Add panels to form
            Controls.Add(chartPanel);
            Controls.Add(controlPanel);
        }
        private void CreateControlElements()
        {
            int yPos = 10;
            int spacing = 45;

            // Chart type buttons
            Label chartTypesLabel = new()
            {
                Text = "Chart Types:",
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(10, yPos),
                Size = new Size(200, 25)
            };
            controlPanel.Controls.Add(chartTypesLabel);
            yPos += 30;

            CreateButton("Line Chart", new Point(10, yPos), ShowLineChart);
            yPos += spacing;

            CreateButton("Bar Chart", new Point(10, yPos), ShowBarChart);
            yPos += spacing;

            CreateButton("Column Chart", new Point(10, yPos), ShowColumnChart);
            yPos += spacing;

            CreateButton("Area Chart", new Point(10, yPos), ShowAreaChart);
            yPos += spacing;

            CreateButton("Scatter Chart", new Point(10, yPos), ShowScatterChart);
            yPos += spacing;

            CreateButton("Pie Chart", new Point(10, yPos), ShowPieChart);
            yPos += spacing;

            CreateButton("Geo Map", new Point(10, yPos), ShowGeoMap);
            yPos += spacing + 20;

            // Animation controls
            Label animationLabel = new()
            {
                Text = "Animation Controls:",
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(10, yPos),
                Size = new Size(200, 25)
            };
            controlPanel.Controls.Add(animationLabel);
            yPos += 30;

            enableAnimationsCheckBox = new CheckBox
            {
                Text = "Enable Animations",
                Location = new Point(10, yPos),
                Size = new Size(150, 25),
                Checked = true
            };
            enableAnimationsCheckBox.CheckedChanged += OnAnimationToggled;
            controlPanel.Controls.Add(enableAnimationsCheckBox);
            yPos += 35;

            animationSpeedLabel = new Label
            {
                Text = "Animation Speed: 1000ms",
                Location = new Point(10, yPos),
                Size = new Size(200, 25)
            };
            controlPanel.Controls.Add(animationSpeedLabel);
            yPos += 25;

            animationSpeedTrackBar = new TrackBar
            {
                Location = new Point(10, yPos),
                Size = new Size(200, 45),
                Minimum = 100,
                Maximum = 3000,
                Value = 1000,
                TickFrequency = 500
            };
            animationSpeedTrackBar.ValueChanged += OnAnimationSpeedChanged;
            controlPanel.Controls.Add(animationSpeedTrackBar);
            yPos += 65;

            easingLabel = new Label
            {
                Text = "Easing Function:",
                Location = new Point(10, yPos),
                Size = new Size(120, 25)
            };
            controlPanel.Controls.Add(easingLabel);
            yPos += 25;

            easingComboBox = new ComboBox
            {
                Location = new Point(10, yPos),
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            easingComboBox.Items.AddRange(
            [
                "Linear", "EaseInQuad", "EaseOutQuad", "EaseInOutQuad",
                "EaseInCubic", "EaseOutCubic", "EaseInOutCubic"
            ]);
            easingComboBox.SelectedIndex = 0;
            easingComboBox.SelectedIndexChanged += OnEasingChanged;
            controlPanel.Controls.Add(easingComboBox);
            yPos += 45;

            refreshDataButton = new Button
            {
                Text = "Refresh Data",
                Location = new Point(10, yPos),
                Size = new Size(200, 35),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            refreshDataButton.Click += OnRefreshData;
            controlPanel.Controls.Add(refreshDataButton);
            yPos += 45;

            clearChartButton = new Button
            {
                Text = "Clear Chart",
                Location = new Point(10, yPos),
                Size = new Size(200, 35),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            clearChartButton.Click += OnClearChart;
            controlPanel.Controls.Add(clearChartButton);
        }
        private Button CreateButton(string text, Point location, Action clickHandler)
        {
            Button button = new()
            {
                Text = text,
                Location = location,
                Size = new Size(200, 35),
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9)
            };
            button.Click += (s, e) => clickHandler();
            controlPanel.Controls.Add(button);
            return button;
        }
        private void ShowCartesianChart()
        {
            cartesianChart.Visible = true;
            pieChart.Visible = false;
            geoMap.Visible = false;
        }
        private void ShowPieChartOnly()
        {
            cartesianChart.Visible = false;
            pieChart.Visible = true;
            geoMap.Visible = false;
        }
        private void ShowGeoMapOnly()
        {
            cartesianChart.Visible = false;
            pieChart.Visible = false;
            geoMap.Visible = true;
        }
        private void ShowLineChart()
        {
            ShowCartesianChart();
            currentChartType = "Line";

            ISeries[] series =
            [
                new LineSeries<double>
                {
                    Values = GenerateRandomData(10),
                    Name = "Series 1",
                    Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 3 },
                    Fill = null,
                    GeometrySize = 8
                },
                new LineSeries<double>
                {
                    Values = GenerateRandomData(10),
                    Name = "Series 2",
                    Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 3 },
                    Fill = null,
                    GeometrySize = 8
                }
            ];

            cartesianChart.Series = series;
        }
        private void ShowBarChart()
        {
            ShowCartesianChart();
            currentChartType = "Bar";

            ISeries[] series =
            [
                new RowSeries<double>
                {
                    Values = GenerateRandomData(8),
                    Name = "Dataset 1",
                    Fill = new SolidColorPaint(SKColors.SkyBlue)
                },
                new RowSeries<double>
                {
                    Values = GenerateRandomData(8),
                    Name = "Dataset 2",
                    Fill = new SolidColorPaint(SKColors.Orange)
                }
            ];

            cartesianChart.Series = series;
        }
        private void ShowColumnChart()
        {
            ShowCartesianChart();
            currentChartType = "Column";

            ISeries[] series =
            [
                new ColumnSeries<double>
                {
                    Values = GenerateRandomData(6),
                    Name = "Q1 Sales",
                    Fill = new SolidColorPaint(SKColors.MediumSeaGreen)
                },
                new ColumnSeries<double>
                {
                    Values = GenerateRandomData(6),
                    Name = "Q2 Sales",
                    Fill = new SolidColorPaint(SKColors.Coral)
                }
            ];

            cartesianChart.Series = series;
        }
        private void ShowAreaChart()
        {
            ShowCartesianChart();
            currentChartType = "Area";

            ISeries[] series =
            [
                new StackedAreaSeries<double>
                {
                    Values = GenerateRandomData(12),
                    Name = "Area 1",
                    Fill = new SolidColorPaint(SKColors.Purple.WithAlpha(100)),
                    Stroke = new SolidColorPaint(SKColors.Purple) { StrokeThickness = 2 }
                },
                new StackedAreaSeries<double>
                {
                    Values = GenerateRandomData(12),
                    Name = "Area 2",
                    Fill = new SolidColorPaint(SKColors.Green.WithAlpha(100)),
                    Stroke = new SolidColorPaint(SKColors.Green) { StrokeThickness = 2 }
                }
            ];

            cartesianChart.Series = series;
        }
        private void ShowScatterChart()
        {
            ShowCartesianChart();
            currentChartType = "Scatter";

            ObservablePoint[] scatterData1 = GenerateScatterData(20);
            ObservablePoint[] scatterData2 = GenerateScatterData(20);

            ISeries[] series =
            [
                new ScatterSeries<ObservablePoint>
                {
                    Values = scatterData1,
                    Name = "Dataset A",
                    Fill = new SolidColorPaint(SKColors.Blue.WithAlpha(150)),
                    Stroke = new SolidColorPaint(SKColors.Blue) { StrokeThickness = 2 },
                    GeometrySize = 12
                },
                new ScatterSeries<ObservablePoint>
                {
                    Values = scatterData2,
                    Name = "Dataset B",
                    Fill = new SolidColorPaint(SKColors.Red.WithAlpha(150)),
                    Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 2 },
                    GeometrySize = 12
                }
            ];

            cartesianChart.Series = series;
        }
        private void ShowPieChart()
        {
            ShowPieChartOnly();
            currentChartType = "Pie";

            ISeries[] pieData =
            [
                new PieSeries<double>
                {
                    Values = [random.Next(10, 50)],
                    Name = "Product A",
                    Fill = new SolidColorPaint(SKColors.Red)
                },
                new PieSeries<double>
                {
                    Values = [random.Next(10, 50)],
                    Name = "Product B",
                    Fill = new SolidColorPaint(SKColors.Blue)
                },
                new PieSeries<double>
                {
                    Values = [random.Next(10, 50)],
                    Name = "Product C",
                    Fill = new SolidColorPaint(SKColors.Green)
                },
                new PieSeries<double>
                {
                    Values = [random.Next(10, 50)],
                    Name = "Product D",
                    Fill = new SolidColorPaint(SKColors.Orange)
                },
                new PieSeries<double>
                {
                    Values = [random.Next(10, 50)],
                    Name = "Product E",
                    Fill = new SolidColorPaint(SKColors.Purple)
                }
            ];

            pieChart.Series = pieData;
        }
        private void ShowGeoMap()
        {
            ShowGeoMapOnly();
            currentChartType = "GeoMap";

            // Create sample heat map data for various countries
            HeatLand[] heatLands = GenerateGeoMapData();

            geoMap.Series =
            [
                new HeatLandSeries
                {
                    Name = "Global Sales Data",
                    Lands = heatLands
                }
            ];

            // Set custom stroke and fill for the map
            geoMap.Stroke = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 1 };
            geoMap.Fill = new SolidColorPaint(SKColors.LightGray);
        }
        private HeatLand[] GenerateGeoMapData()
        {
            // Sample data for various countries using their ISO country codes
            return
            [
                new HeatLand { Name = "usa", Value = random.Next(50, 100) },   // United States
                new HeatLand { Name = "can", Value = random.Next(30, 80) },    // Canada
                new HeatLand { Name = "mex", Value = random.Next(20, 60) },    // Mexico
                new HeatLand { Name = "bra", Value = random.Next(40, 90) },    // Brazil
                new HeatLand { Name = "arg", Value = random.Next(25, 70) },    // Argentina
                new HeatLand { Name = "gbr", Value = random.Next(45, 95) },    // United Kingdom
                new HeatLand { Name = "fra", Value = random.Next(40, 85) },    // France
                new HeatLand { Name = "deu", Value = random.Next(50, 100) },   // Germany
                new HeatLand { Name = "ita", Value = random.Next(35, 75) },    // Italy
                new HeatLand { Name = "esp", Value = random.Next(30, 70) },    // Spain
                new HeatLand { Name = "rus", Value = random.Next(25, 65) },    // Russia
                new HeatLand { Name = "chn", Value = random.Next(60, 100) },   // China
                new HeatLand { Name = "jpn", Value = random.Next(55, 95) },    // Japan
                new HeatLand { Name = "kor", Value = random.Next(45, 85) },    // South Korea
                new HeatLand { Name = "ind", Value = random.Next(40, 80) },    // India
                new HeatLand { Name = "aus", Value = random.Next(35, 75) },    // Australia
                new HeatLand { Name = "zaf", Value = random.Next(20, 60) },    // South Africa
                new HeatLand { Name = "egy", Value = random.Next(15, 55) },    // Egypt
                new HeatLand { Name = "are", Value = random.Next(30, 70) },    // United Arab Emirates
                new HeatLand { Name = "sau", Value = random.Next(25, 65) }     // Saudi Arabia
            ];
        }
        private double[] GenerateRandomData(int count)
        {
            double[] data = new double[count];
            for (int i = 0; i < count; i++)
            {
                data[i] = random.Next(10, 100);
            }
            return data;
        }
        private ObservablePoint[] GenerateScatterData(int count)
        {
            ObservablePoint[] data = new ObservablePoint[count];
            for (int i = 0; i < count; i++)
            {
                data[i] = new ObservablePoint(random.Next(0, 100), random.Next(0, 100));
            }
            return data;
        }
        private void OnAnimationSpeedChanged(object? sender, EventArgs e)
        {
            int speed = animationSpeedTrackBar.Value;
            animationSpeedLabel.Text = $"Animation Speed: {speed}ms";

            // Update animation speed for all charts
            cartesianChart.AnimationsSpeed = TimeSpan.FromMilliseconds(speed);
            pieChart.AnimationsSpeed = TimeSpan.FromMilliseconds(speed);
        }
        private void OnEasingChanged(object? sender, EventArgs e)
        {
            string easingName = easingComboBox.SelectedItem?.ToString() ?? "";
            Func<float, float> easingFunction = GetEasingFunction(easingName);

            cartesianChart.EasingFunction = easingFunction;
            pieChart.EasingFunction = easingFunction;
        }
        private void OnAnimationToggled(object? sender, EventArgs e)
        {
            bool enabled = enableAnimationsCheckBox.Checked;

            if (enabled)
            {
                cartesianChart.AnimationsSpeed = TimeSpan.FromMilliseconds(animationSpeedTrackBar.Value);
                pieChart.AnimationsSpeed = TimeSpan.FromMilliseconds(animationSpeedTrackBar.Value);
            }
            else
            {
                cartesianChart.AnimationsSpeed = TimeSpan.FromMilliseconds(0);
                pieChart.AnimationsSpeed = TimeSpan.FromMilliseconds(0);
            }
        }
        private void OnRefreshData(object? sender, EventArgs e)
        {
            // Refresh the current chart with new data based on the tracked chart type
            switch (currentChartType)
            {
                case "Line":
                    ShowLineChart();
                    break;
                case "Bar":
                    ShowBarChart();
                    break;
                case "Column":
                    ShowColumnChart();
                    break;
                case "Area":
                    ShowAreaChart();
                    break;
                case "Scatter":
                    ShowScatterChart();
                    break;
                case "Pie":
                    ShowPieChart();
                    break;
                case "GeoMap":
                    ShowGeoMap();
                    break;
            }
        }
        private void OnClearChart(object? sender, EventArgs e)
        {
            // Clear the currently visible chart
            if (cartesianChart.Visible)
            {
                cartesianChart.Series = [];
            }
            else if (pieChart.Visible)
            {
                pieChart.Series = [];
            }
            else if (geoMap.Visible)
            {
                geoMap.Series = [];
            }
        }
        private static Func<float, float> GetEasingFunction(string easingName)
        {
            // Simple easing functions
            return easingName switch
            {
                "Linear" => t => t,
                "EaseInQuad" => t => t * t,
                "EaseOutQuad" => t => t * (2f - t),
                "EaseInOutQuad" => t => t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t,
                "EaseInCubic" => t => t * t * t,
                "EaseOutCubic" => t => (--t) * t * t + 1f,
                "EaseInOutCubic" => t => t < 0.5f ? 4f * t * t * t : (t - 1f) * (2f * t - 2f) * (2f * t - 2f) + 1f,
                _ => t => t
            };
        }
    }
}