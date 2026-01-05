using System.Numerics;
using System.Security.Cryptography.X509Certificates;

namespace RayTracerNextWeekCS
{
    public static class Scenes
    {
        public static Stats BouncingSpheres(SceneParameters sceneParameters, string outputFile)
        {
            Camera camera = new Camera();
            camera.Background = new Vector3(0.7f, 0.8f, 1.0f);
            camera.ImageWidth = 300;
            camera.AspectRatio = 16.0 / 9.0;

            camera.VFOV = 20;
            camera.LookFrom = new Vector3(13.0f, 2.0f, 3.0f);
            camera.LookAt = new Vector3(0.0f, 0.0f, 0.0f);
            camera.VUp = new Vector3(0.0f, 1.0f, 0.0f);

            camera.DefocusAngle = 0.6;
            camera.FocusDist = 10.0;

            camera.SamplesPerPixel = sceneParameters.SamplesPerPixel;
            camera.MaxDepth = sceneParameters.MaxDepth;

            var r = Math.Cos(Math.PI / 4.0);

            var checker = new CheckerTexture(0.32, new Vector3(0.2f, 0.3f, 0.1f), new Vector3(0.9f, 0.9f, 0.9f));
            var groundMaterial = new Lambertian(checker);

            var hittables = new Hittables();

            hittables.Add(new Sphere(new Vector3(0.0f, -1000.0f, 0.0f), 1000, groundMaterial));

            for (int a = -11; a < 11; a++)
            {
                for (int b = -11; b < 11; b++)
                {
                    var chooseMat = RandomDouble();
                    var center = new Vector3((float)(a + 0.9 * RandomDouble()), 0.2f, (float)(b + 0.9 * RandomDouble()));
                    if ((center - new Vector3(4.0f, 0.2f, 0.0f)).Length() > 0.9)
                    {
                        Material sphereMaterial;
                        if (chooseMat < 0.8)
                        {
                            // diffuse
                            var albedoColor = RandomVector() * RandomVector();
                            sphereMaterial = new Lambertian(albedoColor);
                            var center2 = center + new Vector3(0.0f, (float)RandomDouble(0.0, 0.5), 0.0f);
                            hittables.Add(new Sphere(center, center2, 0.2, sphereMaterial));
                        }
                        else if (chooseMat < 0.95)
                        {
                            // metal
                            var albedoColor = RandomVector(0.5, 1.0);
                            var fuzz = RandomDouble(0.0, 0.5);
                            sphereMaterial = new Metal(albedoColor, fuzz);
                            hittables.Add(new Sphere(center, 0.2, sphereMaterial));
                        }
                        else
                        {
                            // glass
                            sphereMaterial = new Dielectric(1.5);
                            hittables.Add(new Sphere(center, 0.2, sphereMaterial));
                        }
                    }
                }
            }

            hittables.Add(new Sphere(new Vector3(0.0f, 1.0f, 0.0f), 1.0f, new Dielectric(1.5)));
            hittables.Add(new Sphere(new Vector3(-4.0f, 1.0f, 0.0f), 1.0f, new Lambertian(new Vector3(0.4f, 0.2f, 0.1f))));
            hittables.Add(new Sphere(new Vector3(4.0f, 1.0f, 0.0f), 1.0f, new Metal(new Vector3(0.7f, 0.6f, 0.5f), 0.0)));

            var world = new Hittables();
            world.Add(new BvhNode(hittables));

            var stats = camera.Render(world, outputFile);
            return stats;
        }

        public static Stats CheckeredSpheres(SceneParameters sceneParameters, string outputFile)
        {
            Camera camera = new Camera();
            camera.Background = new Vector3(0.7f, 0.8f, 1.0f);
            camera.ImageWidth = 300;
            camera.AspectRatio = 16.0 / 9.0;

            camera.VFOV = 20;
            camera.LookFrom = new Vector3(13.0f, 2.0f, 3.0f);
            camera.LookAt = new Vector3(0.0f, 0.0f, 0.0f);
            camera.VUp = new Vector3(0.0f, 1.0f, 0.0f);

            camera.DefocusAngle = 0.6;
            // camera.FocusDist = 10.0;

            camera.SamplesPerPixel = sceneParameters.SamplesPerPixel;
            camera.MaxDepth = sceneParameters.MaxDepth;

            var r = Math.Cos(Math.PI / 4.0);

            var checker = new CheckerTexture(0.32, new Vector3(0.2f, 0.3f, 0.1f), new Vector3(0.9f, 0.9f, 0.9f));
            var checkerMaterial = new Lambertian(checker);

            var hittables = new Hittables();

            hittables.Add(new Sphere(new Vector3(0.0f, -10.0f, 0.0f), 10, checkerMaterial));
            hittables.Add(new Sphere(new Vector3(0.0f, 10.0f, 0.0f), 10, checkerMaterial));

            var world = new Hittables();
            world.Add(new BvhNode(hittables));

            var stats = camera.Render(world, outputFile);
            return stats;
        }

