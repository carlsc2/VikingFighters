using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NeuralNet : MonoBehaviour {
	int	num_inputs = 6;// # of inputs
	int	num_outputs = 5;// # of outputs
	int num_hidden_layers = 2;// # of hidden layers
	int neurons_per_hidden_layer = 5;// # of neurons per hidden layer
	List<NeuronLayer> layers;

	float activation_response = .5f;
	float bias = .8f;

	public GameObject opponent;

	// Use this for initialization
	void Awake () {
		layers = new List<NeuronLayer>();
		//create the layers of the network
		if (num_hidden_layers > 0){
			//create first hidden layer
			layers.Add(new NeuronLayer(neurons_per_hidden_layer, num_inputs));
			for (int i=0; i<num_hidden_layers-1; ++i){
				layers.Add(new NeuronLayer(neurons_per_hidden_layer,neurons_per_hidden_layer));
			}
			//create output layer
			layers.Add(new NeuronLayer(num_outputs, neurons_per_hidden_layer));
		}
		else{
			//create output layer
			layers.Add(new NeuronLayer(num_outputs, num_inputs));
		}
	}
	
	// Update is called once per frame
	void Update () {
		//6 inputs:
		//	opponent x position
		//	opponent animation frame #
		//	opponent current action
		//	self current action
		//	self animation frame #
		//  current x position

		/// possible output action:
		///		Stab();
		/// 	Slash();
		/// 	JumpForward();
		/// 	JumpBack();
		/// 	Parry();

		//generate input list
		List<float> inputs = new List<float>();
		inputs.Add(opponent.transform.position.x);//opponent x position
		inputs.Add(opponent.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime%1);//opponent animation time
		inputs.Add(opponent.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).tagHash);//opponent animation id
		inputs.Add(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).tagHash);//current animation id
		inputs.Add(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime%1);//current animation progress
		inputs.Add(transform.position.x);//current x position

		//foreach(float input in inputs){
			//print(input);
		//}

		List<float> outputs = new List<float>();
		
		int cWeight = 0;
		
		//first check that we have the correct amount of inputs
		if (inputs.Count != num_inputs){//just return an empty vector if incorrect.
			return;
		}
		
		//For each layer....
		for (int i=0; i<num_hidden_layers + 1; ++i){	
			if ( i > 0 ){
				inputs = new List<float>(outputs);//set input to output of previous layer
			}
			outputs.Clear();
			
			cWeight = 0;
			
			//for each neuron sum the (inputs * corresponding weights).Throw 
			//the total at our sigmoid function to get the output.
			for (int j=0; j<layers[i].num_neurons; ++j){
				float netinput = 0;
				
				int	NumInputs = layers[i].neurons[j].num_inputs;
				
				//for each weight
				for (int k=0; k<NumInputs - 1; ++k){
					//sum the weights x inputs
					netinput += layers[i].neurons[j].weights[k] * inputs[cWeight++];
				}			

				//add in the bias
				netinput += layers[i].neurons[j].weights[NumInputs-1] * bias;
				
				//we can store the outputs from each layer as we generate them. 
				//The combined activation is first filtered through the sigmoid 
				//function
				outputs.Add(Sigmoid(netinput,activation_response));
				cWeight = 0;

			}
		}
		PlayerControl pc = GetComponent<PlayerControl>();
		if(outputs[0] > .5f){
			pc.JumpBack();
		}
		if(outputs[1] > .5f){
			pc.JumpForward();
		}
		if(outputs[2] > .5f){
			pc.Stab();
		}
		if(outputs[3] > .5f){
			pc.Slash();
		}
		if(outputs[4] > .5f){
			pc.Parry();
		}
	}

	float Sigmoid(float netinput, float response){
		return ( 1 / ( 1 + Mathf.Exp(-netinput / response)));
	}
	
	public List<float> GetWeights(){//return a list of all weights, in order
		//this will hold the weights
		List<float> weights = new List<float>();
		
		//for each layer
		for (int i=0; i<num_hidden_layers + 1; ++i){
			//for each neuron
			for (int j=0; j<layers[i].num_neurons; ++j){
				//for each weight
				for (int k=0; k<layers[i].neurons[j].num_inputs; ++k){
					weights.Add(layers[i].neurons[j].weights[k]);
				}
			}
		}
		return weights;
	}


	public void PutWeights(List<float> weights){//replaces all weights with supplied values
		int cWeight = 0;
		//for each layer
		for (int i=0; i<num_hidden_layers + 1; ++i){
			//for each neuron
			for (int j=0; j<layers[i].num_neurons; ++j){
				//for each weight
				for (int k=0; k<layers[i].neurons[j].num_inputs; ++k){
					layers[i].neurons[j].weights[k] = weights[cWeight++];
				}
			}
		}
	}

	public int GetNumberOfWeights(){//get the number of weights required to fill the neural net (called by GA)
		int weights = 0;
		//for each layer
		for (int i=0; i<num_hidden_layers + 1; ++i){
			//for each neuron
			for (int j=0; j<layers[i].num_neurons; ++j){
				//for each weight
				for (int k=0; k<layers[i].neurons[j].num_inputs; ++k)
					weights++;
			}
		}
		return weights;
	}


}
