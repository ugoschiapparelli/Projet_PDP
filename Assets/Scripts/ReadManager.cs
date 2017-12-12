using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadManager : MonoBehaviour {

	private int nbData = 11;
	private int[] data;

	public bool  ReadFile(){
		data = new int[nbData];
		string fileName = "donnee.txt";
		if (File.Exists (fileName)) {
			TextReader reader;
			reader = new  StreamReader (fileName);

			string line;
			int i = 0;
			while (true) {
				line = reader.ReadLine ();
				if (line == null || line.Equals(""))
					break;
				//Debug.Log ("Je lis " + line);
				data [i] = int.Parse (line);
				//Debug.Log ("Je parse " + data[i]);
				i++;
			}
			reader.Close ();
			if(i == 0 && line.Equals("")){
				return false;
			} else {
				return true;
			}
		} else {
			return false;
		}
	}

	public int[] GetData(){
		return data;
	}

	public int GetNbData(){
		return nbData;
	}
}