        public static Stats Earth(SceneParameters sceneParameters, string texturePath, string outputFile)
        {
            var path = Path.Combine(texturePath, "earthmap.png");
            var imageTexture = new ImageTexture(path);

            Camera camera = new Camera();
            camera.Background = new Vector3(0.7f, 0.8f, 1.0f);
            camera.ImageWidth = 300;
            camera.AspectRatio = 16.0 / 9.0;

            camera.VFOV = 20;
            camera.LookFrom = new Vector3(13.0f, 2.0f, 3.0f);
            camera.LookAt = new Vector3(0.0f, 0.0f, 0.0f);
            camera.VUp = new Vector3(0.0f, 1.0f, 0.0f);

            camera.DefocusAngle = 0.6;
            // camera.FocusDist = 10.0;

            camera.SamplesPerPixel = sceneParameters.SamplesPerPixel;
            camera.MaxDepth = sceneParameters.MaxDepth;

            var r = Math.Cos(Math.PI / 4.0);

            var earthTexture = new ImageTexture(path);
            var earthSurface = new Lambertian(earthTexture);

            var hittables = new Hittables();

            hittables.Add(new Sphere(new Vector3(0.0f, -10.0f, 0.0f), 10, earthSurface));
            hittables.Add(new Sphere(new Vector3(0.0f, 10.0f, 0.0f), 10, earthSurface));

            var world = new Hittables();
            world.Add(new BvhNode(hittables));

            var stats = camera.Render(world, outputFile);
            return stats;
        }

        public static Stats PerlinSphere(SceneParameters sceneParameters, string outputFile)
        {
            Camera camera = new Camera();
            camera.Background = new Vector3(0.7f, 0.8f, 1.0f);
            camera.ImageWidth = 300;
            camera.AspectRatio = 16.0 / 9.0;

            camera.VFOV = 20;
            camera.LookFrom = new Vector3(13.0f, 2.0f, 3.0f);
            camera.LookAt = new Vector3(0.0f, 0.0f, 0.0f);
            camera.VUp = new Vector3(0.0f, 1.0f, 0.0f);

            camera.SamplesPerPixel = sceneParameters.SamplesPerPixel;
            camera.MaxDepth = sceneParameters.MaxDepth;

            camera.DefocusAngle = 0.6;
            // camera.FocusDist = 10.0;

            var r = Math.Cos(Math.PI / 4.0);

            var texture = new NoiseTexture(4.0);

            var hittables = new Hittables();

            hittables.Add(new Sphere(new Vector3(0.0f, -1000.0f, 0.0f), 1000, new Lambertian(texture)));
            hittables.Add(new Sphere(new Vector3(0.0f, 2.0f, 0.0f), 2, new Lambertian(texture)));

            var world = new Hittables();
            world.Add(new BvhNode(hittables));

            var stats = camera.Render(world, outputFile);
            return stats;
        }

