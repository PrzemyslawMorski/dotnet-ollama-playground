
# run ollama and get the tinydolphin model installed
docker run -d -v ollama:/root/.ollama -p 11434:11434 --name ollama ollama/ollama
docker exec -it ollama ollama pull tinydolphin
docker exec -it ollama ollama pull tinyllama
