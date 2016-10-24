using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Neuron{
	public int num_inputs;
	public List<float> weights;

	public Neuron(int n_inputs){
		weights = new List<float>();
		num_inputs = n_inputs + 1;
		for(int i=0; i<num_inputs; i++){//construct with random weights for each input
			weights.Add(Random.Range(-1f,1f));
		}
	}
}

public class NeuronLayer{
	//the number of neurons in this layer
	public int num_neurons;
	
	//the layer of neurons
	public List<Neuron> neurons;
	
	public NeuronLayer(int n_neurons, int num_inputs_per_neuron){
		num_neurons = n_neurons;
		neurons = new List<Neuron>();
		for (int i=0; i<num_neurons; ++i){
			neurons.Add(new Neuron(num_inputs_per_neuron));
		}
	}
}

