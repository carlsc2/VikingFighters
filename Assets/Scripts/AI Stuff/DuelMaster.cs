using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DuelMaster : MonoBehaviour {
	//main controller for simulation

	//controls stepping of genetic algorithm

	public GameObject vikingfab1;
	public GameObject vikingfab2;
	GameObject viking1;
	GameObject viking2;

	GenAlg gena1;
	GenAlg gena2;


	public Text genslot;
	public Text fslot;
	public Text mslot;
	public Text mslot2;

	int population_size = 20;
	float mutation_rate = .005f;
	float crossover_rate = .95f;

	private Animator anim1;
	private Animator anim2;

	private Chromosome chromo1;
	private Chromosome chromo2;

	private int curindex;
	private int total_weights;

	private float start_time;


	void Begin_Duel(){
		//instantiate new copies of the vikings for dueling
		if(viking1 != null){
			Destroy(viking1.transform.root.gameObject);
		}
		if(viking2 != null){
			Destroy(viking2.transform.root.gameObject);
		}

		viking1 = Instantiate(vikingfab1) as GameObject;
		viking2 = Instantiate(vikingfab2) as GameObject;

		NeuralNet n1 = viking1.GetComponentInChildren<NeuralNet>();
		anim1 = n1.GetComponent<Animator>();//get reference to animator 1
		viking1 = n1.gameObject;

		NeuralNet n2 = viking2.GetComponentInChildren<NeuralNet>();
		anim2 = n2.GetComponent<Animator>();//get reference to animator 2
		viking2 = n2.gameObject;

		n1.opponent = viking2;
		n2.opponent = viking1;

		//select 2 chromosomes from population for dueling
		chromo1 = gena1.population[curindex];
		chromo2 = gena2.population[curindex];
		//chromo2 = gena.population[gena.population_size - curindex - 1];
		curindex += 1;
		n1.PutWeights(chromo1.weights);//put chromosome 1 into viking 1
		n2.PutWeights(chromo2.weights);//put chromosome 2 into viking 2

		start_time = Time.time;//get start time of fight
	}

	public void reset(){//link to button --> start from generation zero
		gena1 = new GenAlg(population_size,mutation_rate, crossover_rate, total_weights);
		gena2 = new GenAlg(population_size,mutation_rate, crossover_rate, total_weights);
		curindex = 0;
		Begin_Duel();
	}

	void Start(){
		viking1 = Instantiate(vikingfab1) as GameObject;
		total_weights = viking1.GetComponentInChildren<NeuralNet>().GetNumberOfWeights();
		reset();

	}

	public void set_timeScale(float input){
		Time.timeScale = input * 10f;
	}

	void judge_fight(){
		//after the fight, evaluate the fitness of each chromosome

		int hp1 = anim1.GetInteger("health");
		int hp2 = anim2.GetInteger("health");
		hp1 = hp1 > 0 ? hp1 : 0;
		hp2 = hp2 > 0 ? hp2 : 0;

		chromo1.fitness = hp1;// / (Time.time - start_time); //* anim1.GetInteger("movecount");
		chromo2.fitness = hp2;// / (Time.time - start_time); //* anim2.GetInteger("movecount");

		if(anim1.enabled == false){//if viking 1 is dead
			chromo1.fitness = 0;
		}
		if(anim2.enabled == false){//if viking 2 is dead
			chromo2.fitness = 0;
		}

		//reward higher for not getting hit
		if(hp1 == 10){
			chromo1.fitness *= anim1.GetInteger("hitcount");
		}
		if(hp2 == 10){
			chromo2.fitness *= anim2.GetInteger("hitcount");
		}

		if(curindex < population_size){
			Begin_Duel();
		}else{
			gena1.Repopulate();
			gena2.Repopulate();
			curindex = 0;
			Begin_Duel();
		}

	}
		
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)){
			Application.Quit();
			return;
		}



		genslot.text = "Generation: " + (gena1.current_generation + 1);
		fslot.text = "Fight: " + curindex + " of " + population_size;
		mslot.text = "Mutation Rate: " + (gena1.mutation_rate*100).ToString("0.000") + "%";
		mslot2.text = "Mutation Rate: " + (gena2.mutation_rate*100).ToString("0.000") + "%";

		if(anim1.enabled == false || anim2.enabled == false){//assume viking 1 is dead
			judge_fight();
		}

		if(Time.time - start_time > 10){//smite those who don't move
			if(anim1.GetInteger("movecount") == 0){
				anim1.SetInteger("health",0);
			}
			if(anim2.GetInteger("movecount") == 0){
				anim2.SetInteger("health",0);
			}
		}

		if(Time.time - start_time > 20){
			judge_fight();
		}




	}
}
