// ZLibCodec.cs
// ------------------------------------------------------------------
//
// Copyright (c) 2009 Dino Chiesa and Microsoft Corporation.  
// All rights reserved.
//
// This code module is part of DotNetZip, a zipfile class library.
//
// ------------------------------------------------------------------
//
// This code is licensed under the Microsoft Public License. 
// See the file License.txt for the license details.
// More info on: http://dotnetzip.codeplex.com
//
// ------------------------------------------------------------------
//
// last saved (in emacs): 
// Time-stamp: <2009-November-03 15:40:51>
//
// ------------------------------------------------------------------
//
// This module defines a Codec for ZLIB compression and
// decompression. This code extends code that was based the jzlib
// implementation of zlib, but this code is completely novel.  The codec
// class is new, and encapsulates some behaviors that are new, and some
// that were present in other classes in the jzlib code base.  In
// keeping with the license for jzlib, the copyright to the jzlib code
// is included below.
//
// ------------------------------------------------------------------
// 
// Copyright (c) 2000,2001,2002,2003 ymnk, JCraft,Inc. All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice,
// this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright 
// notice, this list of conditions and the following disclaimer in 
// the documentation and/or other materials provided with the distribution.
// 
// 3. The names of the authors may not be used to endorse or promote products
// derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL JCRAFT,
// INC. OR ANY CONTRIBUTORS TO THIS SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT,
// INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
// OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
// -----------------------------------------------------------------------
//
// This program is based on zlib-1.1.3; credit to authors
// Jean-loup Gailly(jloup@gzip.org) and Mark Adler(madler@alumni.caltech.edu)
// and contributors of zlib.
//
// -----------------------------------------------------------------------


using System;

using Interop = System.Runtime.InteropServices;

namespace Supercell.Magic.Tools.Client.Libs.ZLib
{
	[Interop.GuidAttribute("ebc25cf6-9120-4283-b972-0e5520d0000D"), Interop.ComVisibleAttribute(true), Interop.ClassInterfaceAttribute(Interop.ClassInterfaceType.AutoDispatch)]
#if !NETCF
#endif
	public sealed class ZLibCodec
	{
		public byte[] InputBuffer;

		public int NextIn;

		public int AvailableBytesIn;

		public long TotalBytesIn;

		public byte[] OutputBuffer;

		public int NextOut;

		public int AvailableBytesOut;

		public long TotalBytesOut;

		public string Message;

		internal DeflateManager dstate;
		internal InflateManager istate;

		internal uint m_Adler32;

		public CompressionLevel CompressLevel = CompressionLevel.Default;

		public int WindowBits = ZLibConstants.WindowBitsDefault;

		public CompressionStrategy Strategy = CompressionStrategy.Default;


		public int Adler32
		{
			get
			{
				return (int)m_Adler32;
			}
		}


		public ZLibCodec()
		{
		}

		public ZLibCodec(CompressionMode mode)
		{
			if (mode == CompressionMode.Compress)
			{
				int rc = InitializeDeflate();
				if (rc != ZLibConstants.Z_OK)
				{
					throw new ZLibException("Cannot initialize for deflate.");
				}
			}
			else if (mode == CompressionMode.Decompress)
			{
				int rc = InitializeInflate();
				if (rc != ZLibConstants.Z_OK)
				{
					throw new ZLibException("Cannot initialize for inflate.");
				}
			}
			else
			{
				throw new ZLibException("Invalid ZlibStreamFlavor.");
			}
		}

		public int InitializeInflate()
			=> InitializeInflate(WindowBits);

		public int InitializeInflate(bool expectRfc1950Header)
			=> InitializeInflate(WindowBits, expectRfc1950Header);

		public int InitializeInflate(int windowBits)
		{
			WindowBits = windowBits;
			return InitializeInflate(windowBits, true);
		}

		public int InitializeInflate(int windowBits, bool expectRfc1950Header)
		{
			WindowBits = windowBits;
			if (dstate != null)
			{
				throw new ZLibException("You may not call InitializeInflate() after calling InitializeDeflate().");
			}

			istate = new InflateManager(expectRfc1950Header);
			return istate.Initialize(this, windowBits);
		}

		public int Inflate(FlushType flush)
		{
			if (istate == null)
			{
				throw new ZLibException("No Inflate State!");
			}

			return istate.Inflate(flush);
		}


		public int EndInflate()
		{
			if (istate == null)
			{
				throw new ZLibException("No Inflate State!");
			}

			int ret = istate.End();
			istate = null;
			return ret;
		}

