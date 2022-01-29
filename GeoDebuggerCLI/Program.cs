using Dev4Agriculture.De.GeoDebugger;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using System;
using System.Collections.Generic;

namespace GeoDebuggerCLI
{
    class Program
    {

		static Feature buildSimplePolygon()
        {
			Random random = new Random();
			double offset = random.NextDouble() * 0.3;
			double latitude = 42 + offset;
			double longitude = 7 + offset;
			double width = random.NextDouble() * 3;
			double height = random.NextDouble() * 3;
			List<LineString> lineStrings = new List<LineString>(){
					new LineString(new List<List<double>>{
						new List<double>{ latitude, longitude },
						new List<double>{ latitude + height, longitude },
						new List<double>{ latitude +height, longitude +width },
						new List<double>{ latitude, longitude + width},
						new List<double>{ latitude, longitude }
					})
				};

			Polygon polygon = new Polygon(lineStrings);
			Feature feature = new Feature(polygon);
			feature.Properties.Add("Date", DateTime.Now.ToString());
			feature.Properties.Add("Duration", "45");
			feature.Properties.Add("Area", "405.3");
			feature.Properties.Add("Distance", "5");
			return feature;
		}

		static Feature buildSimplePoint()
        {
			Random random = new Random();
			double offset = random.NextDouble() * 0.3;
			double latitude = 42 + offset;
			double longitude = 7 + offset;
			Point point = new Point(new Position(latitude, longitude ));

			Feature feature = new Feature(point);
			feature.Properties.Add("Date", DateTime.Now.ToString());
			feature.Properties.Add("Duration", "45");
			feature.Properties.Add("Area", "405.3");
			feature.Properties.Add("Distance", "5");
			return feature;

		}

		static Feature buildSimpleLine()
        {
			Random random = new Random();
			double offset = random.NextDouble() * 0.3;
			double latitude = 42 + offset;
			double longitude = 7 + offset;
			double length = random.NextDouble();
			LineString line = new LineString(new List<List<double>>{
						new List<double>{ latitude , longitude},
						new List<double>{ latitude + length * 4, longitude + length*4 },

					});

			Feature feature = new Feature(line);
			feature.Properties.Add("Date", DateTime.Now.ToString());
			feature.Properties.Add("Duration", "45");
			feature.Properties.Add("Area", "405.3");
			feature.Properties.Add("Distance", "5");
			return feature;
		}



		static void Main(string[] args)
		{
			GeoJSONDebug.SetURL("http://127.0.0.1:8083");
			printHelp();
			bool quit = false;
			do
			{
				string text = Console.ReadLine();
				string[] line = text.Split(" ");
				int layer = -1;
				if( line.Length == 3)
                {
					layer = int.Parse(line[2]);
                }
				switch (line[0].ToLower())
				{
					case "add":
						switch (line[1].ToLower())
						{
							case "point":
								GeoJSONDebug.AddFeature(buildSimplePoint(), layer);
								Console.WriteLine("  => Done");
								break;
							case "line":
								GeoJSONDebug.AddFeature(buildSimpleLine(), layer);
								Console.WriteLine("  => Done");
								break;
							case "polygon":
								GeoJSONDebug.AddFeature(buildSimplePolygon(), layer);
								Console.WriteLine("  => Done");
								break;
							default:
								Console.WriteLine("Wrong type, must be point, line or polygon");
								break;
						}
						break;
					case "url":
						GeoJSONDebug.SetURL(line[1]);
						Console.WriteLine("  => Done");
						break;
					case "clear":
						if (line.Length > 1)
						{
							switch (line[1])
							{
								case "points":
									GeoJSONDebug.ClearPoints();
									Console.WriteLine("  => Done clear lines");
									break;
								case "lines":
									GeoJSONDebug.ClearLines();
									Console.WriteLine("  => Done clear lines");
									break;
								case "polygons":
									GeoJSONDebug.ClearPolygons();
									Console.WriteLine("  => Done clear polygons");
									break;
								default:
									Console.WriteLine("Wrong type, must be points, lines or polygons");
									break;

							}
						}
						else
						{
							GeoJSONDebug.ClearAll();
							Console.WriteLine("  => Done clear all");
						}
						break;
					case "exit":
						quit = true;
						break;
					case "go":
						DateTime start = DateTime.Now;
						GeoJSONDebug.Debug();
						DateTime afterDebug = DateTime.Now;
						GeoJSONDebug.WaitDebugger();
						DateTime afterSent = DateTime.Now;
						Console.WriteLine(
							"Debugging complete; Building message took {0} ms, sending took {1} ms",
							afterDebug.Subtract(start).TotalMilliseconds,
							afterSent.Subtract(afterDebug).TotalMilliseconds);
						break;
					default:
						Console.WriteLine("unknown command");
						break;

				}
			} while (quit == false);
		}

        private static void printHelp()
        {
			Console.WriteLine("Welcome to the GeoDebugger Command Line Interface by dev4Agriculture");
			Console.WriteLine("This is a simple CLI to show the functionalities the GeoDebugger.");
			Console.WriteLine("It's not meant to be used in production but as an example implementation of the lib");
			Console.WriteLine("Type 1 of the following commands to proceed");
			Console.WriteLine("   add {type} {layer}: Adds an example Dataset; type can be: Polygon,Line,Point, layer must be number 0 to 10");
			Console.WriteLine("   clear {type}: Clears the List of Elements. type can be: Polygon,Line,Point");
			Console.WriteLine("   go : Send all Data to the GeoDebug Server");
			Console.WriteLine("   url {url}: Upate the URL; default is http://localhost:8083");
			Console.WriteLine("   exit: Quit this example Program");
			Console.WriteLine("");
			Console.WriteLine("Please enter command:");

		}
	}
}
