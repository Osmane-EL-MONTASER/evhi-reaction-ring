import socket
import numpy as np

# Create a socket object
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# Define the host and port
host = 'localhost'
port = 5000

# Bind the socket to the host and port
server_socket.bind((host, port))

# Listen for incoming connections
server_socket.listen(1)
print('Server listening on {}:{}'.format(host, port))

while True:
    # Accept a client connection
    client_socket, client_address = server_socket.accept()
    print('Connected to client:', client_address)

    while True:
        # Receive all data sent by the client
        data = client_socket.recv(1024)
        
        # If the data is empty, break out of the loop
        if not data:
            break

        # Print the received data
        print('Received from client:', data)

        # Convert data to string
        data = data.decode()
        
        data = np.fromstring(data[1:-1], sep=' ')
        
        # If the first element of the array is between 10 and 11, second element between 1 and 1.5, third element between 0.4 and 1.2
        """if (10 <= data[0] <= 11) and (1 <= data[1] <= 1.5) and (0.4 <= data[2] <= 1.2):
           # Simulate the performance arround 85% (70% - 100%) normal
            performance = np.random.normal(0.85, 0.05)
        else:
            performance = 0.0"""
            
        # Let's make it a little bit less restrictive. Each parameter will contribute to the performance.
        # The performance will be a weighted sum of the parameters.
        # The weights will be 1/3, 1/3, 1/3.
        # The performance will be arround 85% (70% - 100%) in total for the three parameters, we will divide it by 3.
        performances = [0.0] * 10
        
        for i in range(len(performances)):
            if (0.55 <= data[i] <= 0.9):
                performances[i] += 3/3 * np.random.normal(85, 5)
            """if (1 <= data[1 + i*3] <= 1.5):
                performances[i] += 1/3 * np.random.normal(85, 5)
            if (0.4 <= data[2 + i*3] <= 1.2):
                performances[i] += 1/3 * np.random.normal(85, 5)"""
            
        
        # Convert the performance to a string
        performances = str(performances)
        
        print(performances)
        
        # Send the performance to the client
        client_socket.sendall(performances.encode())

    # Close the connection with the client
    client_socket.close()