        public static Stats Quads(SceneParameters sceneParameters, string outputFile)
        {
            Camera camera = new Camera();
            camera.Background = new Vector3(0.7f, 0.8f, 1.0f);
            camera.ImageWidth = 300;
            camera.AspectRatio = 1.0;

            camera.VFOV = 80;
            camera.LookFrom = new Vector3(0.0f, 0.0f, 9.0f);
            camera.LookAt = new Vector3(0.0f, 0.0f, 0.0f);
            camera.VUp = new Vector3(0.0f, 1.0f, 0.0f);

            camera.DefocusAngle = 0.6;
            // camera.FocusDist = 10.0;

            camera.SamplesPerPixel = sceneParameters.SamplesPerPixel;
            camera.MaxDepth = sceneParameters.MaxDepth;

            var r = Math.Cos(Math.PI / 4.0);

            var texture = new NoiseTexture(4.0);

            var hittables = new Hittables();


            // Materials
            var left_red = new Lambertian(new Vector3(1.0f, 0.2f, 0.2f));
            var back_green = new Lambertian(new Vector3(0.2f, 1.0f, 0.2f));
            var right_blue = new Lambertian(new Vector3(0.2f, 0.2f, 1.0f));
            var upper_orange = new Lambertian(new Vector3(1.0f, 0.5f, 0.0f));
            var lower_teal = new Lambertian(new Vector3(0.2f, 0.8f, 0.8f));

            // Quads
            hittables.Add(new Quad(new Vector3(-3, -2, 5), new Vector3(0, 0, -4), new Vector3(0, 4, 0), left_red));
            hittables.Add(new Quad(new Vector3(-2, -2, 0), new Vector3(4, 0, 0), new Vector3(0, 4, 0), back_green));
            hittables.Add(new Quad(new Vector3(3, -2, 1), new Vector3(0, 0, 4), new Vector3(0, 4, 0), right_blue));
            hittables.Add(new Quad(new Vector3(-2, 3, 1), new Vector3(4, 0, 0), new Vector3(0, 0, 4), upper_orange));
            hittables.Add(new Quad(new Vector3(-2, -3, 5), new Vector3(4, 0, 0), new Vector3(0, 0, -4), lower_teal));


            var world = new Hittables();
            world.Add(new BvhNode(hittables));

            var stats = camera.Render(world, outputFile);
            return stats;
        }

        public static Stats SimpleLight(SceneParameters sceneParameters, string outputFile)
        {
            Camera camera = new Camera();
            camera.Background = new Vector3();
            camera.ImageWidth = 300;
            camera.AspectRatio = 16.0 / 9.0;

            camera.VFOV = 20;
            camera.LookFrom = new Vector3(26.0f, 3.0f, 6.0f);
            camera.LookAt = new Vector3(0.0f, 2.0f, 0.0f);
            camera.VUp = new Vector3(0.0f, 1.0f, 0.0f);

            camera.DefocusAngle = 0.0;
            // camera.FocusDist = 10.0;

            camera.SamplesPerPixel = sceneParameters.SamplesPerPixel;
            camera.MaxDepth = sceneParameters.MaxDepth;

            var r = Math.Cos(Math.PI / 4.0);

            var texture = new NoiseTexture(4.0);

            var hittables = new Hittables();

            var perTex = new NoiseTexture(4);

            hittables.Add(new Sphere(new Vector3(0.0f, -1000.0f, 0.0f), 1000.0, new Lambertian(perTex)));
            hittables.Add(new Sphere(new Vector3(0.0f, 2.0f, 0.0f), 2.0, new Lambertian(perTex)));

            var diffLight = new DiffuseLight(new Vector3(4.0f, 4.0f, 4.0f));
            hittables.Add(new Quad(new Vector3(3.0f, 1.0f, -2.0f), new Vector3(2.0f, 0.0f, 0.0f), new Vector3(0.0f, 2.0f, 0.0f), diffLight));

            var world = new Hittables();
            world.Add(new BvhNode(hittables));

            var stats = camera.Render(world, outputFile);
            return stats;
        }

        public static Stats CornellBox(SceneParameters sceneParameters, string outputFile)
        {
            Camera camera = new Camera();
            camera.Background = new Vector3();
            camera.ImageWidth = 300;
            camera.AspectRatio = 1.0;

            camera.VFOV = 40;
            camera.LookFrom = new Vector3(278.0f, 278.0f, -800.0f);
            camera.LookAt = new Vector3(278.0f, 278.0f, 0.0f);
            camera.VUp = new Vector3(0.0f, 1.0f, 0.0f);

            camera.DefocusAngle = 0.0;
            // camera.FocusDist = 10.0;

            camera.SamplesPerPixel = sceneParameters.SamplesPerPixel;
            camera.MaxDepth = sceneParameters.MaxDepth;

            var r = Math.Cos(Math.PI / 4.0);

            var texture = new NoiseTexture(4.0);

            var hittables = new Hittables();

            var red = new Lambertian(new Vector3(.65f, .05f, .05f));
            var white = new Lambertian(new Vector3(.73f, .73f, .73f));
            var green = new Lambertian(new Vector3(.12f, .45f, .15f));
            var light = new DiffuseLight(new Vector3(15, 15, 15));

            hittables.Add(new Quad(new Vector3(555, 0, 0), new Vector3(0, 555, 0), new Vector3(0, 0, 555), green));
            hittables.Add(new Quad(new Vector3(0, 0, 0), new Vector3(0, 555, 0), new Vector3(0, 0, 555), red));
            hittables.Add(new Quad(new Vector3(343, 554, 332), new Vector3(-130, 0, 0), new Vector3(0, 0, -105), light));
            hittables.Add(new Quad(new Vector3(0, 0, 0), new Vector3(555, 0, 0), new Vector3(0, 0, 555), white));
            hittables.Add(new Quad(new Vector3(555, 555, 555), new Vector3(-555, 0, 0), new Vector3(0, 0, -555), white));
            hittables.Add(new Quad(new Vector3(0, 0, 555), new Vector3(555, 0, 0), new Vector3(0, 555, 0), white));


            IHittable box1 = Box(new Vector3(0, 0, 0), new Vector3(165, 330, 165), white);
            box1 = new RotateY(box1, 15);
            box1 = new Translate(box1, new Vector3(265.0f, 0.0f, 295.0f));
            IHittable box2 = Box(new Vector3(0, 0, 0), new Vector3(165, 165, 165), white);
            box2 = new RotateY(box2, -18);
            box2 = new Translate(box2, new Vector3(130.0f, 0.0f, 65.0f));
            hittables.Add(box1);
            hittables.Add(box2);

            var world = new Hittables();
            world.Add(new BvhNode(hittables));

            var stats = camera.Render(world, outputFile);
            return stats;
        }

