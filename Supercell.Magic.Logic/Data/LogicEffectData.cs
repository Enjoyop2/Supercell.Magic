using Supercell.Magic.Titan.CSV;

namespace Supercell.Magic.Logic.Data
{
	public class LogicEffectData : LogicData
	{
		public LogicEffectData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			// LogicEffectData.
		}

		public string SWF
		{
			get; protected set;
		}
		public string ExportName
		{
			get; protected set;
		}
		protected string[] ParticleEmitter
		{
			get; set;
		}
		public int EmitterDelayMs
		{
			get; protected set;
		}
		public int CameraShake
		{
			get; protected set;
		}
		public int CameraShakeTimeMS
		{
			get; protected set;
		}
		public bool CameraShakeInReplay
		{
			get; protected set;
		}
		protected bool[] AttachToParent
		{
			get; set;
		}
		protected bool[] DetachAfterStart
		{
			get; set;
		}
		protected bool[] DestroyWhenParentDies
		{
			get; set;
		}
		public bool Looping
		{
			get; protected set;
		}
		protected string[] IsoLayer
		{
			get; set;
		}
		public bool Targeted
		{
			get; protected set;
		}
		public int MaxCount
		{
			get; protected set;
		}
		protected string[] Sound
		{
			get; set;
		}
		protected int[] Volume
		{
			get; set;
		}
		protected int[] MinPitch
		{
			get; set;
		}
		protected int[] MaxPitch
		{
			get; set;
		}
		public string LowEndSound
		{
			get; protected set;
		}
		public int LowEndVolume
		{
			get; protected set;
		}
		public int LowEndMinPitch
		{
			get; protected set;
		}
		public int LowEndMaxPitch
		{
			get; protected set;
		}
		public bool Beam
		{
			get; protected set;
		}

		public override void CreateReferences()
		{
			base.CreateReferences();
		}

		public string GetParticleEmitter(int index)
			=> ParticleEmitter[index];

		public bool GetAttachToParent(int index)
			=> AttachToParent[index];

		public bool GetDetachAfterStart(int index)
			=> DetachAfterStart[index];

		public bool GetDestroyWhenParentDies(int index)
			=> DestroyWhenParentDies[index];

		public string GetIsoLayer(int index)
			=> IsoLayer[index];

		public string GetSound(int index)
			=> Sound[index];

		public int GetVolume(int index)
			=> Volume[index];

		public int GetMinPitch(int index)
			=> MinPitch[index];

		public int GetMaxPitch(int index)
			=> MaxPitch[index];
	}
}