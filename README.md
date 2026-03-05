## Supercell.Magic - Project
## 📦 Requirements:
---
 - Basic knowledge of APK modification
 - [.NET Core SDK 2.2 and runtime ](https://dotnet.microsoft.com/download/dotnet-core/2.2)
 - [WAMP Server](https://wampserver.aviatechno.net)
 - [CouchBase Database Server](https://www.couchbase.com/downloads/?family=couchbase-server)
 - [Redis Server](https://github.com/redis/redis)
 - [APK signing tool](https://github.com/mkcs121/APK-Easy-Tool)
---

## 🖥️ Server Setup:
---
1️⃣Building the Project
 1. You can either build the project yourself or use the prebuilt release version. .NET Core 2.2 must be installed.
 2. Install the WAMP server and start the services.
 3. Paste the `www` folder from the server software over the `www` folder in the WAMP server.
 4. Install the Couchbase database community edition or higher and create a user.
  1. The default user is Administrator, password: 123456. If you create a different user in the environment.json file, change this in the configuration.
  2. Open Couchbase in your browser and create 4 buckets: `magic-players`, `magic-alliances`, `magic-streams`, `magic-seasons`. You can see them in the buckets directory in the environment.json file.

```json
{
	"environment": "dev",
	"servers": {
		"settings": {
			"contentValidationModeEnabled": false,
			"proxy": {
				"startMaintenanceTimeSecs": 0,
				"sessionCapacity": 50
			},
			"admin": {
				"presetLevelFiles": [
					"/level/townhall8.json",
					"/level/townhall9.json",
					"/level/townhall10.json",
					"/level/townhall11.json",
					"/level/test/highlevel1.json",
					"/level/test/highlevel2.json",
					"/level/test/highlevel3.json",
					"/level/test/highlevel4.json",
					"/level/test/highlevel5.json",
					"/level/test/highlevel6.json",
					"/level/test/highlevel7.json",
					"/level/test/highlevel8.json",
					"/level/test/highlevel9.json",
					"/level/test/playtest0.json",
					"/level/test/playtest1.json",
					"/level/test/playtest2.json",
					"/level/test/playtest3.json",
					"/level/test/playtest4.json",
					"/level/test/playtest5.json"
				]
			}
		},
		"admin": ["127.0.0.1:10001"],
		"proxy": ["127.0.0.1:10002"],
		"chat": ["127.0.0.1:10003"],
		"game": ["127.0.0.1:10004"],
		"home": ["127.0.0.1:10005"],
		"stream": ["127.0.0.1:10007"],
		"battle": ["127.0.0.1:10008"],
		"scoring": ["127.0.0.1:10009"],
		"search": ["127.0.0.1:10010"]
	},
	"database": {
		"higher_id_counter_size": 256,
		"couchbase": {
			"servers": [
				{
					"hosts": ["127.0.0.1"],
					"username": "Administrator",
					"password": "123456"
				}
			],
			"buckets": [
				"magic-players:0",
				"magic-alliances:0",
				"magic-streams:0",
				"magic-seasons:0"
			]
		},
		"redis": {
			"databases": [
				{"name": "magic-session", "connectionString": "127.0.0.1:6379"},
				{"name": "magic-admin", "connectionString": "127.0.0.1:6379"}
			]
		}
	}
}
```
---
![Bucket](/Setup/Couchbase%20Add%20Bucket.png)
	3. Let's move on to Views. Actually, this file (https://github.com/Enjoyop2/Supercell.Magic/blob/master/Supercell.Magic.Servers.Scoring/Logic/ScoringSeason.cs) tells you what you need to do, but let's write it down anyway.
	4. Create a player's leaderboard_0 file inside magic-players. Write the following inside it:

 ```javascript
function (doc, meta) {
  if (meta.id.startsWith("game-") && doc.name_set) {
    emit(doc.score, {
    	id_hi: doc.id_hi,
    	id_lo: doc.id_lo,
    	name: doc.name,
    	score: doc.score,
      
      xp_level: doc.xp_level,
      attackWin: doc.attack_win_cnt,
      attackLose: doc.attack_lose_cnt,
      defenseWin: doc.defense_win_cnt,
      defenseLose: doc.defense_lose_cnt,
      leagueType: doc.league_type,
      country: doc.country,
      allianceId_High: doc.alliance_id_high,
      allianceId_Low: doc.alliance_id_low,
      allianceName: doc.alliance_name,
      badgeId: doc.badge_id
    });
  }
}
 ```
 5. Create players leaderboard_1 inside magic-players
 ```javascript
function (doc, meta) {
  if (meta.id.startsWith("game-") && doc.name_set) {
    emit(doc.duel_score, {
    	id_hi: doc.id_hi,
    	id_lo: doc.id_lo,
    	name: doc.name,
    	score: doc.duel_score,
      
      xp_level: doc.xp_level,
      duelWin: doc.duel_win_cnt,
      duelDraw: doc.duel_draw_cnt,
      duelLose: doc.duel_lose_cnt,
      country: doc.country,
      allianceId_High: doc.alliance_id_high,
      allianceId_Low: doc.alliance_id_low,
      allianceName: doc.alliance_name,
      badgeId: doc.badge_id
    });
  }
}
 ```
Yes, save the views.
 6. Create alliances leaderboard 0 inside magic-alliances
 
 ```javascript
function (doc, meta) {
  if (meta.id.startsWith("data-") && doc.member_count > 0) {
    emit(doc.score, {
        id_hi: doc.id_hi,
        id_lo: doc.id_lo,
        name: doc.alliance_name,
        score: doc.score,

      badge_id: doc.badge_id,
      member_count: doc.member_count,
      xp_level: doc.xp_level,
      origin: doc.origin,
    });
  }
}
```

 7. Create alliances leaderboard_1 inside magic-alliances
 
 ```javascript
function (doc, meta) {
  if (meta.id.startsWith("data-") && doc.member_count > 0) {
    emit(doc.score, {
        id_hi: doc.id_hi,
        id_lo: doc.id_lo,
        name: doc.alliance_name,
        score: doc.duel_score,

      badge_id: doc.badge_id,
      member_count: doc.member_count,
      xp_level: doc.xp_level,
      origin: doc.origin,
    });
  }
}
```

![Views](Setup/Couchbase%20Add%20Views.png)

Yes, save the views. Publish the view.
Start the redist database.
Okay, now you can start the server using start.bat or start.sh.
---
## 📱 Client Connection:
[Client Download](https://www.mediafire.com/file/9ju0bhj0hfptvxk/client-v9.256.20.apk/file)
Change the IP address in libg.so (2 x86 and 32-bit versions) to 192.168.1.85.
This is compiled C code; do not edit it with a text editor, modify it with a hex editor.
Put it into the APK and sign it. Install the APK; that's all.

This code may have some shortcomings; I've tried to review it thoroughly. My goal with this code is to create a competitive local mod with 5-6 friends.
For higher versions and versions with more features, contact the original author. I probably won't touch this again.


***Supercell.Magic*** is a Clash of Clans Server.
It was written by myself.
The goal of this server was to implement all the features of Clash of Clans and support millions of players.
Supercell.Magic uses dedicated threads and async operators. 
**Couchbase servers** & a **memory based** saving server will be used to save players.

## About us
I probably won't update the git. I don't have time with the studies to work on it anymore.
If you have any questions, you can contact me on discord (@Mimi8297#8726).
You can use it to create your own private server if you wish. The current version has some logic bugs.
Here is a site that uses this version: https://atrasis.net.
I offer partnerships that allow you to have your own private server. Prices are $350/month. 

---
## 📜 Legal & Disclaimer
This repository is created in accordance with **Supercell's Fan Content Policy**.
🔗 https://supercell.com/en/fan-content-policy/

- This project is **not affiliated with, endorsed, sponsored, or approved by Supercell**.
- All game assets belong to their respective owners.
- This repository is intended for **non-commercial fan and educational use only**.
---

---
## ⭐ Support
If you find this project useful, consider giving it a star ⭐ to support development.
---
