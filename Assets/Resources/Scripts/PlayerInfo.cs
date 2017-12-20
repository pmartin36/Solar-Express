using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class PlayerInfo {
	public bool Upgraded { get; set; }
	public bool SoundOn { get; set; }
	public List<int> LevelStars { get; set; }

	public PlayerInfo() {
		Upgraded = false;
		SoundOn = true;
		LevelStars = new List<int>();
	}
}

