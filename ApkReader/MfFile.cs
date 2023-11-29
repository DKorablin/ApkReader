using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AlphaOmega.Debug
{
	/// <summary>MANIFEST.MF file reader and validator</summary>
	public class MfFile:IEnumerable<KeyValuePair<String, MfFile.HashWithType>>
	{
		/// <summary>Hash type description</summary>
		public class HashWithType
		{
			/// <summary>Hash type</summary>
			public HashType Type { get; set; }
			/// <summary>Hash</summary>
			public String Hash { get; set; }
		}

		/// <summary>Hash type</summary>
		public enum HashType
		{
			/// <summary>Unknown hash type</summary>
			Unknown,
			/// <summary>SHA-256</summary>
			Sha256,
			/// <summary>SHA-1</summary>
			Sha1,
		}

		private readonly Dictionary<String, HashWithType> _fileHash;

		/// <summary>Manifest-Version</summary>
		public String Version { get; }

		/// <summary>Built-By</summary>
		public String BuiltBy { get; }

		/// <summary>Created-By</summary>
		public String CreatedBy { get ; }

		/// <summary>Gets original file hash</summary>
		/// <param name="zipFilePath">Path to file in the apk</param>
		/// <returns>Previosly calculated hash or null if file not found</returns>
		public HashWithType this[String zipFilePath]
			=> this._fileHash.TryGetValue(zipFilePath, out HashWithType result) ? result : null;

		/// <summary>Create instance of MANIFEST.MF file reader</summary>
		/// <param name="payload">MANIFEST.MF file contents</param>
		public MfFile(Byte[] payload)
		{
			if(payload == null || payload.Length == 0)
				throw new ArgumentNullException(nameof(payload));


			String[] lines = Encoding.UTF8.GetString(payload).Split(new Char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			this._fileHash = new Dictionary<String, HashWithType>();
			for(Int32 loop = 0; loop < lines.Length; loop++)
			{
				String line = lines[loop];
				String key, value;
				if(!MfFile.SplitToKeyValue(lines[loop], out key, out value))
					continue;

				switch(key)
				{
				case "Manifest-Version":
					this.Version = value;
					break;
				case "Built-By":
					this.BuiltBy = value;
					break;
				case "Created-By":
					this.CreatedBy = value;
					break;
				case "Name":
					loop++;
					String key1, value1;
					Boolean isSplitted = MfFile.SplitToKeyValue(lines[loop], out key1, out value1);
					if(!isSplitted)
					{
						if(lines[loop - 1].Length == 70)
						{//Если строка больше 70 символов, то содержимое переносится на следующую строку
							value += lines[loop].Trim();
							isSplitted = MfFile.SplitToKeyValue(lines[loop + 1], out key1, out value1);
							if(isSplitted)
								loop++;
						}
						if(!isSplitted)
							continue;
					}

					switch(key1){
					case "SHA-256-Digest":
						this._fileHash.Add(value, new HashWithType() { Type = HashType.Sha256, Hash = value1, });
						break;
					case "SHA1-Digest":
						this._fileHash.Add(value, new HashWithType() { Type = HashType.Sha1, Hash = value1, });
						break;
					}
					break;
				}
			}
		}

		private static Boolean SplitToKeyValue(String line, out String key, out String value)
		{
			key = value = null;
			const String Separator = ": ";
			Int32 indexOfSeparator = line.IndexOf(Separator);
			if(indexOfSeparator == -1)
				return false;

			key = line.Substring(0, indexOfSeparator);
			value = line.Substring(indexOfSeparator + Separator.Length);
			return true;
		}

		/// <summary>Validate hash insid the file with original file</summary>
		/// <remarks>Insinde stream validation we have to convert to Byte[] to correctly validate file</remarks>
		/// <param name="zipFilePath">Path to the file inside APK file</param>
		/// <param name="file">File contents</param>
		/// <param name="fileSize">Size of the fine contents in stream</param>
		/// <returns>Hash validated succesfully or has not found or hash invalid</returns>
		public Boolean ValidateHash(String zipFilePath, Stream file, Int64 fileSize)
		{
			Byte[] fileContents;
			using(BinaryReader reader = new BinaryReader(file))
				fileContents = reader.ReadBytes((Int32)fileSize);
			return ValidateHash(zipFilePath, fileContents);
		}

		/// <summary>Validate hash insid the file with original file</summary>
		/// <param name="zipFilePath">Path to the file inside APK file</param>
		/// <param name="file">File contents</param>
		/// <returns>Hash validated succesfully or has not found or hash invalid</returns>
		public Boolean ValidateHash(String zipFilePath, Byte[] file)
		{
			HashWithType originalHash = this[zipFilePath];
			if(originalHash == null)
				return false;

			Byte[] realHash;
			switch(originalHash.Type)
			{
			case HashType.Sha256:
				using(SHA256 h = SHA256.Create())
					realHash = h.ComputeHash(file);
				break;
			case HashType.Sha1:
				using(SHA1 h = SHA1.Create())
					realHash = h.ComputeHash(file);
				break;
			default:
				throw new NotImplementedException();
			}

			return originalHash.Hash == Convert.ToBase64String(realHash);
		}

		/// <summary>Gets all file and hashes</summary>
		/// <returns>All files with hashes</returns>
		public IEnumerator<KeyValuePair<String, HashWithType>> GetEnumerator()
			=> this._fileHash.GetEnumerator();

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			=> this.GetEnumerator();
	}
}