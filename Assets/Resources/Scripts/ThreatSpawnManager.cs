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
			new LaserShipParameters(0, Colors.Blue, 4, 135, 2),

			new LaserShipParameters(12, Colors.Green, 3, 20, 2, 2),
			new LaserShipParameters(13, Colors.Green, 4f, -80, 4, 3),
			new LaserShipParameters(23, Colors.Green, 2.8f, 170, 3, 1.5f),

			new LaserShipParameters(38, Colors.Red, 3, 0, 1, 7),
			new LaserShipParameters(40, Colors.Green, 4f, 90, 1, 5),
			new LaserShipParameters(42, Colors.Blue, 3, 180, 1, 3),
			new LaserShipParameters(44, Colors.Yellow, 4f, -90, 1, 1f),

			new LaserShipParameters(54, Colors.Red, 4.5f, 90, 2, 9f),
			new LaserShipParameters(60, Colors.Red, 3f, 180, 2, 5.5f),
			new LaserShipParameters(64, Colors.Red, 2.5f, -90, 2, 3f),
			new LaserShipParameters(68, Colors.Red, 2f, 0, 2, 0.5f),

			new LaserShipParameters(79, Colors.Green, 4, 45, 2, 2),
			new LaserShipParameters(80, Colors.Yellow, 4, 135, 2, 2),

			new LevelEnd(95, 2000)
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
