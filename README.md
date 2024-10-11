Api para gerenciamento de tarefas de um projeto

Como se trata de uma api pequena em quantidade de funcionalidades, foi escolhido a abordagem API minimal do asp.net core

Comandos docker para criar a imagem e criar o container com a api

 cd GerenciadorDeTarefasApi/

 docker build -t gt-api . 

 docker run --rm -p 8000:5000 gt-api
