using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThreatSpawnManager : MonoBehaviour {

	public List<IThreatParams> threats;
	int threatIndex = 0;
	float time;

	public void Init(int levelNumber) {
		time = 0;
		if (GameManager.Instance.ThreatParams.ContainsKey(levelNumber)) {
			threats = GameManager.Instance.ThreatParams[levelNumber];
			return;
		}

		//comment after level generation
		GenerateLevel(levelNumber);	
	}

	public void GenerateLevel(int levelNumber) {
		/*
		threats = new List<IThreatParams>() {
			new MeteorParameters(0, Colors.Green, new Vector3(5,5,0), 45),
			new LaserShipParameters(0, Colors.Blue, 2, 135),
			new PointBeamParameters(10, Colors.Yellow, 10, Vector3.one * -3),
			new EMPParameters(10, Colors.Yellow, new Vector2(-1.5f,1f)),
			new OrbitingEnemyParameters(15, Colors.Red, new Vector3(5,-2)),
			new LevelEnd(20)
		};
		*/
		threats = new List<IThreatParams>() {
			new LaserShipParameters(0, Colors.Yellow, 2.5f, 90, 2, 2),
			new PointBeamParameters(3, Colors.Yellow, 7, Vector3.up * -3),

			new LaserShipParameters(10, Colors.Red, 3f, 180, 3, 3),
			new LaserShipParameters(10, Colors.Red, 3f, 0, 2, 5),
			new PointBeamParameters(15, Colors.Red, 15, Vector3.up * 3),

			new LaserShipParameters(32, Colors.Green, 4, 90, 3, 3),
			new PointBeamParameters(39, Colors.Blue, 4, Utils.AngleToVector(75)*3),
			new PointBeamParameters(42, Colors.Blue, 4, Utils.AngleToVector(165)*3),
			new PointBeamParameters(46, Colors.Blue, 4, Utils.AngleToVector(-105)*3),
			new PointBeamParameters(50, Colors.Blue, 4, Utils.AngleToVector(-15)*3),

			new LaserShipParameters(58, Colors.Blue, 3, 65, 3, 3),
			new LaserShipParameters(58, Colors.Red, 3, 25, 3, 4),
			new PointBeamParameters(62, Colors.Yellow, 12, Utils.AngleToVector(-135)*3),

			new LaserShipParameters(80, Colors.Blue, 4, 90, 3, 3),
			new PointBeamParameters(82, Colors.Yellow, 2, Utils.AngleToVector(135)*3),
			new PointBeamParameters(84, Colors.Red, 2, Utils.AngleToVector(180)*3),
			new PointBeamParameters(86, Colors.Green, 2, Utils.AngleToVector(-135)*3),
			new PointBeamParameters(88, Colors.Blue, 2, Utils.AngleToVector(-90)*3),
			new PointBeamParameters(90, Colors.Yellow, 2, Utils.AngleToVector(-45)*3),
			new PointBeamParameters(92, Colors.Red, 2, Utils.AngleToVector(0)*3),
			new PointBeamParameters(94, Colors.Green, 2, Utils.AngleToVector(45)*3),
			new PointBeamParameters(96, Colors.Blue, 2, Utils.AngleToVector(90)*2.5f),

			new LevelEnd(102, -7500)
		};

		//comment when going to create level
		threats = threats.Select( t => {
			if(t is MeteorParameters) {
				//(t as MeteorParameters).Damage = 0;
				(t as MeteorParameters).Velocity *= 0.6f;
			}
			return t;
		}).ToList();
		
		//only uncomment when YOU HAVE CHECKED THE LEVEL NUMBER IS NOT EXISTING YET
		//SaveThreats(levelNumber);
	}

	public void SaveThreats(int levelNumber) {
		var tp = GameManager.Instance.ThreatParams;
		if(tp.ContainsKey(levelNumber)) {
			tp[levelNumber] = threats;
		}
		else {
			tp.Add(levelNumber, threats);
		}
		Serializer<Dictionary<int, List<IThreatParams>>>.Serialize(tp,"Threats.bin");
	}

	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;

		while(threatIndex < threats.Count && threats[threatIndex].SpawnTime < time) {
			IThreatParams t = threats[threatIndex];

			if(t is EMPParameters) {
				EMPFactory.Create(t as EMPParameters);
			}
			else if(t is LaserShipParameters) {
				LaserShipFactory.Create(t as LaserShipParameters);
			}
			else if(t is MeteorParameters) {
				MeteorFactory.Create(t as MeteorParameters);
			}
			else if(t is OrbitingEnemyParameters) {
				OrbitingEnemyFactory.Create(t as OrbitingEnemyParameters);
			}
			else if(t is PointBeamParameters) {
				PointBeamFactory.Create(t as PointBeamParameters);
			}
			else if(t is LevelEnd) {
				var lm = (GameManager.Instance.ContextManager as LevelManager);
				lm.TotalAvailablePoints += (t as LevelEnd).ExtraPoints;
				(GameManager.Instance.ContextManager as LevelManager).BeginEndLevel();
			}

			threatIndex++;
		}
	}
}
