# godot-server-multiplayer
Godot Multiplayer Demo using the Client Server Architecture

## What it is
This **minimal** example demonstrates how to implement a client-server architecture in godot using `NetworkMultiplayerENet` and the `Rpc` calls.

## Client Server Architecture
Normally Godot multiplayer examples show a Peer to Peer architecture, where all clients are interconnected, and where they can freely communicate with each other, for me its easier to implement and prototype, but also this in general and specially in godot is a mess, there are some other drawbacks like:
- Security is hard to achieve
- A client having a bad internet connection might affect other clients
- Latency is greater
- May require port forwarding

### Client-Server
To solve some of those problems a client-server architecture is needed, where there is a central authority (the server) who is in charge of controlling the clients and their interactions, this also makes easier to prevent cheating, since all information first has to go through a server.

![P2P vs Server Based](https://sites.google.com/site/cis3347cruzguzman014/_/rsrc/1480320465440/module-2/client-server-and-peer-to-peer-networking/p2p-network-vs-server.jpg?height=206&width=400 "P2P vs Server Based")

### This implementation
This implementation of the Client-Server architecture isn't ideal, it works ok but the clients are doing too much, remember that in a Server based architecture the client is just a representation of some server value, for example one improvement would be to do all the physics on the server, and just send the results to the client, but for a simple party game, or like in this case, an example, its fine.

#### Contact on Discord Raz#4584
