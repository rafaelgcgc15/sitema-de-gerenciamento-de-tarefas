Api para gerenciamento de tarefas de um projeto

Como se trata de uma api pequena em quantidade de funcionalidades, foi escolhido a abordagem API minimal do asp.net core

Comandos docker para criar a imagem e criar o container com a api

 cd GerenciadorDeTarefasApi/

 docker build -t gt-api . 

 docker run --rm -p 8000:5000 gt-api

 json para teste de post de tarefa
 {
  "titulo": "Tarefa 1",
  "descricao": "Descricao tarefa 1",
  "vencimento": "2024-10-20T16:56:05.677Z",
  "status": 1,
  "prioridade": 1,
  "comentario": "",
  "usuarioId": "92974a6f-a28e-4337-8d8c-7e326ea3c15a",
  "projetoId": "655a71a1-c983-4eb2-95e1-4f08c6bc1498"
}
