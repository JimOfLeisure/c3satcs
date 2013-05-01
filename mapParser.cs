//    Copyright 2013 Jim Nelson
//
//    This file is part of Civ3 Show-And-Tell.

// For console output.
using System;

// For file I/O.
using System.IO;


class KickItOff {
	static void Main() {
		Console.WriteLine("Hello, Civ3'ers!");

		// FIX: Hard-coding the save file name to my test save in the current folder
		string SaveFilePath = "unc-test.sav";

		// Read the file into a byte array.
		byte[] SaveFileByteArray = File.ReadAllBytes(SaveFilePath);
		System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();
		Console.WriteLine(ascii.GetString(SaveFileByteArray, 0, 4));
	}
}