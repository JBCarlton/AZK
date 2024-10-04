namespace IRController
{
    using Microsoft.Azure.Kinect.Sensor;
    using System;
    using KinectPipeline;

    class AzureKinect
    {
        private const string path_to_python_file = "";
        private const string path_to_python_exe = "C:\\Users\\benlv\\AppData\\Local\\Microsoft\\WindowsApps\\PythonSoftwareFoundation.Python.3.12_qbz5n2kfra8p0\\python3.exe";
        private const string path_to_image_folder = "C:\\Users\\benlv\\Pictures\\AZKOutputs\\";

        private readonly Device device;
        private readonly AZKPipeline Pipeline;

        public AzureKinect()
        {
            if (Device.GetInstalledCount() == 0) { Environment.Exit(0); }
            device = Device.Open();
            DeviceConfiguration deviceConfiguration = new() { };
            device.StartCameras(deviceConfiguration);
            Pipeline = new AZKPipeline();
            Pipeline.AddStep(new GenerateCapture());
            Pipeline.AddStep(new GenerateImage());
            Pipeline.AddStep(new GenerateImageMatrix());
            Pipeline.AddStep(new FilterImageMatrix());
            Pipeline.AddStep(new FindPoint());
            Pipeline.AddStep(new FindPosition());
        }

        ~AzureKinect()
        {
            device.StopCameras();
            device?.Dispose();
        }

        public void Execute()
        {
            Pipeline.ExecutePipeline(device); 
        }
    }
}

namespace KinectPipeline 
{
    using OpenCvSharp;
    using Microsoft.Azure.Kinect.Sensor;

    public class Coordinate(int x, int y)
    {
        public int X = x, Y = y;
    }

    public class PairOfCoordinates
    {
        public Coordinate P1;
        public Coordinate P2;
        public PairOfCoordinates()
        {
            P1 = new Coordinate(0, 0);
            P2 = new Coordinate(0, 0);
        }

        public PairOfCoordinates(Coordinate p1, Coordinate p2)
        {
            P1 = p1;
            P2 = p2;
        }

        public PairOfCoordinates(int p1x, int p1y, int p2x, int p2y)
        {
            P1 = new Coordinate(p1x, p1y);
            P2 = new Coordinate(p2x, p2y);
        }
    }

    public interface IPipelineContext
    {
    }

    public interface IPipelineStep
    {
        public void ExecuteAsync(IPipelineContext context);
    }

    public class  GenerateCapture : IPipelineStep 
    {
        public void ExecuteAsync(IPipelineContext context)
        {
            Device? kinect = context as Device;
            Capture? capture = kinect?.GetCapture();
        }
    }

    public class GenerateImage : IPipelineStep
    {
        public void ExecuteAsync(IPipelineContext context)
        {
            Capture? capture = context as Capture;
            Image? image = capture?.IR; 
        }
    }

    public class GenerateImageMatrix : IPipelineStep
    {
        private const UInt16 Threshold = 1000;
        public void ExecuteAsync(IPipelineContext context)
        {
            Image? image = context as Image;
            Mat image_matrix = new(image.HeightPixels, image.WidthPixels, MatType.CV_8UC1);
            for (int i = 0; i < image_matrix.Rows; i++)
            {
                for (int j = 0; j < image_matrix.Cols; j++)
                    image_matrix.Set<ushort>(i, j, image.GetPixel<ushort>(i, j) >= Threshold ?
                        image.GetPixel<ushort>(i, j) : ushort.MinValue);
            }
        }
    }

    public class FilterImageMatrix : IPipelineStep
    {
        public void ExecuteAsync(IPipelineContext context)
        {
            Mat? image_matrix = context as Mat;
            Mat? shallow_image_matrix = new();
            image_matrix?.ConvertTo(shallow_image_matrix, MatType.CV_8UC1);
            context = shallow_image_matrix as IPipelineContext;
        }
    }

    public class FindPoint : IPipelineStep
    {
        public void ExecuteAsync(IPipelineContext context)
        {
            Mat? shallow_image_matrix = context as Mat;
            LineSegmentPoint[] line_segment_points = shallow_image_matrix?.HoughLinesP(1.5, Math.PI / 180, 2000);
            //TODO: implement better searching algorithm
            PairOfCoordinates line_coordinates = new(line_segment_points[0].P1.X, line_segment_points[0].P1.Y, line_segment_points[0].P2.X, line_segment_points[0].P2.Y);
        }
    }

    public class FindPosition : IPipelineStep
    {
        public void ExecuteAsync(IPipelineContext context)
        {
            PairOfCoordinates? line_coordinates = context as PairOfCoordinates; 
            //TODO: implement a way to calibarate the remote to the displays
        }
    }

    public class AZKPipeline
    {
        private readonly List<IPipelineStep> _steps;

        public AZKPipeline()
        {
            _steps = new List<IPipelineStep>();
        }

        public void AddStep(IPipelineStep step)
        {
            _steps.Add(step);
        }

        public void ExecutePipeline(Device device)
        {
            IPipelineContext? context = device as IPipelineContext; 
            foreach (var step in _steps)
            {
                step.ExecuteAsync(context);
            }
        }
    }
}