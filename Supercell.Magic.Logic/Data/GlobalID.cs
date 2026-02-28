namespace Supercell.Magic.Logic.Data
{
	public class GlobalID
	{
		public static int CreateGlobalID(int classId, int instanceId)
			=> 1000000 * classId + instanceId;

		public static int GetInstanceID(int globalId)
			=> globalId % 1000000;

		public static int GetClassID(int globalId)
			=> globalId / 1000000;
	}
}