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
		void DeleteDirectory(string path);
        bool FileExist(string path);
		bool DirectoryExists(string path);
		string[] GetDirectoryFiles(string path);
		T LoadData<T>(string path);
		void SaveData<T>(string path, T data);
		string AddTimeStamp(string fullPath);
		Stream OpenRead(string path);
		Stream OpenWrite(string path);

	}
}
