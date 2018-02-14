using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IThreatParams  {
	float SpawnTime { get; set; }
}

[Serializable]
public class EMPParameters : IThreatParams {
	public float SpawnTime { get; set; }
	public Colors GameColor { get; set; }
	public int DetonationTime { get; set ;}
	public float x,y;

	public EMPParameters(float spawnTime, Colors gameColor, Vector3 position, int detonationTime = 3) {
		SpawnTime = spawnTime;
		GameColor = gameColor;
		DetonationTime = detonationTime;

		x = position.x;
		y = position.y;
	}
}

[Serializable]
public class LaserShipParameters: IThreatParams {
	public float SpawnTime { get; set; }
	public Colors GameColor { get; set; }
	public float EndDistFromShip { get; set; }
	public float StartRotation = 0;
	public int NumberOfShots = 1;
	public float TimeBtwShots = 1f;
	public float RotationBetweenShots = 0;

	public LaserShipParameters(float spawntime, Colors c, float enddist, float startRotation = 0, int numShots = 1, float timebtwshots = 1, float rotationbtwshots = 0) {
		SpawnTime = spawntime;
		GameColor = c;
		EndDistFromShip = enddist;
		StartRotation = startRotation;
		NumberOfShots = numShots;
		TimeBtwShots = timebtwshots;
		RotationBetweenShots = rotationbtwshots;
	}
}

[Serializable]
public class MeteorParameters: IThreatParams {
	public float SpawnTime { get; set; }
	public Colors GameColor { get; set; }
	public float Angle { get; set; }
	public float Velocity { get; set; }
	public float ParticleSize { get; set; }
	public int Damage { get; set; }
	public int Points { get; set; }
	public float x,y;

	public MeteorParameters( float spawntime, Colors c, Vector3 position, float angle = 0f, float vel = 5f, float particleSize = 0.5f, int damage = 1, int points = 2000) {
		SpawnTime = spawntime;
		GameColor = c;
		Angle = angle;
		Velocity = vel;
		ParticleSize = particleSize;
		Damage = damage;
		Points = points;

		x = position.x;
		y = position.y;
	}
}

[Serializable]
public class OrbitingEnemyParameters: IThreatParams {
	public float SpawnTime { get; set; }
	public Colors GameColor { get; set; }
	public float Angle { get; set; }
	public float MoveSpeed { get; set; }
	public float x, y;

	public OrbitingEnemyParameters(float spawntime, Colors c, Vector3 position, float moveSpeed = 2f, float angle = 0) {
		SpawnTime = spawntime;
		GameColor = c;
		Angle = angle;
		MoveSpeed = moveSpeed;

		x = position.x;
		y = position.y;
	}
}

[Serializable]
public class PointBeamParameters: IThreatParams {
	public float SpawnTime { get; set; }
	public Colors GameColor { get; set; }
	public float Duration { get; set; }
	public float x, y;

	public PointBeamParameters(float spawntime, Colors c, float duration, Vector3 position) {
		SpawnTime = spawntime;
		GameColor = c;
		Duration = duration;

		x = position.x;
		y = position.y;
	}
}

[Serializable]
public class LevelEnd : IThreatParams {
	public float SpawnTime { get; set; }

	public LevelEnd(float spawnTime) {
		SpawnTime = spawnTime;
	}
}


public static class EMPFactory {
	public static EMP prefab;
	public static EMP Create(EMPParameters p) {
		prefab = prefab ?? Resources.Load<EMP>("Prefabs/EMP Bomb");
		EMP e = GameObject.Instantiate<EMP>(prefab);
		e.Init(p);
		return e;
	}
}

public static class LaserShipFactory {
	public static LaserShip prefab;
	public static LaserShip Create(LaserShipParameters p ) {
		prefab = prefab ?? Resources.Load<LaserShip>("Prefabs/LaserShip");
		LaserShip e = GameObject.Instantiate<LaserShip>(prefab);
		e.Init(p);
		return e;
	}
}

public static class OrbitingEnemyFactory {
	public static OrbitingEnemy prefab;
	public static OrbitingEnemy Create(OrbitingEnemyParameters p) {
		prefab = prefab ?? Resources.Load<OrbitingEnemy>("Prefabs/Orbiting Enemy");
		OrbitingEnemy e = GameObject.Instantiate<OrbitingEnemy>(prefab);
		e.Init(p);
		return e;
	}
}

public static class MeteorFactory {
	public static Meteor prefab;
	public static Meteor Create(MeteorParameters p) {
		prefab = prefab ?? Resources.Load<Meteor>("Prefabs/Meteor");
		Meteor e = GameObject.Instantiate<Meteor>(prefab);
		e.Init(p);
		return e;
	}
}

public static class PointBeamFactory {
	public static PointBeam prefab;
	public static PointBeam Create(PointBeamParameters p) {
		prefab = prefab ?? Resources.Load<PointBeam>("Prefabs/PointBeam");
		PointBeam e = GameObject.Instantiate<PointBeam>(prefab);
		e.Init(p);
		return e;
	}
}