        public static Stats CornellSmoke(SceneParameters sceneParameters, string outputFile)
        {
            Camera camera = new Camera();
            camera.Background = new Vector3();
            camera.ImageWidth = 300;
            camera.AspectRatio = 1.0;

            camera.VFOV = 40;
            camera.LookFrom = new Vector3(278.0f, 278.0f, -800.0f);
            camera.LookAt = new Vector3(278.0f, 278.0f, 0.0f);
            camera.VUp = new Vector3(0.0f, 1.0f, 0.0f);

            camera.DefocusAngle = 0.0;
            // camera.FocusDist = 10.0;

            camera.SamplesPerPixel = sceneParameters.SamplesPerPixel;
            camera.MaxDepth = sceneParameters.MaxDepth;

            var r = Math.Cos(Math.PI / 4.0);

            var texture = new NoiseTexture(4.0);

            var hittables = new Hittables();

            var red = new Lambertian(new Vector3(.65f, .05f, .05f));
            var white = new Lambertian(new Vector3(.73f, .73f, .73f));
            var green = new Lambertian(new Vector3(.12f, .45f, .15f));
            var light = new DiffuseLight(new Vector3(7, 7, 7));

            hittables.Add(new Quad(new Vector3(555, 0, 0), new Vector3(0, 555, 0), new Vector3(0, 0, 555), green));
            hittables.Add(new Quad(new Vector3(0, 0, 0), new Vector3(0, 555, 0), new Vector3(0, 0, 555), red));
            hittables.Add(new Quad(new Vector3(113, 554, 127), new Vector3(330, 0, 0), new Vector3(0, 0, 305), light));
            hittables.Add(new Quad(new Vector3(0, 555, 0), new Vector3(555, 0, 0), new Vector3(0, 0, 555), white));
            hittables.Add(new Quad(new Vector3(0, 0, 0), new Vector3(555, 0, 0), new Vector3(0, 0, 555), white));
            hittables.Add(new Quad(new Vector3(0, 0, 555), new Vector3(555, 0, 0), new Vector3(0, 555, 0), white));


            IHittable box1 = Box(new Vector3(0, 0, 0), new Vector3(165, 330, 165), white);
            box1 = new RotateY(box1, 15);
            box1 = new Translate(box1, new Vector3(265.0f, 0.0f, 295.0f));
            IHittable box2 = Box(new Vector3(0, 0, 0), new Vector3(165, 165, 165), white);
            box2 = new RotateY(box2, -18);
            box2 = new Translate(box2, new Vector3(130.0f, 0.0f, 65.0f));
            hittables.Add(new ConstantMedium(box1, 0.01, new Vector3(0, 0, 0)));
            hittables.Add(new ConstantMedium(box2, 0.01, new Vector3(1, 1, 1)));

            var world = new Hittables();
            world.Add(new BvhNode(hittables));

            var stats = camera.Render(world, outputFile);
            return stats;
        }

