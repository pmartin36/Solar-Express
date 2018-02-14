using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManagerThreatSpawn : LevelManager {

	private float LevelLength = 125f;
	private float CurrentLevelTime = 0f;
	private ThreatSpawnManager threatManager;

	protected override void Update() {
		base.Update();
		if (!PlayerDead && LevelStarted) {
			CurrentLevelTime += Time.deltaTime;
			ProgressBar.UpdateProgressBar(CurrentLevelTime / LevelLength);
		}
	}

	public override void StartLevelSpawn() {
		base.StartLevelSpawn();
		threatManager = gameObject.AddComponent<ThreatSpawnManager>();
		threatManager.Init(LevelNumber);

		LevelLength = threatManager.threats.Last().SpawnTime;
	}

	public override void PlayerDied() {
		PlayerDead = true;
		Destroy(threatManager);
		StartCoroutine(base.PlayerDeathRoutine());
	}

	public override void BeginEndLevel() {
		PointCutoffs = new List<int>() {
			RoundToNearest500(TotalAvailablePoints*0.25f),
			RoundToNearest500(TotalAvailablePoints*0.50f),
			RoundToNearest500(TotalAvailablePoints*0.9f)
		};
		base.BeginEndLevel();
	}

	public int RoundToNearest500(float num) {
		return (int)(Mathf.Round(num/500f) * 500f);
	}
}
