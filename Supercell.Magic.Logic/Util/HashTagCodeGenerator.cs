using Supercell.Magic.Titan.Math;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Util
{
	public class HashTagCodeGenerator
	{
		public static readonly HashTagCodeGenerator m_instance = new HashTagCodeGenerator();

		public const string CONVERSION_TAG = "#";
		public const string CONVERSION_CHARS = "0289PYLQGRJCUV";

		private readonly LogicLongToCodeConverterUtil m_codeConverterUtil;

		private HashTagCodeGenerator()
		{
			m_codeConverterUtil = new LogicLongToCodeConverterUtil(HashTagCodeGenerator.CONVERSION_TAG, HashTagCodeGenerator.CONVERSION_CHARS);
		}

		public string ToCode(LogicLong logicLong)
			=> m_codeConverterUtil.ToCode(logicLong);

		public LogicLong ToId(string value)
		{
			LogicLong id = m_codeConverterUtil.ToId(value);

			if (IsIdValid(id))
			{
				return id;
			}

			return null;
		}

		public bool IsIdValid(LogicLong id)
			=> id.GetHigherInt() != -1 && id.GetHigherInt() != -1;
	}
}