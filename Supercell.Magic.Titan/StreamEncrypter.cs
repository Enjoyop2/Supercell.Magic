namespace Supercell.Magic.Titan
{
	public class StreamEncrypter
	{
		public virtual int Decrypt(byte[] input, byte[] output, int length)
			=> 0;

		public virtual int Encrypt(byte[] input, byte[] output, int length)
			=> 0;

		public virtual int GetOverheadEncryption()
			=> 0;

		public virtual void Destruct()
		{
			// Destruct.
		}
	}
}