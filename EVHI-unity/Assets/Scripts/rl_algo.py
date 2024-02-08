#from stable_baselines3.droQ.droQ import DroQ

from stable_baselines3 import PPO

from stick_env import CustomStickEnv
from stable_baselines3.common.callbacks import CheckpointCallback

# Créez l'environnement Gym
env = CustomStickEnv(target_performance=85)

checkpoint_callback = CheckpointCallback(
  save_freq=1,
  save_path="./logs/",
  name_prefix="rl_model",
  save_replay_buffer=True,
  save_vecnormalize=True,
)

# Load the last model trained
#model = PPO.load("./logs/rl_model_5_steps", env=env)

model = PPO("MlpPolicy", env, verbose=1)

# Train the agent and save it each episode
model.learn(total_timesteps=150000, log_interval=10, callback=checkpoint_callback)