		public int SyncInflate()
		{
			if (istate == null)
			{
				throw new ZLibException("No Inflate State!");
			}

			return istate.Sync();
		}

		public int InitializeDeflate()
			=> m_InternalInitializeDeflate(true);

		public int InitializeDeflate(CompressionLevel level)
		{
			CompressLevel = level;
			return m_InternalInitializeDeflate(true);
		}


		public int InitializeDeflate(CompressionLevel level, bool wantRfc1950Header)
		{
			CompressLevel = level;
			return m_InternalInitializeDeflate(wantRfc1950Header);
		}


		public int InitializeDeflate(CompressionLevel level, int bits)
		{
			CompressLevel = level;
			WindowBits = bits;
			return m_InternalInitializeDeflate(true);
		}

		public int InitializeDeflate(CompressionLevel level, int bits, bool wantRfc1950Header)
		{
			CompressLevel = level;
			WindowBits = bits;
			return m_InternalInitializeDeflate(wantRfc1950Header);
		}

		private int m_InternalInitializeDeflate(bool wantRfc1950Header)
		{
			if (istate != null)
			{
				throw new ZLibException("You may not call InitializeDeflate() after calling InitializeInflate().");
			}

			dstate = new DeflateManager();
			dstate.WantRfc1950HeaderBytes = wantRfc1950Header;

			return dstate.Initialize(this, CompressLevel, WindowBits, Strategy);
		}

		public int Deflate(FlushType flush)
		{
			if (dstate == null)
			{
				throw new ZLibException("No Deflate State!");
			}

			return dstate.Deflate(flush);
		}

		public int EndDeflate()
		{
			if (dstate == null)
			{
				throw new ZLibException("No Deflate State!");
			}

			// TODO: dinoch Tue, 03 Nov 2009  15:39 (test this)
			//int ret = dstate.End();
			dstate = null;
			return ZLibConstants.Z_OK; //ret;
		}

		public void ResetDeflate()
		{
			if (dstate == null)
			{
				throw new ZLibException("No Deflate State!");
			}

			dstate.Reset();
		}


		public int SetDeflateParams(CompressionLevel level, CompressionStrategy strategy)
		{
			if (dstate == null)
			{
				throw new ZLibException("No Deflate State!");
			}

			return dstate.SetParams(level, strategy);
		}


		public int SetDictionary(byte[] dictionary)
		{
			if (istate != null)
			{
				return istate.SetDictionary(dictionary);
			}

			if (dstate != null)
			{
				return dstate.SetDictionary(dictionary);
			}

			throw new ZLibException("No Inflate or Deflate state!");
		}

		// Flush as much pending output as possible. All deflate() output goes
		// through this function so some applications may wish to modify it
		// to avoid allocating a large strm->next_out buffer and copying into it.
		// (See also read_buf()).
		internal void flush_pending()
		{
			int len = dstate.pendingCount;

			if (len > AvailableBytesOut)
			{
				len = AvailableBytesOut;
			}

			if (len == 0)
			{
				return;
			}

			if (dstate.pending.Length <= dstate.nextPending || OutputBuffer.Length <= NextOut || dstate.pending.Length < dstate.nextPending + len ||
				OutputBuffer.Length < NextOut + len)
			{
				throw new ZLibException(string.Format("Invalid State. (pending.Length={0}, pendingCount={1})", dstate.pending.Length, dstate.pendingCount));
			}

			Array.Copy(dstate.pending, dstate.nextPending, OutputBuffer, NextOut, len);

			NextOut += len;
			dstate.nextPending += len;
			TotalBytesOut += len;
			AvailableBytesOut -= len;
			dstate.pendingCount -= len;
			if (dstate.pendingCount == 0)
			{
				dstate.nextPending = 0;
			}
		}

		// Read a new buffer from the current input stream, update the adler32
		// and total number of bytes read.  All deflate() input goes through
		// this function so some applications may wish to modify it to avoid
		// allocating a large strm->next_in buffer and copying from it.
		// (See also flush_pending()).
		internal int read_buf(byte[] buf, int start, int size)
		{
			int len = AvailableBytesIn;

			if (len > size)
			{
				len = size;
			}

			if (len == 0)
			{
				return 0;
			}

			AvailableBytesIn -= len;

			if (dstate.WantRfc1950HeaderBytes)
			{
				m_Adler32 = Adler.Adler32(m_Adler32, InputBuffer, NextIn, len);
			}

			Array.Copy(InputBuffer, NextIn, buf, start, len);
			NextIn += len;
			TotalBytesIn += len;
			return len;
		}
	}
}