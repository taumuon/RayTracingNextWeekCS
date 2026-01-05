namespace RayTracerNextWeekCS
{
    internal class Program
    {
        static void WriteHelp()
        {
            Console.WriteLine("First argument should be an integer for one of the following scenes:");
            Console.WriteLine("1 Bouncing Spheres");
            Console.WriteLine("2 Checkered Spheres");
            Console.WriteLine("3 Earth");
            Console.WriteLine("4 Perlin Sphere");
            Console.WriteLine("5 Quads");
            Console.WriteLine("6 Simple Light");
            Console.WriteLine("7 Cornell Box");
            Console.WriteLine("8 Cornell Smoke");
            Console.WriteLine("9 Final Scene");
        }

        static void Main(string[] args)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var texturePath = @"E:\temp\";
            var outputFile = @"E:\temp\render\test2.ppm";

            if (args.Length == 0)
            {
                WriteHelp();
                throw new Exception("No scene specified");
            }

            if (!int.TryParse(args[0], out var scene))
            {
                WriteHelp();
                throw new Exception($"Unrecognised scene {scene}");
            }

            var sceneParameters = new SceneParameters()
            {
                SamplesPerPixel = 80, // 100
                MaxDepth = 2, // 30
            };

            Stats stats = null;
            switch (scene)
            {
                case 1:
                    stats = Scenes.BouncingSpheres(sceneParameters, outputFile);
                    break;
                case 2:
                    stats = Scenes.CheckeredSpheres(sceneParameters, outputFile);
                    break;
                case 3:
                    stats = Scenes.Earth(sceneParameters, texturePath, outputFile);
                    break;
                case 4:
                    stats = Scenes.PerlinSphere(sceneParameters, outputFile);
                    break;
                case 5:
                    stats = Scenes.Quads(sceneParameters, outputFile);
                    break;
                case 6:
                    stats = Scenes.SimpleLight(sceneParameters, outputFile);
                    break;
                case 7:
                    stats = Scenes.CornellBox(sceneParameters, outputFile);
                    break;
                case 8:
                    stats = Scenes.CornellSmoke(sceneParameters, outputFile);
                    break;
                case 9:
                    stats = Scenes.FinalScene(sceneParameters, texturePath, outputFile);
                    break;
                default:
                    WriteHelp();
                    throw new Exception($"Unrecognised scene {scene}");
            }


            Console.WriteLine($"Complete in {watch.ElapsedMilliseconds}ms");

            Console.WriteLine($"Stats: {stats.AllocatedBytes} elapsed:{stats.ElapsedMilliseconds}ms");
        }
    }
}
