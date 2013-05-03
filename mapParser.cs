//    Copyright 2013 Jim Nelson
//
//    This file is part of Civ3 Show-And-Tell.

// For console output.
using System;

// For file I/O.
using System.IO;

// For Lists.
using System.Collections.Generic;

// Extending BinaryReader to read 4-char strings from stream.
class MyBinaryReader: System.IO.BinaryReader {
	// Constructor.
	public MyBinaryReader(System.IO.Stream input) :base(input) {
	}
	
	// Read char[] of given length and return string.
	public string ReadCharString(int length) {
		char[] charArray = this.ReadChars(length);
		string myString = new string(charArray);
		return myString;
	}
}

// Base class for reading SAV files.
class GenericSection {
	protected string Name;
	// Constructor.
	public GenericSection(MyBinaryReader saveStream) {
		Console.WriteLine("GenericSection constrcutor");
		this.Name = saveStream.ReadCharString(4);
		Console.WriteLine(this.Name);
	}
}

class SectionWithLength: GenericSection {
	protected int Length;
	protected byte[] buffer;
	// Constructor.
	public SectionWithLength(MyBinaryReader saveStream) :base(saveStream) {
		Console.WriteLine("SectionWithLength constrcutor");
		this.Length = saveStream.ReadInt32();
		Console.WriteLine(this.Name);
		Console.WriteLine(this.Length);
		buffer = saveStream.ReadBytes(this.Length);
		

	}
}

// The object to read the save game stream and extract info.
class civ3Game {
	protected GenericSection civ3Section;
	protected List<SectionWithLength> tileSection;

	// Constructor.
	public civ3Game(MyBinaryReader saveStream) {
		this.civ3Section = new GenericSection(saveStream);

		// FIX: hard-seeking to first TILE section of my test file.
		saveStream.BaseStream.Seek(0x34a4, SeekOrigin.Begin);
		
		// Initialize empty tile list.
		this.tileSection = new List<SectionWithLength>();
		
		// FIX: Need to load all tiles. Load a single tile into the list.
		this.tileSection.Add(new SectionWithLength(saveStream));
		this.tileSection.Add(new SectionWithLength(saveStream));
		this.tileSection.Add(new SectionWithLength(saveStream));
		this.tileSection.Add(new SectionWithLength(saveStream));
		this.tileSection.Add(new SectionWithLength(saveStream));
		this.tileSection.Add(new SectionWithLength(saveStream));
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
		using (MyBinaryReader saveStream = new MyBinaryReader(File.Open(SaveFilePath,FileMode.Open))) {
			//Console.WriteLine("Inside the using file statement");
			//Console.WriteLine(saveStream.ReadChars(4));
			
			//GenericSection TempVar = new GenericSection(saveStream);
			//SectionWithLength TempVar = new SectionWithLength(saveStream);
			civ3Game game = new civ3Game(saveStream);
		}
	}
}