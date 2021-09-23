using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Skyhoshi.Configuration.InternalCrypto
{
	public class CryptoInformation
	{
		public CryptoInformation() : this(true)
		{

            if (keyArray == null)
            {
                aes = new AesManaged();
                aes.GenerateKey();
                keyArray = aes.Key;
            }


            DebugOutputArray(keyArray, "Key Array: ");

            if (IVArray == null)
            {
                if (aes == null) aes = new AesManaged();
                aes.GenerateIV();
                IVArray = aes.IV;
            }

            DebugOutputArray(IVArray, "IV Array");
            SaveCryptInfo();

        }

        public CryptoInformation(bool LoadInfo)
        {
            LoadCryptoInfo();
        }

		public bool UseCustomKey { get; set; } = false;
		public bool UseCustomInitializationVector { get; set; } = true;
		public AesManaged aes { get; set; }

		private static byte[] keyArray = null;
		public byte[] Key
		{
			get => keyArray;
			set => keyArray = value;
		}
		private static byte[] IVArray = null;
		public byte[] IV
		{
			get => IVArray;
			set => IVArray = value;
		}

		public char[] KeyChars => Encoding.Default.GetChars(Key);
		public string TypeNameString { get; set; }


		public string parameterFileLocation { get; set; } = $@"k:\Data\parameterFile.ckpf";
		public string binaryFileLocation { get; set; } = $@"k:\Data\encodedFile.ckpf";


		public void SaveCryptInfo()
		{
			if (File.Exists(parameterFileLocation) && File.Exists(binaryFileLocation)) { return; }
			string writeValue = "";

			StringBuilder sb = new StringBuilder();
			sb.AppendLine(Key.Length.ToString());
			sb.AppendLine(IV.Length.ToString());
			sb.AppendLine(UseCustomKey.ToString());
			sb.AppendLine(UseCustomInitializationVector.ToString());
			writeValue = sb.ToString();

			using (FileStream fileStream = new FileStream(parameterFileLocation, FileMode.CreateNew))
			{
				byte[] writeBytesValues = GetBytesFromString(writeValue);
				fileStream.WriteAsync(writeBytesValues, 0, writeBytesValues.Length);
			}

			Stream keystream = new FileStream(binaryFileLocation, FileMode.CreateNew);
			using (System.IO.BinaryWriter binaryWriter = new BinaryWriter(keystream))
			{
				binaryWriter.Write(Key);
				binaryWriter.Write(IV);
				binaryWriter.Write(UseCustomKey);
				binaryWriter.Write(UseCustomInitializationVector);
			}
		}

		public void LoadCryptoInfo()
		{
			int KeyLength = 0;
			int IVLength = 0;

			using (FileStream fileStream = new FileStream(parameterFileLocation, FileMode.Open))
			{
				fileStream.Position = 0;
				using (StreamReader reader = new StreamReader(fileStream))
				{
					string KeyValue = reader.ReadLine().ToString();
					KeyLength = int.Parse(KeyValue);
					string IVValue = reader.ReadLine().ToString();
					IVLength = int.Parse(IVValue);
					UseCustomKey = bool.Parse(reader.ReadLine().ToString());
					UseCustomInitializationVector = bool.Parse(reader.ReadLine().ToString());
				}

			}

			Stream keystream = new FileStream(binaryFileLocation, FileMode.Open);
			using (System.IO.BinaryReader binaryReader = new BinaryReader(keystream))
			{
				keyArray = binaryReader.ReadBytes(KeyLength);
				IVArray = binaryReader.ReadBytes(IVLength);
			}
		}

		public byte[] StringToByteArray(string hex)
		{
			if (hex.Contains("-"))
			{
				hex = hex.Replace("-", "");
			}
			int NumberChars = hex.Length;
			byte[] bytes = new byte[NumberChars / 2];
			for (int i = 0; i < NumberChars; i += 2)
			{
				bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
			}
			return bytes;
		}

		internal string ByteArrayToHex(byte[] bit)
		{
			return BitConverter.ToString(bit);//.Replace("-","")
		}

		internal byte[] GetBytesFromString(string textToBytes)
		{
			return Encoding.Default.GetBytes(textToBytes);
		}

		public string GetString(byte[] array)
		{
			return Encoding.Default.GetString(array);
		}

		public void DebugOutputArray(byte[] array, string outputMessage = "Array: ")
		{
			string bytesOfArray = "";
			foreach (byte b in array)
			{
				bytesOfArray += $"{b.ToString()},";
			}

			bytesOfArray = bytesOfArray.Remove(bytesOfArray.Length - 1);
			//System.Diagnostics.Debug.WriteLine($"{outputMessage}{bytesOfArray}");
		}
	}
}
