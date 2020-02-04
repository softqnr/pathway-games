﻿using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using PathwayGames.Sensors;
using PathwayGames.Services.Engangement;
using PathwayGames.Services.Sensors;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Input;
using Xamarin.Forms;

namespace PathwayGames.ViewModels
{
    public class SensorsViewModel : ViewModelBase
    {
        private const int PlotUpdateInterval = 20;
        private const int NumberOfSeries = 1;
        private bool Disposed;
        private readonly Timer timer;
        private readonly Stopwatch watch = new Stopwatch();
        //Services
        private readonly ISensorLogWriterService _sensorLowWriterService;
        private readonly IEngangementService _engangementService;

        private PlotModel _plotModel;

        public PlotModel PlotModel
        {
            get => _plotModel;
            private set => SetProperty(ref _plotModel, value);
        }

        public ICommand EyeGazeChangedCommand
        {
            get
            {
                return new Command<FaceAnchorChangedEventArgs>((e) =>
                {
                    //_sensorLowWriterService.WriteToLog(e.Reading.ToString());
                    // Invoke engangement service
                    //_engangementService
                });
            }
        }

        public SensorsViewModel (ISensorLogWriterService sensorLogWriterService,
            IEngangementService engangementService)
        {
            _sensorLowWriterService = sensorLogWriterService;
            _engangementService = engangementService;

            Title = "Live";

            //StartSensorRecording();
            CreatePlot();
        }

        private void CreatePlot()
        {
            PlotModel = new PlotModel();

            PlotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                MajorGridlineStyle = LineStyle.Dot,
                Minimum = 0,
                Maximum = 100,
                AbsoluteMinimum = 0,
                AbsoluteMaximum = 100,
                MinorStep = 100,
                MajorStep = 10
            });
            
            //DateTimeAxis dtx = new DateTimeAxis
            //{
            //    Position = AxisPosition.Bottom,
            //    MajorGridlineStyle = LineStyle.Dot,
            //    AbsoluteMinimum = DateTimeAxis.ToDouble(batteryGraph.Min(v => v.LogTimeStamp)) - (0.5d / 60),
            //    MajorStep = 5d,
            //    StringFormat = "MM-dd-yy",
            //    AbsoluteMaximum = DateTimeAxis.ToDouble(batteryGraph.Max(v => v.LogTimeStamp)) + (0.5d / 6),
            //    Minimum = DateTimeAxis.ToDouble(batteryGraph.Min(v => v.LogTimeStamp)) - (0.5d / 60),
            //    Maximum = DateTimeAxis.ToDouble(batteryGraph.Max(v => v.LogTimeStamp)) + (0.5d / 6)
            //};
            //plotModel.Axes.Add(dtx);

            var areaSerie = new AreaSeries
            {
                StrokeThickness = 2.0
            };

            areaSerie.Points.Add(new DataPoint(0, 50));
            areaSerie.Points.Add(new DataPoint(10, 60));
            areaSerie.Points.Add(new DataPoint(20, 140));
            areaSerie.Points2.Add(new DataPoint(0, 50));
            areaSerie.Points2.Add(new DataPoint(5, 70));
            areaSerie.Points2.Add(new DataPoint(15, 60));

            PlotModel.Series.Add(areaSerie);
        }

        private void Update()
        {

        }

        private void StartSensorRecording()
        {
            _sensorLowWriterService.LogPrefix = "\"FaceAnchorData\": [";
            _sensorLowWriterService.LogSuffix = "] ";

            _sensorLowWriterService.Start($"sensor_{Guid.NewGuid().ToString()}.json", ",");
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            //_sensorLowWriterService.Cancel();
        }
    }
}
