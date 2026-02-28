using Supercell.Magic.Titan.CSV;

namespace Supercell.Magic.Logic.Data
{
	public class LogicBoomboxData : LogicData
	{
		private bool m_enabled;
		private bool m_enabledLowMemory;
		private bool m_preLoading;
		private bool m_preLoadingLowMemory;

		private string[] m_disabledDevices;
		private string[] m_supportedPlatforms;
		private string[] m_supportedPlatformsVersion;
		private string[] m_allowedDomains;
		private string[] m_allowedUrls;

		public LogicBoomboxData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			// LogicBoomboxData.
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			m_enabled = GetBooleanValue("Enabled", 0);
			m_enabledLowMemory = GetBooleanValue("EnabledLowMem", 0);
			m_preLoading = GetBooleanValue("PreLoading", 0);
			m_preLoadingLowMemory = GetBooleanValue("PreLoadingLowMem", 0);

			m_disabledDevices = new string[GetArraySize("DisabledDevices")];

			for (int i = 0; i < m_disabledDevices.Length; i++)
			{
				m_disabledDevices[i] = GetValue("DisabledDevices", i);
			}

			m_supportedPlatforms = new string[GetArraySize("SupportedPlatforms")];

			for (int i = 0; i < m_supportedPlatforms.Length; i++)
			{
				m_supportedPlatforms[i] = GetValue("SupportedPlatforms", i);
			}

			m_supportedPlatformsVersion = new string[GetArraySize("SupportedPlatformsVersion")];

			for (int i = 0; i < m_supportedPlatformsVersion.Length; i++)
			{
				m_supportedPlatformsVersion[i] = GetValue("SupportedPlatformsVersion", i);
			}

			m_allowedDomains = new string[GetArraySize("AllowedDomains")];

			for (int i = 0; i < m_allowedDomains.Length; i++)
			{
				m_allowedDomains[i] = GetValue("AllowedDomains", i);
			}

			m_allowedUrls = new string[GetArraySize("AllowedUrls")];

			for (int i = 0; i < m_allowedUrls.Length; i++)
			{
				m_allowedUrls[i] = GetValue("AllowedUrls", i);
			}
		}
	}
}