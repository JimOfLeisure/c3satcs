//    Copyright 2013 Jim Nelson
//
//    This file is part of Civ3 Show-And-Tell.

// For console output.
using System;

// For file I/O.
using System.IO;

// Base class for reading SAV files.
class GenericSection {
	private string Name;
	private int Length;
	// Constructor.
	public GenericSection(BinaryReader SaveStream) {
		Console.WriteLine("GenericSection constrcutor");
		//this.Name = (SaveStream.ReadChars(4));
		Console.WriteLine(SaveStream.ReadChars(4));
	}
}

class KickItOff {
	static void Main() {
		Console.WriteLine("Hello, Civ3'ers!");

		// FIX: Hard-coding the save file name to my test save in the current folder
		string SaveFilePath = "unc-test.sav";

/* Trying BinaryReader instead of File.ReadAllBytes
		// Read the file into a byte array.
		byte[] SaveFileByteArray = File.ReadAllBytes(SaveFilePath);
		System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();
		Console.WriteLine(ascii.GetString(SaveFileByteArray, 0, 4));
*/

		// Open the binary file.
		// Does the 'using' statement do anything for me here? Seems to crash if the file doesn't exist.
		using (BinaryReader SaveStream = new BinaryReader(File.Open(SaveFilePath,FileMode.Open))) {
			Console.WriteLine("Inside the using file statement");
			Console.WriteLine(SaveStream.ReadChars(4));
			
			// FIX: hard-seeking to first TILE section of my test file.
			SaveStream.BaseStream.Seek(0x34a4, SeekOrigin.Begin);
			GenericSection TempVar = new GenericSection(SaveStream);
		}
	}
}