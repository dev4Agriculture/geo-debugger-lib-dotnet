using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dev4Agriculture.De.GeoDebugger
{
	internal class MessageToSend
    {
		public String url;
		public String content;
    }

	internal class FeatureBlock
	{
		private FeatureCollection collection;
		private DateTime lastUpdate;
		private int Index;
		internal FeatureBlock(int index)
		{
			this.lastUpdate = DateTime.Now;
			this.collection = new FeatureCollection();
			this.Index = index;
		}

		internal void Add(Feature feature)
		{
			this.lastUpdate = DateTime.Now;
			this.collection.Features.Add(feature);
		}

		internal bool hasUpdates(DateTime lastSendingTime)
		{
			return lastSendingTime.CompareTo(lastUpdate) < 0;
		}

		internal FeatureCollection GetFeatureCollection()
		{
			return this.collection;
		}

		internal int GetIndex()
		{
			return this.Index;
		}

		internal void ClearFeatures()
		{
			this.collection.Features.Clear();
			this.lastUpdate = DateTime.Now;
		}


	}

	internal class FeaturesBlockList
	{
		private List<FeatureBlock> _blocks;
		String SubURL;

		internal FeaturesBlockList(string subURL)
		{
			this.SubURL = subURL;
			this._blocks = new List<FeatureBlock>();
		}

		internal void ClearAll()
		{
			lock(_blocks){
				//REMARK: We never delete a Block from the list of Features but we delete the content of the Block
				//		  The reason is that - as soon as a block is removed - QGis might display an unknown Object Type
				foreach(var block in _blocks)
                {
					block.ClearFeatures();
                }
			}
		}

		internal async Task DebugClearAll(String debugURL)
		{
			lock (_blocks)
			{
				foreach (var entry in this._blocks)
				{
					entry.ClearFeatures();
				}
			}
			await this.Debug(debugURL, DateTime.Now);
			ClearAll();
		}

		internal void clear(int index)
		{
			lock (_blocks)
			{
				foreach (FeatureBlock block in this._blocks)
				{
					if (block.GetIndex() == index)
					{
						block.ClearFeatures();
						break;
					}
				}
			}
		}

		internal void AddFeature(Feature feature, int index = 0)
		{
			lock(_blocks){
				while (_blocks.Count <= index)
				{
					this._blocks.Add(new FeatureBlock(_blocks.Count));
				}
				this._blocks[index].Add(feature);
			}
		}


		internal int GetSize()
		{
			return _blocks.Count;
		}

		internal FeatureBlock getBlock(int index)
		{
			lock (_blocks)
			{
				return _blocks[index];
			}
		}

		internal IEnumerable<FeatureBlock> GetBlocks()
		{
			lock (_blocks)
			{
				return _blocks;
			}
		}

		internal async Task Debug(string debugUrl, DateTime lastDebug)
		{
				lock (_blocks)
				{
					foreach (var featureBlock in this._blocks)
					{
						if (featureBlock.hasUpdates(lastDebug))
						{
							GeoJSONDebug.addMessageToSend(debugUrl + this.SubURL + "/" + featureBlock.GetIndex(), featureBlock.GetFeatureCollection());
						}
					}
				}
		}

	}

	public class GeoJSONDebug
	{
		static EventWaitHandle _waitHandle = new AutoResetEvent(false);
		private static bool _quitThread = false;
		private static List<MessageToSend> messagesToSend = new List<MessageToSend>();
		private static Thread sendThread;
		private static String DebugUrl;
		private static DateTime lastDebug;
		private static FeaturesBlockList PolygonFeatureBlocks = new FeaturesBlockList("polygons");
		private static FeaturesBlockList LineFeatureBlocks = new FeaturesBlockList("lines");
		private static FeaturesBlockList PointFeatureBlocks = new FeaturesBlockList("points");

		public static void SetURL(string url)
		{
			DebugUrl = url + "/api/put/";
		}

		internal static void addMessageToSend(string url, FeatureCollection featureCollection)
        {
            try
            {
				bool checkThread = false;
				string content = JsonConvert.SerializeObject(featureCollection);
                lock (messagesToSend)
                {
					messagesToSend.Add(new MessageToSend()
					{
						url = url,
						content = content
					});
					if( messagesToSend.Count >= 1)
                    {
						checkThread = true;
                    }
                }
				if( checkThread == true)
                {
					if (sendThread == null || sendThread.IsAlive == false)
					{
						sendThread = new Thread(GeoJSONDebug.sendMessagesThread);
						sendThread.Start();
					}
					_waitHandle.Set();

				}
			} catch (Exception e)
            {
				Console.WriteLine("ERROR: Could not convert feature collection to JSON Tree. Error: "+ e.Message);
            }

        }


		private static async void sendMessagesThread()
        {
            while (true)
            {
				String url = "";
				String value = "";
				bool shallStop = false;
				lock (messagesToSend)
				{
					if (messagesToSend.Count > 0)
					{
						url = messagesToSend[0].url;
						value = messagesToSend[0].content;
						messagesToSend.RemoveAt(0);
					} else
                    {
						shallStop = true;
                    }
				}
				if( shallStop == false)
                {
					try
					{
						using (HttpClient client = new HttpClient())
						{

							StringContent myStringContent = new StringContent(value, Encoding.UTF8, "application/json");
							HttpResponseMessage result = await client.PostAsync(url, myStringContent);
							if (result.IsSuccessStatusCode == false)
							{
								Trace.WriteLine("Error sending feature collection to " + url);
							}
						}
					}
					catch (Exception e)
					{
						Trace.WriteLine("Could not send HTTP Request. Error: " + e.Message);
						Trace.WriteLine("		Called URL: " + url);
					}
				} else
                {
					if (_quitThread == true)
					{
						_quitThread = false;
						return;
					}
					_waitHandle.WaitOne();
				}
			}
		}

		public static async void Debug()
		{

			await PolygonFeatureBlocks.Debug(DebugUrl, lastDebug);
			await PointFeatureBlocks.Debug(DebugUrl, lastDebug);
			await LineFeatureBlocks.Debug(DebugUrl, lastDebug);
			lastDebug = DateTime.Now;
		}

		public static void ClearPolygons(int index = -1)
		{
			if (index == -1)
			{
				PolygonFeatureBlocks.ClearAll();
			}
			else
			{
				PolygonFeatureBlocks.clear(index);
			}
		}

		public static void ClearPoints(int index = -1)
		{
			if (index == -1)
			{
				PointFeatureBlocks.ClearAll();
			}
			else
			{
				PointFeatureBlocks.clear(index);
			}
		}
		public static void ClearLines(int index = -1)
		{
			if (index == -1)
			{
				LineFeatureBlocks.ClearAll();
			}
			else
			{
				LineFeatureBlocks.clear(index);
			}
		}

		public static void AddFeature(Feature feature, int index = 0)
		{
			switch (feature.Geometry.Type)
			{
				case GeoJSON.Net.GeoJSONObjectType.Polygon:
					PolygonFeatureBlocks.AddFeature(feature, index);
					break;
				case GeoJSON.Net.GeoJSONObjectType.Point:
					PointFeatureBlocks.AddFeature(feature, index);
					break;
				case GeoJSON.Net.GeoJSONObjectType.LineString:
					LineFeatureBlocks.AddFeature(feature, index);
					break;
			}
		}

		public static async Task DebugClearFeatures()
		{
			await PointFeatureBlocks.DebugClearAll(DebugUrl);
			await PolygonFeatureBlocks.DebugClearAll(DebugUrl);
			await LineFeatureBlocks.DebugClearAll(DebugUrl);
		}


		public static void WaitDebugger()
        {
			if(sendThread == null || sendThread.IsAlive == false)
            {
				return;
            }
			bool goOn = false;
			do
			{
				sendThread.Join(40);
				lock (messagesToSend)
				{
					if (messagesToSend.Count == 0)
					{
						Thread.Sleep(200);//A final sleep to send this last message
						goOn = false;
					}
					else
					{
						goOn = true;
					}
				}
			} while (goOn == true);
			quitSendThread();
        }

        private static void quitSendThread()
        {
			_quitThread = true;
        }

		public static void Quit()
        {
			quitSendThread();
        }

        public static void ClearAll()
        {
			ClearPoints();
			ClearLines();
			ClearPolygons();
        }
    }
}
