using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class GenAlg{

	//The neural network is encoded by reading all the weights from left to right
	//and from the first hidden layer upwards and storing them in a vector.
	//genome is list of floats where each float represents a weight


	//this holds the entire population of chromosomes
	public List<Chromosome> population;
	
	//size of population
	public int population_size;
	public float mutation_rate;//rate of mutation (between 0 and 1)
	float crossover_rate;//rate of crossover (between 0 and 1)
	float mutation_amount = 1;//how much each mutation affects the weights
	int chromosome_size;//size of each chromosome

	public int current_generation;


	public GenAlg(int popsize, float mr, float cr, int cs){
		population_size = popsize;
		mutation_rate = mr;
		crossover_rate = cr;
		current_generation = 0;
		chromosome_size = cs;

		//initialise population with chromosomes consisting of random
		//weights and all fitnesses set to zero
		population = new List<Chromosome>();
		for (int i=0; i<population_size; i++){
			population.Add(new Chromosome());
			for (int j=0; j<chromosome_size; j++){
				population[i].weights.Add(Random.Range(-1f,1f));
			}
		}
	}

	void Mutate(Chromosome chromo){//mutates a chromosome
		for (int i=0; i<chromo.weights.Count; i++){
			if (Random.Range(0f,1f) < mutation_rate){
				chromo.weights[i] += (Random.Range(-1f,1f) * mutation_amount);
			}
		}
	}

	void Crossover(Chromosome mom, Chromosome dad, ref Chromosome child1, ref Chromosome child2){
		//just return parents as offspring dependent on the rate
		//or if parents are the same
		if((Random.Range(0f,1f) > crossover_rate) || (mom == dad)){ 
			child1 = new Chromosome(mom.weights);;
			child2 = new Chromosome(dad.weights);
			return;
		}

		List<float> c1 = new List<float>();
		List<float> c2 = new List<float>();
		
		//determine a crossover point
		int crosspoint = Random.Range(0, dad.weights.Count - 1);
		
		//create the offspring
		for (int i=0; i<crosspoint; i++){
			c1.Add(mom.weights[i]);
			c2.Add(dad.weights[i]);
		}
		
		for (int i=crosspoint; i<mom.weights.Count; i++){
			c1.Add(dad.weights[i]);
			c2.Add(mom.weights[i]);
		}

		child1 = new Chromosome(c1);
		child2 = new Chromosome(c2);

		return;
	}

	public void Repopulate(){
		//first, log old population
		LogToFile();


		//create a new population based on the old one

		List<Chromosome> tmp = new List<Chromosome>(population);//temporary hold for edge case where everybody is dead

		//sort the population (for scaling and elitism)
		population = population.Where(t => t.fitness != 0).ToList();//ignore all things with 0 fitness


		if(population.Count == 0){//if everybody died, take the dead into account for selection
			population = new List<Chromosome>(tmp);
		}

		population = population.OrderBy(t => t.fitness).ToList();
		population.Reverse();//highest fitness first

		//Adjust mutation rate based on homogeny of population
		int h = 0;
		float c = 0;
		for (int i=0; i < population.Count-1; i++){
			Chromosome cur = population[i];
			for (int j=i+1; j < population.Count; j++){
				Chromosome other = population[j];
				if(cur != other){
					for(int k=0; k<other.weights.Count; k++){
						c += 1;
						if(cur.weights[k] == other.weights[k]) h += 1;
					}
				}
			}
		}
		float homogeny = c > 0 ? (h/c) : 1;
		mutation_rate = Mathf.Log(1 + homogeny) * .05f + .005f;
									
									
		List<Chromosome> newpop = new List<Chromosome>();


		//add elitism --> make 2 clones of most fit individual; mutate one
		population.Add(new Chromosome(population[0].weights));
		Chromosome tchr = new Chromosome(population[0].weights);
		Mutate(tchr);//allow elites to be mutated
		population.Add(tchr);


				
		//repeat until a new population is generated
		float tf = population.Sum(t => t.fitness);
		Debug.Log ("total fitness: " + tf);
		while (newpop.Count < population_size){
			//grab two chromosones
			Chromosome mom = RWS(tf);
			Chromosome dad = RWS(tf);
			
			//create offspring via crossover
			Chromosome child1 = null;
			Chromosome child2 = null;
			Crossover(mom, dad, ref child1, ref child2);
			
			//call mutation function on each child chromosome
			Mutate(child1);
			Mutate(child2);

			newpop.Add(new Chromosome(child1.weights));
			newpop.Add(new Chromosome(child2.weights));
		}

		population = new List<Chromosome>(newpop);		
		current_generation += 1;

	}

	Chromosome RWS(float totalFitness){//returns chromosome picked via roulette wheel sampling
		float Slice = (Random.Range(0f,1f) * totalFitness);
		
		//this will be set to the chosen chromosome
		Chromosome output = null;
		
		//go through the chromosones adding up the fitness so far
		float FitnessSoFar = 0;
		
		for (int i=0; i<population.Count; i++){
			FitnessSoFar += population[i].fitness;
			if (FitnessSoFar >= Slice){
				output = population[i];
				break;
			}
			
		}
		
		return output;
	}

	public void LogToFile(){


		string file1 = Application.dataPath + "/GA_log_all.txt";
		string file2 = Application.dataPath + "/GA_log.txt";
		StreamWriter sw = new StreamWriter(file1, true);//keep track of all historical populations
		StreamWriter sw2 = new StreamWriter(file2);//keep up to date with most recent population

		//write the current population to the specified file
		string output = "Generation " + current_generation + "\n";
		foreach(Chromosome chr in population){
			output += chr.fitness + " : " + chr.serialized() + "\n";
		}

		sw.WriteLine(output);
		sw.Close();
		sw2.WriteLine(output);
		sw2.Close();
		//System.IO.File.WriteAllText(filename, output);
	}




}

public class Chromosome {
	public List<float> weights;
	public float fitness;
	
	public Chromosome(){
		weights = new List<float>();
		fitness = 0;
	}

	public Chromosome(List<float> w){
		weights = new List<float>(w);
		fitness = 0;
	}
	
	public Chromosome(List<float> w, float f){
		weights = new List<float>(w);
		fitness = f;
	}

	public string serialized(){//serialize the chromosome for writing to file
		string ret = "";
		foreach(float weight in weights){
			ret += weight.ToString() + " ";
		}
		return ret;
	}
}
