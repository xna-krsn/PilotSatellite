using System.Collections.Generic;
using UnityEngine;

using DataManagement;
using Assets.Scripts.Planets;

public class SolarSystemPlanets : MonoBehaviour
{
    public GameObject[] Planets;

    //public GameObject Mercury;
    //public GameObject Venus;
    //public GameObject Earth;
    //public GameObject Mars;
    //public GameObject Jupiter;
    //public GameObject Saturn;
    //public GameObject Uranus;
    //public GameObject Neptune;
    //public GameObject Pluto;

    private const float SunRadius = 6.96e+08f;
    private const float ScaleFactor = 2 * SunRadius / 10;

    private readonly List<string> _planetNames = new List<string> { "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune", "Pluto" };
    private readonly List<Planet> _planetsInfo = new List<Planet>();    
    private readonly  SolarSystemDataManager _dataManager = new SolarSystemDataManager();
    
    private List<Vector3> _startPositions;
    private List<Vector3> _startVelocities;

    private void Awake()
    {
        Planets = new GameObject[_planetNames.Count];

        _startPositions = new List<Vector3>
        {
            new Vector3(3.98e+07f, 2.69e+07f, -1.43e+06f),
            new Vector3(1.08e+08f, -7.51e+06f, -6.36e+06f),
            new Vector3(1.16e+08f, 9.21e+07f, -1.56e+04f),
            new Vector3(-1.53e+08f, 1.93e+08f, 7.78e+06f),
            new Vector3(-1.33e+08f, 7.62e+08f, -2.48e+05f),
            new Vector3(-1.06e+09f, -1.23e+09f, 6.01e+07f),
            new Vector3(2.95e+09f, 5.53e+08f, -3.61e+07f),
            new Vector3(4.04e+09f, -1.95e+09f, -5.28e+07f),
            new Vector3(9.07e+08f, -4.78e+09f, 2.49e+08f),
        };

        _startVelocities = new List<Vector3>
        {
            new Vector3(-3.71e+01f, 4.23e+01f, 6.86e+00f),
            new Vector3(2.16e+00f, 3.48e+01f, 3.52e-01f),
            new Vector3(-1.90e+01f, 2.32e+01f, -9.39e-04f),
            new Vector3(-1.81e+01f, -1.30e+01f, 1.71e-01f),
            new Vector3(-1.30e+01f, -1.59e+00f, 2.98e-01f),
            new Vector3(6.17e+00f, -6.99e+00f, -1.24e-01f),
            new Vector3(-1.31e+00f, 6.38e+00f, 4.06e-02f),
            new Vector3(2.33e+00f, 4.92e+00f, -1.55e-01f),
            new Vector3(5.44e+00f, -8.04e-02f, -1.57e+00f)
        };
    }

	private void Start ()
	{
	    CreatePlanets();
	    foreach (var planet in _planetsInfo)
	    {
	        planet.Planet3D.transform.localScale = Vector3.one * 2 * planet.MeanRadius/ScaleFactor;
	        planet.Planet3D.transform.position = planet.CurrentPosition/ScaleFactor;

            Debug.Log(string.Format("Planet {0}, Scale = {1}, Position = {2}, ", planet.Name, planet.Planet3D.transform.localScale, planet.Planet3D.transform.position));
	    }
	}
	
	private void Update () {
	
	}

    private void CreatePlanets()
    {
        var solarSystemItems = _dataManager.GetCelestialBodyCharacteristics().Value;
        var characteristicOrders = _dataManager.GetCharacteristicOrders();
        int i = 0;
        foreach (var planetName in _planetNames)
        {
            Debug.Log(planetName);
            Debug.Log(solarSystemItems.Subobjects.Count);
            var planetInfo = solarSystemItems.Subobjects[planetName];
            var planet = new Planet(
                name: planetName,
                mass: new BigNumber
                {
                    Number = float.Parse(planetInfo.Mass),
                    Power = float.Parse(characteristicOrders.MassOrder)
                },
                radius: float.Parse(planetInfo.Radius),
                distance: new BigNumber
                {
                    Number = float.Parse(planetInfo.Distance),
                    Power = float.Parse(characteristicOrders.DistanceOrder)
                });
            planet.CurrentPosition = _startPositions[i];
            planet.CurrentVelocity = _startVelocities[i];
            planet.Planet3D = (GameObject) Instantiate(Planets[i++]);

            _planetsInfo.Add(planet);
        }
    }
}
