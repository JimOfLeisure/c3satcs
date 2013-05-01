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

		// Open the binary file.
		// Does the 'using' statement do anything for me here? Seems to crash if the file doesn't exist.
		using (BinaryReader SaveStream = new BinaryReader(File.Open(SaveFilePath,FileMode.Open))) {
			Console.WriteLine("Inside the using file statement");
			byte[] SaveFileByteArray = SaveStream.ReadAllBytes();
			System.Text.ASCIIEncoding ascii = new ASCIIEncoding();
			Console.WriteLine(ascii.GetString(SaveFileByteArray, 0, 4));
		}
	}
}