from stable_baselines3.droQ.droQ import DroQ
from stick_env import CustomStickEnv

# Créez l'environnement Gym
env = CustomStickEnv(target_performance=85)

# Load the trained agent
"""model = DroQ.load("F:/Documents/Master S2/Projets Unity/EVHI TP/evhi-reaction-ring/EVHI-unity/Assets/Scripts/model/droq_0")
model.set_env(env)

# Set the parameters of the model
model.gradient_steps = 250
model.learning_starts = 0"""

model = DroQ("MlpPolicy", env, verbose=1, learning_rate=0.03, gradient_steps=100, dropout_rate=0.01)
c
# Train the agent and save it each episode
for i in range(100):
    model.learn(total_timesteps=150, log_interval=10)
    #model.save(f"F:/Documents/Master S2/EVHI/model/droq_0")