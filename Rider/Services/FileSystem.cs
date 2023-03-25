using Rider.Contracts;
using Rider.Contracts.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace Rider.Services
{
	internal class FileSystem : IFileSystem
	{
		private ITime Time { get; }

		public FileSystem(ITime time) 
		{
			Time = time;
		}
		public string GetFileNameWithoutExtension(string path) 
		{
			try
			{
				return Path.GetFileNameWithoutExtension(path);
			}
			catch 
			{
				return string.Empty;
			}
		}
		public string GetDirectoryName(string path)
		{
			try
			{
				return Path.GetDirectoryName(path) ?? string.Empty;
			}
			catch { return string.Empty; }

		}
		public string GetApplicationDirectory()
		{
			string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
			return System.IO.Path.GetDirectoryName(path) ?? throw new RiderException($"Invalid appliation path: {path}");
		}
		public void CreateDirectory(string path)
		{
			Directory.CreateDirectory(path);
		}
		public bool FileExist(string path)
		{
			return File.Exists(path);
		}
		public bool DirectoryExists(string path) 
		{
			return Directory.Exists(path);
		}
		public string[] GetDirectoryFiles(string path)
		{
			return Directory.GetFiles(path);
		}
		public T LoadData<T>(string path)
		{
			using(FileStream stream = File.OpenRead(path))
			{
				return JsonSerializer.Deserialize<T>(stream)?? throw new RiderException($"Cannot read file: {path}");
			}
		}
		public void SaveData<T>(string path, T data)
		{
			using (FileStream stream = File.Create(path))
			{
				JsonSerializerOptions options  = new() { WriteIndented = true };
				JsonSerializer.Serialize<T>(stream, data, options);
			}
		}
		public Stream OpenRead(string path)
		{
			return File.OpenRead(path);
		}
		public Stream OpenWrite(string path)
		{
			return File.OpenWrite(path);
		}
		public string AddTimeStamp(string fullPath)
		{
			try
			{
				string? ext = Path.GetExtension(fullPath); // returns .exe
				string? name = Path.GetFileNameWithoutExtension(fullPath); // returns File
				string? dir = Path.GetDirectoryName(fullPath); // returns C:\Program Files\Program
				string timeStamp = Time.TimeStamp;
				return $"{dir}\\{name}_{timeStamp}{ext}";

			}catch (Exception)
			{

			}
			return fullPath;
		}


	}
}
