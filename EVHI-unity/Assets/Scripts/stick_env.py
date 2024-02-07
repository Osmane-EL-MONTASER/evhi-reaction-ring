import gymnasium as gym
from gymnasium import spaces
import numpy as np

# Import the socket module
import socket

# Paramètres pour la simulation
target_performance = 85  # Performance cible
scale = 20  # Échelle pour la fonction de récompense

NB_STICKS = 10
NB_PARAMS = 2

# Vitesses possibles pour les bâtons
possibleSpeeds = [1, 2, 4, 6, 8, 12, 16, 24, 32, 48, 64]
# Longueurs possibles pour les bâtons
possibleLengths = [0.1, 0.2, 0.4, 0.6, 0.8, 1.2, 1.6]

class CustomStickEnv(gym.Env):
    metadata = {'render.modes': ['human']}

    def __init__(self, target_performance):
        super(CustomStickEnv, self).__init__()

        # Définition des plages d'action pour length, width, speed
        
        # Instead of defining the action space for one stick, we will define it for 10 sticks with speed and length.
        
        self.action_space = spaces.MultiDiscrete([len(possibleSpeeds)]* NB_STICKS + [len(possibleLengths)] * NB_STICKS)

        """self.action_space = spaces.Box(low=np.array([borne_min_speed] * 10), 
                                       high=np.array([borne_max_speed] * 10), 
                                       dtype=np.float32)"""

        # On suppose que l'observation est la performance actuelle du joueur et les paramètres length, speed
        self.observation_space = spaces.Box(low=np.array([0.0]*10), 
                                            high=np.array([100.0]*10), 
                                            dtype=np.float32)
        
        # We need to establish a connection with the game and send the action.
        # The game will return the performance.
        # We will communicate with the game through a socket connection on localhost port 5000.
        
        # Create a socket connection
        s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        s.connect(('localhost', 5000))
        self.s = s

        # Définissez la performance cible et l'échelle de la fonction de récompense
        self.target_performance = target_performance
        self.scale = 20  # Ajustez cette valeur pour rendre la fonction de récompense plus ou moins pénalisante

    def step(self, action):
        # Appliquer l'action au jeu et obtenir la nouvelle performance
        #performance = simulate_performance(action[0], action[1], action[2]) for debugging purpose
        
        # Send the action to the game as a string
        #print(f'Sending Action: {action}')
        
        # Convert the action to the corresponding speed and length
        speed = [possibleSpeeds[i] for i in action[:NB_STICKS]]
        length = [possibleLengths[i] for i in action[NB_STICKS:]]
        
        action = speed + length
        print(f'Sending Action: {action}')
        
        self.s.sendall(str(action).encode())
        
        # Receive the performance from the game
        print('Receiving performance...')
        performances = self.s.recv(1024)
        
        # Convert the performance to a list of floats
        performances = np.fromstring(performances[1:-1], sep=' ')
        print(f'Received Performances: {performances}')
        
        # If there is a performance between 0.7 and 1.0 in performances , we will set the reward to 50.0 else 0.0
        reward = 0.0
        
        for perf in performances:
            if (70.0 <= perf <= 100.0):
                reward += 1.0
        
        # Calculate the mean performance
        performance = np.mean(performances) / 100
        
        # Calculer la récompense
        reward += self.reward_function_1(performance)

        # Vérifier si l'épisode est terminé
        done = True

        # On suppose que l'observation suivante est la nouvelle performance
        info = {}  # Informations supplémentaires optionnelles, par ex. pour le débogage
        # Returns the new state, the reward, whether the episode is done, and additional info
        
        print(f'Action: {action}, Performance: {performance}, Reward: {reward}')
        
        # Merge the performances and the actions
        observations = np.concatenate((performances, action))
        print(f'Observations: {observations.shape}')
        
        return observations, reward, done, False, info

    def reset(self, seed=None, options=None):
        # Réinitialiser l'état de l'environnement pour un nouvel épisode
        initial_performance = 0.85 # On suppose que le joueur commence avec une performance de 85%
        return np.array([initial_performance] * 10).astype(np.float32), None

    def render(self, mode='console'):
        if mode != 'console':
            raise NotImplementedError()
        # Afficher l'état de l'environnement (facultatif pour le débogage)
        print(f'Performance: {self.state}')

    def exp_reward(self, performance, target, scale):
        return np.exp(-np.abs(performance - target)**2 / scale)
    
    def reward_function_2(self, performance, target=0.85, max_reward=10):
        return max_reward / (1 + 50 * abs(performance - target))

    def reward_function_1(self, performance, target=0.85, max_reward=10):
        """ Reward increases exponentially as performance approaches the target, decreases otherwise. """
        return max_reward * np.exp(-10 * (performance - target)**2)

    def close(self):
        self.s.close()
        pass