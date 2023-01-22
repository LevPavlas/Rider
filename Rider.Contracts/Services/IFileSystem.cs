using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rider.Contracts.Services
{
	public interface IFileSystem
	{
		string GetFileNameWithoutExtension(string path);
		string GetDirectoryName(string path);
		string GetApplicationDirectory();
		void CreateDirectory(string path);
		bool FileExist(string path);
		T LoadData<T>(string path);
		void SaveData<T>(string path, T data);
		string AddTimeStamp(string fullPath);
		Stream OpenRead(string path);
		Stream OpenWrite(string path);

	}
}
