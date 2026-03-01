using Supercell.Magic.Titan.CSV;
using Supercell.Magic.Titan.Debug;
using Supercell.Magic.Titan.Util;

namespace Supercell.Magic.Logic.Data
{
	public static class LogicResources
	{
		public static LogicArrayList<LogicDataTableResource> CreateDataTableResourcesArray()
		{
			LogicArrayList<LogicDataTableResource> arrayList = new LogicArrayList<LogicDataTableResource>(LogicDataTables.TABLE_COUNT);

			arrayList.Add(new LogicDataTableResource("logic/buildings.csv", DataType.BUILDING, 0));
			arrayList.Add(new LogicDataTableResource("logic/locales.csv", DataType.LOCALE, 0));
			arrayList.Add(new LogicDataTableResource("logic/resources.csv", DataType.RESOURCE, 0));
			arrayList.Add(new LogicDataTableResource("logic/characters.csv", DataType.CHARACTER, 0));
			// arrayList.Add(new LogicDataTableResource("csv/animations.csv", LogicDataType.ANIMATION, 0));
			arrayList.Add(new LogicDataTableResource("logic/projectiles.csv", DataType.PROJECTILE, 0));
			arrayList.Add(new LogicDataTableResource("logic/building_classes.csv", DataType.BUILDING_CLASS, 0));
			arrayList.Add(new LogicDataTableResource("logic/obstacles.csv", DataType.OBSTACLE, 0));
			arrayList.Add(new LogicDataTableResource("logic/effects.csv", DataType.EFFECT, 0));
			arrayList.Add(new LogicDataTableResource("csv/particle_emitters.csv", DataType.PARTICLE_EMITTER, 0));
			arrayList.Add(new LogicDataTableResource("logic/experience_levels.csv", DataType.EXPERIENCE_LEVEL, 0));
			arrayList.Add(new LogicDataTableResource("logic/traps.csv", DataType.TRAP, 0));
			arrayList.Add(new LogicDataTableResource("logic/alliance_badges.csv", DataType.ALLIANCE_BADGE, 0));
			arrayList.Add(new LogicDataTableResource("logic/globals.csv", DataType.GLOBAL, 0));
			arrayList.Add(new LogicDataTableResource("logic/townhall_levels.csv", DataType.TOWNHALL_LEVEL, 0));
			arrayList.Add(new LogicDataTableResource("logic/alliance_portal.csv", DataType.ALLIANCE_PORTAL, 0));
			arrayList.Add(new LogicDataTableResource("logic/npcs.csv", DataType.NPC, 0));
			arrayList.Add(new LogicDataTableResource("logic/decos.csv", DataType.DECO, 0));
			arrayList.Add(new LogicDataTableResource("csv/resource_packs.csv", DataType.RESOURCE_PACK, 0));
			arrayList.Add(new LogicDataTableResource("logic/shields.csv", DataType.SHIELD, 0));
			arrayList.Add(new LogicDataTableResource("logic/missions.csv", DataType.MISSION, 0));
			arrayList.Add(new LogicDataTableResource("csv/billing_packages.csv", DataType.BILLING_PACKAGE, 0));
			arrayList.Add(new LogicDataTableResource("logic/achievements.csv", DataType.ACHIEVEMENT, 0));
			arrayList.Add(new LogicDataTableResource("csv/credits.csv", DataType.CREDIT, 0));
			arrayList.Add(new LogicDataTableResource("csv/faq.csv", DataType.FAQ, 0));
			arrayList.Add(new LogicDataTableResource("logic/spells.csv", DataType.SPELL, 0));
			arrayList.Add(new LogicDataTableResource("csv/hints.csv", DataType.HINT, 0));
			arrayList.Add(new LogicDataTableResource("logic/heroes.csv", DataType.HERO, 0));
			arrayList.Add(new LogicDataTableResource("logic/leagues.csv", DataType.LEAGUE, 0));
			arrayList.Add(new LogicDataTableResource("csv/news.csv", DataType.NEWS, 0));
			arrayList.Add(new LogicDataTableResource("logic/war.csv", DataType.WAR, 0));
			arrayList.Add(new LogicDataTableResource("logic/regions.csv", DataType.REGION, 0));
			arrayList.Add(new LogicDataTableResource("csv/client_globals.csv", DataType.CLIENT_GLOBAL, 0));
			arrayList.Add(new LogicDataTableResource("logic/alliance_badge_layers.csv", DataType.ALLIANCE_BADGE_LAYER, 0));
			arrayList.Add(new LogicDataTableResource("logic/alliance_levels.csv", DataType.ALLIANCE_LEVEL, 0));
			arrayList.Add(new LogicDataTableResource("csv/helpshift.csv", DataType.HELPSHIFT, 0));
			arrayList.Add(new LogicDataTableResource("logic/variables.csv", DataType.VARIABLE, 0));
			arrayList.Add(new LogicDataTableResource("logic/gem_bundles.csv", DataType.GEM_BUNDLE, 0));
			arrayList.Add(new LogicDataTableResource("logic/village_objects.csv", DataType.VILLAGE_OBJECT, 0));
			arrayList.Add(new LogicDataTableResource("logic/calendar_event_functions.csv", DataType.CALENDAR_EVENT_FUNCTION, 0));
			arrayList.Add(new LogicDataTableResource("csv/boombox.csv", DataType.BOOMBOX, 0));
			arrayList.Add(new LogicDataTableResource("csv/event_entries.csv", DataType.EVENT_ENTRY, 0));
			arrayList.Add(new LogicDataTableResource("csv/deeplinks.csv", DataType.DEEPLINK, 0));
			arrayList.Add(new LogicDataTableResource("logic/leagues2.csv", DataType.LEAGUE_VILLAGE2, 0));

			return arrayList;
		}

		public static void Load(LogicArrayList<LogicDataTableResource> resources, int idx, CSVNode node)
		{
			LogicDataTableResource resource = resources[idx];

			switch (resource.GetTableType())
			{
				case 0:
					LogicDataTables.InitDataTable(node, resource.GetTableIndex());
					break;
				case 3:
					// LogicStringTable.
					break;
				default:
					Debugger.Error("LogicResources::Invalid resource type");
					break;
			}

			if (resources.Size() - 1 == idx)
			{
				LogicDataTables.CreateReferences();
			}
		}
	}
}