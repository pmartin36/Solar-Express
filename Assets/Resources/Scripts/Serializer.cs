using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public static class Serializer<T> where T : class
{
	//"C:/Users/Paul/AppData/LocalLow/Silvae/

	public static void Serialize(T obj, string filename) {
		string file = Path.Combine(Application.persistentDataPath, filename);
		using (FileStream stm = new FileStream(file, FileMode.Create))
		{
			BinaryFormatter fmt = new BinaryFormatter();
			fmt.Serialize(stm, obj);
		}
	}

	public static T Deserialize(string filename) {
		string file = Path.Combine(Application.persistentDataPath, filename);
		if(!File.Exists(file)) {
			return default(T);
		}
		using (FileStream stm = new FileStream(file, FileMode.Open))
		{
			BinaryFormatter fmt = new BinaryFormatter();
			return fmt.Deserialize(stm) as T;
		}
	}

	public static T DeserializeLocal(string fileName) {
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
		TextAsset textAsset = Resources.Load(fileNameWithoutExtension) as TextAsset;
		Stream stream = new MemoryStream(textAsset.bytes);
		BinaryFormatter formatter = new BinaryFormatter();
		return formatter.Deserialize(stream) as T;
	}
}

