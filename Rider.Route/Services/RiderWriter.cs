using Rider.Contracts.Services;
using Rider.Route.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rider.Route.Services
{
	internal interface IRiderWriter
	{
		void Export(RiderData data, string file);
	}
	internal class RiderWriter: IRiderWriter
	{
		const int Reserved = 0;

		private IFileSystem FileSystem { get; }

		public RiderWriter(IFileSystem fileSystem)
		{
			FileSystem = fileSystem;
		}
		public void Export(RiderData data, string file)
		{
			if (data?.Route?.Points?.Count > 2)
			{
				using (Stream stream = FileSystem.OpenWrite(file))
				{
					using (BinaryWriter writer = new BinaryWriter(stream))
					{
						WriteTrack(writer, data.Route.Points);
					}
				}
				Console.WriteLine($"Created file: {file}");

				string smyFile = System.IO.Path.ChangeExtension(file, "smy");
				using (Stream stream = FileSystem.OpenWrite(smyFile))
				{
					using (BinaryWriter writer = new BinaryWriter(stream))
					{
						WriteSmy(writer, data.Route);
					}
				}

				Console.WriteLine($"Created file: {smyFile}");

				string tinfoFile = System.IO.Path.ChangeExtension(file, "tinfo");
				using (Stream stream = FileSystem.OpenWrite(tinfoFile))
				{
					using (BinaryWriter writer = new BinaryWriter(stream))
					{
						var challenges = from challenge in data.Challenges
										 orderby challenge.Start
										 select challenge;
						WriteTinfo(writer, challenges);
					}
				}
				Console.WriteLine($"Created file: {tinfoFile}");
			}
		}

		public void WriteTrack(BinaryWriter writer, IReadOnlyList<IPoint> points)
		{
			for (int i = 0; i < points.Count; i++)
			{
				IPoint p = points[i];
				writer.Write(Convert.ToInt32(p.Latitude * 1000000.0));
				writer.Write(Convert.ToInt32(p.Longitude * 1000000.0));
				writer.Write(Convert.ToInt16(p.Elevation));
				writer.Write((short)(byte)Convert.ToInt32(p.Grade));
				writer.Write(Reserved);
			}
		}

		public void WriteSmy(BinaryWriter writer, IRoute route)
		{
			//int altitudegain = 798;

			byte[] reserved01 = new byte[] { 1, 0 }; //dword 2
			writer.Write(reserved01);                               // byte 0-1
			short count = (short)route.Points.Count;
			writer.Write(count);                                    // byte 2-3
			writer.Write(Convert.ToInt32(route.LatitudeMaxNorth * 1000000.0));  // byte 4-7
			writer.Write(Convert.ToInt32(route.LatitudeMinSouth * 1000000.0));  // byte 8-11
			writer.Write(Convert.ToInt32(route.LongitudeMaxEast * 1000000.0));  // byte 12-15
			writer.Write(Convert.ToInt32(route.LongitudeMinWest * 1000000.0));  // byte 16-19
			writer.Write(Convert.ToInt32(route.Distance));          // byte 20-23 

			writer.Write(Convert.ToInt16(route.ElevationMax));      // byte 24-25 maximum altitude																	
			writer.Write(Convert.ToInt16(0));      // byte 26-27 minimum altitude ??? i have no clue what it is.
			writer.Write(new byte[32]);                             // byte 28-59
			writer.Write(Convert.ToInt16(route.ElevationGain));     // byte 60-61 elevation gain
			writer.Write(new byte[6]);                              // byte 62-67

		}
		public void WriteTinfo(BinaryWriter writer, IEnumerable<ClimbChallenge> challenges)
		{

			// POI 
			// 2 bytes for coordinate index
			// 1 byte for direction
			//		0x18 - turn-over
			//		0x1c - ferry
			//		0x65 - peak
			//		0x03 - left
			//		0x02 - right
			//		0x07 - close left
			//		0x06 - close right
			//		0x05 - slight left
			//		0x04 - slight right
			//		0x01 - go ahead
			//		0x01 - go ahead
			//		0x08 - exit right
			//		0x07 - uturn left
			//		0x01 - go ahead
			//		0x01 - go ahead
			//		0x09 - exit left
			//		0x08 - exit right
			//		0x01 - go ahead
			//		0xff - none
			// 1 byte reserved 0x00
			// 2 byte distance in meters
			// 2 byte reserved 0x00 0x00
			// 2 byte time in seconds
			// 2 byte reserved 0x00 0x00
			// 32 byte instruction description

			// Climb info
			// 2 bytes for coordinate index
			// BE - start
			// 1 byte reserved 0x00
			// 8 bytes FF
			// 32 bytes 0 ? descritpion??
			// 2 bytes for coordinate index
			// BF - end
			// 1 byte reserved 0x00
			// 8 bytes FF
			// 32 bytes 0 ? descritpion??

			byte start = 0xBE;
			byte end = 0xBF;
			byte reserved0 = (byte)0;
			byte[] reservedFF = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
			byte[] descriptor = new byte[]
			{
				0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0
			};

			foreach (ClimbChallenge c in challenges)
			{
				writer.Write((ushort)c.Start); //word 0
				writer.Write(start);
				writer.Write(reserved0);

				writer.Write(reservedFF); //word 1 -2
				writer.Write(descriptor);// word 3- 10
				writer.Write((ushort)c.End);//word 11
				writer.Write(end);
				writer.Write(reserved0);
				writer.Write(reservedFF); //word 12 - 13
				writer.Write(descriptor); //word 14 - 21
			}
		}

	}
}