        public static Stats FinalScene(SceneParameters sceneParameters, string texturePath, string outputFile)
        {
            Camera camera = new Camera();
            camera.Background = new Vector3();
            camera.ImageWidth = 300;
            camera.AspectRatio = 1.0;

            camera.VFOV = 40;
            camera.LookFrom = new Vector3(478.0f, 278.0f, -600.0f);
            camera.LookAt = new Vector3(278.0f, 278.0f, 0.0f);
            camera.VUp = new Vector3(0.0f, 1.0f, 0.0f);

            camera.DefocusAngle = 0.0;

            camera.SamplesPerPixel = sceneParameters.SamplesPerPixel;
            camera.MaxDepth = sceneParameters.MaxDepth;

            var boxes1 = new Hittables();
            var ground = new Lambertian(new Vector3(0.48f, 0.83f, 0.53f));

            int boxes_per_side = 20;
            var w = 100.0;
            for (int i = 0; i < boxes_per_side; i++)
            {
                for (int j = 0; j < boxes_per_side; j++)
                {
                    var x0 = -1000.0 + i * w;
                    var z0 = -1000.0 + j * w;
                    var y0 = 0.0;
                    var x1 = x0 + w;
                    var y1 = RandomDouble(1, 101);
                    var z1 = z0 + w;

                    boxes1.Add(Box(new Vector3((float)x0, (float)y0, (float)z0), new Vector3((float)x1, (float)y1, (float)z1), ground));
                }
            }

            var world = new Hittables();

            world.Add(new BvhNode(boxes1));
            var light = new DiffuseLight(new Vector3(7, 7, 7));
            world.Add(new Quad(new Vector3(123, 554, 147), new Vector3(300, 0, 0), new Vector3(0, 0, 265), light));

            var center1 = new Vector3(400, 400, 200);
            var center2 = center1 + new Vector3(30, 0, 0);

            var sphereMaterial = new Lambertian(new Vector3(0.7f, 0.3f, 0.1f));
            world.Add(new Sphere(center1, center2, 50.0, sphereMaterial));

            world.Add(new Sphere(new Vector3(260, 150, 40), 50, new Dielectric(1.5)));
            world.Add(new Sphere(new Vector3(0, 150, 145), 50, new Metal(new Vector3(0.8f, 0.8f, 0.9f), 1.0)));

            var boundary = new Sphere(new Vector3(360, 150, 145), 70, new Dielectric(1.5));
            world.Add(boundary);
            world.Add(new ConstantMedium(boundary, 0.2, new Vector3(0.2f, 0.4f, 0.9f)));

            var boundary2 = new Sphere(new Vector3(), 5000, new Dielectric(1.5));
            world.Add(new ConstantMedium(boundary2, 0.0001, new Vector3(1, 1, 1)));

            var emat = new Lambertian(new ImageTexture(Path.Combine(texturePath, "earthmap.png")));
            world.Add(new Sphere(new Vector3(400, 200, 400), 100, emat));
            var perText = new NoiseTexture(0.2);
            world.Add(new Sphere(new Vector3(220, 280, 300), 80, new Lambertian(perText)));

            var spheres = new Hittables();
            var white = new Lambertian(new Vector3(0.73f, 0.73f, 0.73f));
            for (var i = 0; i < 10; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    for (var k = 0; k < 10; k++)
                    {
                        spheres.Add(new Sphere(new Vector3(i * 30.01f, j * 30.02f, k * 30.03f), 10, white));
                    }
                }
            }

            world.Add(new Translate(new RotateY(new BvhNode(spheres), 15), new Vector3(-100, 270, 395)));

            var stats = camera.Render(world, outputFile);
            return stats;
        }

        private static Hittables Box(Vector3 a, Vector3 b, Material mat)
        {
            // Returns the 3D box (six sides) that contains the two opposite vertices a & b.

            var sides = new Hittables();

            // Construct the two opposite vertices with the minimum and maximum coordinates.
            var min = new Vector3(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
            var max = new Vector3(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));

            var dx = new Vector3(max.X - min.X, 0            , 0            );
            var dy = new Vector3(0            , max.Y - min.Y, 0            );
            var dz = new Vector3(0            , 0            , max.Z - min.Z);

            sides.Add(new Quad(new Vector3(min.X, min.Y, max.Z),  dx,  dy, mat)); // front
            sides.Add(new Quad(new Vector3(max.X, min.Y, max.Z), -dz,  dy, mat)); // right
            sides.Add(new Quad(new Vector3(max.X, min.Y, min.Z), -dx,  dy, mat)); // back
            sides.Add(new Quad(new Vector3(min.X, min.Y, min.Z),  dz,  dy, mat)); // left
            sides.Add(new Quad(new Vector3(min.X, max.Y, max.Z),  dx, -dz, mat)); // top
            sides.Add(new Quad(new Vector3(min.X, min.Y, min.Z),  dx,  dz, mat)); // bottom

            return sides;
        }
    }
}
