using Supercell.Magic.Titan.CSV;

namespace Supercell.Magic.Logic.Data
{
	public class LogicProjectileData : LogicGameObjectData
	{
		private LogicSpellData m_hitSpellData;
		private LogicEffectData m_effectData;
		private LogicEffectData m_destroyedEffectData;
		private LogicEffectData m_bounceEffectData;
		private LogicParticleEmitterData m_particleEmiterData;

		private string m_swf;
		private string m_exportName;
		private string m_shadowSWF;
		private string m_shadowExportName;
		private string m_particleEmitter;

		private int m_startHeight;
		private int m_startOffset;

		private int m_speed;
		private int m_scale;
		private int m_slowdownDefensePercent;
		private int m_hitSpellLevel;
		private int m_ballisticHeight;
		private int m_trajectoryStyle;
		private int m_fixedTravelTime;
		private int m_damageDelay;
		private int m_targetPosRandomRadius;

		private bool m_randomHitPosition;
		private bool m_ballistic;
		private bool m_playOnce;
		private bool m_useRotate;
		private bool m_useTopLayer;
		private bool m_trackTarget;
		private bool m_useDirection;
		private bool m_scaleTimeline;
		private bool m_directionFrame;

		public LogicProjectileData(CSVRow row, LogicDataTable table) : base(row, table)
		{
			// LogicProjectileData.
		}

		public override void CreateReferences()
		{
			base.CreateReferences();

			m_swf = GetValue("SWF", 0);
			m_exportName = GetValue("ExportName", 0);
			m_shadowSWF = GetValue("ShadowSWF", 0);
			m_shadowExportName = GetValue("ShadowExportName", 0);

			m_startHeight = GetIntegerValue("StartHeight", 0);
			m_startOffset = GetIntegerValue("StartOffset", 0);
			m_randomHitPosition = GetBooleanValue("RandomHitPosition", 0);

			string particleEmiter = GetValue("ParticleEmitter", 0);

			if (particleEmiter.Length > 0)
			{
				m_particleEmiterData = LogicDataTables.GetParticleEmitterByName(particleEmiter, this);
			}

			m_ballistic = GetBooleanValue("IsBallistic", 0);
			m_speed = (GetIntegerValue("Speed", 0) << 9) / 100;
			m_playOnce = GetBooleanValue("PlayOnce", 0);
			m_useRotate = GetBooleanValue("UseRotate", 0);
			m_useTopLayer = GetBooleanValue("UseTopLayer", 0);
			m_scale = GetIntegerValue("Scale", 0);

			if (m_scale == 0)
			{
				m_scale = 100;
			}


			m_slowdownDefensePercent = GetIntegerValue("SlowdownDefencePercent", 0);
			m_hitSpellData = LogicDataTables.GetSpellByName(GetValue("HitSpell", 0), this);
			m_hitSpellLevel = GetIntegerValue("HitSpellLevel", 0);
			m_trackTarget = GetBooleanValue("DontTrackTarget", 0) ^ true;
			m_ballisticHeight = GetIntegerValue("BallisticHeight", 0);
			m_trajectoryStyle = GetIntegerValue("TrajectoryStyle", 0);
			m_fixedTravelTime = GetIntegerValue("FixedTravelTime", 0);
			m_damageDelay = GetIntegerValue("DamageDelay", 0);
			m_useDirection = GetBooleanValue("UseDirections", 0);
			m_scaleTimeline = GetBooleanValue("ScaleTimeline", 0);
			m_targetPosRandomRadius = GetIntegerValue("TargetPosRandomRadius", 0);
			m_directionFrame = GetBooleanValue("DirectionFrame", 0);
			m_effectData = LogicDataTables.GetEffectByName(GetValue("Effect", 0), this);
			m_destroyedEffectData = LogicDataTables.GetEffectByName(GetValue("DestroyedEffect", 0), this);
			m_bounceEffectData = LogicDataTables.GetEffectByName(GetValue("BounceEffect", 0), this);
		}

		public LogicSpellData GetHitSpell()
			=> m_hitSpellData;

		public LogicEffectData GetEffect()
			=> m_effectData;

		public LogicEffectData GetDestroyedEffect()
			=> m_destroyedEffectData;

		public LogicEffectData GetBounceEffect()
			=> m_bounceEffectData;

		public LogicParticleEmitterData GetParticleEmiter()
			=> m_particleEmiterData;

		public string GetSwf()
			=> m_swf;

		public string GetExportName()
			=> m_exportName;

		public string GetShadowSWF()
			=> m_shadowSWF;

		public string GetShadowExportName()
			=> m_shadowExportName;

		public string GetParticleEmitter()
			=> m_particleEmitter;

		public int GetStartHeight()
			=> m_startHeight;

		public int GetStartOffset()
			=> m_startOffset;

		public int GetSpeed()
			=> m_speed;

		public int GetScale()
			=> m_scale;

		public int GetSlowdownDefensePercent()
			=> m_slowdownDefensePercent;

		public int GetHitSpellLevel()
			=> m_hitSpellLevel;

		public int GetBallisticHeight()
			=> m_ballisticHeight;

		public int GetTrajectoryStyle()
			=> m_trajectoryStyle;

		public int GetFixedTravelTime()
			=> m_fixedTravelTime;

		public int GetDamageDelay()
			=> m_damageDelay;

		public int GetTargetPosRandomRadius()
			=> m_targetPosRandomRadius;

		public bool GetRandomHitPosition()
			=> m_randomHitPosition;

		public bool IsBallistic()
			=> m_ballistic;

		public bool GetPlayOnce()
			=> m_playOnce;

		public bool GetUseRotate()
			=> m_useRotate;

		public bool GetUseTopLayer()
			=> m_useTopLayer;

		public bool GetTrackTarget()
			=> m_trackTarget;

		public bool GetUseDirection()
			=> m_useDirection;

		public bool GetScaleTimeline()
			=> m_scaleTimeline;

		public bool GetDirectionFrame()
			=> m_directionFrame;
	}
}