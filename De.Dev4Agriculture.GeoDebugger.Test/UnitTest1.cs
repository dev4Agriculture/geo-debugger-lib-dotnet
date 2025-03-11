using De.Dev4Agriculture.GeoDebugger;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace GeoDebuggerTest
{
    [TestClass]
    public class UnitTest1
    {
		public List<Point> buildPoints(int number)
        {
			List<Point> points = new List<Point>();
			Random randomLat = new Random();
			Random randomLong = new Random();
			for ( int index = 0; index< number; index++)
            {
				points.Add(new Point(new Position(42 + randomLat.NextDouble(), 7 + randomLong.NextDouble())));
            }
			return points;
        }

		public List<List<double>> PointsToList(List<Point> points)
        {
			List<List<double>> entries = new List<List<double>>();
			foreach(var entry in points)
            {
				entries.Add(new List<double>() { entry.Coordinates.Latitude, entry.Coordinates.Longitude });
            }

			return entries;
        }

		public List<LineString> buildLineStrings(int lineNumber, int pointNumber=4, bool closed = false)
        {
			List<LineString> lineStrings = new List<LineString>();
			for(int index=0; index < lineNumber; index++)
            {
				List<Point> points = buildPoints(pointNumber);
				if( closed == true)
                {
					points.Add(points[0]);//Add first point as last point to generate a valid polygon
                }
				LineString lineString = new LineString(PointsToList(points));
				lineStrings.Add(lineString);
            }
			return lineStrings;
        }

		public List<List<List<double>>> LineStringsToList(List<LineString> lineStrings)
        {
			List<List<List<double>>> entries = new List<List<List<double>>>();
			foreach(var entry in lineStrings)
            {
				List<List<double>> linePoints = new List<List<double>>();
				foreach(var line in entry.Coordinates)
                {
					linePoints.Add(new List<double>() { line.Latitude, line.Longitude });
                }
				entries.Add(linePoints);
            }

			return entries;
        }


		public List<Polygon> buildPolygonList(int polygonNumber, int pointNumber = 4)
        {
			List<Polygon> polygons = new List<Polygon>();
			for(int index=0; index< polygonNumber; index++)
            {
				List<LineString> lineStrings = buildLineStrings(1, pointNumber, true);
				Polygon polygon = new Polygon(LineStringsToList(lineStrings));
				polygons.Add(polygon);
            }
			return polygons;
        }

		public Feature buildFeatureFromElement(IGeometryObject entry, int numberOfAttributes)
        {
			Feature feature = new Feature(entry);
			for(int index=0; index< numberOfAttributes;index++)
            {
				feature.Properties.Add("prop_" + index, index);
            }
			return feature;
        }

		public List<Feature> buildFeaturesFromPolygonList(List<Polygon>entries, int numberOfAttributes)
        {
			List<Feature> features = new List<Feature>();
			foreach (var entry in entries)
			{
				Feature feature = buildFeatureFromElement(entry, numberOfAttributes);
				features.Add(feature);
			}
			return features;

		}

		public List<Feature> buildFeaturesFromLineStringList(List<LineString> entries, int numberOfAttributes)
		{
			List<Feature> features = new List<Feature>();
			foreach (var entry in entries)
			{
				Feature feature = buildFeatureFromElement(entry, numberOfAttributes);
				features.Add(feature);
			}
			return features;

		}
		public List<Feature> buildFeaturesFromPointList(List<Point> entries, int numberOfAttributes)
		{
			List<Feature> features = new List<Feature>();
			foreach (var entry in entries)
			{
				Feature feature = buildFeatureFromElement(entry, numberOfAttributes);
				features.Add(feature);
			}
			return features;

		}



		public void initTest()
        {
			GeoJSONDebug.SetURL("http://localhost:8083");
			GeoJSONDebug.ClearLines();
			GeoJSONDebug.ClearPoints();
			GeoJSONDebug.ClearPolygons();
		}
		[TestMethod]
        public void sendSinglePolygonMultipleTimesWith1SecondWaiting()
        {
			initTest();
			GeoJSONDebug.Debug();
			for (int a = 0; a < 10; a++)
			{
				List<Polygon> polygons = buildPolygonList(1,4);

				List<Feature> features = buildFeaturesFromPolygonList(polygons, 5);
				GeoJSONDebug.AddFeature(features[0]);
				GeoJSONDebug.Debug();
				Thread.Sleep(1000);
			}
        }

		[TestMethod]
		public void sendSinglePolygonMultipleTimesInstantly()
		{
			initTest();
			GeoJSONDebug.Debug();
			for (int a = 0; a < 10; a++)
			{
				List<Polygon> polygons = buildPolygonList(1, 4);

				List<Feature> features = buildFeaturesFromPolygonList(polygons, 5);
				GeoJSONDebug.AddFeature(features[0]);
				GeoJSONDebug.Debug();
				Thread.Sleep(4);
			}

			GeoJSONDebug.WaitDebugger();
		}

		[TestMethod]
		public void sendSinglePointMultipleTimesWith1SecondWaiting()
		{
			initTest();
			GeoJSONDebug.Debug();
			for (int a = 0; a < 10; a++)
			{
				List<Point> points = buildPoints(4);

				List<Feature> features = buildFeaturesFromPointList(points, 5);
				GeoJSONDebug.AddFeature(features[0]);
				GeoJSONDebug.Debug();
				Thread.Sleep(1000);
			}
		}

		[TestMethod]
		public void sendSinglePointMultipleTimesInstantly()
		{
			initTest();
			GeoJSONDebug.Debug();
			for (int a = 0; a < 10; a++)
			{
				List<Point> points = buildPoints(4);

				List<Feature> features = buildFeaturesFromPointList(points, 5);
				GeoJSONDebug.AddFeature(features[0]);
				GeoJSONDebug.Debug();
				Thread.Sleep(4);
			}

			GeoJSONDebug.WaitDebugger();
		}

		[TestMethod]
		public void sendSingleLineStringMultipleTimesWith1SecondWaiting()
		{
			initTest();
			GeoJSONDebug.Debug();
			for (int a = 0; a < 10; a++)
			{
				List<LineString> lines = buildLineStrings(1, 4);

				List<Feature> features = buildFeaturesFromLineStringList(lines, 5);
				GeoJSONDebug.AddFeature(features[0]);
				GeoJSONDebug.Debug();
				Thread.Sleep(1000);
			}
		}

		[TestMethod]
		public void sendSingleLineStringMultipleTimesInstantly()
		{
			initTest();
			GeoJSONDebug.Debug();
			for (int a = 0; a < 10; a++)
			{
				List<LineString> lines = buildLineStrings(1, 4);

				List<Feature> features = buildFeaturesFromLineStringList(lines, 5);
				GeoJSONDebug.AddFeature(features[0]);
				GeoJSONDebug.Debug();
				Thread.Sleep(4);
			}

			GeoJSONDebug.WaitDebugger();
		}


	}

}
