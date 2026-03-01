using System;
using System.IO;
using System.Security.Cryptography;

using SevenZip;
using SevenZip.Compression.LZMA;

using Supercell.Magic.Titan.Json;

namespace Supercell.Magic.Tools.PatchGenerator
{
	internal class FileAsset
	{
		private readonly string m_filePath;

		private bool m_isCompressed;

		public byte[] Content
		{
			get; private set;
		}

		public FileAsset(string filePath, byte[] content)
		{
			m_filePath = filePath;
			Content = content;
		}

		public void Compress()
		{
			if (m_filePath.EndsWith(".csv"))
			{
				Encoder compressor = new Encoder();

				using (MemoryStream iStream = new MemoryStream(Content))
				{
					using (MemoryStream oStream = new MemoryStream())
					{
						CoderPropID[] coderPropIDs =
						{
							CoderPropID.DictionarySize, CoderPropID.PosStateBits, CoderPropID.LitContextBits, CoderPropID.LitPosBits,
							CoderPropID.Algorithm, CoderPropID.NumFastBytes, CoderPropID.MatchFinder, CoderPropID.EndMarker
						};

						object[] properties =
						{
							262144, 2, 3, 0, 2, 32, "bt4", false
						};

						compressor.SetCoderProperties(coderPropIDs, properties);
						compressor.WriteCoderProperties(oStream);

						oStream.Write(BitConverter.GetBytes(iStream.Length), 0, 4);

						compressor.Code(iStream, oStream, iStream.Length, -1L, null);

						Content = oStream.ToArray();
					}
				}
			}
			else if (m_filePath.EndsWith(".sc"))
			{
			}

			m_isCompressed = true;
		}

		public string GetSha()
		{
			if (!m_isCompressed)
			{
				throw new InvalidOperationException();
			}

			using (SHA1 sha = new SHA1Managed())
			{
				return BitConverter.ToString(sha.ComputeHash(Content)).Replace("-", string.Empty).ToLower();
			}
		}

		public void WriteTo(string output)
		{
			string filePath = output + "/" + m_filePath;

			Directory.CreateDirectory(Path.GetDirectoryName(filePath));
			File.WriteAllBytes(filePath, Content);
		}

		internal LogicJSONObject SaveToJson()
		{
			LogicJSONObject jsonRoot = new LogicJSONObject();

			/*if (m_filePath.Contains("highres_tex"))
			{
				jsonRoot.Put("defer", new LogicJSONBoolean(true));
			}*/

			jsonRoot.Put("file", new LogicJSONString(
				m_filePath.Replace("\\", "\\/").Replace("/", "\\/")
			));
			jsonRoot.Put("sha", new LogicJSONString(GetSha()));

			return jsonRoot;
		}
	}
}