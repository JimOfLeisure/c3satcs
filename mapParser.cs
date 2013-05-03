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
	protected internal string Name;
	protected internal byte[] buffer;
	// Constructor.
	public GenericSection(MyBinaryReader saveStream) {
		Console.WriteLine("GenericSection constrcutor");
		this.Name = saveStream.ReadCharString(4);
		Console.WriteLine(this.Name);
	}

	// Constructor with specified data length.
	public GenericSection(MyBinaryReader saveStream, int length) {
		Console.WriteLine("GenericSection constrcutor");
		this.Name = saveStream.ReadCharString(4);
		Console.WriteLine(this.Name);
		buffer = saveStream.ReadBytes(length);
	}

}

class SectionWithLength: GenericSection {
	protected int Length;
	// protected byte[] buffer;
	// Constructor.
	public SectionWithLength(MyBinaryReader saveStream) :base(saveStream) {
		Console.WriteLine("SectionWithLength constrcutor");
		this.Length = saveStream.ReadInt32();
		//Console.WriteLine(this.Name);
		Console.WriteLine(this.Length);
		buffer = saveStream.ReadBytes(this.Length);
		

	}
}

class SectionWithLengthNoName {
	protected int Length;
	protected byte[] buffer;
	// Constructor.
	public SectionWithLengthNoName(MyBinaryReader saveStream) {
		Console.WriteLine("SectionWithLengthNoName constrcutor");
		this.Length = saveStream.ReadInt32();
		Console.WriteLine(this.Length);
		buffer = saveStream.ReadBytes(this.Length);
	}
}

class SectionWithSubrecords: GenericSection {
	protected int numSubRecords;
	protected List<SectionWithLengthNoName> subRecord;
	public SectionWithSubrecords(MyBinaryReader saveStream) :base(saveStream) {
		this.subRecord = new List<SectionWithLengthNoName>();
		this.numSubRecords = saveStream.ReadInt32();
		for (int i = 0; i < this.numSubRecords; i++) {
			this.subRecord.Add(new SectionWithLengthNoName(saveStream));
			//Console.WriteLine(i);
		}
	}
}

class BicqSection: GenericSection {
	protected SectionWithSubrecords verNum, game;
	public BicqSection(MyBinaryReader saveStream) :base (saveStream) {
		this.verNum = new SectionWithSubrecords(saveStream);
		this.game = new SectionWithSubrecords(saveStream);
	}
}

// The object to read the save game stream and extract info.
class civ3Game {
	protected GenericSection civ3Section, gameSection;
	protected SectionWithLength bicSection;
	protected BicqSection bicqSection;
	protected List<SectionWithLength> tileSection, pileOfSectionsToSortLater;
	protected List<byte[]> bytePads;

	// Constructor.
	public civ3Game(MyBinaryReader saveStream) {
		// Initialize my list of sections I don't know what to do with yet.
		this.pileOfSectionsToSortLater = new List<SectionWithLength>();

		// Initialize empty byte[] pad list.
		this.bytePads = new List<byte[]>();
		
		// Initialize empty tile list.
		this.tileSection = new List<SectionWithLength>();
		
		// Read save file stream assuming fixed order and general structure.
		// Trying to use class constructors but reading some padding in manually.
		// FIX: hard-coding CIV3 length of 0x1a
		this.civ3Section = new GenericSection(saveStream, 0x1a);
		this.bicSection = new SectionWithLength(saveStream);
		this.bicqSection = new BicqSection(saveStream);
		this.gameSection = new GenericSection(saveStream, 0x0ecf);

		// reading in five sections (DATE, PLGI, PLGI, DATE and DATE)
		this.pileOfSectionsToSortLater.Add(new SectionWithLength(saveStream));
		this.pileOfSectionsToSortLater.Add(new SectionWithLength(saveStream));
		this.pileOfSectionsToSortLater.Add(new SectionWithLength(saveStream));
		this.pileOfSectionsToSortLater.Add(new SectionWithLength(saveStream));
		this.pileOfSectionsToSortLater.Add(new SectionWithLength(saveStream));

		// FIX: hard-coded byte padding here. Is it algined? Other way to realign?
		this.bytePads.Add(saveStream.ReadBytes(8));

		// reading CNSL section
		this.pileOfSectionsToSortLater.Add(new SectionWithLength(saveStream));

		// reading three WRLD sections
		this.pileOfSectionsToSortLater.Add(new SectionWithLength(saveStream));
		this.pileOfSectionsToSortLater.Add(new SectionWithLength(saveStream));

		// FIX: Ugly hack to grab presumed map width and height.
		int mapWidth = BitConverter.ToInt32(pileOfSectionsToSortLater[pileOfSectionsToSortLater.Count - 1].buffer, 0x04);
		int mapHeight = BitConverter.ToInt32(pileOfSectionsToSortLater[pileOfSectionsToSortLater.Count - 1].buffer, 0x18);
		Console.WriteLine(mapWidth);
		Console.WriteLine(mapHeight);

		this.pileOfSectionsToSortLater.Add(new SectionWithLength(saveStream));
		
		// FIX: hard-seeking to first TILE section of my test file.
		// saveStream.BaseStream.Seek(0x34a4, SeekOrigin.Begin);
		
		// Read all TILE sections. There are mapWidth/2 * mapHeight * 4 of them
		// FIX: I really want one entity per game tile, but there are four TILE sections per game tile
		for (int i = 0; i < (mapWidth / 2) * mapHeight * 4; i++) {
			this.tileSection.Add(new SectionWithLength(saveStream));
		}
		Console.WriteLine(tileSection.Count);


/*
		// FIX: Need to load all tiles. Load a single tile into the list.
		this.tileSection.Add(new SectionWithLength(saveStream));
		this.tileSection.Add(new SectionWithLength(saveStream));
		this.tileSection.Add(new SectionWithLength(saveStream));
		this.tileSection.Add(new SectionWithLength(saveStream));
		this.tileSection.Add(new SectionWithLength(saveStream));
		this.tileSection.Add(new SectionWithLength(saveStream));
*/
		// Iterating and printing out section names of my junk pile.
		foreach (SectionWithLength mySection in this.pileOfSectionsToSortLater) {
			Console.WriteLine(mySection.Name);
		}
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