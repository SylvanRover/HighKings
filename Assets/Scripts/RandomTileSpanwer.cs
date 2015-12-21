using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomTileSpanwer : MonoBehaviour {	
	
	/// Poisson-disc sampling using Bridson's algorithm.
	/// Adapted from Mike Bostock's Javascript source: http://bl.ocks.org/mbostock/19168c663618b7f07158
	///
	/// See here for more information about this algorithm:
	///   http://devmag.org.za/2009/05/03/poisson-disk-sampling/
	///   http://bl.ocks.org/mbostock/dbb02448b0f93e4c82c3
	///
	/// Usage:
	///   PoissonDiscSampler sampler = new PoissonDiscSampler(10, 5, 0.3f);
	///   foreach (Vector2 sample in sampler.Samples()) {
	///       // ... do something, like instantiate an object at (sample.x, sample.y) for example:
	///       Instantiate(someObject, new Vector3(sample.x, 0, sample.y), Quaternion.identity);
	///   }
	///
	/// Author: Gregory Schlomoff (gregory.schlomoff@gmail.com)
	/// Released in the public domain
	public class PoissonDiscSampler
	{
		private const int k = 30;  // Maximum number of attempts before marking a sample as inactive.
		
		private readonly Rect rect;
		private  float _radius2;  // radius squared
		private readonly float cellSize;

		private float radius2 {
			get {
				return _radius2 * Random.Range(0.7f, 2.5f);
			}
			set {
				_radius2 = value;
			}
		}

		private Vector2[,] grid;
		private  List<Vector2> activeSamples = new List<Vector2>();
		
		/// Create a sampler with the following parameters:
		///
		/// width:  each sample's x coordinate will be between [0, width]
		/// height: each sample's y coordinate will be between [0, height]
		/// radius: each sample will be at least `radius` units away from any other sample, and at most 2 * `radius`.
		public PoissonDiscSampler(float width, float height, float radius, float cell = 0f)
		{
			rect = new Rect(0, 0, width, height);
			radius2 = radius * radius;
			cellSize = radius / Mathf.Sqrt(2);
			if (cell != 0f) {
				cellSize = cell;
			}
			grid = new Vector2[Mathf.CeilToInt(width / cellSize),
			                   Mathf.CeilToInt(height / cellSize)];
		}
		
		/// Return a lazy sequence of samples. You typically want to call this in a foreach loop, like so:
		///   foreach (Vector2 sample in sampler.Samples()) { ... }
		public IEnumerable<Vector2> Samples()
		{
			// First sample is choosen randomly
			//yield return AddSample(new Vector2(Random.value * rect.width, Random.value * rect.height));

			yield return AddSample(new Vector2(0.5f * rect.width, 0.5f * rect.height));
			
			while (activeSamples.Count > 0) {
				
				// Pick a random active sample
				int i = (int) Random.value * activeSamples.Count;
				Vector2 sample = activeSamples[i];
				
				// Try `k` random candidates between [radius, 2 * radius] from that sample.
				bool found = false;
				for (int j = 0; j < k; ++j) {
					
					float angle = 2 * Mathf.PI * Random.value;
					float r = Mathf.Sqrt(Random.value * 3 * radius2 + radius2); // See: http://stackoverflow.com/questions/9048095/create-random-number-within-an-annulus/9048443#9048443
					Vector2 candidate = sample + r * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
					
					// Accept candidates if it's inside the rect and farther than 2 * radius to any existing sample.
					if (rect.Contains(candidate) && IsFarEnough(candidate)) {
						found = true;
						yield return AddSample(candidate);
						break;
					}
				}
				
				// If we couldn't find a valid candidate after k attempts, remove this sample from the active samples queue
				if (!found) {
					activeSamples[i] = activeSamples[activeSamples.Count - 1];
					activeSamples.RemoveAt(activeSamples.Count - 1);
				}
			}
		}
		
		private bool IsFarEnough(Vector2 sample)
		{
			GridPos pos = new GridPos(sample, cellSize);
			
			int xmin = Mathf.Max(pos.x - 2, 0);
			int ymin = Mathf.Max(pos.y - 2, 0);
			int xmax = Mathf.Min(pos.x + 2, grid.GetLength(0) - 1);
			int ymax = Mathf.Min(pos.y + 2, grid.GetLength(1) - 1);
			
			for (int y = ymin; y <= ymax; y++) {
				for (int x = xmin; x <= xmax; x++) {
					Vector2 s = grid[x, y];
					if (s != Vector2.zero) {
						Vector2 d = s - sample;
						if (d.x * d.x + d.y * d.y < radius2) return false;
					}
				}
			}
			
			return true;
			
			// Note: we use the zero vector to denote an unfilled cell in the grid. This means that if we were
			// to randomly pick (0, 0) as a sample, it would be ignored for the purposes of proximity-testing
			// and we might end up with another sample too close from (0, 0). This is a very minor issue.
		}
		
		/// Adds the sample to the active samples queue and the grid before returning it
		private Vector2 AddSample(Vector2 sample)
		{
			activeSamples.Add(sample);
			GridPos pos = new GridPos(sample, cellSize);
			grid[pos.x, pos.y] = sample;
			return sample;
		}
		
		/// Helper struct to calculate the x and y indices of a sample in the grid
		private struct GridPos
		{
			public int x;
			public int y;
			
			public GridPos(Vector2 sample, float cellSize)
			{
				x = (int)(sample.x / cellSize);
				y = (int)(sample.y / cellSize);
			}
		}
	}

	public static void Shuffle<T>(List<T> list)  
	{  
		System.Random rng = new System.Random();  
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = rng.Next(n + 1);  
			T value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}  
	}

	public int questCount;
	public int settlementCount;
	public int crtCount;

	public GameObject prefabQuest;
	public GameObject prefabSettlement;
	public GameObject prefabCRT;

	public enum TileTypes {
		crt,
		settlement,
		quest
	};

	List<TileTypes> tileTypes = new List<TileTypes>();

	public float realmSize = 100f;
	public float minDistance = 3f;

	// Use this for initialization
	void Start () {

		for (int i = 0; i < questCount; ++i) {
			tileTypes.Add(TileTypes.quest);
		}
		for (int i = 0; i < settlementCount; ++i) {
			tileTypes.Add(TileTypes.settlement);
		}
		for (int i = 0; i < crtCount; ++i) {
			tileTypes.Add(TileTypes.crt);
		}

		Shuffle (tileTypes);

		float halfSize = realmSize * 0.5f;

		PoissonDiscSampler sampler = new PoissonDiscSampler(realmSize, realmSize, minDistance, 1f);
	   foreach (Vector2 sample in sampler.Samples()) {
	       // ... do something, like instantiate an object at (sample.x, sample.y) for example:
	       //Instantiate(someObject, new Vector3(sample.x, 0, sample.y), Quaternion.identity);

			if (tileTypes.Count > 0) {
				switch (tileTypes[tileTypes.Count - 1]) {
					case TileTypes.crt:
					Instantiate(prefabCRT, new Vector3(sample.x - halfSize, 0, sample.y - halfSize), Quaternion.identity);
					break;
				case TileTypes.quest:
					Instantiate(prefabQuest, new Vector3(sample.x - halfSize, 0, sample.y - halfSize), Quaternion.identity);
					break;
				case TileTypes.settlement:
					Instantiate(prefabSettlement, new Vector3(sample.x - halfSize, 0, sample.y - halfSize), Quaternion.identity);
					break;
				}

				tileTypes.RemoveAt(tileTypes.Count - 1);
			} else {
				break;
			}
		}
	}

}
